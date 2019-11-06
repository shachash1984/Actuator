using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class FadeIn : MonoBehaviour
{
    public Image _imageRef;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            ToggleImage(true);
        }

        if (Input.GetKeyDown(KeyCode.Y))
        {
            ToggleImage(false);
        }
    }

    public void ToggleImage(bool boolRef)
    {
        if (boolRef)
        {
            //_imageRef.DOFade(1, 1);
            //_imageRef.transform
        }
        else
        {
            //_imageRef.DOFade(0, 1);
        }
    }
}
