using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuest : Quest
{
    public enum TutorialActionType { Coat, Chart, Gun, DoctorSearch}

    private bool _interactedWithCoat;
    private bool _interactedWithGun;
    private bool _interactedWithChart;
    private bool _searchedDoctorInDB;

    private const string DOCTOR_NAME = "DoctorQuintonRhyne";
    public const string COAT = "Coat1";
    public const string GUN = "Gun";
    public const string CHART = "MedicalChart1";

    public override string Name => "TutorialQuest";

    public override bool IsComplete()
    {
        return _interactedWithCoat &&
                _interactedWithChart &&
                _interactedWithGun &&
                _searchedDoctorInDB;
    }

    public override void OnStartQuest()
    {
        _interactedWithCoat = false;
        _interactedWithChart = false;
        _interactedWithGun = false;
        _searchedDoctorInDB = false;
        MessageBox.OnDialogueStatusUpdate += HandleDialogueStatusUpdate;
        ArticleBox.OnCloseArticle += HandleArticleClosed;
    }

    private void HandleDialogueStatusUpdate(string dialogueName, MessageBox.DialogueStatus dialogueStatus, bool block)
    {
        switch (dialogueName)
        {
            case COAT:
                if(dialogueStatus == MessageBox.DialogueStatus.Ended)
                {
                    _interactedWithCoat = true;
                }
                break;
            case CHART:
                if (dialogueStatus == MessageBox.DialogueStatus.Ended)
                {
                    _interactedWithChart = true;
                }
                break;
            case GUN:
                if (dialogueStatus == MessageBox.DialogueStatus.Ended)
                {
                    _interactedWithGun = true;
                }
                break;
            default:
                break;
        }
        if(IsComplete())
        {
            QuestFinished(this);
        }
    }

    private void HandleArticleClosed(string articleName)
    {
        if(articleName.Equals(DOCTOR_NAME))
        {
            _searchedDoctorInDB = true;
        }
        if(IsComplete())
        {
            QuestFinished(this);
        }
    }

    public override void OnEndQuest()
    {
        MessageBox.OnDialogueStatusUpdate -= HandleDialogueStatusUpdate;
        ArticleBox.OnCloseArticle -= HandleArticleClosed;
    }
}
