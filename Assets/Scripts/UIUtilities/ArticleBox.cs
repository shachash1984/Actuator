using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public enum ArticleType { Text, Image}
public class Article
{
    public string name;
    public ArticleType articleType;
    public Vector2 topLeft; 
    public Vector2 bottomRight; 
    public List<string> contents; 
    public int fontSize;
    public string interactiveWord;
    public string imagePath;

    public Article() { }

    public Article(string n, ArticleType at ,Vector2 tl, Vector2 br, string h, List<string> l, int _fontSize = 14, string iWord = "", string image = "")
    {
        name = n;
        articleType = at;
        topLeft = tl;
        bottomRight = br;
        contents = l;
        fontSize = _fontSize;
        interactiveWord = iWord;
        imagePath = image;
    }

    public void Set(string n, ArticleType at, Vector2 tl, Vector2 br, string h, List<string> l, int _fontSize = 14, string iWord = "", string image = "")
    {
        name = n;
        articleType = at;
        topLeft = tl;
        bottomRight = br;
        contents = l;
        fontSize = _fontSize;
        interactiveWord = iWord;
        imagePath = image;
    }
}

public class ArticleBox : MonoBehaviour
{
    public Vector2 topLeft;
    public Vector2 bottomRight;
    public string searchValue;
    public Article[] articles;
    public Text searchValueText;
    public Button xButton;
    public const string INACTIVE_TAG = "InactiveArticle";
    public const string ACTIVE_TAG = "ActiveArticle";
    public static event Action OnOpenArticle;
    public static event Action<string> OnCloseArticle;
    private static int s_openArticlesCount;

    private void Awake()
    {
        ++s_openArticlesCount;
        gameObject.tag = ACTIVE_TAG;
        OnOpenArticle?.Invoke();
        if (!xButton)
        {
            xButton = transform.GetChild(0).GetComponent<Button>();
        }
        xButton.onClick.RemoveAllListeners();
        xButton.onClick.AddListener(() =>
        {
            gameObject.tag = INACTIVE_TAG;
            OnCloseArticle?.Invoke(searchValue);
            UIHandler.S.DismissArticle();
        });
    }

    private void OnDestroy()
    {
        s_openArticlesCount--;
    }

    public static int GetOpenArticleCount()
    {
        return s_openArticlesCount;
    }

}
