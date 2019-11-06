#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class DogTag : Interactable
{
    [SerializeField] private Outline[] _outlines;
    [SerializeField] private AudioClip _clickSound;

    private void Start()
    {
        active = false;
        //_rend = GetComponent<MeshRenderer>();
        _outlines = GetComponentsInChildren<Outline>();
        ToggleOutlines(false);
    }

    public override void HandleEndInteraction()
    {
        interacting = false;
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

    public override void OnCursorDown()
    {
        OnInteraction(this, new InteractionPopDialogParams(Scene1Manager.DOGTAGS, 0f));

    }

    public override void OnCursorOver()
    {
        if(Scene1Manager.CanPlayerInteract() && !outlined)
        {
            ToggleOutlines(true);
            ToggleIndicator(true);
            outlined = true;
        }
    }

    public override void OnCursorExit()
    {
        ToggleOutlines(false);
        ToggleIndicator(false);
        outlined = false;
    }

    private void ToggleOutlines(bool on)
    {
        foreach (Outline outline in _outlines)
        {
            outline.color = 0;
            outline.enabled = on;
        }
    }

    public override void OnInteraction(Interactable sender, InteractionParams ip)
    {
        if (active)
        {
            base.OnInteraction(sender, ip);
            PlaySound(_clickSound);
        }
        else
        {
            //Movement.S.SetDestination(GetClosestGroundPos());
        }
    }
    
}
