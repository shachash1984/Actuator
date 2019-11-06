using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class InteractiveWord : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    private GameObject _searchField;
    private string _text;
    private Transform _parent;
    private Vector3 _initialPosition;
    private Camera _cam;

    public void OnPointerDown(PointerEventData eventData)
    {
        _searchField = GameObject.FindGameObjectWithTag("SearchField");
        if (!_searchField)
            Debug.LogError("SearchField not found");
        _text = transform.GetChild(0).GetComponent<Text>().text;
        _parent = transform.parent;
        _initialPosition = transform.position;
        transform.SetParent(GetComponentInParent<Canvas>().transform);
        _cam = Camera.main;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!eventData.dragging)
            return;

        if (IsInRange(_searchField))
        {
            
            _searchField.GetComponentInChildren<InputField>().text = _text;
            GetComponent<CanvasGroup>().DOFade(0, 0.5f).OnComplete(() =>
            {
                UIHandler.S.RemoveInteractiveWord(_text);
                Destroy(gameObject);
            });
        }
        else
        {
            transform.DOMove(_initialPosition, 1f).OnComplete(() => 
            {
                transform.SetParent(_parent);
            });
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    private bool IsInRange(GameObject go)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(go.GetComponent<RectTransform>(), transform.position);
        //return Vector3.Distance(go.transform.position, transform.position) < 30f;
    }
}
