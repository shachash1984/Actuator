#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CrowdHandler : MonoBehaviour
{
    [SerializeField] private NavMeshAgent[] _crowd;
    [SerializeField] private Vector3 _target;
    [SerializeField] private Vector3[] _crowdPositions;
    [SerializeField] private Vector3[] _crowdPositionsInHostageRoom;
    Scene1Manager _sceneManager;
    private bool _playedCrowdAnimation = false;


    private void Awake()
    {
        //_crowd = GetComponentsInChildren<NavMeshAgent>(true);
        _sceneManager = FindObjectOfType<Scene1Manager>();
    }

    private void OnEnable()
    {
        _sceneManager.OnSceneEventTriggerred += HandleSceneEvent;
    }

    private void OnDisable()
    {
        _sceneManager.OnSceneEventTriggerred -= HandleSceneEvent;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12)
        {
            other.transform.parent.gameObject.SetActive(false);
            if(other.GetComponentInChildren<AudioSource>())
            {
                other.GetComponentInChildren<AudioSource>().Stop();
            }
        }
    }

    public void ActivateCrowd()
    {
        foreach (NavMeshAgent agent in _crowd)
        {
            agent.gameObject.SetActive(true);
            agent.enabled = true;
            agent.SetDestination(_target);
        }
        _crowd[0].GetComponentInChildren<AudioSource>().Play();
    }

    private void HandleSceneEvent(SceneEventParams obj)
    {
        PlayerEnteredNewRoomEventParams playerEventParams = obj as PlayerEnteredNewRoomEventParams;
        if(playerEventParams != null)
        {
            if (playerEventParams.currentRoom == Room.HostageRoom)
                MoveCrowdToPosition();
            else if (!_playedCrowdAnimation && playerEventParams.currentRoom == Room.Corridor && playerEventParams.previousRoom == Room.MainRoom)
                ActivateCrowd();
            _playedCrowdAnimation = true;
        }
    }

    private void MoveCrowdToPosition()
    {
        for (int i = 0; i < _crowdPositionsInHostageRoom.Length; i++)
        {
            _crowd[i].transform.localPosition = _crowdPositionsInHostageRoom[i];
            _crowd[i].gameObject.SetActive(true);
            _crowd[i].enabled = false;
            _crowd[i].GetComponentInChildren<Animator>().SetTrigger("Idle");
        }
    }
}
