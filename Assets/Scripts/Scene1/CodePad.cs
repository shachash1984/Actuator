#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class CodePad : MonoBehaviour
{
    [SerializeField] private Text[] _digits;
    private int _currentDigit = 0;
    private CodePadQuest _codePadQuest;

    private void OnEnable()
    {
        ClearScreen();
    }

    private void OnDisable()
    {
        Quest.OnQuestFinished -= HandleQuestFinished;
    }

    public void Init(Quest q)
    {
        _codePadQuest = q as CodePadQuest;
        Quest.OnQuestFinished += HandleQuestFinished;
        ClearScreen();
    }

    private void HandleQuestFinished(Quest q)
    {
        StartCoroutine(QuestFinishedCoroutine(q));
    }

    private IEnumerator QuestFinishedCoroutine(Quest q)
    {
        if(q == _codePadQuest)
        {
            //paint digits green
            foreach (Text t in _digits)
            {
                t.DOColor(Color.green, 0.2f);
            }
            //Play correct sound
            
            yield return new WaitForSeconds(1);
            //Tween to size 0
            transform.DOScale(0, 1f).OnComplete(() => Destroy(gameObject));
        }
    }

    public void ClearScreen()
    {
        foreach (Text t in _digits)
        {
            t.DOFade(0, 0);
        }
        _currentDigit = 0;
    }

    public void EnterDigit(int num)
    {
        if(_currentDigit < _digits.Length)
        {
            _digits[_currentDigit].text = num.ToString();
            _digits[_currentDigit].DOFade(1, 0);
            _currentDigit++;
        }
    }

    public void Delete()
    {
        if(_currentDigit >= 0)
        {
            _currentDigit--;
            _digits[_currentDigit].DOFade(0, 0);
        }
    }

    public void EnterCode()
    {
        int code = 0;
        int mult = 1;
        for (int i = _digits.Length -1; i >= 0; i--)
        {
            code += int.Parse(_digits[i].text) * mult;
            mult *= 10;
        }
        //Debug.Log("code: " + code);
        if(!_codePadQuest.CheckCode(code))
        {
            foreach (Text t in _digits)
            {
                t.DOColor(Color.red, 0.2f);
            }
            
            transform.DOShakePosition(0.5f, 10, 40).OnComplete(() => 
            {
                ClearScreen();
                Color clearWhite = new Color(1, 1, 1, 0);
                foreach (Text t in _digits)
                {
                    t.DOColor(clearWhite, 0);
                }
            });
            
        }
    }
}
