using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scene2Manager : MonoBehaviour, ISceneManager
{
    public List<Interactable> interactables { get; set; }
    public int lastPickedAnswer { get; set; }
    public event Action<SceneEventParams> OnSceneEventTriggerred;
    private const string LOBBY_PERSON1 = "LobbyPerson1";
    private const string LOBBY_PERSON2 = "LobbyPerson2";
    private const string LOBBY_PERSON3 = "LobbyPerson3";

    private IEnumerator Start()
    {
        Init();
        yield return null;
        OnSceneEventTriggerred?.Invoke(null);
        //Movement.S.SetNavMeshAgent(true);
    }

    private void OnDisable()
    {
        DeInit();
    }

    public void Init()
    {
        Movement.S.RegisterToSceneManager(this);
        Interactable.InteractionEvent += HandleInteractionEvent;
        MessageBox.OnButtonClicked += HandleMessageBoxPlayerClicked;
    }

    public void DeInit()
    {
        Movement.S.DeregisterFromSceneManager();
        Interactable.InteractionEvent -= HandleInteractionEvent;
        MessageBox.OnButtonClicked -= HandleMessageBoxPlayerClicked;
    }

    private void HandleInteractionEvent(InteractionParams obj)
    {
        
    }

    private void HandleMessageBoxPlayerClicked(string dialogueName, int buttonPressed, bool block)
    {
        switch (dialogueName)
        {
            case LOBBY_PERSON1:
            case LOBBY_PERSON2:
            case LOBBY_PERSON3:
                if (buttonPressed != MessageBox.CONTINUE_DIALOGUE)
                {
                    OnSceneEventTriggerred?.Invoke(null);
                    lastPickedAnswer = buttonPressed;
                }
                break;
            default:
                break;
        }
    }
}
