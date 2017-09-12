using UnityEngine;
using System.Collections;
using System.Xml.Serialization;
using System.IO;
using System.Collections.Generic;

public class Main : MonoBehaviour {

    public GameObject Canvas;

    GameManager gameManager;
    char[][] gridChars;

	// Use this for initialization
	void Start () {
        gameManager = GameManager.Instance;
        InitializeModel();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void InitializeModel()
    {
        LoadModel();
        LoadPrefabs();
    }

    private void LoadModel()
    {
        List<string> lines = new List<string>();
        string line;
        System.IO.StreamReader file = new StreamReader(gameManager.PathToGridFile);
        while ((line = file.ReadLine()) != null)
        {
            lines.Add(line);
        }
        gridChars = new char[lines.Count][];
        int counter = 0;
        foreach (string row in lines)
        {
            gridChars[counter] = row.ToCharArray();
            counter++;
        }
        Debug.Log(gridChars);
        file.Close();
    }

    private void LoadPrefabs()
    {
        Vector3 canvasLocalScale = Canvas.transform.localScale;
        int xScale = gridChars[0].Length;
        int yScale = gridChars.Length;
        Canvas.transform.localScale = new Vector3(canvasLocalScale.x * xScale, canvasLocalScale.y * yScale, canvasLocalScale.z);
        for (int j = 0; j < gridChars.Length; j++)
        {
            for (int i = 0; i < gridChars[j].Length; i++)
            {
                GameObject newObject = null;
                switch (gridChars[j][i])
                {
                    case '|':
                    {
                        newObject = Instantiate(Resources.Load("Prefabs/" + "WallVertical", typeof(GameObject))) as GameObject;
                    } break;
                    case '-':
                    {
                        newObject = Instantiate(Resources.Load("Prefabs/" + "WallHorizontal", typeof(GameObject))) as GameObject;
                    }
                    break;
                }
                if (newObject != null)
                {
                    newObject.transform.position = new Vector3(i - xScale/2, j - yScale/2, -1);
                }
            }
        }
    }
}
