using Microsoft.MixedReality.Toolkit.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UIElements;
public class exportToolpath : MonoBehaviour
{
    // Export CSV with tool path 
    public StringBuilder sb = new System.Text.StringBuilder();
    public StringBuilder sb2 = new System.Text.StringBuilder();

    //printer params
    public int box_x = 280;
    public int box_y = 265;
    public int box_z = 305;

    //printing params
    private const double nozzleW = 6.0;
    private const double layerH = 3.0;
    private const double printSpeed = 40.0;

    //toolpath variables
    public List<Vector3> newPos = new List<Vector3>();
    public List<double> segmentL = new List<double>();
    public List<double> e = new List<double>();
    public List<double> partResult = new List<double>();
    public List<double> modE = new List<double>();


    public GameObject DialogPrefabSmall;

    private const string startGCode = ";;; START GCODE ;;; \nM82 ;absolute extrusion mode \nG28; Home \nG1 X207.5 Y202.5 Z20 F10000; Move X and Y to center, Z to 20mm high \nG1 E2000 F20000 ; !!Prime Extruder \nG92 E0 \nG1 F30000 E-150 \n;;; ====== \n";
    private const string endGCode = ";;; === END GCODE === \nM83 ;Set to Relative Extrusion Mode \nG28 Z ;Home Z \n; === DEPRESSURIZE === \nG91 \nG91 \nG1 E-1300 F4000 \nG90 \nG90";

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void exportToolPath()
    {
        generateControlPoints controlPoints = GetComponent<generateControlPoints>();
        sb2.AppendLine("nbPoints,radius");
        sb2.AppendLine(controlPoints.nbPoints.ToString() + ',' + controlPoints.radius.ToString());
        sb2.AppendLine("cx,cy,cz");
        sb2.AppendLine(controlPoints.pos.ToString().Trim('(', ')'));
        sb2.AppendLine("x,y,z");
        for (int i = 0; i < controlPoints.path.Count; i++)
        {
            Vector3 pos = controlPoints.path[i].transform.localPosition;
            sb2.AppendLine(string.Format("{0:F5},{1:F5},{2:F5}", pos.x, pos.y, pos.z));
        }
        var folder = Application.streamingAssetsPath;
        if (!Directory.Exists(folder)) folder = Application.persistentDataPath;
        //folder = "Assets";


        var filePath = folder + "/toolpath.csv";

        using (var writer2 = new StreamWriter(filePath, false))
        {
            writer2.Write(sb2.ToString());
        }
        Debug.Log("Exported csv");
        Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, "Export Completed", "Toolpath exported", true);
    }

    public void centerPoints()
    {
        generateControlPoints controlPoints = GetComponent<generateControlPoints>();
        for (int i = 0; i < controlPoints.path.Count; i++)
        {
            Vector3 v = new Vector3(controlPoints.path[i].transform.localPosition.x * 1000 + box_x/2, controlPoints.path[i].transform.localPosition.z * 1000 + box_y / 2, controlPoints.path[i].transform.localPosition.y * 1000);
            newPos.Add(v);
        }
    }

    public void generateGCode()
    {
        print("SegmentL");
        print("test1");
        double temp = layerH / nozzleW;
        print(temp + " " + box_x + " " + (4 / System.Math.PI + layerH / nozzleW));
        //generateControlPoints controlPoints = GetComponent<generateControlPoints>();
        centerPoints();

        //calculate length of each line segments between points
        for (int i = 1; i < newPos.Count; i++)
        {
            segmentL.Add(Math.Abs(Vector3.Distance(newPos[i], newPos[i-1])));
            print(segmentL[i-1]);
        }
        segmentL.Insert(0, 0);
        print("extrusions");

        //calculate extrusion multiplier
        extrusionMultiplier();
        print("Test3");

        //perform mass addition for partial results
        partialMassAdd(e);
        print("Test4");

        //multiply extrusion for correct filament thickness
        //adjustExt(partResult);
        print("Test5");

        //calculate force (10000 for first line, then 2400)
        double f = 10000;

        //concat toolpath variables
        for (int i = 0; i < newPos.Count; i++)
        {
            if (i > 0) f = printSpeed * 60;
            Vector3 pos = newPos[i];
            roundGCode(pos.x, pos.y, pos.z, partResult[i], f);
        }
        print(sb.ToString());

        //export gcode
        var folder = Application.streamingAssetsPath;
        if (!Directory.Exists(folder)) folder = Application.persistentDataPath;
        // folder = "Assets";
        var filePath = folder + "/gcode.txt";
        
        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(startGCode);
            writer.Write(sb.ToString());
            writer.Write(endGCode);
        }

        Debug.Log("Exported gcode");
        Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, "Export Completed", "Toolpath exported", true);
    }

    public void extrusionMultiplier()
    {
        double multiplier = Mathf.Pow((float)(nozzleW / 1.91), 2);
        print(multiplier);
        foreach (double seg in segmentL)
        {
            e.Add(seg * layerH / nozzleW * (4 / System.Math.PI + layerH / nozzleW));

        }
    }

    public List<double> partialMassAdd(List<double> extrusions)
    {
        double massAdd = 0.0;
        double multiplier = Mathf.Pow((float)(nozzleW / 1.91), 2);


        for (int i = 0; i < extrusions.Count; i++)
        { 
            massAdd += extrusions[i]*multiplier;
            print(extrusions[i]);
            partResult.Add(massAdd);
        }
        return partResult;
    }

    public void adjustExt(List<double> extrusions)
    {
        float min = 0.8f;
        float max = 1.5f;

        double maxIdx = extrusions.Count;
        double midIdx = maxIdx * 0.5;
        for(int i = 0; i < extrusions.Count; i++)
        {
            modE.Add(extrusions[i] * map(i, 0, maxIdx - 1, max, min));
        }
    }
    public void roundGCode(float x, float y, float z, double e, double f)
    {
        double _x = (x * 100) / 100.0;
        double _y = (y * 100) / 100.0;
        double _z = (z * 100) / 100.0;
        double _e = (e * 10000) / 10000.0;
        double _f = (f * 100) / 100.0;

        sb.AppendLine(string.Format("{0:F0} {1:F5}{2:F0} {3:F5}{4:F5} {5:F5}{6:F5} {7:F5}{8:F0} {9:F5}{10:F5}", "G1", "F",_f,"X",_x,"Y",_y,"Z",_z, "E", _e));
    }

    //helper
    public double map(int value, int istart, double istop, double ostart, double ostop)
    {
        return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
    }
}