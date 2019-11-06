using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodePadQuest : Quest
{
    public override string Name => "Code Quest";
    [SerializeField] private const int CODE = 11010;
    public int CodeAttempt { get; set; }

    public override bool IsComplete()
    {
        return CodeAttempt == CODE;
    }

    public override void OnEndQuest()
    {
        
    }

    public override void OnStartQuest()
    {

    }

    public bool CheckCode(int _codeAttempt)
    {
        CodeAttempt = _codeAttempt;
        if (IsComplete())
        {
            QuestFinished(this);
            return true;
            //Debug.Log("Correct Code");
        }
        return false;
        //Debug.Log("Wrong Code");
    }
}
