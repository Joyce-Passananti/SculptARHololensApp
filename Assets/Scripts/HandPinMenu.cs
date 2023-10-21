using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.MixedReality.Toolkit.UI;
using Microsoft.MixedReality.Toolkit.Utilities.Solvers;
using UnityEngine;

public class HandPinMenu : MonoBehaviour
{
    public ObjectManipulator[] objectManipulators;
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
        foreach (var manipulator in objectManipulators)
        {
            if (manipulator != null)
            {
                manipulator.OnManipulationStarted.AddListener(HandleManipulationStarted);
            }
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
    private void HandleManipulationStarted(ManipulationEventData eventData)
    {
        menuSolverHandler.UpdateSolvers = false;
        pinned = true;
        // Your custom code here
        Debug.Log("Manipulation Ened");
    }
}
