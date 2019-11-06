using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class Quest 
{
    public abstract bool IsComplete();
    public abstract void OnStartQuest();
    public abstract void OnEndQuest();
    public static event Action<Quest> OnQuestFinished;
    public abstract string Name { get; }

    protected virtual void QuestFinished(Quest q)
    {
        OnQuestFinished?.Invoke(q);
    }
}
