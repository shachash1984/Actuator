#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DatabaseMapMarker : MonoBehaviour
{
    [SerializeField] private GameObject _marker;
    [SerializeField] private Button _button;

    private void Start()
    {
        _button.onClick.AddListener(ShowMarker);
        HideMarker();
    }

    public void HideMarker()
    {
        _marker.SetActive(false);
    }

    public void ShowMarker()
    {
        _marker.SetActive(true);
    }
}
