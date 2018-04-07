using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextLoading : MonoBehaviour
{
    private Text text;
    private int dotsToAdd = 3;
    private float timer;

    private void Start()
    {
        text = GetComponent<Text>();
    }
	
	// Update is called once per frame
	void Update ()
	{
	    timer += Time.deltaTime;
	    if (timer > 0.25f)
	    {
	        timer = 0;
	        if (dotsToAdd > 0)
	        {
	            dotsToAdd--;
	            text.text += ".";
	        }
	        else
	        {
	            dotsToAdd = 3;
	            text.text = text.text.Substring(0, text.text.Length - 3);
	        }
	    }
	}
}
