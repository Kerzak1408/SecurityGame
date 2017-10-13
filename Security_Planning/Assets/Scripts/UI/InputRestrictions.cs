using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InputRestrictions : MonoBehaviour
{
	void Start ()
    {
        GetComponent<InputField>().onValidateInput += NumberValidation;
	}

    public char NumberValidation(string text, int charIndex, char addedChar) 
    {
        if (addedChar >= '0' && addedChar <= '9')
        {
            return addedChar;
        }
        return '\0';
    }
}
