#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

public class Chart : Interactable
{
    //private MeshRenderer _rend;
    private Outline _outline;
    [SerializeField] private AudioClip _chartSound;

    private void Start()
    {
        //_rend = GetComponent<MeshRenderer>();
        _outline = GetComponent<Outline>();
        _audioSource = GetComponent<AudioSource>();
        _outline.enabled = false;
    }

    public override void OnCursorOver()
    {
        if(Scene1Manager.CanPlayerInteract() && !outlined)
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

    public override void OnPlayerBecameFar()
    {
        active = false;
    }

    public override void OnPlayerBecameNear()
    {
        active = true;
        _outline.enabled = true;
    }

    public override void OnInteraction(Interactable sender, InteractionParams ip)
    {
        if(active)
        {
            base.OnInteraction(sender, ip);
            PlaySound(_chartSound);
        }
        else
        {
            //Movement.S.SetDestination(GetClosestGroundPos());
        }
    }

    public override void OnCursorDown()
    {
        OnInteraction(this, new InteractionPopDialogParams(Scene1Manager.CHART, 0f));
        
    }

    public override void HandleEndInteraction()
    {
        interacting = false;
    }

    public override void OnNPCBecameNear()
    {
        throw new System.NotImplementedException();
    }

    public override void OnNPCBecameFar()
    {
        throw new System.NotImplementedException();
    }
}
