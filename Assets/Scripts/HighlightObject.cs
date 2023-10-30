using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour, IMixedRealityFocusHandler
{
    public GameObject hovered = null;         
    //public GameObject hoverSelected;
    generateControlPoints controlPoints;

    public int hoverCounter = 0;
    private bool startCounter = false;

    public void Start()
    {
        controlPoints = GetComponent<generateControlPoints>();
    }
   
    public void OnFocusEnter(FocusEventData eventData)
    {
        print("colorchange");
        GetComponent<Renderer>().material.color = new Color(95 / 255f, 213 / 255f, 223 / 255f);
        
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
        
    }

    public void unselect()
    {
        
    }
}