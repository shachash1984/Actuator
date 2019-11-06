using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    private void OnEnable()
    {
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= HandleSceneLoaded;
    }

    private void HandleSceneLoaded(Scene s, LoadSceneMode lsm)
    {
        if(s.buildIndex == 1)
        {
            Scene2Manager s2m = GetComponent<Scene2Manager>();
            if (s2m)
                Destroy(s2m);
            Scene1Manager s1m = GetComponent<Scene1Manager>();
            if(!s1m)
            {
                s1m = gameObject.AddComponent<Scene1Manager>();
            }
        }
        else if(s.buildIndex == 2)
        {
            Scene1Manager s1m = GetComponent<Scene1Manager>();
            if (s1m)
                Destroy(s1m);
            Scene2Manager s2m = GetComponent<Scene2Manager>();
            if (!s2m)
            {
                s2m = gameObject.AddComponent<Scene2Manager>();
            }

            UIHandler.S.ToggleBlackScreen(true, 0);
            UIHandler.S.ToggleBlackScreen(false, 5);
        }
    }
}
