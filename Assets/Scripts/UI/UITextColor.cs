using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UITextColor : MonoBehaviour
{
    [Header("Text")]
    [SerializeField] TMP_Text targetText;
    [Header("Colors")]
    [SerializeField] Color defaultColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color pressedColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Color disabledColor;
    public void setDefault(){
        targetText.color = defaultColor;
    }
    public void setHighlighted(){
        targetText.color = highlightedColor;
    }
    public void setPressed(){
        targetText.color = pressedColor;
    }
    public void setSelected(){
        targetText.color = selectedColor;
    }
    public void setDisabled(){
        targetText.color = disabledColor;
    }
}
