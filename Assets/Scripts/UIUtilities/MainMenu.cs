#pragma warning disable 0649
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour
{
    AudioSource _audioSource;
    [SerializeField] private AudioMixer _masterVolume;

    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }
    public void NewGame()
    {
        _audioSource.Play();
        SceneManager.LoadScene(1);
        
    }

    public void QuitGame()
    {
        _audioSource.Play();
        StartCoroutine(QuitGameCoroutine());
    }

    IEnumerator QuitGameCoroutine()
    {
        yield return new WaitUntil(() => !_audioSource.isPlaying);
        Application.Quit();
    }

    public void SetVolume(float volume)
    {
        _masterVolume.SetFloat("volume", volume);
    }
}
