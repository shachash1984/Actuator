using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DoorJamesRoom : DoorBase
{
    [SerializeField] protected GameObject _doorLeft;
    [SerializeField] protected GameObject _doorRight;
    [SerializeField] protected Vector3 _doorLeftClosedPos;
    [SerializeField] protected Vector3 _doorLeftOpenPos;
    [SerializeField] protected Vector3 _doorRightClosedPos;
    [SerializeField] protected Vector3 _doorRightOpenPos;
    [SerializeField] protected AudioClip _openSound;
    [SerializeField] protected AudioClip _closeSound;
    protected AudioSource _audio;


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

    public override void Close()
    {
        _doorLeft.transform.DOLocalMove(_doorLeftClosedPos, 0.5f);
        _doorRight.transform.DOLocalMove(_doorRightClosedPos, 0.5f);
        PlaySound(_closeSound);
        IsOpen = false;
    }

    public override void HandleEndInteraction()
    {
        //throw new System.NotImplementedException();
    }

    public override void OnCursorDown()
    {
        if(!Locked)
        {
            Direction dir = GetPasserDirection(Movement.S.transform);
            PassThroughActive(Movement.S.transform, dir);
        }
    }

    public override void OnCursorExit()
    {
        
    }

    public override void OnCursorOver()
    {
        
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

    protected Direction GetPasserDirection(Transform passer)
    {
        if (Vector3.Distance(passer.position, FirstDoorStep.position) <= Vector3.Distance(passer.position, SecondDoorStep.position))
            return Direction.FirstToSecond;
        else
            return Direction.SecondToFirst;
    }

    public override void PassThroughPassive(Transform objectToPass, Direction dir, params Component[] componentsToHandle)
    {
        Vector3 previousPos = objectToPass.transform.position;
        float prevTime = Time.time;
        float currentTime = Time.time;
        Sequence seq = DOTween.Sequence();
        if (dir == DoorBase.Direction.FirstToSecond)
        {
            seq.Append(objectToPass.DOLookAt(SecondDoorStep.position, 0.5f));
            seq.Append(objectToPass.DOMove(SecondDoorStep.position, 1f));
        }
        else
        {
            seq.Append(objectToPass.DOLookAt(FirstDoorStep.position, 0.5f));
            seq.Append(objectToPass.DOMove(FirstDoorStep.position, 1f));
        }
        seq.Play().OnUpdate(()=> 
        {
            currentTime = Time.time;
            float deltaDistance = Vector3.Distance(previousPos, objectToPass.position);
            float deltaTime = currentTime - prevTime;
            if (deltaDistance > 0 && deltaTime > 0)
            {
                Animator anim = objectToPass.GetComponent<Animator>();
                if (anim)
                    anim.SetFloat("Speed", deltaDistance / deltaTime);
            }
            previousPos = objectToPass.position;
            prevTime = currentTime;
            
        });
    }

    public override void PassThroughActive(Transform objectToPass, Direction dir)
    {
        if(dir == DoorBase.Direction.FirstToSecond)
        {
            objectToPass.GetComponent<Movement>().SetDestination(SecondDoorStep.position);
        }
        else
        {
            objectToPass.GetComponent<Movement>().SetDestination(FirstDoorStep.position);
        }
    }
}
