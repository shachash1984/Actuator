using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;

public static class AStarUtil 
{
    static public SortedQueue<SNode> openList;
    static public HashSet<SNode> closedList;
    static public SNode asyncStartNode;
    static public SNode asyncEndNode;
    static public bool IsCalculating;
    //static public List<SNode> asyncPath;

    private static List<SNode> CalculatePath(SNode sn)
    {
        List<SNode> list = new List<SNode>();
        while (sn != null)
        {
            list.Add(sn);
            sn = (SNode)sn.parent;
        }
        list.Reverse();
        return list;
    }

    private static float GetHeuristicEstimateCost(SNode curNode, SNode goalNode)
    {
        Vector3 vecCost = curNode.position - goalNode.position;
        return vecCost.magnitude;
    }

    public static void SetAsyncData(SNode start, SNode end, List<SNode> path)
    {
        asyncStartNode = start;
        asyncEndNode = end;
        //asyncPath = path;
    }

    public static IEnumerator FindPathAsync(SNode start, SNode goal, List<SNode> asyncPath)
    {
        openList = new SortedQueue<SNode>();
        openList.Push(start);
        start.initialCost = 0.0f;
        start.estimatedCost = GetHeuristicEstimateCost(start, goal);
        start.nodeTotalCost = start.initialCost + start.estimatedCost;

        closedList = new HashSet<SNode>();
        SNode sNode = null;

        while (openList.Length != 0)
        {
            sNode = openList.First;
            if (sNode.position == goal.position)
            {
                asyncPath = CalculatePath(sNode);
            }


            List<SNode> neighbors = new List<SNode>();
            GridHandler.S.GetNeighbors(sNode, neighbors);

            for (int i = 0; i < neighbors.Count; i++)
            {
                SNode neighborNode = neighbors[i];
                if (!closedList.Contains(neighborNode))
                {
                    neighborNode.estimatedCost = GetHeuristicEstimateCost(neighborNode, goal);
                    neighborNode.initialCost = sNode.initialCost + 1;
                    neighborNode.nodeTotalCost = neighborNode.estimatedCost + neighborNode.initialCost;

                    neighborNode.parent = sNode;
                    if (!openList.Contains(neighborNode))
                    {
                        openList.Push(neighborNode);
                    }
                }
            }
            
            closedList.Add(sNode);
            openList.Remove(sNode);
            //Debug.Log("iteration");
            
        }
        yield return null;
        //If finished looping and cannot find the goal then return null
        if (sNode.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            asyncPath = null;
        }
        else
        {
            //Calculate the path based on the final node
            asyncPath = CalculatePath(sNode);
        }
       
    }

    public static List<SNode> FindPath(SNode start, SNode goal)
    {
        if (goal.bObstacle)
            return null;
        IsCalculating = true;
        openList = new SortedQueue<SNode>();
        openList.Push(start);
        start.initialCost = 0.0f;
        start.estimatedCost = GetHeuristicEstimateCost(start, goal);
        start.nodeTotalCost = start.initialCost + start.estimatedCost;

        closedList = new HashSet<SNode>();
        SNode sNode = null;

        while (openList.Length != 0)
        {
            sNode = openList.First;
            if (sNode == null)
            {
                IsCalculating = false;
                return null;
            }
                
            if (sNode.position == goal.position)
            {
                List<SNode> newPath = CalculatePath(sNode);
                IsCalculating = false;
                return newPath;
            }
                

            List<SNode> neighbors = new List<SNode>();
            GridHandler.S.GetNeighbors(sNode, neighbors);

            for (int i = 0; i < neighbors.Count; i++)
            {
                SNode neighborNode = neighbors[i];
                if(!closedList.Contains(neighborNode))
                {
                    neighborNode.estimatedCost = GetHeuristicEstimateCost(neighborNode, goal);
                    neighborNode.initialCost = sNode.initialCost + 1;
                    neighborNode.nodeTotalCost = neighborNode.estimatedCost + neighborNode.initialCost;

                    neighborNode.parent = sNode;
                    if(!openList.Contains(neighborNode))
                    {
                        openList.Push(neighborNode);
                    }
                }
            }
            if (sNode == null)
                return null;
            closedList.Add(sNode);
            openList.Remove(sNode);
        }

        //If finished looping and cannot find the goal then return null
        if (sNode.position != goal.position)
        {
            Debug.LogError("Goal Not Found");
            IsCalculating = false;
            return null;
        }

        //Calculate the path based on the final node
        List<SNode> Path = CalculatePath(sNode);
        IsCalculating = false;
        return Path;
    }
}
