#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using System;

public class CodePadInteractable : Interactable
{
    private Outline _outline;
    [SerializeField] private AudioClip _codePadSound;
    private bool _enteredCorrectCode = false;

    private void Start()
    {
        //_rend = GetComponent<MeshRenderer>();
        _outline = GetComponentInChildren<Outline>();
        _audioSource = GetComponent<AudioSource>();
        _outline.enabled = false;
    }

    private void OnEnable()
    {
        Quest.OnQuestFinished += HandleQuestFinished;
    }

    private void OnDisable()
    {
        Quest.OnQuestFinished -= HandleQuestFinished;
    }

    private void HandleQuestFinished(Quest obj)
    {
        if(obj is CodePadQuest)
        {
            _enteredCorrectCode = true;
        }
    }

    public override void OnCursorOver()
    {
        if (Scene1Manager.CanPlayerInteract() && !outlined)
        {
            _outline.enabled = true;
            ToggleIndicator(true);
            outlined = true;
        }
    }

    public override void OnCursorExit()
    {
        _outline.enabled = false;
        ToggleIndicator(false);
        outlined = false;
    }

    public override void OnInteraction(Interactable sender, InteractionParams ip)
    {
        if (active)
        {
            base.OnInteraction(sender, ip);
            PlaySound(_codePadSound);
        }
        
    }

    public override void OnCursorDown()
    {
        if (!_enteredCorrectCode)
        {
            interacting = false;
            OnInteraction(this, new InteractionCodeParams(0));
        }
            

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
        active = false;
    }

    public override void OnPlayerBecameNear()
    {
        active = true;
        //_outline.enabled = true;
    }
}

public class InteractionCodeParams : InteractionParams
{
    public int id;
    public InteractionCodeParams(int _id)
    {
        id = _id;
    }
}
