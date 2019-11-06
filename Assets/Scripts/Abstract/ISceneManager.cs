using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SceneEventParams
{
    
}

public interface ISceneManager  {

    List<Interactable> interactables { get; set; }
    int lastPickedAnswer { get; set; }
    event Action<SceneEventParams> OnSceneEventTriggerred;
    void Init();
    void DeInit();
}

public enum Room { Null, Corridor, HostageRoom, MainRoom }

public class SceneDialogueEventParams : SceneEventParams
{
    public float delay;
    public string dialogueKey;
    public SceneDialogueEventParams(float del, string key)
    {
        delay = del;
        dialogueKey = key;
    }
}

public class PlayerEnteredNewRoomEventParams : SceneEventParams
{
    public Room currentRoom { get; private set; }
    public Room previousRoom { get; private set; }
    public bool hidePreviousRoom;
    public PlayerEnteredNewRoomEventParams(Room _currentRoom, Room _prevRoom, bool hide)
    {
        currentRoom = _currentRoom;
        previousRoom = _prevRoom;
        hidePreviousRoom = hide;
    }
}

