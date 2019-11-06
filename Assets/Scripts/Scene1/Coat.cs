#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;
using System;
using DG.Tweening;

public class Coat : Interactable
{

    //private MeshRenderer _rend;
    private Outline _outline;
    [SerializeField] private AudioClip _clickSound;
    [SerializeField] private GameObject _notePrefab;

    private void Start()
    {
        active = false;
        //_rend = GetComponent<MeshRenderer>();
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    public override void OnCursorDown()
    {
        if (Scene1Manager.currentQuest is TutorialQuest)
            OnInteraction(this, new InteractionPopDialogParams(Scene1Manager.COAT, 0f));
        else if (Scene1Manager.currentQuest is CodePadQuest)
            DisplayNote();
        
    }

    private void DisplayNote()
    {
        StartCoroutine(DisplayNoteCoroutine());
    }

    private IEnumerator DisplayNoteCoroutine()
    {
        GameObject noteCanvas = Instantiate(_notePrefab);
        GameObject note = noteCanvas.transform.GetChild(0).gameObject;
        note.transform.localScale = Vector3.zero;
        note.transform.DOScale(1, 0.5f);
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Escape));
        note.transform.DOScale(0, 0.5f).OnComplete(() => { Destroy(note); });

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

    public override void OnInteraction(Interactable sender, InteractionParams ip)
    {
        if(active)
        {
            base.OnInteraction(sender, ip);
            PlaySound(_clickSound);
        }
        else
        {
            //Movement.S.SetDestination(GetClosestGroundPos());
        }
    }

    public override void OnPlayerBecameFar()
    {
        active = false;
        //Debug.Log("Player became far from: " + name);
        //_rend.material.SetFloat("_OutlineWidth", 1.0f);
        //_outline.enabled = false;
    }

    public override void OnPlayerBecameNear()
    {
        active = true;
        //Debug.Log("Player became near from : "+name);
        //_rend.material.SetFloat("_OutlineWidth", 1.1f);
        //_outline.enabled = true;
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

public class InteractionPopDialogParams : InteractionParams
{
    public string dialogKey;
    public float delay;
    public InteractionPopDialogParams(string key, float delay)
    {
        dialogKey = key;
        this.delay = delay;
    }
}


