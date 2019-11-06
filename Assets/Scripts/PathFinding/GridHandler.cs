using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridHandler : MonoBehaviour
{
    public static GridHandler S;
    [Flags] public enum BoundStatus { UnChecked = 0 ,In = 2, OutLeft = 4, OutUp = 8, OutRight = 16, OutDown = 32}
    public int numCols;
    public int numRows;
    public float cellSize;
    public bool showGrid;
    public bool showObstacles;
    public bool useDiagonal;
    public float obstaclePadding = 1f;

    private Vector3 _origin;
    private GameObject[] _obstacles;
    public SNode[,] nodes;
    public Vector3 origin { get { return _origin; } }

    //private void Reset()
    //{
    //    _obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
    //    //numCols = 100;
    //    //numRows = 100;
    //    //cellSize = 0.25f;
    //    numCols = 100;
    //    numRows = 100;
    //    cellSize = 0.25f;
    //    obstaclePadding = 1f;
    //    useDiagonal = true;
    //    showGrid = true;
    //    showObstacles = true;
    //    CalculateObstacles();
    //}

    private void Awake()
    {
        if (S != null)
            Destroy(S.gameObject);
        S = this;
        _origin = transform.position;
        if (_obstacles == null || _obstacles.Length == 0)
        {
            _obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
            
            CalculateObstacles();
        }
    }

    private void OnDrawGizmos()
    {
        _origin = transform.position;
        if (showGrid)
        {
            DebugDrawGrid(transform.position, numRows, numCols, cellSize, Color.blue);
            Gizmos.DrawSphere(transform.position, 0.5f);
        }

        
        if(showObstacles)
        {
            Vector3 obstacleSize = new Vector3(cellSize, 1, cellSize);
            if(_obstacles != null && _obstacles.Length > 0 && nodes != null && nodes.Length > 0)
            {
                //foreach (GameObject obs in _obstacles)
                //{
                //    Gizmos.DrawCube(GetGridCellCenter(GetGridIndex(obs.transform.position)), obstacleSize);
                //}

                foreach (Node n in nodes)
                {
                    if (n.bObstacle)
                    {
                        Gizmos.DrawCube(GetGridCellCenter(GetGridIndex(n.position)), obstacleSize);
                    }
                }
            }
        }
    }

    private void DebugDrawGrid(Vector3 position, int numRows, int numCols, float cellSize, Color color)
    {
        float width = numCols * cellSize;
        float height = numRows * cellSize;

        //draw the horizontal grid lines
        for (int i = 0; i < numRows; i++)
        {
            Vector3 startPos = new Vector3(origin.x, origin.y, origin.z + i * cellSize);
            Vector3 endPos = new Vector3(origin.x + width, origin.y, origin.z + i * cellSize);
            Debug.DrawLine(startPos, endPos, color);
        }

        //draw the vertical grid lines
        for (int i = 0; i < numCols; i++)
        {
            Vector3 startPos = new Vector3(origin.x + i * cellSize, origin.y, origin.z);
            Vector3 endPos = new Vector3(origin.x + i * cellSize, origin.y, origin.z + height);
            Debug.DrawLine(startPos, endPos, color);
        }
    }

    private void CalculateObstacles()
    {
        nodes = new SNode[numCols, numRows];

        int index = 0;
        for (int i = 0; i < numCols; i++)
        {
            for (int j = 0; j < numRows; j++)
            {
                Vector3 cellPos = GetGridCellCenter(index);
                nodes[i, j] = new SNode(cellPos);
                index++;
            }
        }

        if(_obstacles != null && _obstacles.Length > 0)
        {
            foreach (GameObject obs in _obstacles)
            {
                //int indexCell = GetGridIndex(obs.transform.position);
                //int col = GetColumn(indexCell);
                //int row = GetRow(indexCell);
                if(obs.GetComponent<Collider>())
                {
                    
                    int[] obstacleBoundsIndexes = GetGridIndexesInBounds(obs.GetComponent<Collider>().bounds);
                    foreach (int indexCell in obstacleBoundsIndexes)
                    {
                        int col = GetColumn(indexCell);
                        int row = GetRow(indexCell);
                        nodes[col, row].MarkAsObstacle();
                    }
                }
                else
                {
                    int[] obstacleBoundsIndexes = GetGridIndexesInBounds(obs.GetComponent<Renderer>().bounds);
                    foreach (int indexCell in obstacleBoundsIndexes)
                    {
                        int col = GetColumn(indexCell);
                        int row = GetRow(indexCell);
                        nodes[col, row].MarkAsObstacle();
                    }
                }
                
                
            }
        }
    }

    public Vector3 GetGridCellCenter(int index)
    {
        Vector3 cellPosition = GetGridCellPosition(index);
        cellPosition.x += cellSize / 2;
        cellPosition.z += cellSize / 2;

        return cellPosition;
    }

    public Vector3 GetGridCellPosition(int index)
    {
        int row = GetRow(index);
        int col = GetColumn(index);
        float xPosInGrid = col * cellSize;
        float zPosInGrid = row * cellSize;

        return origin + new Vector3(xPosInGrid, 0, zPosInGrid);
    }

    public int GetColumn(int index)
    {
        return index % numCols;
    }

    public int GetRow(int index)
    {
        return index / numCols;
    }

    public bool IsInBounds(Vector3 pos)
    {
        float width = numCols * cellSize;
        float height = numRows * cellSize;

        return (pos.x >= origin.x && pos.x <= origin.x + width && pos.z >= origin.z && pos.z <= origin.z + height);
    }

    public BoundStatus GetBoundsStatus(Vector3 pos)
    {
        BoundStatus bs = BoundStatus.UnChecked;
        float width = numCols * cellSize;
        float height = numRows * cellSize;
        if (pos.x < origin.x)
            bs |= BoundStatus.OutLeft;
        if (pos.x > origin.x + width)
            bs |= BoundStatus.OutRight;
        if (pos.z < origin.z)
            bs |= BoundStatus.OutDown;
        if (pos.z > origin.z + height)
            bs |= BoundStatus.OutUp;
        if (bs == BoundStatus.UnChecked)
            bs = BoundStatus.In;
        return bs;
    }

    public void GetNeighbors(SNode node, List<SNode> neighbors)
    {
        Vector3 neighborPos = node.position;
        int neighborIndex = GetGridIndex(neighborPos);

        int row = GetRow(neighborIndex);
        int col = GetColumn(neighborIndex);

        //Bottom
        int nodeRow = row - 1;
        int nodeCol = col;
        AssignNeighbor(nodeRow, nodeCol, neighbors);

        //Top
        nodeRow = row + 1;
        nodeCol = col;
        AssignNeighbor(nodeRow, nodeCol, neighbors);

        //Right
        nodeRow = row;
        nodeCol = col + 1;
        AssignNeighbor(nodeRow, nodeCol, neighbors);

        //Left
        nodeRow = row;
        nodeCol = col - 1;
        AssignNeighbor(nodeRow, nodeCol, neighbors);

        if(useDiagonal)
        {
            //BottomLeft
            nodeRow = row - 1;
            nodeCol = col - 1;
            AssignNeighbor(nodeRow, nodeCol, neighbors);

            //TopLeft
            nodeRow = row + 1;
            nodeCol = col - 1;
            AssignNeighbor(nodeRow, nodeCol, neighbors);

            //TopRight
            nodeRow = row + 1;
            nodeCol = col + 1;
            AssignNeighbor(nodeRow, nodeCol, neighbors);

            //BottomRight
            nodeRow = row - 1;
            nodeCol = col + 1;
            AssignNeighbor(nodeRow, nodeCol, neighbors);
        }
    }

    private void AssignNeighbor(int nodeRow, int nodeCol, List<SNode> neighbors)
    {
        if(nodeRow != -1 && nodeCol != -1 && nodeRow < numRows && nodeCol < numCols)
        {
            SNode nodeToAdd = nodes[nodeRow, nodeCol];
            if(!nodeToAdd.bObstacle)
            {
                neighbors.Add(nodeToAdd);
            }
        }
    }

    public int GetGridIndex(Vector3 pos)
    {
        if(!IsInBounds(pos))
        {
            return -1;
        }

        pos -= origin;
        int col = (int)(pos.x / cellSize);
        int row = (int)(pos.z / cellSize);

        return (row * numCols + col);
    }

    public bool IsObstacle(Vector3 pos)
    {
        int cellIndex = GetGridIndex(pos);
        return IsObstacle(cellIndex);
    }

    public bool IsObstacle(int cellIndex)
    {
        int col = GetColumn(cellIndex);
        int row = GetRow(cellIndex);
        return nodes[row, col].bObstacle;
    }

    private void ClampPosToBounds(ref Vector3 pos, BoundStatus bs)
    {
        float width = numCols * cellSize;
        float height = numRows * cellSize;
        if ((bs & BoundStatus.OutDown) != 0)
            pos.z = origin.z;
        if ((bs & BoundStatus.OutLeft) != 0)
            pos.x = origin.x;
        if ((bs & BoundStatus.OutUp) != 0)
            pos.z = origin.z + height - cellSize;
        if ((bs & BoundStatus.OutRight) != 0)
            pos.x = origin.x + width - cellSize;
    }

    private int[] GetGridIndexesInBounds(Bounds bnd)
    {
        List<int> indexList = new List<int>();

        Vector3 blVector = new Vector3(bnd.min.x, bnd.min.y, bnd.min.z);
        //blVector *= obstaclePadding;
        blVector = bnd.center + (blVector - bnd.center) * obstaclePadding;
        //Debug.Log(GetBoundsStatus(blVector));
        BoundStatus boundStatus = GetBoundsStatus(blVector);
        if (boundStatus != BoundStatus.In)
            ClampPosToBounds(ref blVector, boundStatus);
        int blIndex = GetGridIndex(blVector);

        Vector3 trVector = new Vector3(bnd.max.x, bnd.max.y, bnd.max.z);
        //trVector *= obstaclePadding;
        trVector = bnd.center + (trVector - bnd.center) * obstaclePadding;
        //Debug.Log(GetBoundsStatus(trVector));
        boundStatus = GetBoundsStatus(trVector);
        if (boundStatus != BoundStatus.In)
            ClampPosToBounds(ref trVector, boundStatus);
        int trIndex = GetGridIndex(trVector);

        for (int i = GetColumn(blIndex); i <= GetColumn(trIndex); i++)
        {
            for (int j = GetRow(blIndex); j <= GetRow(trIndex); j++)
            {
                if (IsInBounds(nodes[i,j].position))
                {
                    indexList.Add(GetGridIndex(nodes[i, j].position));
                }
            }
        }
        return indexList.ToArray();
    }

}
