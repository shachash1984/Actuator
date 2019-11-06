#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryTriggerManager : MonoBehaviour
{
    [SerializeField] private MemoryTrigger[] _triggers;

    private void Start()
    {
        foreach (MemoryTrigger trigger in _triggers)
        {
            trigger.SetTriggerActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 10)
        {
            foreach (MemoryTrigger trigger in _triggers)
            {
                trigger.SetTriggerActive(true);
            }
        }
    }
}
