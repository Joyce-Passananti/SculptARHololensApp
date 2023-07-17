using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    //Coil initial parameters
    public TextMeshProUGUI coilRadius;
    public TextMeshProUGUI nbPoints;
    public TextMeshProUGUI nbLayers;
    public TextMeshProUGUI layerHeight;

    public Slider coilRadiusSlider;
    public Slider nbPointsSlider;
    public Slider nbLayersSlider;
    public Slider layerHeightSlider;

    //Coil manipulation parameters
    public TextMeshProUGUI manipulationShape;
    public TextMeshProUGUI brushStyle;
    public TextMeshProUGUI brushHeight;
    public TextMeshProUGUI brushWidth;

    public Dropdown manipulationShapeDropdown;
    public Dropdown brushStyleDropdown;
    public Slider brushHeightSlider;
    public Slider brushWidthSlider;

    // Start is called before the first frame update
    void Start()
    {
        coilRadiusSlider.onValueChanged.AddListener(UpdateCoilRadius);
        nbPointsSlider.onValueChanged.AddListener(UpdateNbPoints);
        nbLayersSlider.onValueChanged.AddListener(UpdateNnbLayers);
        layerHeightSlider.onValueChanged.AddListener(UpdateLayerHeight);

        manipulationShapeDropdown.onValueChanged.AddListener(UpdateManipulationShape);
        brushStyleDropdown.onValueChanged.AddListener(UpdateBrushStyle);
        brushHeightSlider.onValueChanged.AddListener(UpdateBrushHeight);
        brushWidthSlider.onValueChanged.AddListener(UpdateBrushWidth);
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

    private void UpdateManipulationShape(int value)
    {
        manipulationShape.text = "Coil Manipulation: " + manipulationShapeDropdown.options[value].text;
    }
    private void UpdateBrushStyle(int value)
    {
        brushStyle.text = "Brush Style: " + brushStyleDropdown.options[value].text;
    }
    private void UpdateBrushHeight(float value)
    {
        brushHeight.text = string.Format("Brush Height: {0:F0}", value);
    }
    private void UpdateBrushWidth(float value)
    {
        brushWidth.text = string.Format("Layer Width: {0:F0}", value);
    }
}
