using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;
using UnityEngine.UI;

public class ChooseFile : MonoBehaviour {

    public InputField MapInputField;

    public void ChooseMapFile()
    {
        OpenFileDialog dialog = new OpenFileDialog();
        DialogResult dialogResult = dialog.ShowDialog();
        if (dialogResult == DialogResult.OK)
        {
            string fileName = dialog.FileName;
            MapInputField.text = fileName;
        }
    }
}
