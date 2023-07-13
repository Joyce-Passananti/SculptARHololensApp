using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;

public class generateControlPoints : MonoBehaviour
{
    GameObject sphere;

    public Vector3 position = new Vector3(0, 0, 0);
    public float radius;
    public float layerHeight;
    public float nbLayers;
    public int nbPoints; // points in layer

    public List<Vector3> path = new List<Vector3>();

    public float sphereSize;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        initialToolPath();

        lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
        lineRenderer.positionCount = 2;
        lineRenderer.useWorldSpace = true;

        //For drawing line in the world space, provide the x,y,z values
        lineRenderer.SetPosition(0, path[0]); //x,y and z position of the starting point of the line
        lineRenderer.SetPosition(1, path[1]); //x,y and z position of the end point of the line
    }

    // Update is called once per frame
    void Update()
    {

    }

    void initialToolPath()
    {
        // vectors = []
        for (int j = 0; j < nbLayers; j++)
        {
            for (int i = 0; i < nbPoints; i++)
            {
                float angle = 360 / nbPoints;
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3((position.x + radius * Mathf.Cos(i * angle * Mathf.PI / 180))*.01f, (position.z + layerHeight * j) * .01f, (position.y + radius * Mathf.Sin(i * angle * Mathf.PI / 180)) * .01f);
                sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
                addObjectComponents(sphere);
            }
        }
    }

    private void addObjectComponents(GameObject obj)
    {
        obj.AddComponent<ConstraintManager>();
        obj.AddComponent<Interactable>();
        obj.AddComponent<ObjectManipulator>();
        obj.GetComponent<ObjectManipulator>().OnManipulationStarted.AddListener(HandleOnManipulationStarted);
        // obj.GetComponent<Interactable>().OnClick.AddListener(selected.selectObject(obj));

    }
    private void HandleOnManipulationStarted(ManipulationEventData eventData)
    {
        // selected.selectObject(eventData.ManipulationSource);
    }

}
