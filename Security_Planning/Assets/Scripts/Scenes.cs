﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Scenes : MonoBehaviour
{
    private const string MAP_EDITOR = "MapEditor";
    private const string MAIN_MENU = "MainMenu";
    private const string MAIN_SCENE = "MainScene";

    private static Dictionary<string, string> Parameters { get; set; }

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

    private Scenes()
    {
        
    }

    public void LoadEditor()
    {
        Load(MAP_EDITOR);
    }
}