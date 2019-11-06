using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;

public class Kidnapper : MonoBehaviour
{
    private Transform _hostage;
    private Scene1Manager _sceneManager;
    public static event Action<string> OnKidnapperDialogue;
    public static event Action OnShootQuickEvent;
    public static event Action<bool> OnKidnapperDied;
    private bool _kidnapperGaveWarning = false;

    private void Awake()
    {
        _sceneManager = FindObjectOfType<Scene1Manager>();
        ToggleVisibility(false);
    }

    private void OnEnable()
    {
        _sceneManager.OnSceneEventTriggerred += HandleSceneEvent;
        MessageBox.OnButtonClicked += HandleMessageBoxEvent;
    }

    private void OnDisable()
    {
        _sceneManager.OnSceneEventTriggerred -= HandleSceneEvent;
        MessageBox.OnButtonClicked -= HandleMessageBoxEvent;
    }

    private void HandleMessageBoxEvent(string arg1, int arg2, bool block)
    {
        if(arg1 == Scene1Manager.KIDNAP1 && arg2 == MessageBox.END_DIALOGUE)
        {
            OnKidnapperDialogue?.Invoke(Scene1Manager.KIDNAP2);
        }
        if(arg1 == Scene1Manager.KIDNAP2 && arg2 == MessageBox.END_DIALOGUE)
        {
            StartCoroutine(TriggerTheShootDialogue());
        }
        if (arg1 == Scene1Manager.KIDNAP3)
        {
            if (arg2 == MessageBox.QUICK_EVENT_HIT)
                Die(true);
            else
                Die(false);
        }
            
    }

    IEnumerator TriggerTheShootDialogue()
    {
        yield return new WaitForSeconds(1f);
        OnShootQuickEvent?.Invoke();
    }

    private void HandleSceneEvent(SceneEventParams obj)
    {
        PlayerEnteredNewRoomEventParams playerEventParams = obj as PlayerEnteredNewRoomEventParams;
        if(playerEventParams != null)
        {
            if(!_kidnapperGaveWarning && playerEventParams.currentRoom == Room.HostageRoom)
            {
                ToggleVisibility(true);
                OnKidnapperDialogue?.Invoke(Scene1Manager.KIDNAP1);
                _kidnapperGaveWarning = true;
            }
        }
    }

    private void Die(bool shotByJames)
    {
        GetComponentInChildren<Animator>().SetTrigger("Die");
        OnKidnapperDied?.Invoke(shotByJames);
        GetComponent<AudioSource>().Play();
    }

    private void ToggleVisibility(bool on)
    {
        foreach (Transform t in transform)
        {
            if (!t.Equals(this.transform))
                t.gameObject.SetActive(on);
        }
    }
}
