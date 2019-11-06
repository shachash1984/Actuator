using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Diagnostics;
using Debug = UnityEngine.Debug;
using DG.Tweening;
using System.Threading;

public class CharacterMovement : MonoBehaviour
{
    private Vector3 _startPos, _endPos;
    private SNode _startNode, _goalNode;
    private Rigidbody rb;
    private Animator _animator;
    private List<SNode> _path;
    //public Transform goal;
    public int BezierPointsNum = 4;
    public float lerpParam = 0.25f;
    [Min(1)]
    public float speed = 2f;
    List<Vector3> actualPath = new List<Vector3>();
    [SerializeField] private float _pathLookDirOffset;

    public bool isMoving;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
    }


    public IEnumerator MoveToPosition(Vector3 goalPos)
    {
        
        int cellIndex = GridHandler.S.GetGridIndex(goalPos);
        //Debug.Log("grid index: " + cellIndex + " col: " + GridHandler.S.GetColumn(cellIndex) + " row: " + GridHandler.S.GetRow(cellIndex));
        //Debug.Log("obstacle: " + GridHandler.S.IsObstacle(goalPos));
       
        goalPos.y = transform.position.y;
        
        
        _startPos = transform.position;
        _endPos = goalPos;
        _path = null;
        yield return StartCoroutine(FindPath());
        
        if (_path != null && _path.Count > 0)
        {
            float currentY = transform.position.y;
            actualPath.Clear();
            for (int i = 0; i < _path.Count; i++)
            {
                Vector3 nextPos = _path[i].position;

                List<Vector3> bezierPts = new List<Vector3>();
                for (int j = i; j < i + BezierPointsNum; j++)
                {
                    if (j < _path.Count)
                        bezierPts.Add(_path[j].position);
                }
                nextPos = Utils.Bezier(lerpParam, bezierPts);
                nextPos.y = currentY;
                actualPath.Add(nextPos);
            }
            if (actualPath.Count >= 3)
                transform.DOLookAt(actualPath[2], 0.25f);
            else
                transform.DOLookAt(actualPath[actualPath.Count - 1], 0.25f);
            
            yield return new WaitUntil(() => !DOTween.IsTweening(transform));
            isMoving = true;
            var tween = transform.DOPath(actualPath.ToArray(), actualPath.Count * 1 / speed).SetEase(Ease.OutCubic);

            Tweener lookTween = null;
            tween.OnWaypointChange((currentIndex) =>
            {
                //Debug.Log("OnWaypointChange: " + currentIndex);
                int nextWayPoint = currentIndex + 1;
                if (nextWayPoint < actualPath.Count - 1)
                    lookTween = transform.DOLookAt(actualPath[nextWayPoint], Time.deltaTime * 2);


            });
            Vector3 previousPos = transform.position;
            float prevTime = Time.time;
            float currentTime = Time.time;
            float animationSpeed = 0;
            tween.OnUpdate(() => 
            {
                currentTime = Time.time;
                float deltaDistance = Vector3.Distance(previousPos, transform.position);
                float deltaTime = currentTime - prevTime;
                if (deltaDistance > 0 && deltaTime > 0f)
                {
                    if (deltaDistance / deltaTime <= 0.1f)
                        tween.Complete();
                    _animator.SetFloat("Speed", deltaDistance / deltaTime);
                }
                else
                {
                    //_animator.SetFloat("Speed", 0f);
                    tween.Complete();
                }
                animationSpeed = _animator.GetFloat("Speed");
                previousPos = transform.position;
                prevTime = currentTime;
            });
            tween.OnComplete(() =>
            {
                isMoving = false;
                DOTween.To(() => animationSpeed, x => animationSpeed = x, 0, 0.2f).OnUpdate(() => _animator.SetFloat("Speed", animationSpeed) );
                
            });
        }
        else
        {
            isMoving = false;
        }
        
    }

    //private void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.Space))
    //    {
    //        stopwatch.Reset();
    //        stopwatch.Start();
    //        _startPos = transform.position;
    //        _endPos = goal.position;
    //        FindPath();
    //        float currentY = transform.position.y;
    //        actualPath.Clear();
    //        if(_path != null && _path.Count > 0)
    //        {
    //            for (int i = 0; i < _path.Count; i++)
    //            {
    //                Vector3 nextPos = _path[i].position;

    //                List<Vector3> bezierPts = new List<Vector3>();
    //                for (int j = i; j < i + BezierPointsNum; j++)
    //                {
    //                    if (j < _path.Count)
    //                        bezierPts.Add(_path[j].position);
    //                }
    //                nextPos = Utils.Bezier(lerpParam, bezierPts);
    //                nextPos.y = currentY;
    //                actualPath.Add(nextPos);
    //            }
    //            stopwatch.Stop();
    //            Debug.Log("Calculation time: " + stopwatch.ElapsedMilliseconds);
    //            transform.DOPath(actualPath.ToArray(), _path.Count * 1 / speed).SetEase(Ease.Linear);
    //        }
    //        if (stopwatch.IsRunning)
    //            stopwatch.Stop();
    //    }
        
    //}

    private IEnumerator FindPath()
    {
        _startNode = new SNode(GridHandler.S.GetGridCellCenter(GridHandler.S.GetGridIndex(_startPos)));
        _goalNode = new SNode(GridHandler.S.GetGridCellCenter(GridHandler.S.GetGridIndex(_endPos)));

        
        ThreadStart threadStart = delegate
        {
            _path = AStarUtil.FindPath(_startNode, _goalNode);
        };
        //threadStart.Invoke();
        Thread t = new Thread(threadStart);
        t.Start();

        yield return new WaitUntil(() => !AStarUtil.IsCalculating);
    }

    void OnDrawGizmos()
    {
        if (_path != null && _path.Count > 0)
        {
            int index = 1;
            foreach (Node node in _path)
            {
                if (index < _path.Count)
                {
                    SNode nextNode = _path[index];
                    Debug.DrawLine(node.position, nextNode.position, Color.green);
                    index++;
                }
            };
            int vecIndex = 1;
            if(actualPath.Count > 0)
            {
                foreach (Vector3 vec in actualPath)
                {
                    if(vecIndex < actualPath.Count)
                    {
                        Vector3 nextVec = actualPath[vecIndex];
                        Debug.DrawLine(vec, nextVec, Color.red);
                        vecIndex++;
                    }
                    
                }
            }
            
        }
    }
}
