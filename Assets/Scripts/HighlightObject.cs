using Microsoft.MixedReality.Toolkit.Input;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightObject : MonoBehaviour, IMixedRealityFocusHandler
{

    public void OnFocusEnter(FocusEventData eventData)
    {
        GetComponent<Renderer>().material.color = new Color(95 / 255f, 213 / 255f, 223 / 255f);
    }

    public void OnFocusExit(FocusEventData eventData)
    {
        GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f);
    }
}
