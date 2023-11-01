using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class AutoSizeTextField : MonoBehaviour
{
    public Vector2 offset;

    public TMP_Text textField;    

    private void Start()
    {        
        Vector2 textSize = textField.GetPreferredValues(textField.text);
        textField.rectTransform.sizeDelta = textSize + offset;
    }
}
