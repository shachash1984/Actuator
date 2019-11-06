#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using DG.Tweening;
using System;

public class HallwayPerson : Interactable
{
    [SerializeField] private string _dialogueKey;
    [SerializeField] private Renderer _renderer;
    private Outline _outline;
    private Quaternion _initialRotation;

    private void OnEnable()
    {
        Scene1Manager.OnPlayerExitedMainRoom += HandlePlayerExitedMainRoom;
        Scene1Manager.OnPlayerEnteredHostageRoom += HandlePlayerEnteredHostageRoom;
        _initialRotation = transform.rotation;
    }

    private void OnDisable()
    {
        Scene1Manager.OnPlayerExitedMainRoom -= HandlePlayerExitedMainRoom;
        Scene1Manager.OnPlayerEnteredHostageRoom -= HandlePlayerEnteredHostageRoom;
    }

    private void HandlePlayerEnteredHostageRoom()
    {
        _renderer.enabled = false;
    }

    private void Start()
    {
        _renderer = GetComponentInChildren<Renderer>();
        _outline = GetComponentInChildren<Outline>();
        _outline.enabled = false;
        _renderer.enabled = false;
    }

    public override void OnPlayerBecameNear()
    {
        active = true;
    }

    public override void OnPlayerBecameFar()
    {
        active = false;
        TurnBack();
    }

    public override void OnCursorDown()
    {
        if(active)
        {
            interacting = false;
            OnInteraction(this, new InteractionPopDialogParams(_dialogueKey, 0f));
            TurnToPlayer(Movement.S.transform.position);
        }
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

    private void HandlePlayerExitedMainRoom()
    {
        _renderer.enabled = true;
    }

    private void TurnToPlayer(Vector3 speakerPos)
    {
        transform.DOLookAt(speakerPos, 1);
    }

    private void TurnBack()
    {
        transform.DORotate(_initialRotation.eulerAngles, 1);
    }

    public override void HandleEndInteraction()
    {
    }

    public override void OnNPCBecameNear()
    {
    }

    public override void OnNPCBecameFar()
    {
    }
}
