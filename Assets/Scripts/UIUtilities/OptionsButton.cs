using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OptionsButton : MonoBehaviour
{
    private Button _button;
    private Image _image;
    public Sprite selectedSprite;
    public Sprite unSelectedSprite;
    private Image _notificationImage;

    private void Start()
    {
        _button = GetComponent<Button>();
        _image = GetComponent<Image>();
        _notificationImage = transform.GetChild(0).GetComponent<Image>();
        ToggleButtonSpriteChange(false);
        ToggleNewNotificationEffect(false);
    }

    private void OnEnable()
    {
        UIHandler.OnNotificationStatusChanged += ToggleNewNotificationEffect;
    }

    private void OnDisable()
    {
        UIHandler.OnNotificationStatusChanged -= ToggleNewNotificationEffect;
    }

    public void ToggleButtonSpriteChange(bool on)
    {
        if (on)
        {
            _image.sprite = selectedSprite;
        }
        else
        {
            _image.sprite = unSelectedSprite;
        }
    }

    private void ToggleNewNotificationEffect(bool on)
    {
        if(on)
        {
            _notificationImage.DOFade(0, 0).OnComplete(() => 
            {
                
            });
            _notificationImage.DOFade(1, 0.3f).SetLoops(-1, LoopType.Yoyo);
        }
        else
        {
            _notificationImage.DOKill(true);
            _notificationImage.DOFade(0, 0.3f);
        }
    }
}
