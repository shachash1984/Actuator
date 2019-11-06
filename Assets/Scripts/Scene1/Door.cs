#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using UnityEngine.AI;

public class Door : Interactable {

    private bool _isLocked;
    [SerializeField] private List<Transform> _sceneElementsToLoad;
    [SerializeField] private List<Transform> _sceneElementsToHide;
    //[SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _openSound; 
    [SerializeField] private AudioClip _closeSound;
    private Transform _leftDoorPart;
    private Transform _rightDoorPart;
    [SerializeField] Vector3 _leftClosedPos;
    [SerializeField] Vector3 _rightClosedPos;
    [SerializeField] Vector3 _leftOpenPos;
    [SerializeField] Vector3 _rightOpenPos;
    [SerializeField] Room thisRoom;
    [SerializeField] Room otherRoom;
    private bool _sceneElementsToLoadVisible = false;
    private bool _sceneElementsToHideVisible = true;

    private ISceneManager _currentSceneManager;
    

    private void Awake()
    {
        _currentSceneManager = FindObjectOfType<Scene1Manager>();
        _leftDoorPart = transform.GetChild(0);
        _rightDoorPart = transform.GetChild(1);
        _isLocked = true;
        //ToggleSceneElementsToLoad(false);
        SetSceneElementsVisibilty(_sceneElementsToLoad, false);
        SetSceneElementsCollision(_sceneElementsToLoad, false);
        _audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        //_currentSceneManager.OnSceneEventTriggerred += HandleSceneEvent;
    }

    private void OnDisable()
    {
        //_currentSceneManager.OnSceneEventTriggerred -= HandleSceneEvent;
    }

    private void HandleSceneEvent(SceneEventParams obj)
    {
        SceneDialogueEventParams dialogueParams = obj as SceneDialogueEventParams;
        if(dialogueParams != null && dialogueParams.dialogueKey == Scene1Manager.CROWD_OMG)
        {
            _isLocked = false;
            SetSceneElementsCollision(_sceneElementsToLoad, true);
        }
        PlayerEnteredNewRoomEventParams penepParams = obj as PlayerEnteredNewRoomEventParams;
        if(penepParams != null)
        {
            if (penepParams.previousRoom == otherRoom && penepParams.currentRoom == thisRoom)
            {
                if (thisRoom == Room.Corridor && !GetSceneElementsVisibility(_sceneElementsToLoad))
                {
                    SetSceneElementsVisibilty(_sceneElementsToLoad, true);
                }
                if (thisRoom == Room.HostageRoom && GetSceneElementsVisibility(_sceneElementsToHide))
                {
                    SetSceneElementsVisibilty(_sceneElementsToHide, false);
                    SetSceneElementsCollision(_sceneElementsToHide, false);

                    SetSceneElementsVisibilty(_sceneElementsToLoad, true);
                    SetSceneElementsCollision(_sceneElementsToLoad, true);
                }
            }
        }
    }


    public override void OnPlayerBecameNear()
    {
        //active = true;
        //gameObject.layer = 9;
        //if (!_isLocked && GetComponentInChildren<Renderer>().enabled)
        //    PlayDoorAnimation(true);
    }

    public override void OnPlayerBecameFar()
    {
        //active = false;
        //gameObject.layer = 2;
        //if (!_isLocked && GetComponentInChildren<Renderer>().enabled)
        //    PlayDoorAnimation(false);
    }

    public override void OnNPCBecameNear()
    {
        active = true;
        gameObject.layer = 9;
        PlayDoorAnimation(true);
    }

    public override void OnNPCBecameFar()
    {
        active = false;
        gameObject.layer = 2;
        PlayDoorAnimation(false);
    }

    private bool GetSceneElementsVisibility(List<Transform> sceneElements)
    {
        Renderer r = null;
        for (int i = 0; i < sceneElements.Count; i++)
        {
            r = sceneElements[i].GetComponent<Renderer>();
            if (r)
                break;
            else
            {
                r = sceneElements[i].GetComponentInChildren<Renderer>();
                if (r)
                    break;
            }
        }
        return r.enabled;
    }

