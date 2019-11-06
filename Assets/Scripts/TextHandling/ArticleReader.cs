using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public class ArticleReader : MonoBehaviour
{
    static public ArticleReader S;
    private const string SEARCH = "<SEARCH>";
    private const string FONT_SIZE = "<FONT_SIZE>";
    private const string NEW_LINE = "\n";
    private const string WINDOW_TOP_LEFT = "<WINDOW_TOP_LEFT>";
    private const string WINDOW_BOTTOM_RIGHT = "<WINDOW_BOTTOM_RIGHT>";
    private const string ARTICLE_TOP_LEFT = "<ARTICLE_TOP_LEFT>";
    private const string ARTICLE_BOTTOM_RIGHT = "<ARTICLE_BOTTOM_RIGHT>";
    private const string CONTENT_START = "<CONTENT_START>";
    private const string CONTENT_END = "<CONTENT_END>";
    private const string ARTICLE_START = "<ARTICLE_START>";
    private const string ARTICLE_END = "<ARTICLE_END>";
    private const string INTERACTIVE = "<INTERACTIVE>";
    private const string IMAGE = "<IMAGE>";
    private string _gameArticlePath;

    private void Awake()
    {
        if (S != null)
            Destroy(this);
        S = this;
        _gameArticlePath = Application.dataPath + "/Resources/Articles/";
    }

    private bool IsCornerVectorInRange(Vector2 vec)
    {
        return vec.x >= 0 && vec.x <= 1 && vec.y >= 0 && vec.y <= 1;
    }

    public void GetArticleBoxFromText(string key, string text, ref ArticleBox ab)
    {
        List<Article> articles = new List<Article>();
        int articleCounter = 0;
        string[] data = text.Split(char.Parse(NEW_LINE));
        Vector2 windowTopLeft = Vector2.zero;
        Vector2 windowBottomRight = Vector2.zero;
        Vector2 articleTopLeft = Vector2.zero;
        Vector2 articleBottomRight = Vector2.zero;
        List<List<string>> contents = new List<List<string>>();
        string search = "";
        int contentCounter = 0;
        int fontSize = 14;
        string iWord = "";
        string imagePath = "";
        ArticleType at = ArticleType.Text;
        for (int i = 0; i < data.Length; i++)
        {
            string[] lineValue = data[i].Split(new string[] { ">>" }, StringSplitOptions.None);

            string lineKey = string.Concat(lineValue[0], ">");
            string[] values;
            switch (lineKey)
            {
                case WINDOW_TOP_LEFT:
                    values = lineValue[1].Split(',');
                    windowTopLeft.x = float.Parse(values[0].Trim());
                    windowTopLeft.y = float.Parse(values[1].Trim());
                    if (!IsCornerVectorInRange(windowTopLeft))
                        Debug.LogError("<color=red>The top left corner of dialogue: " + key + " is not between 0 and 1</color>");
                    break;
                case WINDOW_BOTTOM_RIGHT:
                    values = lineValue[1].Split(',');
                    windowBottomRight.x = float.Parse(values[0].Trim());
                    windowBottomRight.y = float.Parse(values[1].Trim());
                    if (!IsCornerVectorInRange(windowBottomRight))
                        Debug.LogError("<color=red>The top left corner of dialogue: " + key + " is not between 0 and 1</color>");
                    break;
                case ARTICLE_START:
                    articles.Add(new Article());
                    break;
                case ARTICLE_END:
                    if (at == ArticleType.Text)
                        articles[articleCounter].Set(key, at, articleTopLeft, articleBottomRight, search, contents[articleCounter], fontSize, iWord, imagePath);
                    else
                        articles[articleCounter].Set(key, at, articleTopLeft, articleBottomRight, search, null, fontSize, iWord, imagePath);
                    iWord = "";
                    articleCounter++;
                    contentCounter = 0;
                    break;
                case ARTICLE_TOP_LEFT:
                    values = lineValue[1].Split(',');
                    articleTopLeft.x = float.Parse(values[0].Trim());
                    articleTopLeft.y = float.Parse(values[1].Trim());
                    if (!IsCornerVectorInRange(articleTopLeft))
                        Debug.LogError("<color=red>The top left corner of dialogue: " + key + " is not between 0 and 1</color>");
                    break;
                case ARTICLE_BOTTOM_RIGHT:
                    values = lineValue[1].Split(',');
                    articleBottomRight.x = float.Parse(values[0].Trim());
                    articleBottomRight.y = float.Parse(values[1].Trim());
                    if (!IsCornerVectorInRange(articleBottomRight))
                        Debug.LogError("<color=red>The bottom right corner of dialogue: " + key + " is not between 0 and 1</color>");
                    break;
                case SEARCH:
                    search = lineValue[1].Trim();
                    break;
                case CONTENT_START:
                    List<string> newContent = new List<string>();
                    newContent.Add("");
                    contents.Add(newContent);
                    break;
                case CONTENT_END:
                    contentCounter++;
                    //contents.Clear();
                    break;
                case IMAGE:
                    imagePath = lineValue[1].Trim();
                    at = ArticleType.Image;
                    break;
                case FONT_SIZE:
                    fontSize = int.Parse(lineValue[1].Trim());
                    break;
                case INTERACTIVE:
                    iWord = lineValue[1].Trim();
                    break;
                default:
                    contents[articleCounter][contentCounter] = (data[i].Trim());
                    break;
            }
        }
        ab.topLeft = windowTopLeft;
        ab.bottomRight = windowBottomRight;
        ab.searchValue = search;
        ab.articles = articles.ToArray();
        
    }

    public string GetTextFromFolder(string path)
    {
        string fileFromfolder = _gameArticlePath + path + ".txt";

        StreamReader reader = new StreamReader(fileFromfolder);
        string requestedText = reader.ReadToEnd();
        reader.Close();
        return requestedText;
    }
}
