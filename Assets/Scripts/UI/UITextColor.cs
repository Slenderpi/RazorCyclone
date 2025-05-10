using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITextColor : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] TMP_Text targetText;
    [SerializeField] Button button;
    [Header("Colors")]
    [SerializeField] Color defaultColor;
    [SerializeField] Color highlightedColor;
    [SerializeField] Color pressedColor;
    [SerializeField] Color selectedColor;
    [SerializeField] Color disabledColor;
    UnityEngine.UI.Selectable.SelectionState buttonStates;

    void Update()
    {
        buttonStates = button.currentSelectionState;
    }
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
