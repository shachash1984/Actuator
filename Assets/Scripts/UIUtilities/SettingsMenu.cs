#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using DG.Tweening;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private Button _settingsButton;
    [SerializeField] private GameObject _settingsPanel;
    private bool _isOpen;

    [SerializeField] private AudioMixer _masterVolume;

    private void Start()
    {
        _isOpen = false;
        ToggleSettingsPanel();
    }

    public void ToggleSettingsPanel()
    {
        _settingsPanel.SetActive(_isOpen);
        _isOpen = !_isOpen;
    }

    public void SetVolume(float volume)
    {
        _masterVolume.SetFloat("volume", volume);
    }

    public void SetFullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
}
