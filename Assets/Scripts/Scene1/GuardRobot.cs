#pragma warning disable 0649
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardRobot : MonoBehaviour
{
    [SerializeField] private Renderer _renderer;
    bool _playerdEnteredHostageRoom = false;
    private void OnEnable()
    {
        Scene1Manager.OnPlayerEnteredHostageRoom += HandlePlayerEnteredHostageRoom;
    }

    private void OnDisable()
    {
        Scene1Manager.OnPlayerEnteredHostageRoom -= HandlePlayerEnteredHostageRoom;
    }

    private void HandlePlayerEnteredHostageRoom()
    {
        Kidnapper kid = FindObjectOfType<Kidnapper>();
        transform.LookAt(kid.transform);
        _playerdEnteredHostageRoom = true;
        ToggleVisibility(true);
    }

    private void ToggleVisibility(bool on)
    {
        _renderer.enabled = on;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("HostageRoom") && !_playerdEnteredHostageRoom)
        {
            ToggleVisibility(false);
            Animator anim = GetComponentInChildren<Animator>();
            if (anim)
                anim.SetTrigger("Idle");
            AudioSource audio = GetComponentInChildren<AudioSource>();
            if (audio)
                audio.enabled = false;
        }

    }
}