    private void SetSceneElementsVisibilty(List<Transform> sceneElements, bool on)
    {
        foreach (Transform t in sceneElements)
        {
            Renderer ren = t.GetComponent<Renderer>();
            if (ren)
                ren.enabled = on;
            Renderer[] rens = t.GetComponentsInChildren<Renderer>();
            foreach (Renderer child in rens)
            {
                child.enabled = on;
            }

        }
    }

    private bool GetSceneElementsCollision(List<Transform> sceneElements)
    {
        Collider c = null;
        for (int i = 0; i < sceneElements.Count; i++)
        {
            c = sceneElements[i].GetComponent<Collider>();
            if (c)
                break;
        }
        return c.enabled;
    }

    private void SetSceneElementsCollision(List<Transform> sceneElements, bool on)
    {
        foreach (Transform t in sceneElements)
        {
            Collider col = t.GetComponent<Collider>();
            if (col)
                col.enabled = on;
            Collider[] cols = t.GetComponentsInChildren<Collider>();
            foreach (Collider child in cols)
            {
                child.enabled = on;
            }

        }
        _sceneElementsToLoadVisible = on;
    }

    private void ToggleSceneElementsToLoad(bool on)
    {
        foreach (Transform t in _sceneElementsToLoad)
        {
            Renderer ren = t.GetComponent<Renderer>();
            if (ren)
                ren.enabled = on;
            Collider col = t.GetComponent<Collider>();
            if (col)
                col.enabled = on;
            foreach (Transform child in t)
            {
                Renderer rend = child.GetComponent<Renderer>();
                if (rend)
                    rend.enabled = on;
                Collider c = child.GetComponent<Collider>();
                if (c)
                    c.enabled = on;
            }
            
        }
        _sceneElementsToLoadVisible = on;
    }

    private void ToggleSceneElementsToHide(bool on)
    {
        foreach (Transform t in _sceneElementsToHide)
        {
            Renderer ren = t.GetComponent<Renderer>();
            if (ren)
                ren.enabled = on;
            foreach (Transform child in t)
            {
                Renderer rend = child.GetComponent<Renderer>();
                if (rend)
                    rend.enabled = on;
            }
        }
        _sceneElementsToHideVisible = on;
    }

    public void LockUnlockRoom(bool newLockCondition)
    {
        _isLocked = newLockCondition;
    }

    void PlayDoorAnimation(bool open)
    {
        if(open)
        {
            _leftDoorPart.DOLocalMove(_leftOpenPos, 0.5f);
            _rightDoorPart.DOLocalMove(_rightOpenPos, 0.5f);
            _audioSource.clip = _openSound;
            _audioSource.Play();
        }
        else
        {
            _leftDoorPart.DOLocalMove(_leftClosedPos, 0.5f);
            _rightDoorPart.DOLocalMove(_rightClosedPos, 0.5f);
            _audioSource.clip = _closeSound;
            _audioSource.Play();
        }
    }

    IEnumerator DoorAnimationCoroutine()
    {
        yield return null;
    }

    public override void HandleEndInteraction()
    {
        throw new NotImplementedException();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == PLAYER_LAYER || other.gameObject.layer == NPC_LAYER)
        {
            NavMeshAgent nav = other.GetComponent<NavMeshAgent>();
            if (nav)
                nav.speed = 1;
            if (other.gameObject.layer == PLAYER_LAYER)
            {
                OnPlayerBecameNear();
            }
            else if (other.gameObject.layer == NPC_LAYER)
            {
                OnNPCBecameNear();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == PLAYER_LAYER || other.gameObject.layer == NPC_LAYER)
        {
            NavMeshAgent nav = other.GetComponent<NavMeshAgent>();
            if (nav)
                nav.speed = 1f;

            if (other.gameObject.layer == PLAYER_LAYER)
            {
                OnPlayerBecameFar();
            }
            else if (other.gameObject.layer == NPC_LAYER)
            {
                OnNPCBecameFar();
            }
        }
    }

    public override void OnCursorOver()
    {
        throw new NotImplementedException();
    }

    public override void OnCursorExit()
    {
        throw new NotImplementedException();
    }

    public override void OnCursorDown()
    {
        throw new NotImplementedException();
    }
}
