using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Microsoft.MixedReality.Toolkit.UI;

public class InputController : MonoBehaviour
{

    // Min Max values of sliders
    private const float radiusMin = 2;
    private const float radiusMax = 20;
    private const float layerHeightMin = 1;
    private const float layerHeightMax = 6;
    private const int nbLayersMin = 5;
    private const int nbLayersMax = 50;
    private const int nbPointsMin = 5;
    private const int nbPointsMax = 30;

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
    public TextMeshPro coilRadiusTitle;
    public TextMeshPro nbPointsTitle;
    public TextMeshPro nbLayersTitle;
    public TextMeshPro layerHeightTitle;

    public PinchSlider coilRadiusSlider;
    public PinchSlider nbPointsSlider;
    public PinchSlider nbLayersSlider;
    public PinchSlider layerHeightSlider;

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

        coilRadiusSlider.OnInteractionEnded.AddListener(UpdateCoilRadius);
        nbPointsSlider.OnInteractionEnded.AddListener(UpdateNbPoints);
        nbLayersSlider.OnInteractionEnded.AddListener(UpdateNnbLayers);
        layerHeightSlider.OnInteractionEnded.AddListener(UpdateLayerHeight);

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

    public void InitControlPoints()
    {

        //testSlider.SliderStepDivisions = nbLayersMax - nbLayersMin;
        //testSlider.UseSliderStepDivisions = true;
        //float testSliderValue = Remap(nbLayers, nbLayersMin, nbLayersMax, 0f, 1f);
        //testSlider.SliderValue = testSliderValue;
        //testLabel.text = $"{nbLayers:F2}";

        controller.updateParams("radius", radius);
        float coilRadiusSliderValue = Remap(radius, radiusMin, radiusMax, 0f, 1f);
        coilRadiusSlider.SliderValue = coilRadiusSliderValue;
        coilRadiusTitle.text = $"{radius:F2}";

        controller.updateParams("nbPoints", nbPoints);
        nbPointsSlider.SliderStepDivisions = nbPointsMax - nbPointsMin;
        nbPointsSlider.UseSliderStepDivisions = true;
        float nbPointsSliderValue = Remap(nbPoints, nbPointsMin, nbPointsMax, 0f, 1f);
        nbPointsSlider.SliderValue = nbPointsSliderValue;
        nbPointsTitle.text = $"{nbPoints:F2}";

        controller.updateParams("nbLayers", nbLayers);
        nbLayersSlider.SliderStepDivisions = nbLayersMax - nbLayersMin;
        nbLayersSlider.UseSliderStepDivisions = true;
        float nbLayersSliderValue = Remap(nbLayers, nbLayersMin, nbLayersMax, 0f, 1f);
        nbLayersSlider.SliderValue = nbLayersSliderValue;
        nbLayersTitle.text = $"{nbLayers:F2}";


        controller.updateParams("layerHeight", layerHeight * .1f);
        float layerHeightSliderValue = Remap(layerHeight, layerHeightMin, layerHeightMax, 0f, 1f);
        layerHeightSlider.SliderValue = layerHeightSliderValue;
        layerHeightTitle.text = $"{layerHeight:F2}";

        controller.initialToolPath();
        controller.updateParams("manipulationType", manipulationType);
        brushStyleDropdown.value = 1;
        controller.updateParams("brushStyle", brushStyle);
        manipulationShapeDropdown.value = 0;
        controller.updateParams("brushSizeHeight", brushSizeHeight);
        brushHeightSlider.value = brushSizeHeight;
        controller.updateParams("brushSizeWidth", brushSizeWidth);
        brushWidthSlider.value = brushSizeWidth;

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

    //private void TestSlider(SliderEventData eventData)
    //{
    //    float originalValue = eventData.NewValue;  // Assuming NewValue is between 0 and 1 for a typical slider.

    //    // Define your desired minimum and maximum values
    //    float desiredMin = 1;
    //    float desiredMax = 5;

    //    float mappedValue = Remap(originalValue, 0f, 1f, desiredMin, desiredMax);
    //    testLabel.text = $"{mappedValue:F2}";
    //}

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void UpdateCoilRadius(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, radiusMin, radiusMax);
        coilRadiusTitle.text = $"{mappedValue:F2}";
        controller.updateParams("radius", mappedValue);
        controller.initialToolPath();
    }

    private void UpdateNbPoints(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, nbPointsMin, nbPointsMax);
        nbPointsTitle.text = $"{mappedValue:F2}";
        controller.updateParams("nbPoints", (int)mappedValue);

        int brushWidthMaxvAL = ((int)((int)mappedValue / 4)) - 1;
        brushWidthMax.text = string.Format("{0:F0}", brushWidthMaxvAL);
        brushWidthSlider.maxValue = brushWidthMaxvAL;
        brushWidthSlider.value = 1;
        controller.updateParams("brushSizeWidth", (int)1);

        controller.initialToolPath();
    }

    private void UpdateNnbLayers(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, nbLayersMin, nbLayersMax);
        nbLayersTitle.text = $"{mappedValue:F2}";
        controller.updateParams("nbLayers", (int)mappedValue);

        brushHeightMax.text = string.Format("{0:F0}", (int)mappedValue);
        brushHeightSlider.maxValue = (int)mappedValue;
        brushHeightSlider.value = 1;
        controller.updateParams("brushSizeHeight", (int)1);

        controller.initialToolPath();
    }

    private void UpdateLayerHeight(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, layerHeightMin, layerHeightMax);
        layerHeightTitle.text = $"{mappedValue:F2}";
        controller.updateParams("layerHeight", mappedValue * .1f);
        controller.drawToolpath();
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
