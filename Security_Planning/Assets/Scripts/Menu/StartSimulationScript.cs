using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSimulationScript : MonoBehaviour {

    public InputField MapInputField;

    public void StartSimulation()
    {
        string path = MapInputField.text;
        if (path == "")
        {
            MessageBox.Show("You did not specify any map file path.");
        }
        else if (!File.Exists(path))
        {
            MessageBox.Show("The file you entered does not exist.");
        }
        else
        {
            SceneManager.LoadScene("MainScene");
        }
    }
}
