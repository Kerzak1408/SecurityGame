﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    public const string MAP_EDITOR = "MapEditor";
    public const string MAIN_MENU = "MainMenu";
    public const string MAIN_SCENE = "MainScene";
    public const string MAP_CHOICE_SCREEN = "MapChoiceScreen";

    private void Start()
    {
        Application.targetFrameRate = 120;
    }

    public static Dictionary<string, string> Parameters { get; private set; }
    public static Dictionary<string, object> ObjectParameters { get; private set; }

    public static void Load(string sceneName, Dictionary<string, object> objectParameters)
    {
        ObjectParameters = objectParameters;
        SceneManager.LoadScene(sceneName);
    }

    public static void Load(string sceneName, Dictionary<string, string> parameters = null)
    {
        Parameters = parameters;
        SceneManager.LoadScene(sceneName);
    }

    public static void Load(string sceneName, string paramKey, string paramValue)
    {
        Parameters = new Dictionary<string, string>();
        Parameters.Add(paramKey, paramValue);
        SceneManager.LoadScene(sceneName);
    }

    public static string GetParam(string paramKey)
    {
        if (Parameters == null) return "";
        return Parameters[paramKey];
    }

    public static object GetObjectParam(string paramKey)
    {
        if (ObjectParameters == null) return null;
        return ObjectParameters[paramKey];
    }

    private Scenes()
    {
        
    }

    public void LoadEditor()
    {
        Load(MAP_EDITOR);
    }

    public void LoadMainMenu()
    {
        Load(MAIN_MENU);
    }

    public void LoadMapChoiceScreen()
    {
        Load(MAP_CHOICE_SCREEN);
    }

    public static bool IsCurrentScene(string sceneName)
    {
        return SceneManager.GetActiveScene().name == sceneName;
    }
}