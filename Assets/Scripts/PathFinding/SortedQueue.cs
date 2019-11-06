using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortedQueue<T> where T : Node
{
    private List<T> _nodes = new List<T>();

    public int Length { get { return _nodes.Count; } }
    public T First
    {
        get
        {
            if (_nodes.Count > 0)
            {
                return _nodes[0];
            }
            return null;
        }
    }

    public bool Contains(T node)
    {
        return _nodes.Contains(node);
    }

    public void Push(T node)
    {
        _nodes.Add(node);
        _nodes.Sort();
    }

    public void Remove(T node)
    {
        if(_nodes.Contains(node))
        {
            _nodes.Remove(node);
            _nodes.Sort();
        }
    }
    
}
