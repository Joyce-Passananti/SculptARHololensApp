using Microsoft.MixedReality.Toolkit.Input;
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
    public void Update()
    {
        if (startCounter)
            hoverCounter++;
        // turn red after 5 seconds
        if (hoverCounter > 500 && hovered)
            controlPoints.hoverSelected.GetComponent<Renderer>().material.color = new Color(1f, 0f, 0f);
    }
    public void OnFocusEnter(FocusEventData eventData)
    {
        GetComponent<Renderer>().material.color = new Color(95 / 255f, 213 / 255f, 223 / 255f);
        if (hovered == GetComponent<GameObject>())
            startCounter = true;
        else
        {
            hovered = GetComponent<GameObject>();
            hoverCounter = 0;
            startCounter = false;
        }
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
        hovered = null;
        hoverCounter = 0;
        startCounter = false;
    }
}
