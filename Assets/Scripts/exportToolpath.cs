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

    public GameObject DialogPrefabSmall;

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


        var filePath = Path.Combine(folder, "toolpath.csv");

        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(sb.ToString());
        }

        Debug.Log("Exported csv");
        Dialog.Open(DialogPrefabSmall, DialogButtonType.OK, "Export Completed", "Toolpath exported", true);
    }
}
