#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System;

public class DatabaseSearch : MonoBehaviour
{
    private string _articlesFolder;
    private const string _articleDataFile = "/ArticlesData.txt";
    private const char _newArticle = '\n';

    [SerializeField] private List<DatabaseArticle> _articlesList;
    [SerializeField] private InputField _searchBar;
    [SerializeField] private Text _seachResultText;
    public GameObject articlePrefab;
    public static event Action OnWindowOpen;
    public static event Action OnAllWindowsClosed;
    private List<DatabaseArticle> _openWindows = new List<DatabaseArticle>();

    private void Start()
    {
        _articlesFolder = Application.dataPath + "/Resources/Articles/";
        _searchBar.onEndEdit.AddListener(delegate { FindArticlesByName(_searchBar.text); });
        //_searchBar.onValueChanged.AddListener(() => )
    }

    private void OnEnable()
    {
        DatabaseArticle.OnWindowClosed += HandleWindowClosed;
    }

    private void OnDisable()
    {
        DatabaseArticle.OnWindowClosed -= HandleWindowClosed;
    }

    private void HandleWindowClosed(DatabaseArticle da)
    {
        _openWindows.Remove(da);
        if (_openWindows.Count <= 0)
            OnAllWindowsClosed?.Invoke();
    }

    public void FindArticlesByName(string name)
    {
        int articlesFound = 0;

        string fullDirectory = _articlesFolder + name + _articleDataFile;

        string[] articles = GetTextFromDataFile(fullDirectory);

        if (articles != null)
        {
            //Debug.Log("articles found");
            float xPos = 30f;
            float yPos = 30f;

            foreach (string article in articles)
            {
                //Debug.Log(article.Trim());
                string path = "Articles/" + name + "/" + article;
                articlePrefab = Resources.Load<GameObject>(path);

                if(articlePrefab!=null)
                {
                    Vector3 spawnPos = new Vector3(this.transform.position.x + xPos, this.transform.position.y + yPos, 0f);
                    GameObject articleFound = Instantiate(articlePrefab, spawnPos, Quaternion.identity);

                    articleFound.transform.SetParent(this.transform);
                    articlesFound++;
                    // set the next article ready in new position
                    xPos -= 30f;
                    yPos -= 30f;
                    _openWindows.Add(articleFound.GetComponent<DatabaseArticle>());
                    OnWindowOpen?.Invoke();
                    articlePrefab = null;
                }

                
            }
        }

        SearchResult(articlesFound);

        /*
        int articlesFound = 0;

        foreach (DatabaseArticle article in _articlesList)
        {
            if (article.GetArticleName() == name)
            {
                article.OpenArticle();
                articlesFound++;
            }
        }
        */
        //SearchResult(articlesFound);
    }

    public string[] GetTextFromDataFile(string path)
    {
        //StreamReader reader = new StreamReader(path);
        //string requestedText = reader.ReadToEnd();
        //reader.Close();
        //string[] articles = requestedText.Split(_newArticle);

        
        if(File.Exists(path))
        {
            string[] articles = File.ReadAllLines(path);
            Debug.Log(articles.Length);
            return articles;
        }
        return null;
    }

    private void SearchResult(int articlesFound)
    {
        // Nothing found
        if (articlesFound == 0)
        {
            _seachResultText.text = "No results\nwere found";
        }
        else // Found articles
        {
            _seachResultText.text = articlesFound + " results found";
        }
    }
}
