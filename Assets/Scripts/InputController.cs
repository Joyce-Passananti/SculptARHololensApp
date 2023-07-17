using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    public TextMeshProUGUI coilRadius;
    public TextMeshProUGUI nbPoints;
    public TextMeshProUGUI nbLayers;
    public TextMeshProUGUI layerHeight;

    public Slider coilRadiusSlider;
    public Slider nbPointsSlider;
    public Slider nbLayersSlider;
    public Slider layerHeightSlider;

    // Start is called before the first frame update
    void Start()
    {
        coilRadiusSlider.onValueChanged.AddListener(UpdateCoilRadius);
        nbPointsSlider.onValueChanged.AddListener(UpdateNbPoints);
        nbLayersSlider.onValueChanged.AddListener(UpdateNnbLayers);
        layerHeightSlider.onValueChanged.AddListener(UpdateLayerHeight);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void UpdateCoilRadius(float value)
    {
        coilRadius.text = string.Format("Coil Radius: {0:F1}", value);
    }
    private void UpdateNbPoints(float value)
    {
        nbPoints.text = string.Format("Coil Control Points: {0:F1}", value);
    }
    private void UpdateNnbLayers(float value)
    {
        nbLayers.text = string.Format("Coil Layers: {0:F1}", value);
    }
    private void UpdateLayerHeight(float value)
    {
        layerHeight.text = string.Format("Layer Height: {0:F1}", value);
    }
}
