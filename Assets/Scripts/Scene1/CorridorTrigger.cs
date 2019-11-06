using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CorridorTrigger : MonoBehaviour
{
    public int index;
    public static event Action<int> OnCorridorTriggerHit;
    bool triggerred = false;
    private void OnTriggerEnter(Collider other)
    {
        if (!triggerred)
            OnCorridorTriggerHit?.Invoke(index);
        triggerred = true;
    }
}
