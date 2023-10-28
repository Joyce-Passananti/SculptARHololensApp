using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightPlane : MonoBehaviour, IMixedRealityFocusHandler
{

    public GameObject plane;
    public GameObject sceneManager;
    generateControlPoints controlPoints;
    public void Start()
    {
        controlPoints = sceneManager.GetComponent<generateControlPoints>();
    }

    public void OnFocusEnter(FocusEventData eventData)
    {
        plane.GetComponent<Renderer>().material.color = new Color(95 / 255f, 213 / 255f, 223 / 255f);
        controlPoints.cube.GetComponent<ObjectManipulator>().enabled = true;
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        plane.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
        controlPoints.cube.GetComponent<ObjectManipulator>().enabled = false ;
    }
}
