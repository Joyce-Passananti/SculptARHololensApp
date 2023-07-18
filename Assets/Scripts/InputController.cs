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
    private generateControlPoints controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<generateControlPoints>();

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
        controller.updateParams("radius", value);
        controller.initialToolPath();
    }
    private void UpdateNbPoints(float value)
    {
        nbPoints.text = string.Format("Coil Control Points: {0:F1}", value);
        controller.updateParams("nbPoints", (int)value);
        controller.initialToolPath();
    }
    private void UpdateNnbLayers(float value)
    {
        nbLayers.text = string.Format("Coil Layers: {0:F1}", value);
        controller.updateParams("nbLayers", (int)value);
        controller.initialToolPath();
    }
    private void UpdateLayerHeight(float value)
    {
        layerHeight.text = string.Format("Layer Height: {0:F1}", value);
        controller.updateParams("layerHeight", value);
        controller.initialToolPath();
    }

    private void UpdateManipulationShape(int value)
    {
        manipulationShape.text = "Coil Manipulation: " + manipulationShapeDropdown.options[value].text;
        controller.updateParams("manipulationType", manipulationShapeDropdown.options[value].text.ToLower());
    }
    private void UpdateBrushStyle(int value)
    {
        brushStyle.text = "Brush Style: " + brushStyleDropdown.options[value].text;
        controller.updateParams("brushStyle", brushStyleDropdown.options[value].text.ToLower());
    }
    private void UpdateBrushHeight(float value)
    {
        brushHeight.text = string.Format("Brush Height: {0:F0}", value);
        controller.updateParams("brushSizeHeight", value);
    }
    private void UpdateBrushWidth(float value)
    {
        brushWidth.text = string.Format("Layer Width: {0:F0}", value);
        controller.updateParams("brushSizeWidth", value);
    }
}
