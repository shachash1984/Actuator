#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using cakeslice;

//Used for Gun instead of clothes
public class JamesClothes : Interactable
{
    //private MeshRenderer _rend;
    private Outline _outline;
    [SerializeField] private AudioClip _zipperSound;
    [SerializeField] private AudioClip _clickSound;

    private void Start()
    {
        _outline = GetComponentInChildren<Outline>();
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

    public override void OnCursorDown()
    {
        OnInteraction(this, new InteractionPopDialogParams(Scene1Manager.GUN, 0f));
    }

    IEnumerator ZipperSoundCoroutine()
    {
        yield return new WaitForSeconds(1f);
        PlaySound(_zipperSound);
    }

    public override void HandleEndInteraction()
    {
        
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
