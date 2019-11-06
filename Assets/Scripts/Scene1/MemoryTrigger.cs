#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemoryTrigger : Interactable
{
    [SerializeField] private bool _allowInteraction = true;

    [SerializeField] private string _key;

    private void OnTriggerEnter(Collider other)
    {
        if(_allowInteraction && other.gameObject.layer == 10)
        {
            OnInteraction(this, new InteractionPopDialogParams(_key, 0));
            _allowInteraction = false;
        }
    }

    public void SetTriggerActive(bool state)
    {
        _allowInteraction = state;
    }

    public override void HandleEndInteraction()
    {
        
    }

    public override void OnNPCBecameFar()
    {
        
    }

    public override void OnNPCBecameNear()
    {
        
    }

    public override void OnPlayerBecameFar()
    {
        
    }

    public override void OnPlayerBecameNear()
    {

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
