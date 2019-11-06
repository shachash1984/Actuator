using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public struct Dialogue
{
    private string name; //The name of the file
    public Vector2 topLeft; // Must
    public Vector2 bottomRight; // Must
    public string header; // Must
    public List<List<string>> contents; // Must at least one line
    public string[] answers; // Not must
    public bool blockMovement;
    public bool background;
    public List<KeyCode> quickEventKeys;
    public float quickEventTime;
    public bool isQuickEvent;
    public int fontSize;
    public string interactiveWord;

    public Dialogue(string n, Vector2 tl, Vector2 br, string h, List<List<string>> l, string[] a, bool _block, bool _BG = true ,int _fontSize = 14, string iWord = "" , bool isEvent = false, List<KeyCode> kc = null, float eventTime = 0)
    {
        name = n;
        topLeft = tl;
        bottomRight = br;
        header = h;
        contents = l;
        answers = a;
        blockMovement = _block;
        background = _BG;
        isQuickEvent = isEvent;
        quickEventKeys = kc;
        quickEventTime = eventTime;
        fontSize = _fontSize;
        interactiveWord = iWord;
    }

    public string GetName()
    {
        return name;
    }

    public override string ToString()
    {
        return topLeft.ToString() + " " + header.ToString();
    }
}

public class DialogueReader : MonoBehaviour
{
    public static DialogueReader S;

    private const string HEADER = "<HEADER>";
    private const string FONT_SIZE = "<FONT_SIZE>";
    private const string NEW_LINE = "\n";
    private const string QUICK_EVENT_KEY = "<QUICK_EVENT_KEY>";
    private const string EVENT_TIME = "<EVENT_TIME>";
    private const string ANSWER = "<ANSWER>";
    private const string TOP_LEFT = "<TOP_LEFT>";
    private const string BOTTOM_RIGHT = "<BOTTOM_RIGHT>";
    private const string CONTENT_START = "<CONTENT_START>";
    private const string CONTENT_END = "<CONTENT_END>";
    private const string INTERACTIVE = "<INTERACTIVE>";
    private const string BLOCK = "<BLOCK>";
    private const string BACKGROUND = "<BACKGROUND>";

    private string _gameDialoguePath;

    private void Awake()
    {
        if (S != null)
            Destroy(gameObject);
        S = this;
        _gameDialoguePath = Application.dataPath + "/Resources/Dialogues/";
    }

    public string GetTextFromFolder(string path)
    {
        string fileFromfolder = _gameDialoguePath + path + ".txt";

        StreamReader reader = new StreamReader(fileFromfolder);
        string requestedText = reader.ReadToEnd();
        reader.Close();
        return requestedText;
    }

    private bool IsCornerVectorInRange(Vector2 vec)
    {
        return vec.x >= 0 && vec.x <= 1 && vec.y >= 0 && vec.y <= 1;
    }

    public Dialogue GetDialogueFromText(string key, string text)
    {
        string[] data = text.Split(char.Parse(NEW_LINE));
        Vector2 topLeft = Vector2.zero;
        Vector2 bottomRight = Vector2.zero;
        List<List<string>> contents = new List<List<string>>();
        List<string> answers = new List<string>();
        bool isQuickEvt = false;
        List<KeyCode> keyCodes = null;
        float eventTime = 0;
        bool hasTimeLimit = false;
        string header = "";
        int contentCounter = 0;
        int fontSize = 14;
        string iWord = "";
        bool block = true;
        bool bg = true;
        for (int i = 0; i < data.Length; i++)
        {
            string[] lineValue = data[i].Split(new string[] { ">>"}, StringSplitOptions.None);

            string lineKey = string.Concat(lineValue[0], ">");
            string[] values;
            switch (lineKey)
            {
                case TOP_LEFT:
                    values = lineValue[1].Split(',');
                    topLeft.x = float.Parse(values[0].Trim());
                    topLeft.y = float.Parse(values[1].Trim());
                    if (!IsCornerVectorInRange(topLeft))
                        Debug.LogError("<color=red>The top left corner of dialogue: " + key + " is not between 0 and 1</color>");
                    break;
                case BOTTOM_RIGHT:
                    values = lineValue[1].Split(',');
                    bottomRight.x = float.Parse(values[0].Trim());
                    bottomRight.y = float.Parse(values[1].Trim());
                    if (!IsCornerVectorInRange(bottomRight))
                        Debug.LogError("<color=red>The bottom right corner of dialogue: " + key + " is not between 0 and 1</color>");
                    break;
                case ANSWER:
                    answers.Add(lineValue[1].Trim());
                    break;
                case HEADER:
                    header = lineValue[1].Trim();
                    break;
                case CONTENT_START:
                    contents.Add(new List<string>());
                    break;
                case CONTENT_END:
                    contentCounter++;
                    break;
                case QUICK_EVENT_KEY:
                    isQuickEvt = true;
                    if (keyCodes == null)
                        keyCodes = new List<KeyCode>();
                    values = lineValue[1].Split(',');

                    for (int j = 0; j < values.Length; j++)
                    {
                        KeyCode kc = (KeyCode)Enum.Parse(typeof(KeyCode), values[j]);
                        keyCodes.Add(kc);
                        if (kc != KeyCode.None)
                        {
                            if (j < 1)
                                contents[contentCounter].Add(string.Concat("<color=red><size=20><b>", values[j].Trim(), "</b></size></color>"));
                            else
                            {
                                int rowCount = contents[contentCounter].Count - 1;
                                contents[contentCounter][rowCount] += string.Concat("<color=red><size=20><b> + ", values[j].Trim(), "</b></size></color>");
                            }
                        }
                    }
                    break;
                case EVENT_TIME:
                    hasTimeLimit = float.TryParse(lineValue[1], out eventTime);
                    break;
                case FONT_SIZE:
                    fontSize = int.Parse(lineValue[1].Trim());
                    break;
                case INTERACTIVE:
                    iWord = lineValue[1].Trim();
                    break;
                case BLOCK:
                    block = bool.Parse(lineValue[1].Trim());
                    break;
                case BACKGROUND:
                    bg = bool.Parse(lineValue[1].Trim());
                    break;
                default:
                    contents[contentCounter].Add(data[i].Trim());
                    break;
            }
        }
        if(isQuickEvt)
        {
            if (hasTimeLimit)
                return new Dialogue(key, topLeft, bottomRight, header, contents, answers.ToArray(), block, bg ,fontSize, iWord, true, keyCodes, eventTime);
            else
                return new Dialogue(key, topLeft, bottomRight, header, contents, answers.ToArray(), block, bg ,fontSize, iWord , true, keyCodes, float.PositiveInfinity);
        }
            

        return new Dialogue(key, topLeft, bottomRight, header, contents, answers.ToArray(), block, bg ,fontSize, iWord);
    }
}
