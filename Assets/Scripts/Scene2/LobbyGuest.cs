using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class LobbyGuest : Interactable
{
    Outline _outline;
    bool _outlined;

    private void Start()
    {
        _outline = GetComponentInChildren<Outline>();
        _outline.enabled = false;
        
    }

    public override void OnCursorOver()
    {
        if (Scene1Manager.CanPlayerInteract() && !outlined)
        {
            outlined = true;
            _outline.enabled = true;
            ToggleIndicator(true);
        }
    }

    public override void OnCursorExit()
    {
        outlined = false;
        _outline.enabled = false;
        ToggleIndicator(false);
    }

    public override void HandleEndInteraction()
    {
        
    }

    public override void OnCursorDown()
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
        active = false;
    }

    public override void OnPlayerBecameNear()
    {
        active = true;
    }
}
