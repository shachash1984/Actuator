#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class Movement : MonoBehaviour {

    static public Movement S;
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private NavMeshAgent _agent;
    //[SerializeField] private bool allowedToMove = false;
    private Animator _animator;
    private AudioSource _audioSource;
    private ISceneManager currentSceneManager;
    [SerializeField] private Room _currentRoom;
    [SerializeField] private Room _previousRoom;
    [SerializeField] private Vector3 _dialogueWithDoctorPosition;
    [SerializeField] private Vector3 _hostageRoomPosition;
    public Room currentRoom { get { return _currentRoom; } private set { _currentRoom = value; } }
    public Room previousRoom { get { return _previousRoom; } private set { _previousRoom = value; } }
    public static event Action<Room, Room> OnPlayerEnteredRoom;
    private Rigidbody _rigidBody;
    private CharacterMovement _characterMovement;
    private Vector3 _prevPos;
    private Vector3 _destination;
    public GameObject clickIndicator;
    [SerializeField] private GameObject _gun;
    [SerializeField] private Transform _barrel;
    [SerializeField] private GameObject _shotEffect;
    public bool IsInCutScene { get; private set; }
    private float _prevTime;

    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        _rigidBody = GetComponent<Rigidbody>();
        _characterMovement = GetComponent<CharacterMovement>();
        Init();
    }

    private void OnDisable()
    {
        DeInit();
    }

    private void OnTriggerEnter(Collider other)
    {
        previousRoom = currentRoom;
        switch (other.tag)
        {
            case "MainRoom":
                currentRoom = Room.MainRoom;
                break;
            case "Corridor":
                currentRoom = Room.Corridor;
                break;
            case "HostageRoom":
                if (Scene1Manager.currentQuest == null)
                    currentRoom = Room.HostageRoom;
                break;
            default:
                break;
        }
        OnPlayerEnteredRoom?.Invoke(currentRoom, previousRoom);
    }

    public void PlayerEnteredHostageRoom()
    {
        if (Scene1Manager.currentQuest == null)
            currentRoom = Room.HostageRoom;
        OnPlayerEnteredRoom?.Invoke(Room.HostageRoom, Room.MainRoom);
    }

    public void Init()
    {
        Interactable.InteractionEvent += HandleInteractableEvent;
        DatabaseSearch.OnWindowOpen += HandleDataBaseWindowOpen;
        DatabaseSearch.OnAllWindowsClosed += HandleAllDataBaseWindowClosed;
        Scene1Manager.OnFinishedTutorial += MoveToDialogueWithDoctorPosition;
        Doctor.OnDoctorArrivedAtPosition += LookAtPosition;
        if (!_agent)
            _agent = GetComponent<NavMeshAgent>();
        currentRoom = Room.MainRoom;
        previousRoom = Room.Null;
        _animator = GetComponentInChildren<Animator>();
        if (SceneManager.GetActiveScene().buildIndex == 1 || SceneManager.GetActiveScene().buildIndex == 3)
        {
            ToggleRootMotion(true);
            _animator.Play("SittingIdle");
        }
            
        _audioSource = GetComponent<AudioSource>();
        _prevPos = transform.position;
        _destination = transform.position;
        HideGun();
    }

    public void DeInit()
    {
        Interactable.InteractionEvent -= HandleInteractableEvent;
        DatabaseSearch.OnWindowOpen -= HandleDataBaseWindowOpen;
        DatabaseSearch.OnAllWindowsClosed -= HandleAllDataBaseWindowClosed;
        Scene1Manager.OnFinishedTutorial -= MoveToDialogueWithDoctorPosition;
        Doctor.OnDoctorArrivedAtPosition -= LookAtPosition;
    }

    public bool IsMoving()
    {
        return _characterMovement.isMoving;
    }

    public void LookAt(Vector3 pos)
    {
        transform.DOLookAt(pos, 0.5f);
    }

    public void ToggleRootMotion(bool on)
    {
        _animator.applyRootMotion = on;
    }

    public bool CanMoveToPosition(Vector3 pos)
    {
        return !GridHandler.S.IsObstacle(pos);
    }

    private void DrawClickIndicator(Vector3 pos)
    {
        pos.y += 0.01f;
        Instantiate(clickIndicator, pos, Quaternion.Euler(new Vector3(90, 0, 0)));
    }

    public void HandleMouseClick(Vector3 point)
    {
        _audioSource.Play();
        _destination = point;
        SetDestination(_destination);
    }

    //private void LateUpdate()
    //{
    //    if (IsMoving())
    //    {
    //        float currentSpeed = Vector3.Distance(_destination, transform.position);
    //        if (currentSpeed > 0.25f)
    //        {
    //            _animator.SetFloat("Speed", Vector3.Distance(_destination, transform.position));
    //        }
    //        else
    //        {
    //            _animator.SetFloat("Speed", Vector3.Distance(_prevPos, transform.position));
    //        }
                
    //    }
    //    else
    //    {
    //        _animator.SetFloat("Speed", 0);
    //    }
    //    _prevPos = transform.position;
       
    //}

    public void ShowGun()
    {
        _gun.SetActive(true);
    }

    public void HideGun()
    {
        _gun.SetActive(false);
    }

    public void DrawGun()
    {
        Kidnapper kid = FindObjectOfType<Kidnapper>();
        transform.DOLookAt(kid.transform.position, 0.5f);
        _animator.SetTrigger("DrawGun");
        Invoke("ShowGun", 1f);
    }

    public void Shoot()
    {
        _animator.SetTrigger("Shoot");
        Instantiate(_shotEffect, _barrel.position, Quaternion.identity);
    }

    public void HolsterGun()
    {
        _animator.SetTrigger("HolsterGun");
        Invoke("HideGun", 1f);
    }

    private void MoveToHostageRoomPosition()
    {
        _hostageRoomPosition.y = transform.position.y;
        SetDestination(_hostageRoomPosition);
        Kidnapper k = FindObjectOfType<Kidnapper>();
        LookAtPosition(k.transform.position);
    }


    private void MoveToDialogueWithDoctorPosition()
    {
        StartCoroutine(MoveToDialogueWithDoctorPositionCoroutine());
    }

    private IEnumerator MoveToDialogueWithDoctorPositionCoroutine()
    {
        IsInCutScene = true;
        _dialogueWithDoctorPosition.y = transform.position.y;
        SetDestination(_dialogueWithDoctorPosition);
        //while (Vector3.Distance(transform.position, _dialogueWithDoctorPosition) > 1f)
        //{

        //    yield return null;
        //}
        //_animator.SetFloat("Speed", 0);
        yield return new WaitUntil(() => !IsMoving());
        IsInCutScene = false;
    }

    private void LookAtPosition(Vector3 pos)
    {
        StartCoroutine(LookAtPositionCoroutine(pos));
    }

    private IEnumerator LookAtPositionCoroutine(Vector3 pos)
    {
        yield return new WaitForSeconds(1);
        //_agent.enabled = false;
        //transform.DOLookAt(pos, 0.5f).OnComplete(() => _agent.enabled = true);
        transform.DOLookAt(pos, 0.5f);
    }

    public void SetDestination(Vector3 dst)
    {
        //if (allowedToMove)
        //    _agent.SetDestination(dst);
        StartCoroutine(_characterMovement.MoveToPosition(dst));
    }

    public Vector3 GetCurrentPosition()
    {
        return transform.position;
    }

    public void HandleDataBaseWindowOpen()
    {
        SetAllowedToMove(false);
    }

    public void HandleAllDataBaseWindowClosed()
    {
        SetAllowedToMove(true);
    }

    public bool IsAllowedToMove()
    {
        return true;// allowedToMove;
    }

    public void SetAllowedToMove(bool on)
    {
        if (on)
            ToggleRootMotion(false);
        //allowedToMove = on;
    }

    public void RegisterToSceneManager(ISceneManager ism)
    {
        currentSceneManager = ism;
    }

    public void DeregisterFromSceneManager()
    {
        currentSceneManager = null;
    }

    public void HandleInteractableEvent(InteractionParams ip)
    {
        //allowedToMove = ip.allowedToMove;
    }

    public void SetNavMeshAgent(bool on)
    {
        _agent.enabled = on;
    }


}
