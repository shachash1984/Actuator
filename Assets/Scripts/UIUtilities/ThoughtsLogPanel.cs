using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ThoughtsLogPanel : MonoBehaviour
{
    public static bool IsMouseOverPanel()
    {
        return EventSystem.current.IsPointerOverGameObject();
    }
}
