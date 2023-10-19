using Microsoft.MixedReality.Toolkit.UI;
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
    public double e = 0;
    public double nozzleW;
    public double layerH;
    public double segmentL;
    public double printSpeed;

    public GameObject DialogPrefabSmall;

    public string startGCode = ";;; START GCODE ;;; \n M82 ;absolute extrusion mode \n G28; Home \n G1 X207.5 Y202.5 Z20 F10000; Move X and Y to center, Z to 20mm high \n G1 E4000 F40000 ; !!Prime Extruder \n G92 E0 \n G1 F30000 E-150 \n ;;; ====== ";
    public string endGCode = ";;; === END GCODE === \n M83 ;Set to Relative Extrusion Mode \n G28 Z ;Home Z \n ; === DEPRESSURIZE === \n G91 \n G91 \n G1 E-1300 F4000 \n G90 \n G9";
   
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
        sb.AppendLine("nbPoints,radius");
        sb.AppendLine(controlPoints.nbPoints.ToString() + ',' + controlPoints.radius.ToString());
        sb.AppendLine("cx,cy,cz");
        sb.AppendLine(controlPoints.pos.ToString().Trim('(', ')'));
        sb.AppendLine("x,y,z");
        for (int i = 0; i < controlPoints.path.Count; i++)
        {
            Vector3 pos = controlPoints.path[i].transform.localPosition;
            sb.AppendLine(string.Format("{0:F5},{1:F5},{2:F5}", pos.x, pos.y, pos.z));
        }

        var folder = Application.streamingAssetsPath;

        if (!Directory.Exists(folder)) folder = Application.persistentDataPath;


        var filePath = folder + "/toolpath.csv";

        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(sb.ToString());
        }

        Debug.Log("Exported csv");
        Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, "Export Completed", "Toolpath exported", true);
    }

    public void generateGCode(double nozzleW, double layerH, double segmentL, double printSpeed)
    {
        generateControlPoints controlPoints = GetComponent<generateControlPoints>();
        extrusion(nozzleW, layerH, segmentL);

        double f =  printSpeed * 60;

        for (int i = 0; i < controlPoints.path.Count; i++)
        {
            Vector3 pos = controlPoints.path[i].transform.localPosition;
            roundGCode(pos.x, pos.y, pos.z, e, f);
        }

        var folder = Application.streamingAssetsPath;
        if (!Directory.Exists(folder)) folder = Application.persistentDataPath;
        var filePath = folder + "/gcode.txt";

        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(startGCode);
            writer.Write(sb.ToString());
            writer.Write(startGCode);
        }

        Debug.Log("Exported gcode");
        Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, "Export Completed", "Toolpath exported", true);
    }


    public void roundGCode(float x, float y, float z, double e, double f)
    {
        int _x = (int)((x * 100) / 100.0);
        int _y = (int)((y * 100) / 100.0);
        int _z = (int)((z * 100) / 100.0);
        int _e = (int)((e * 10000) / 10000.0);
        int _f = (int)((f * 100) / 100.0);

        sb.AppendLine(string.Format("{0:F5},{1:F5},{2:F5},{3:F5},{4:F5},{5:F5},{6:F5},{7:F5},{8:F5},{9:F5},{10:F5}", "G1", "F",f,"X",_x,"Y",_y,"Z",_z, "E", e));
    }

    public void extrusion(double nozzleW, double layerH, double segmentL)
    {
        e = segmentL * layerH / nozzleW * (4 / System.Math.PI + layerH / nozzleW);
    }

    public void extrusionMultiplier()
    {
        double multiplier = Mathf.Pow((float)(nozzleW / 1.91), 2);
    }

    public List<double> partialMassAdd(List<double> extrusions)
    {
        double massAdd = 0.0;
        List<double> partResult = new List<double>();

        for (int i = 0; i < extrusions.Count; i++)
        {
            massAdd += extrusions[i];
            partResult.Add(massAdd);
        }
        return partResult;
    }

    public void adjustExt(List<double> extrusions)
    {
        float min = 0.8f;
        float max = 1.5f;

        List<double> modE = new List<double>();

     
        double maxIdx = extrusions.Count;
        double midIdx = maxIdx * 0.5;
        for(int i = 0; i < extrusions.Count; i++)
        {
            modE.Add(extrusions[i] * map(i, 0, maxIdx - 1, max, min));
        }
    }

    public double map(int value, int istart, double istop, double ostart, double ostop)
    {
        return ostart + (ostop - ostart) * ((value - istart) / (istop - istart));
    }
}
