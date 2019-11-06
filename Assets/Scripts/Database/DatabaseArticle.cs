#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class DatabaseArticle : MonoBehaviour
{
    [SerializeField] private string _key;
    private GameObject _article;
    [SerializeField] private Button _closeButton;
    public static event Action<DatabaseArticle> OnWindowClosed;

    private void Start()
    {
        _article = this.gameObject;
        _closeButton.onClick.AddListener(CloseArticle);
    }

    public void CloseArticle()
    {
        //_article.SetActive(false);
        Destroy(gameObject);
        OnWindowClosed?.Invoke(this);
    }

    public void OpenArticle()
    {
        _article.SetActive(true);
    }

    public string GetArticleName()
    {
        return _key;
    }
}
