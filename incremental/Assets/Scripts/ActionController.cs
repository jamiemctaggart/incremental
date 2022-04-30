using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ActionController : MonoBehaviour
{
    public TextMeshProUGUI currentActionText;

    public void FoodFocus()
    {
        Debug.Log("FoodFocusButton pressed"); //TODO remove, only for testing purposes
        currentActionText.text = "Current Action: Food Focus";

    }
}
