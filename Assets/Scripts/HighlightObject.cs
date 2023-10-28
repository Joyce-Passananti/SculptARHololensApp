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
    public void Update()
    {
        if (startCounter)
        {
            hoverCounter++;
            print(hoverCounter);
        }

        // turn red after 5 seconds
        if (hoverCounter > 200 && hovered)                                                
        {
            hovered.GetComponent<Renderer>().material.color = new Color(0f, 1f, 0f);
            generateControlPoints.instance.hoverSelected = hovered;
            hovered.GetComponent<ObjectManipulator>().enabled = true;
        }

    }
    public void OnFocusEnter(FocusEventData eventData)
    {
        print("colorchange");
        GetComponent<Renderer>().material.color = new Color(95 / 255f, 213 / 255f, 223 / 255f);
        if (hovered == GetComponent<Renderer>().gameObject)
        {
            startCounter = true;
            print("keep hover");
        }
        else
        {
            print("Start hover");
            hovered = GetComponent<Renderer>().gameObject;
            hoverCounter = 0;
            startCounter = true;
        }
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);

        hovered.GetComponent<ObjectManipulator>().enabled = false;
        hovered = null;
        if (controlPoints)
            controlPoints.hoverSelected = null;
        hoverCounter = 0;
        startCounter = false;
    }
}