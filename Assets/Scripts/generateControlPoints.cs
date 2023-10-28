using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;
using System.Reflection;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class generateControlPoints : MonoBehaviour
{
    FieldInfo[] fields;
    public static generateControlPoints instance;

    // initial coil parameters
    public GameObject cube;
    public Vector3 pos;
    public float radius;
    public float layerHeight;
    public int nbLayers;
    public int nbPoints; // points in layer

    private GameObject sphere;
    public float sphereSize;
    public List<GameObject> path = new List<GameObject>();
    public List<List<Vector3>> oldPath;

    // manipulation params
    public float brushSizeHeight;
    public float brushSizeWidth;
    public string brushStyle;
    public string manipulationType;

    public GameObject selected = null;
    public GameObject hoverSelected;

    private Vector3 initialSelPos;
    private Boolean updatePath = false;

    private LineRenderer lineRenderer;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        hoverSelected = null;

        Type scriptClass = this.GetType();
        fields = scriptClass.GetFields();

        lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        //Material lineMaterial = new Material(Shader.Find("Particles/Standard Unlit"));
        //lineRenderer.material = lineMaterial;
        //Color newColor = new Color(95 / 255f, 223 / 255f, 194 / 255f);
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.001f;
        lineRenderer.endWidth = 0.001f;
        lineRenderer.useWorldSpace = true;

        cube.GetComponent<ObjectManipulator>().OnManipulationEnded.AddListener(x => {
            cube.transform.rotation = Quaternion.identity;
            cube.transform.GetChild(0).transform.rotation = Quaternion.identity;
            drawToolpath(); });
        cube.GetComponent<ObjectManipulator>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (updatePath)
        {
            drawToolpath();
        }
    }

    public void initialToolPath()
    {
        path.ForEach(x => { Destroy(x); });
        path.Clear();
        pos = Vector3.zero;

        // vectors = []
        for (int j = 0; j < nbLayers; j++)
        {
            for (int i = 0; i < nbPoints; i++)
            {
                float angle = 360 / nbPoints;
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.SetParent(cube.transform);
                sphere.transform.localPosition = new Vector3(pos.x + (radius * Mathf.Cos(i * angle * Mathf.PI / 180)) * .01f, pos.y + (layerHeight * j) * .01f, pos.z + (radius * Mathf.Sin(i * angle * Mathf.PI / 180)) * .01f);
                sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
                addObjectComponents(sphere);
                path.Add(sphere);
            }
        }
        oldPath = new List<List<Vector3>>();
        oldPath.Add(savePath(path));
        drawToolpath();
    }

    public void drawToolpath()
    {
        lineRenderer.positionCount = path.Count;

        for(int i = 0; i<path.Count; i++)
        {
            lineRenderer.SetPosition(i, path[i].transform.position); //x,y and z position of the starting point of the line
        }
    }

    public void updateParams(string param, object value)
    {
        foreach (var field in fields)
        {
            if (field.Name == param)
            {
                if(param == "layerHeight")
                {
                    float oldLH = layerHeight;
                    field.SetValue(this, value);
                    foreach(GameObject p in path)
                    {
                        p.transform.localPosition = new Vector3(p.transform.localPosition.x, p.transform.localPosition.y * (layerHeight/oldLH), p.transform.localPosition.z);
                    }
                    drawToolpath();
                } 
                else
                    field.SetValue(this, value);
            }
        }
    }


    private void addObjectComponents(GameObject obj)
    {
        obj.AddComponent<ConstraintManager>();
        obj.AddComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>();

        obj.AddComponent<ObjectManipulator>();
        obj.GetComponent<ObjectManipulator>().AllowFarManipulation = true;
        obj.GetComponent<ObjectManipulator>().OnManipulationStarted.AddListener(HandleOnManipulationStarted);
        obj.GetComponent<ObjectManipulator>().OnManipulationEnded.AddListener(HandleOnManipulationEnded);

        obj.AddComponent<HighlightObject>();

        //obj.GetComponent<ObjectManipulator>().enabled = false;
    }
    private void HandleOnManipulationStarted(ManipulationEventData eventData) 
    {
        oldPath.Add(savePath(path));
        //if (hoverSelected)
        //{
        //    //hoverSelected.GetComponent<ObjectManipulator>().AllowFarManipulation = true;
        //    selectObject(hoverSelected, true);
        //}
        //else
        selectObject(eventData.ManipulationSource, true);
    }

    private void HandleOnManipulationEnded(ManipulationEventData eventData)
    {
        //if (hoverSelected)
        //{
        //    selectObject(hoverSelected, false);
        //    hoverSelected.GetComponent<ObjectManipulator>().enabled = false;
        //}
        //else
        selectObject(eventData.ManipulationSource, false);
    }

    private void selectObject(GameObject sel, Boolean status)
    {
        if (status)
        {
            // save initial position
            initialSelPos = sel.transform.localPosition;

            // update toolpath with new position
            this.selected = sel;
            updatePath = true;
        }
        else
        {
            // correct y position to initial y position
            sel.transform.localPosition = new Vector3(sel.transform.localPosition.x, initialSelPos.y, sel.transform.localPosition.z);
            manipulate();
            drawToolpath();

            // stop updating toolpath
            this.selected = null;
            updatePath = false;
        }
    }
    private void manipulate()
    {
        //oldPath = new List<GameObject>(path);

        // sync old names
        int pointsInLayers = nbPoints;
        float brushSizeZ = brushSizeHeight;
        float brushWidth = brushSizeWidth;
        int layers = nbLayers;

        int selectedIndex = path.IndexOf(selected);

                 
        if (manipulationType == "shape")
        {
            int column = selectedIndex % pointsInLayers;

            // new distance calculation
            // based on displacement from past position
            float disp = Vector3.Distance(initialSelPos, selected.transform.localPosition);

            // check if new radius is smaller --> displacement shoudld be negative
            float pasth = Vector3.Distance(pos, initialSelPos);                
            float newh = Vector3.Distance(pos, selected.transform.localPosition);

            if (pasth > newh)
            {
                disp *= -1;
            }


            if (disp != 0)
            {
                float zSize = (float)(Math.Floor(brushSizeZ * layers * layerHeight) * pointsInLayers);
                // print(brushSizeZ * layerHeight, zSize)

                int lowerColBound = Math.Max(0, (int)(selectedIndex - (int)(Math.Floor(brushSizeZ*pointsInLayers - pointsInLayers / 2))));
                int upperColBound = Math.Min((int)(pointsInLayers * layers), (int)(Math.Floor(brushSizeZ*pointsInLayers - pointsInLayers / 2) + selectedIndex));
                // print("lower", lowerColBound, "upper", upperColBound)

                for (int c = lowerColBound; c < upperColBound; c++)
                {
                    float oldx, oldy, oldz;
                    if (path[c] != selected)
                    {
                        oldx = path[c].transform.localPosition.x;
                        oldy = path[c].transform.localPosition.y;
                        oldz = path[c].transform.localPosition.z;
                    }
                    else
                    {
                        oldx = initialSelPos.x;
                        oldy = initialSelPos.y;
                        oldz = initialSelPos.z;
                    }

                    float oldh = (float)Math.Sqrt(Math.Pow(oldx, 2) + Math.Pow(oldz, 2));
                    float h = (float)((Math.Sqrt(Math.Pow(oldx, 2) + Math.Pow(oldz, 2)) + disp) / oldh);
                    float w = 1;

                    float zdist = Math.Abs(c - selectedIndex)/pointsInLayers ;
                    if (zdist >= 1)
                    {
                        // zdist = zdist / layerHeight;
                        // for exp
                        if (brushStyle == "squared")
                        {
                            w = (float)(1 - Math.Sqrt((zdist) / (brushSizeZ + 1)));
                        }
                        // for lin 
                        if (brushStyle == "linear")
                        {
                            w = 1 - (zdist) / (brushSizeZ + 1);
                        }
                        // for exp 2
                        if (brushStyle == "exponential")
                        {
                            w = (float)(1 - Math.Pow((zdist) / (brushSizeZ + 1), 2));
                        }
                    }

                    float newX = (oldx * (2 - w) + oldx * h * w) / 2;
                    float newZ = (oldz * (2 - w) + oldz * h * w) / 2;

                    path[c].transform.localPosition = new Vector3(newX, oldy, newZ);
                }
                  
            }
        }
        else if (manipulationType == "pattern")
        {
            int column = selectedIndex % pointsInLayers;

            // new distance calculation
            // based on displacement from past position
            float disp = Vector3.Distance(initialSelPos, selected.transform.localPosition);

            // check if new radius is smaller --> displacement shoudld be negative
            float pasth = Vector3.Distance(pos, initialSelPos);
            float newh = Vector3.Distance(pos, selected.transform.localPosition);

            if (pasth > newh)
            {
                disp *= -1;
            }


            if (disp != 0)
            {
                float zSize = (float)(Math.Floor(brushSizeZ * layers * layerHeight) * pointsInLayers);

                int lowerColBound = Math.Max(0, (int)(selectedIndex - (int)(Math.Floor(brushSizeZ * pointsInLayers - pointsInLayers / 2))));
                int upperColBound = Math.Min((int)(pointsInLayers * layers), (int)(Math.Floor(brushSizeZ * pointsInLayers - pointsInLayers / 2) + selectedIndex));

                for (int c = lowerColBound; c < upperColBound; c++)
                {
                    if(c%2 == 0)
                    {
                        float oldx, oldy, oldz;
                        if (path[c] != selected)
                        {
                            oldx = path[c].transform.localPosition.x;
                            oldy = path[c].transform.localPosition.y;
                            oldz = path[c].transform.localPosition.z;
                        }
                        else
                        {
                            oldx = initialSelPos.x;
                            oldy = initialSelPos.y;
                            oldz = initialSelPos.z;
                        }

                        float oldh = (float)Math.Sqrt(Math.Pow(oldx, 2) + Math.Pow(oldz, 2));
                        float h = (float)((Math.Sqrt(Math.Pow(oldx, 2) + Math.Pow(oldz, 2)) + disp) / oldh);
                        float w = 1;

                        float zdist = Math.Abs(path[c].transform.localPosition.y - selected.transform.localPosition.y);
                        if (zdist >= layerHeight)
                        {
                            zdist = zdist / layerHeight;
                            // for exp
                            if (brushStyle == "exponential")
                            {
                                w = (float)(1 - Math.Sqrt((zdist) / (zSize / pointsInLayers + 1)));
                            }
                            // for lin 
                            if (brushStyle == "linear")
                            {
                                w = 1 - (zdist) / (zSize / pointsInLayers + 1);
                            }
                        }

                        float newX = (oldx * (2 - w) + oldx * h * w) / 2;
                        float newZ = (oldz * (2 - w) + oldz * h * w) / 2;

                        path[c].transform.localPosition = new Vector3(newX, oldy, newZ);

                    }
                }

            }
        }
        else if(manipulationType == "point")
        {
            int column = selectedIndex % pointsInLayers;

            float newX = selected.transform.localPosition.x;
            float newZ = selected.transform.localPosition.z;

            for (int i = 0; i < pointsInLayers; i++)
            {
                if (Math.Abs(i - column) < brushWidth + 1)
                {
                    for(int c=i; c<pointsInLayers*layers; c+= pointsInLayers)
                    {
                        if(Math.Abs(c - selectedIndex) / pointsInLayers < brushSizeZ)

                        {
                            float dist = 1; 
                            float cdist = 1;

                            if(brushStyle == "linear")
                            {
                                cdist = Math.Max(0, Math.Abs(i - column) / (brushWidth + 1));
                                dist = Math.Min(1, Math.Abs(c - selectedIndex) / pointsInLayers / (brushSizeZ + 1) );
                                Debug.Log("col" + i + "cdist" + cdist + "dist " + dist + "w " + cdist*dist);
                            }
                            else if (brushStyle == "exponential")
                            {
                                dist = (float)Math.Pow(Math.Min(1, Math.Abs(c - selectedIndex) / pointsInLayers / (brushSizeZ + 1)), 2) ;
                                cdist = (float)Math.Pow(Math.Max(0, Math.Abs(i - column) / (brushWidth + 1)), 2);
                            }
                            float weight = Math.Min(dist + cdist, 1);
                            path[c].transform.localPosition = new Vector3((newX * (1 - weight) + path[c].transform.localPosition.x * weight), path[c].transform.localPosition.y, (newZ * (1 - weight) + path[c].transform.localPosition.z * weight));
                        }
                    }
                }
            }
                
        }
        drawToolpath();
    }

    public List<Vector3> savePath(List<GameObject> path)
    {
        List<Vector3> oldP = new List<Vector3>();
        foreach (GameObject p in path)
        {
            oldP.Add(p.transform.localPosition);
        }

        return oldP;
    }
    public void undo()
    {
        // path = oldPath;
        if(oldPath.Count > 0)
        {
            for (int i = 0; i<path.Count; i++)
            {
                path[i].transform.localPosition = oldPath[oldPath.Count-1][i];
            }
            oldPath.RemoveAt(oldPath.Count-1);

            drawToolpath();
            print("undo!");
        }
        
    } 
}
/*float cdist = (Math.Abs(i - column)); //, Math.Abs(i - column - pointsInLayers)
                if (cdist <= brushWidth)
                {
                    for(int c=i; c<pointsInLayers*layers; c+= pointsInLayers)
                    {
                        if(path[c] != selected)
                        {
                            float w = 1; 
                            float cw = 1;

                            float zdist = Math.Abs(c - selectedIndex) / pointsInLayers;
                            if (zdist >= 1) 
                            {
                                if (brushStyle == "linear")
                                {
                                    w = (zdist) / (brushSizeZ + 1);
                                    //cw = Math.Max(0, cdist / (brushWidth + 1));
                                }
                                else if (brushStyle == "exponential")
                                {
                                    w = (float)(Math.Sqrt((zdist) / (brushSizeZ + 1)));
                                    //cw = (float)Math.Sqrt(Math.Max(0, cdist / (brushWidth + 1)));
                                }
                            }
                            float weight = Math.Min(w*cw, 1);
                            path[c].transform.position = new Vector3((newX * (1 - weight) + path[c].transform.position.x * weight), path[c].transform.position.y, (newZ * (1 - weight) + path[c].transform.position.z * weight));

                        }
                    }
                }*/