#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LobbyPerson : NPC
{
    private const float MIN_TIME_TO_MOVE = 3f;
    private const float MAX_TIME_TO_MOVE = 6f;
    private float _currentTimeToMove;
    private float _timer;
    private int _currentDialogueKey;
    [SerializeField] private string[] _dialogueKeys;
    private Animator _animator;
    private NavMeshAgent _agent;

    private void Start()
    {
        _animator = GetComponent<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        MoveToNewPosition();
    }

    private void Update()
    {
        if(IsInPosition())
        {
            if (active)
                return;
            if (TimeIsUp())
            {
                ResetTimer();
                MoveToNewPosition();
            }
        }
    }

    private void LateUpdate()
    {
        _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    private void MoveToNewPosition()
    {
        StartCoroutine(MoveToNewPositionCoroutine());
    }

    private IEnumerator MoveToNewPositionCoroutine()
    {
        Vector3 wantedPos = GetRandomNavmeshLocation(3f);
        while (wantedPos == Vector3.zero)
        {
            wantedPos = GetRandomNavmeshLocation(3f);
            //Debug.Log("Recalculating Random Position");
            yield return null;
        }
        GetComponent<NavMeshAgent>().SetDestination(wantedPos);
    }

    private bool IsInPosition()
    {
        NavMeshAgent nma = GetComponent<NavMeshAgent>();
        return nma.remainingDistance <= nma.stoppingDistance;
    }

    private Vector3 GetRandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = Vector3.zero;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, 1))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }

    private void ResetTimer()
    {
        _timer = Time.time;
        _currentTimeToMove = Random.Range(MIN_TIME_TO_MOVE, MAX_TIME_TO_MOVE);
    }

    private bool TimeIsUp()
    {
        return (Time.time - _currentTimeToMove) >= _timer;
    }

    private void OnMouseDown()
    {
        if(active)
        {
            OnInteraction(this, new InteractionPopDialogParams(_dialogueKeys[_currentDialogueKey], 0f));
        }
    }

    public override void OnPlayerBecameNear()
    {
        active = true;
    }

    public override void OnPlayerBecameFar()
    {
        active = false;
    }

    public override void OnInteraction(Interactable i, InteractionParams ip)
    {
        if (active)
            base.OnInteraction(i, ip);
    }

    public override void OnCursorOver()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCursorExit()
    {
        throw new System.NotImplementedException();
    }

    public override void OnCursorDown()
    {
        throw new System.NotImplementedException();
    }
}
