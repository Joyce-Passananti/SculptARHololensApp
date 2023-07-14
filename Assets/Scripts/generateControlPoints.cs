using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Microsoft.MixedReality.Toolkit.Input;
using Microsoft.MixedReality.Toolkit.UI;
using UnityEngine.UI;

public class generateControlPoints : MonoBehaviour
{
    // initial coil parameters
    public GameObject cube;
    public Vector3 position = new Vector3(0f,0f,0f);
    public float radius;
    public float layerHeight;
    public int nbLayers;
    public int nbPoints; // points in layer

    private GameObject sphere;
    public float sphereSize;
    public List<GameObject> path = new List<GameObject>();

    // manipulation params
    public float brushSizeHeight;
    public float brushSizeWidth;
    public string brushStyle;
    public string manipulationType;

    private GameObject selected = null;
    private Vector3 initialSelPos;
    private Boolean updatePath = false;

    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = new GameObject("Line").AddComponent<LineRenderer>();
        lineRenderer.startColor = Color.white;
        lineRenderer.endColor = Color.white;
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.useWorldSpace = true;

        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = position;
        cube.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);

        cube.AddComponent<Interactable>();
        cube.AddComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>();

        cube.AddComponent<ObjectManipulator>();
        cube.GetComponent<ObjectManipulator>().OnManipulationEnded.AddListener(x => { initialToolPath(); Debug.Log("MOVED"); });

        initialToolPath();
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
        position = cube.transform.position;
        Debug.Log(position);

        // vectors = []
        for (int j = 0; j < nbLayers; j++)
        {
            for (int i = 0; i < nbPoints; i++)
            {
                float angle = 360 / nbPoints;
                sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                sphere.transform.position = new Vector3(position.x + (radius * Mathf.Cos(i * angle * Mathf.PI / 180)) * .01f, position.z + (layerHeight * j) * .01f, position.y + (radius * Mathf.Sin(i * angle * Mathf.PI / 180)) * .01f);
                sphere.transform.localScale = new Vector3(sphereSize, sphereSize, sphereSize);
                addObjectComponents(sphere);
                path.Add(sphere);
            }
        }
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

    private void addObjectComponents(GameObject obj)
    {
        obj.AddComponent<ConstraintManager>();
        obj.AddComponent<Interactable>();
        obj.AddComponent<Microsoft.MixedReality.Toolkit.Input.NearInteractionGrabbable>();

        obj.AddComponent<ObjectManipulator>();
        obj.GetComponent<ObjectManipulator>().OnManipulationStarted.AddListener(HandleOnManipulationStarted);
        obj.GetComponent<ObjectManipulator>().OnManipulationEnded.AddListener(HandleOnManipulationEnded);

        // obj.GetComponent<Interactable>().OnClick.AddListener(selected.selectObject(obj));

    }
    private void HandleOnManipulationStarted(ManipulationEventData eventData)
    {
        selectObject(eventData.ManipulationSource, true);
    }

    private void HandleOnManipulationEnded(ManipulationEventData eventData)
    {
        selectObject(eventData.ManipulationSource, false);
    }

    private void selectObject(GameObject sel, Boolean status)
    {
        if (status)
        {
            // save initial position
            initialSelPos = sel.transform.position;

            // update toolpath with new position
            this.selected = sel;
            updatePath = true;
        }
        else
        {
            // correct y position to initial y position
            sel.transform.position = new Vector3(sel.transform.position.x, initialSelPos.y, sel.transform.position.z);
            manipulate();
            drawToolpath();

            // stop updating toolpath
            this.selected = null;
            updatePath = false;
        }
    }
    private void manipulate()
    {
        // sync old names
        int pointsInLayers = nbPoints;
        float brushSizeZ = brushSizeHeight;
        int layers = nbLayers;

        int selectedIndex = path.IndexOf(selected);


        if (manipulationType == "shape")
        {
            int column = selectedIndex % pointsInLayers;

            // new distance calculation
            // based on displacement from past position
            float disp = Vector3.Distance(initialSelPos, selected.transform.position);

            // check if new radius is smaller --> displacement shoudld be negative
            float pasth = Vector3.Distance(position, initialSelPos);                
            float newh = Vector3.Distance(position, selected.transform.position);

            if (pasth > newh)
            {
                disp *= -1;
            }


            if (disp != 0)
            {
                float zSize = (float)(Math.Floor(brushSizeZ * layers * layerHeight) * pointsInLayers);
                // print(brushSizeZ * layerHeight, zSize)

                int lowerColBound = Math.Max(0, (int)(selectedIndex - (int)(Math.Floor(pointsInLayers / 2 + zSize))));
                int upperColBound = Math.Min((int)(pointsInLayers * layers), (int)(Math.Floor(pointsInLayers / 2 + zSize) + selectedIndex));
                // print("lower", lowerColBound, "upper", upperColBound)

                for (int c = lowerColBound; c < upperColBound; c++)
                {
                    float oldx, oldy, oldz;
                    if (path[c] != selected)
                    {
                        oldx = path[c].transform.position.x;
                        oldy = path[c].transform.position.y;
                        oldz = path[c].transform.position.z;
                    }
                    else
                    {
                        oldx = initialSelPos.x;
                        oldy = initialSelPos.y;
                        oldz = initialSelPos.z;
                    }

                    float oldh = (float)Math.Sqrt(Math.Pow(oldx, 2) + Math.Pow(oldy, 2));
                    float h = (float)((Math.Sqrt(Math.Pow(oldx, 2) + Math.Pow(oldy, 2)) + disp) / oldh);
                    float w = 1;

                    float zdist = Math.Abs(path[c].transform.position.y - selected.transform.position.y);
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
                        // print(zSize / pointsInLayers, zdist, w)
                    }

                    float newX = (oldx * (2 - w) + oldx * h * w) / 2;
                    float newZ = (oldz * (2 - w) + oldz * h * w) / 2;

                    // c = Point3D(newX, newY, oldZ)
                    path[c].transform.position = new Vector3(newX, oldy, newZ);
                }
                  
            }
        }
        else if(manipulationType == "point")
        {

        }
    }
}
    /*
    """Provides a scripting component.
    Inputs:
        selected: the point thta was selected/dragged by the user
        points: the list of all points
        layers: number of layers(rows)
        pointsInLayers: number of points in each layer()columns
       bSize: size of "brush" or user manipulation
       bStyle: size of "brush" or user manipulation
   Output:
        a: The a output variable"""

__author__ = "joyce"
__version__ = "2023.05.22"

import rhinoscriptsyntax as rs
from math import sqrt, floor
from Rhino.Geometry import Point3d

def distXY(p0, p1) :
  return sqrt((p0.X- p1.X)*(p0.X- p1.X) + (p0.Y- p1.Y)*(p0.Y- p1.Y))#+(p0.Z- p1.Z)*(p0.Z- p1.Z)
  
def dist(p0, p1):
  return sqrt((p0.X- p1.X)*(p0.X- p1.X) + (p0.Y- p1.Y)*(p0.Y- p1.Y)+(p0.Z- p1.Z)*(p0.Z- p1.Z))

def colTransform():       
    column = selected%pointsInLayers
# print(column)

# cols = points[column::pointsInLayers]
# print[len(cols)]

    newX, newY = points[selected].X, points[selected].Y
    print(newX, newY)
    
    for i in range(pointsInLayers) :
        if(abs(i-column) < brushWidth* pointsInLayers/360):
            for c in points[i::pointsInLayers]:
                if(not c == points[selected]):
                    if(bStyle == 0):    
                        cdist = max(0, abs(i-column)/(brushWidth/360*pointsInLayers))
                        dist = min(1, abs(c.Z - points[selected].Z)/layerHeight/layers/brushSizeZ)
                    elif(bStyle == 1) :
                        dist = pow(min(1, abs(c.Z - points[selected].Z)/layerHeight/layers/brushSizeZ), 2)
                        cdist = pow(max(0, abs(i-column)/(brushWidth/360*pointsInLayers)),2)   
                    
                    weight = min(dist + cdist, 1)
                    c.X =  (newX*(1-weight) + c.X* weight)
                    c.Y =  (newY*(1-weight) + c.Y* weight)
#                    print(dist)
#            print("lin")

def shapeTransform():      
    print(brushSizeZ* layers*layerHeight)
    column = selected%pointsInLayers

# new distance calculation
# based on displacement from past position
    disp = dist(pastpos, points[selected])

# check if new adius is smaller --> displacement shoudld be negative
    pasth, newh = dist(Point3d(0,0,0), pastpos), dist(Point3d(0,0,0), points[selected]) 
    if(pasth > newh):
        disp *= -1
    print(disp)
    
    if(disp != 0):
        zSize = floor(brushSizeZ* layers*layerHeight)*pointsInLayers
         print(brushSizeZ* layerHeight, zSize)
        lowerColBound = max(0, int(selected-int(floor(pointsInLayers/2 + zSize))))
        upperColBound =  min(int(pointsInLayers* layers), int(floor(pointsInLayers/2 + zSize) + selected))
        print("lower", lowerColBound, "upper", upperColBound)
        for c in points[lowerColBound:upperColBound:]:
            if(c != points[selected]):
                oldx, oldy, oldz = c.X, c.Y, c.Z
            else:
                oldx, oldy, oldz = pastpos.X, pastpos.Y, pastpos.Z
            oldh = sqrt(pow(oldx, 2) + pow(oldy, 2))


            h = (sqrt(pow(oldx,2) + pow(oldy,2)) + disp)/oldh
            w = 1


            zdist = abs(c.Z - points[selected].Z)
            if(zdist >= layerHeight):
                zdist = zdist/layerHeight
#               for exp
                if(bStyle==1):
                    w = 1 - sqrt((zdist)/(zSize/pointsInLayers + 1))
#               for lin 
                if(bStyle==0):
                    w = 1 - (zdist)/(zSize/pointsInLayers + 1)
                print(zSize/pointsInLayers, zdist, w)


            newX, newY = (oldx*(2-w) + oldx* h*w)/2, (oldy*(2-w) + oldy* h*w)/2
            
        #    c = Point3D(newX, newY, oldZ)
            c.X = newX
            c.Y = newY
            c.Z = oldz



#MAIN

#TODO: replace with actual past (initial) position of selected point before manipulation
pastpos = Point3d(points[selected])

# remove when testing through fologram
points[selected].X *= 1.8
points[selected].Y *= 1.8

#print("checking?" + str(selected)  + "prev" + str(prev))
if(selected is not None): 
    new = points[selected]
    if(prev is None):
        prev = points[selected]
    if(prev is not None):
        if (dist(points[selected], prev)>0.05):
#            print("i should do the col transform now")
            if(manipulation ==0):
                shapeTransform()
            elif(manipulation == 2) :
                colTransform()



a = points
*/