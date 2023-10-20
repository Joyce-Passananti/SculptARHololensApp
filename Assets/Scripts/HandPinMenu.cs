using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class HandPinMenu : MonoBehaviour
{
    public ObjectManipulator objectManipulator;
    private SolverHandler menuSolverHandler;
    public GameObject menuContent;

    private bool pinned;

    private void Awake()
    {
        menuSolverHandler = GetComponent<SolverHandler>();
    }

    // Start is called before the first frame update
    void Start()
    {
        pinned = false;
        if (objectManipulator != null)
        {
            objectManipulator.OnManipulationEnded.AddListener(HandleManipulationEnded);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CloseMenu()
    {
        pinned = false;
        menuContent.SetActive(false);
        menuSolverHandler.UpdateSolvers = true;
    }

    public void OpenMenu()
    {
        pinned = false;
        menuContent.SetActive(true);
        menuSolverHandler.UpdateSolvers = true;
    }

    public void HandLost()
    {
        if (!pinned)
        {
            menuContent.SetActive(false);
        }
    }

    public void SwitchHandMenu()
    {
        if (!pinned)
        {
            menuContent.SetActive(false);
        } 
    }
    private void HandleManipulationEnded(ManipulationEventData eventData)
    {
        menuSolverHandler.UpdateSolvers = false;
        pinned = true;
        // Your custom code here
        Debug.Log("Manipulation Ened");
    }
}
