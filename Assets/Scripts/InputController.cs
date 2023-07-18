using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputController : MonoBehaviour
{
    // Initial coil values
    public float radius;
    public float layerHeight;
    public int nbLayers;
    public int nbPoints;

    // Initial manipulation params
    public float brushSizeHeight;
    public float brushSizeWidth;
    // 0 exponential
    // 1 linear
    public string brushStyle;
    // 0 shape
    // 1 point
    // 0 pattern
    public string manipulationType;

    //Coil initial parameters
    public TextMeshProUGUI coilRadiusTitle;
    public TextMeshProUGUI nbPointsTitle;
    public TextMeshProUGUI nbLayersTitle;
    public TextMeshProUGUI layerHeightTitle;

    public Slider coilRadiusSlider;
    public Slider nbPointsSlider;
    public Slider nbLayersSlider;
    public Slider layerHeightSlider;

    //Coil manipulation parameters
    public TextMeshProUGUI manipulationShapeTitle;
    public TextMeshProUGUI brushStyleTitle;
    public TextMeshProUGUI brushHeightTitle;
    public TextMeshProUGUI brushWidthTitle;

    public Dropdown manipulationShapeDropdown;
    public Dropdown brushStyleDropdown;
    public Slider brushHeightSlider;
    public Slider brushWidthSlider;
    private generateControlPoints controller;

    public TextMeshProUGUI brushHeightMax;
    public TextMeshProUGUI brushWidthMax;

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

        InitControlPoints();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitControlPoints ()
    {
        controller.updateParams("radius", radius);
        coilRadiusSlider.value = radius;
        controller.updateParams("nbPoints", nbPoints);
        nbPointsSlider.value = nbPoints;
        controller.updateParams("nbLayers", nbLayers);
        nbLayersSlider.value = nbLayers;
        controller.updateParams("layerHeight", layerHeight * .1f);
        layerHeightSlider.value = layerHeight;
        controller.initialToolPath();
        controller.updateParams("manipulationType", manipulationType);
        brushStyleDropdown.value = 1;
        controller.updateParams("brushStyle", brushStyle);
        manipulationShapeDropdown.value = 0;
        controller.updateParams("brushSizeHeight", brushSizeHeight);
        brushHeightSlider.value = brushSizeHeight;
        controller.updateParams("brushSizeWidth", brushSizeWidth);
        brushWidthSlider.value = brushSizeWidth;

        coilRadiusTitle.text = string.Format("Coil Radius (cm): {0:F1}", radius);
        nbPointsTitle.text = string.Format("Coil Control Points (num): {0:F0}", nbPoints);
        nbLayersTitle.text = string.Format("Coil Layers (num): {0:F0}", nbLayers);
        layerHeightTitle.text = string.Format("Layer Height (mm): {0:F1}", layerHeight);
        manipulationShapeTitle.text = "Coil Manipulation: " + manipulationType;
        brushStyleTitle.text = "Brush Style: " + brushStyle;
        brushHeightTitle.text = string.Format("Brush Height: {0:F0}", brushSizeHeight);
        brushWidthTitle.text = string.Format("Brush Width: {0:F0}", brushSizeWidth);

        brushHeightMax.text = string.Format("{0:F0}", nbLayers);
        brushHeightSlider.maxValue = nbLayers;
        int brushWidthMaxvAL = ((int)nbPoints / 4) - 1;
        brushWidthMax.text = string.Format("{0:F0}", brushWidthMaxvAL);
        brushWidthSlider.maxValue = brushWidthMaxvAL;
    }

    private void UpdateCoilRadius(float value)
    {
        coilRadiusTitle.text = string.Format("Coil Radius (cm): {0:F1}", value);
        controller.updateParams("radius", value);
        controller.initialToolPath();
    }
    private void UpdateNbPoints(float value)
    {
        nbPointsTitle.text = string.Format("Coil Control Points (num): {0:F0}", value);
        controller.updateParams("nbPoints", (int)value);
        int brushWidthMaxvAL = ((int)((int)value / 4)) - 1;
        brushWidthMax.text = string.Format("{0:F0}", brushWidthMaxvAL);
        brushWidthSlider.maxValue = brushWidthMaxvAL;
        brushWidthSlider.value = 1;
        controller.updateParams("brushSizeWidth", (int)1);
        controller.initialToolPath();
    }
    private void UpdateNnbLayers(float value)
    {
        nbLayersTitle.text = string.Format("Coil Layers (num): {0:F0}", value);
        controller.updateParams("nbLayers", (int)value);
        brushHeightMax.text = string.Format("{0:F0}", (int)value);
        brushHeightSlider.maxValue = (int)value;
        brushHeightSlider.value = 1;
        controller.updateParams("brushSizeHeight", (int)1);
        controller.initialToolPath();
    }
    private void UpdateLayerHeight(float value)
    {
        layerHeightTitle.text = string.Format("Layer Height (mm): {0:F1}", value);
        controller.updateParams("layerHeight", value * .1f);
        controller.initialToolPath();
    }

    private void UpdateManipulationShape(int value)
    {
        manipulationShapeTitle.text = "Coil Manipulation: " + manipulationShapeDropdown.options[value].text;
        controller.updateParams("manipulationType", manipulationShapeDropdown.options[value].text.ToLower());
    }
    private void UpdateBrushStyle(int value)
    {
        brushStyleTitle.text = "Brush Style: " + brushStyleDropdown.options[value].text;
        controller.updateParams("brushStyle", brushStyleDropdown.options[value].text.ToLower());
    }
    private void UpdateBrushHeight(float value)
    {
        brushHeightTitle.text = string.Format("Brush Height: {0:F0}", value);
        controller.updateParams("brushSizeHeight", value);
    }
    private void UpdateBrushWidth(float value)
    {
        brushWidthTitle.text = string.Format("Brush Width: {0:F0}", value);
        controller.updateParams("brushSizeWidth", value);
    }
}
