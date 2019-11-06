using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;

public class MessageBox : MonoBehaviour {
    
    public enum DialogueStatus { None = 0, Ended = 10, Continued = 11, QuickEventHit = 12, QuickEventMissed = 13 ,Answer1 = 1, Answer2 = 2, Answer3 = 3, Answer4 = 4}
    public const string ACTIVE_TAG = "ActiveMessage";
    public const string INACTIVE_TAG = "InactiveMessage";
    public const int LINE_HEIGHT = 16;
    public const float NEXT_BUTTON_X_PADDING = 40;
    public const float NEXT_BUTTON_Y_PADDING = 40;
    public const float NEXT_BUTTON_WIDTH = 42;
    public const float NEXT_BUTTON_HEIGHT = 42;
    public const int END_DIALOGUE = 0;
    public const int CONTINUE_DIALOGUE = -1;
    public const int QUICK_EVENT_HIT = 10;
    public const int QUICK_EVENT_MISSED = -10;
    public const float DISMISS_DELAY = 0.2f;
    public int currentContent;
    public Dialogue dialogue;
    public Text headerText;
    public Text[] contentText;
    public Button nextButton;
    public Button[] answers;
    public Font font;
    public int fontSize;
    public static event Action<string, int, bool> OnButtonClicked;
    public static event Action<string, DialogueStatus, bool> OnDialogueStatusUpdate;
    protected EventSystem _eventSystem;
    private static int s_openMessageBoxCounter;

    private void Awake()
    {
        ++s_openMessageBoxCounter;
        _eventSystem = FindObjectOfType<EventSystem>();
        
    }
    private void OnDestroy()
    {
        s_openMessageBoxCounter--;
    }
    public static int GetOpenMessagesCount()
    {
        return s_openMessageBoxCounter;
    }
    public void UpdateNextButton()
    {
        if (nextButton != null)
        {
            nextButton.onClick.RemoveAllListeners();
            _eventSystem.SetSelectedGameObject(nextButton.gameObject, new BaseEventData(_eventSystem));
            bool clickedOnce = false;
            nextButton.onClick.AddListener(() =>
            {
                if(!clickedOnce && !UIHandler.S.IsTweening())
                {
                    if (currentContent == dialogue.contents.Count - 1)
                    {
                        this.tag = INACTIVE_TAG;
                        UIHandler.S.DismissMessage(DISMISS_DELAY);
                        OnButtonClicked?.Invoke(dialogue.GetName(), END_DIALOGUE, dialogue.blockMovement);
                        OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.Ended, dialogue.blockMovement);
                    }
                    else
                    {
                        UIHandler.S.NextText();
                        OnButtonClicked?.Invoke(dialogue.GetName(), CONTINUE_DIALOGUE, true);
                        OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.Continued, true);
                    }
                    clickedOnce = true;
                }
            });
        }
    }
    public void InitQuickEvent()
    {
        if (dialogue.isQuickEvent)
            StartCoroutine(ListenForKey(dialogue.quickEventKeys, dialogue.quickEventTime));
    }
    public void InitAnswerButtons()
    {
        if(answers != null)
        {
            _eventSystem.SetSelectedGameObject(null, null);
            
            foreach (Button button in answers)
            {
                Text t = button.transform.GetChild(0).GetComponent<Text>();
                t.gameObject.AddComponent<AnswerButton>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() =>
                {
                    if (OnButtonClicked != null)
                    {
                        int num = int.Parse(button.name[button.name.Length - 1].ToString());
                        OnButtonClicked(dialogue.GetName(), num + 1, dialogue.blockMovement);
                        switch (num)
                        {
                            case 0:
                                OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.Answer1, dialogue.blockMovement);
                                break;
                            case 1:
                                OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.Answer2, dialogue.blockMovement);
                                break;
                            case 2:
                                OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.Answer3, dialogue.blockMovement);
                                break;
                            case 3:
                                OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.Answer4, dialogue.blockMovement);
                                break;
                            default:
                                break;
                        }
                    }

                    this.GetComponent<CanvasGroup>().interactable = false;
                    t.color = Color.white;
                    this.tag = INACTIVE_TAG;
                    UIHandler.S.DismissMessage(DISMISS_DELAY);
                });
            }
        }
    }
    IEnumerator ListenForKey(List<KeyCode> keyCodes, float time)
    {
        _eventSystem.SetSelectedGameObject(null, null);
        float initialTime = time;
        if (time <= 0)
        {
            yield return new WaitUntil(() =>
            {
                int keyCounter = 0;
                for (int i = 0; i < keyCodes.Count; i++)
                {
                    if(Input.GetKeyDown(keyCodes[i]))
                    {
                        keyCounter++;
                    }
                }
                return keyCounter == keyCodes.Count;
            });
            this.tag = INACTIVE_TAG;
            UIHandler.S.DismissMessage(DISMISS_DELAY);
            OnButtonClicked?.Invoke(dialogue.GetName(), END_DIALOGUE, dialogue.blockMovement);
            OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.Ended, dialogue.blockMovement);
        }
        else
        {
            int keyCounter = 0;
            while (keyCounter != keyCodes.Count)
            {
                if (time <= 0)
                    break;
                if (keyCounter < keyCodes.Count)
                    keyCounter = 0;
                for (int i = 0; i < keyCodes.Count; i++)
                {
                    if (Input.GetKey(keyCodes[i]))
                    {
                        keyCounter++;
                    }
                }
                time -= Time.deltaTime;
                if (keyCodes[0] != KeyCode.None)
                    UIHandler.UpdateTimer(time / initialTime);
                yield return new WaitForEndOfFrame();
            }
            this.tag = INACTIVE_TAG;
            UIHandler.S.DismissMessage(DISMISS_DELAY);
            if(time > 0)
            {
                OnButtonClicked?.Invoke(dialogue.GetName(), QUICK_EVENT_HIT, dialogue.blockMovement);
                OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.QuickEventHit, dialogue.blockMovement);
            }
            else
            {
                OnButtonClicked?.Invoke(dialogue.GetName(), QUICK_EVENT_MISSED, dialogue.blockMovement);
                OnDialogueStatusUpdate?.Invoke(dialogue.GetName(), DialogueStatus.QuickEventMissed, dialogue.blockMovement);
            }
                
        }
    }
}
