using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SNode : Node
{
    public float initialCost;
    public SNode()
    {
        this.estimatedCost = 0.0f;
        this.initialCost = 1.0f;
        this.nodeTotalCost = this.estimatedCost + initialCost;
        this.bObstacle = false;
        this.parent = null;
    }

    public SNode(Vector3 pos)
    {
        this.estimatedCost = 0.0f;
        this.initialCost = 1.0f;
        this.nodeTotalCost = this.estimatedCost + initialCost;
        this.bObstacle = false;
        this.parent = null;

        this.position = pos;
    }

    public override int CompareTo(object obj)
    {
        SNode node = (SNode)obj;
        if (this.nodeTotalCost < node.nodeTotalCost)
            return -1;
        if (this.nodeTotalCost > node.nodeTotalCost)
            return 1;

        return 0;
    }
}
