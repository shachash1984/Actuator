using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class AnswerButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Text>().DOColor(Color.yellow, 0.2f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Text>().DOColor(Color.white, 0.2f);
    }

    
}
