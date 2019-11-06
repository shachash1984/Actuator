using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;
using DG.Tweening;

public class InteractionParams
{
    
}

[RequireComponent(typeof(Collider))]
public abstract class Interactable : MonoBehaviour {

    public const int PLAYER_LAYER = 10;
    public const int NPC_LAYER = 12;
    public GameObject indicatorPrefab;
    public GameObject indicator;
    public Vector3 indicatorPos;
    protected bool active;
    protected bool interacting;
    protected bool outlined;
    protected Camera _cam;
    protected AudioSource _audioSource;
    public static event Action<InteractionParams> InteractionEvent;
    public virtual void OnInteraction(Interactable sender, InteractionParams ip)
    {
        if (!interacting)
            InteractionEvent?.Invoke(ip);
        interacting = true;
        
    }
    public abstract void HandleEndInteraction();
    public abstract void OnPlayerBecameNear();
    public abstract void OnPlayerBecameFar();
    public abstract void OnNPCBecameNear();
    public abstract void OnNPCBecameFar();

    public void PlaySound(AudioClip _clip)
    {
        if (!_audioSource)
            _audioSource = GetComponent<AudioSource>();
        if (!_audioSource.enabled)
            return;
        _audioSource.clip = _clip;
        _audioSource.Play();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PLAYER_LAYER)
        {
            OnPlayerBecameNear();
        }
        else if(other.gameObject.layer == NPC_LAYER)
        {
            OnNPCBecameNear();
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

    protected Vector3 GetClosestGroundPos()
    {
        Vector3 origin = transform.position;
        float radius = GetComponent<Collider>().bounds.max.magnitude;
        RaycastHit hitInfo;
        if(Physics.SphereCast(origin, radius, transform.forward ,out hitInfo))
        {
            GameObject ground = hitInfo.collider.gameObject;
            if(ground && ground.layer == 11)
            {
                return ground.transform.position;
            }
        }
        Vector3 groundPos = transform.position;
        groundPos.y = 0;
        return groundPos;
    }

    public void ToggleIndicator(bool on)
    {
        if(on)
        {
            if (!indicator)
                indicator = Instantiate(indicatorPrefab);
            if (!_cam)
                _cam = Camera.main;
            indicator.GetComponent<Canvas>().worldCamera = _cam;
            indicator.transform.localScale = Vector3.zero;
            indicator.transform.position = indicatorPos;

            
            indicator.transform.DOLookAt(_cam.transform.position, 0f);
            Vector3 rotFix = indicator.transform.rotation.eulerAngles;
            rotFix.z = 0;
            indicator.transform.rotation = Quaternion.Euler(rotFix);
            indicator.transform.DOScale(1, 0.2f).SetEase(Ease.InOutBack);
        }
        else
        {
            if(indicator)
                indicator.transform.DOScale(0, 0.2f).SetEase(Ease.InOutBack);
        }
    }

    public abstract void OnCursorOver();

    public abstract void OnCursorExit();

    public abstract void OnCursorDown();
}
