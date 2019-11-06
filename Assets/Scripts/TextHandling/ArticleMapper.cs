using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable]
public struct ArticlePathInfo
{
    public string key;
    public string folderPath;
    public string filePath;
}

public class ArticleMapper : MonoBehaviour
{
    public List<ArticlePathInfo> articlePathInfos;
    static Dictionary<string, ArticlePathInfo> articlePathMap;

    private void Init()
    {
        articlePathMap = new Dictionary<string, ArticlePathInfo>();
        articlePathInfos = new List<ArticlePathInfo>();
        string filePath = Application.dataPath + "/Resources/ArticleMap.txt";
        if (File.Exists(filePath))
        {
            string[] data = File.ReadAllLines(filePath);
            foreach (string str in data)
            {
                string key = str.Split(':')[0].Trim();
                ArticlePathInfo api = new ArticlePathInfo();
                api.key = key;
                api.folderPath = str.Split(':')[1].Trim().Split('_')[0];
                api.filePath = str.Split(':')[1].Trim().Split('_')[1];
                articlePathMap.Add(key, api);
                articlePathInfos.Add(api);
            }

        }
    }

    private void Awake()
    {
        Init();
    }

    public static string GetArticlePath(string articleKey)
    {
        string s = string.Concat(articlePathMap[articleKey].folderPath, "/", articlePathMap[articleKey].filePath);

        return s;
    }

    public static bool ContainsPath(string articleKey)
    {
        return articlePathMap.ContainsKey(articleKey);
    }
}
