using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ActionText : MonoBehaviour, IPointerEnterHandler
{
    public Text ActionTxt;
    public string ActionString;
    public void OnPointerEnter(PointerEventData eventData)
    {
        ActionTxt.text = ActionString;
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        ActionTxt.text = "";
    }
}
