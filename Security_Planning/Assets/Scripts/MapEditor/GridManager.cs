using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{

    public InputField InputWidth;
    public InputField InputHeight;
    public GameObject Grid;
    public GameObject Panel;
    public GameObject PanelStart;
    public GameObject Tiles;

    private GameObject[,] GridObjects;
    private UnityEngine.Object[] allResources;
    private GameObject HoveredObject;

    private int width;
    private int height;
    private bool replacePhase;

	// Use this for initialization
	private void Start ()
    {
        allResources = Resources.LoadAll("Prefabs/");
        width = 10;
        height = 10;
        InputWidth.text = width.ToString();
        InputHeight.text = height.ToString();
        InputWidth.onValidateInput += NumberValidationFunction;
        InputHeight.onValidateInput += NumberValidationFunction;
        GridObjects = new GameObject[height, width];
        UnityEngine.Object emptySquare = allResources.FindByName("Ground0");
        for (int i = 0; i < height; i++) 
            for (int j = 0; j < width; j++)
            {
                GameObject newObject = Instantiate(emptySquare, transform) as GameObject;
                GridObjects[i, j] = newObject;
                newObject.transform.position = new Vector3(j - width / 2, i - height / 2, 0);
                newObject.AddComponent<BoxCollider>();
            }
	}
	
	// Update is called once per frame
	private void Update ()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (scroll != 0)
        {
            ScrollLogic(scroll, ray);
        } 
        else if (Input.GetMouseButtonUp(0))
        {
            LeftButtonUpLogic(ray);
        }
        else
        {
            HoverLogic(ray);
        }
    }

    private void LeftButtonUpLogic(Ray ray)
    {

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject HitObject = hit.transform.gameObject;
            if (replacePhase)
            {
                if (HitsChildOf(PanelStart, HitObject))
                {
                    UnityEngine.Object item = allResources.FindByName(HitObject.name);
                    GameObject newObject = Instantiate(item) as GameObject;
                    newObject.name = item.name;
                }
            }
            else
            {
                HoverEnded();
                
                Debug.Log("Mouse button up");
                HitObject.GetComponent<Renderer>().material.color = Color.red;
                replacePhase = true;

                Panel.SetActive(true);
                Vector3 currentPosition = PanelStart.transform.position;
                foreach (UnityEngine.Object item in allResources)
                {
                    GameObject newObject = Instantiate(item) as GameObject;
                    newObject.transform.position = currentPosition;
                    newObject.transform.parent = Tiles.transform;
                    newObject.transform.localScale *= 4;
                    newObject.name = item.name;
                    currentPosition.x += 8;
                }
            }
        }
    }

    private void HoverLogic(Ray ray)
    {
        if (replacePhase)
        {

        }
        else
        {
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                GameObject HitObject = hit.transform.gameObject;
                if (HitObject != HoveredObject)
                {
                    HoverEnded();
                    if (HitsChildOf(Grid, HitObject))
                    {
                        Debug.Log("Mouse hovering");
                        HoveredObject = HitObject;
                        HoveredObject.GetComponent<Renderer>().material.color = Color.green;
                    }
                }

            }
            else
            {
                HoverEnded();
                HoveredObject = null;
            }
        }
        
    }

    private void HoverEnded()
    {
        if (HoveredObject != null)
        {
            Debug.Log("Hover ended");
            HoveredObject.GetComponent<Renderer>().material.color = Color.white;
        }
    }

    private bool HitsChildOf(GameObject gameObject, GameObject hitObject)
    {
        foreach (Transform transform in gameObject.transform)
        {
            if (transform.gameObject == hitObject)
            {
                return true;
            }
        }
        return false;
    }

    private void ScrollLogic(float scroll, Ray ray)
    {
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Tiles.transform.position += new Vector3(25 * scroll, 0, 0);
        }
    }

    private char NumberValidationFunction(string text, int charIndex, char addedChar)
    {
        if (text.Length < 2 && addedChar >= '0' && addedChar <= '9')
        {
            return addedChar;
        }
        return '\0';
    }

    public void OnWidthChanged()
    {
        width = int.Parse(InputWidth.text);
    }

    public void OnHeightChanged()
    {
        height = int.Parse(InputHeight.text);
    }
}
