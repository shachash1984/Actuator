#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;
using DG.Tweening;

public class Doctor : Interactable
{
    private NavMeshAgent _agent;
    public Vector3 greetingPosition;
    [SerializeField] private Vector3 _hearNoisePosition;
    [SerializeField] private Vector3 _kidnapperRoomPosition;
    [SerializeField] private Vector3 _kidnapperRoomEnterancePosition;
    public static event Action<string, float> OnDoctorDialogue;
    public static event Action OnDoctorSteppedOutOfRoom;
    public static event Action<Vector3> OnDoctorArrivedAtPosition;
    public const string FINISHED_DIALOGUE_1 = "Finished1";
    private Animator _animator;
    private bool allowedToMove = false;
    [SerializeField] private Renderer _renderer;
    private bool _doctorEnteredHostageRoom = false;
    

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        ToggleVisibility(false);
    }

    private void OnEnable()
    {
        Scene1Manager.OnFinishedTutorial += MoveToGreetingPosition;
        Scene1Manager.OnPlayerEnteredHostageRoom += HandlePlayerEnteredHostageRoom;
        MessageBox.OnButtonClicked += HandleMessageBoxPlayerClick;
        Scene1Manager.OnCrowdNoise += StepOutOfTheRoom;
        Scene1Manager.OnPlayerExitedMainRoom += HandlePlayerExitedMainRoom;
        Kidnapper.OnKidnapperDied += HandleKidnapperDied;
        _animator = GetComponent<Animator>();
    }

    private void HandlePlayerEnteredHostageRoom()
    {
        allowedToMove = false;
        _kidnapperRoomPosition.y = transform.position.y;
        transform.position = _kidnapperRoomPosition;
        transform.DORotate(new Vector3(0, -96.173f, 0), 0);
        ToggleVisibility(true);
        _animator.Play("Captured");
    }

    private void OnDisable()
    {
        Scene1Manager.OnFinishedTutorial -= MoveToGreetingPosition;
        Scene1Manager.OnPlayerEnteredHostageRoom -= HandlePlayerEnteredHostageRoom;
        MessageBox.OnButtonClicked -= HandleMessageBoxPlayerClick;
        Scene1Manager.OnCrowdNoise -= StepOutOfTheRoom;
        Scene1Manager.OnPlayerExitedMainRoom -= HandlePlayerExitedMainRoom;
        Kidnapper.OnKidnapperDied -= HandleKidnapperDied;
    }

    private void Update()
    {
        if (allowedToMove)
            _animator.SetFloat("Speed", _agent.velocity.magnitude);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("MainRoom"))
        {
            ToggleVisibility(true);
        }
        else if(other.CompareTag("HostageRoom"))
        {
            ToggleVisibility(false);
            _animator.speed = 1;
            _doctorEnteredHostageRoom = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainRoom"))
        {
            ToggleVisibility(false);
        }
    }

    private void MoveToGreetingPosition()
    {
        allowedToMove = true;
        _agent.enabled = true;
        SetDestination(greetingPosition);
        StartCoroutine(DoctorMainDialogue());
    }

    private void SetDestination(Vector3 pos)
    {
        _agent.SetDestination(pos);
    }

    private void StepOutOfTheRoom()
    {
        StartCoroutine(StepOutOfRoomCoroutine());
    }

    private IEnumerator StepOutOfRoomCoroutine()
    {
        yield return new WaitForSeconds(3f);
        Tweener lookTween = transform.DOLookAt(_hearNoisePosition, 1f);
        yield return new WaitForSeconds(1);
        _agent.enabled = true;
        _agent.SetDestination(_hearNoisePosition);
        yield return new WaitUntil(() => _agent.path.status == NavMeshPathStatus.PathComplete);
        OnDoctorSteppedOutOfRoom?.Invoke();
        yield return new WaitForSeconds(3f);
        _agent.SetDestination(_kidnapperRoomPosition);

    }

    private IEnumerator DoctorMainDialogue()
    {
        //yield return new WaitForSeconds(1);
        yield return new WaitUntil(() => Vector3.Distance(greetingPosition, transform.position) <= 1.5f);
        OnDoctorArrivedAtPosition?.Invoke(transform.position);
        transform.DOLookAt(Movement.S.transform.position, 0.2f);
        //_agent.enabled = false;
        OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR1, 0f);
    }

    private void HandleMessageBoxPlayerClick(string dialogueName, int buttonPressed, bool block)
    {
        switch (dialogueName)
        {
            case Scene1Manager.DOCTOR1:
                OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR2, 0.1f);
                break;
            case Scene1Manager.DOCTOR2:
                if (buttonPressed == MessageBox.END_DIALOGUE)
                    OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR3, 0.1f);
                break;
            case Scene1Manager.DOCTOR3:
                OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR4, 0.1f);
                break;
            case Scene1Manager.DOCTOR4:
                if (buttonPressed == MessageBox.END_DIALOGUE)
                    OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR5, 0.1f);
                break;
            case Scene1Manager.DOCTOR5:
                if(buttonPressed == 1)
                    OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR6, 0.1f);
                else
                {
                    if (buttonPressed != MessageBox.CONTINUE_DIALOGUE)
                        OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR8, 0.1f);
                }
                    
                break;
            case Scene1Manager.DOCTOR6:
                if (buttonPressed == MessageBox.END_DIALOGUE)
                    OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR7, 0.1f);
                break;
            case Scene1Manager.DOCTOR7:
                OnDoctorDialogue?.Invoke(FINISHED_DIALOGUE_1, 0.1f);
                break;
            case Scene1Manager.DOCTOR8:
                if (buttonPressed == MessageBox.END_DIALOGUE)
                    OnDoctorDialogue?.Invoke(Scene1Manager.DOCTOR9, 0.1f);
                break;
            case Scene1Manager.DOCTOR9:
                OnDoctorDialogue?.Invoke(FINISHED_DIALOGUE_1, 0.1f);
                break;
            default:
                break;
        }
    }

    public override void OnPlayerBecameFar()
    {
        
    }

    public override void OnPlayerBecameNear()
    {
        
    }

    public void HandlePlayerExitedMainRoom()
    {
        if (!_doctorEnteredHostageRoom)
        {
            ToggleVisibility(true);
            _agent.speed = 3.5f;
            _animator.speed = 1.1f;
        }
            
    }

    private void ToggleVisibility(bool on)
    {
        _renderer.enabled = on;
    }

    void HandleKidnapperDied(bool shotByJames)
    {
        _animator.SetTrigger("InvaderDied");
    }

    public override void OnCursorOver()
    {
        return;
    }

    public override void OnCursorExit()
    {
        return;
    }

    public override void OnCursorDown()
    {
        return;
    }

    public override void HandleEndInteraction()
    {
        throw new NotImplementedException();
    }

    public override void OnNPCBecameNear()
    {
        throw new NotImplementedException();
    }

    public override void OnNPCBecameFar()
    {
        throw new NotImplementedException();
    }
}
