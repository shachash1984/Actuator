using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

[System.Serializable]
public struct DialoguePathInfo
{
    public string key;
    public string folderPath;
    public string filePath;
}

public class DialogueMapper : MonoBehaviour {

    public List<DialoguePathInfo> dialoguePathInfos;
    static Dictionary<string, DialoguePathInfo> dialoguePathMap;

    private void Init()
    {
        dialoguePathMap = new Dictionary<string, DialoguePathInfo>();
        dialoguePathInfos = new List<DialoguePathInfo>();
        string filePath = Application.dataPath + "/Resources/DialogueMap.txt";
        if (File.Exists(filePath))
        {
            string[] data = File.ReadAllLines(filePath);
            foreach (string str in data)
            {
                string key = str.Split(':')[0].Trim();
                DialoguePathInfo di = new DialoguePathInfo();
                di.key = key;
                di.folderPath = str.Split(':')[1].Trim().Split('_')[0];
                di.filePath = str.Split(':')[1].Trim().Split('_')[1];
                dialoguePathMap.Add(key, di);
                dialoguePathInfos.Add(di);
            }
            
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Init();
    }

    public static string GetDialoguePath(string dialogueKey)
    {
        string s = string.Concat(dialoguePathMap[dialogueKey].folderPath, "/", dialoguePathMap[dialogueKey].filePath);
        
        return s;
    }

}
