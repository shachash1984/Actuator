using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorKidnapRoom : DoorJamesRoom
{
    private void Awake()
    {
        _audio = GetComponent<AudioSource>();
        Locked = true;
        IsOpen = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.gameObject.layer);
        if (other.gameObject.layer == PLAYER_LAYER)
        {
            OnPlayerBecameNear();
        }
        else if (other.gameObject.layer == NPC_LAYER)
        {
            OnNPCBecameNear();
            Direction dir = GetPasserDirection(other.transform);
            PassThroughPassive(other.transform, dir, null);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == PLAYER_LAYER)
        {
            OnPlayerBecameFar();

        }
        else if (other.gameObject.layer == NPC_LAYER)
        {
            OnNPCBecameFar();
        }
    }

    public override void OnCursorDown()
    {
        if (!Locked)
        {
            Direction dir = GetPasserDirection(Movement.S.transform);
            PassThroughActive(Movement.S.transform, dir);
            Open();
            Invoke("PlayerEnteredHostageRoom", 1f);
        }
    }

    public void PlayerEnteredHostageRoom()
    {
        Movement.S.PlayerEnteredHostageRoom();
    }

    public override void OnNPCBecameFar()
    {
        Close();
    }

    public override void OnNPCBecameNear()
    {
        Open();
    }

    public override void OnPlayerBecameFar()
    {
        if (IsOpen)
            Close();
    }

    public override void OnPlayerBecameNear()
    {
        if (!Locked && !IsOpen)
            Open();
    }

    public override void Open()
    {
        _doorLeft.transform.DOLocalMove(_doorLeftOpenPos, 0.5f);
        _doorRight.transform.DOLocalMove(_doorRightOpenPos, 0.5f);
        PlaySound(_openSound);
        IsOpen = true;
    }
}
