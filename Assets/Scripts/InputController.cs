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
    private const int layerHeightMin = 1;
    private const int layerHeightMax = 4;
    private const int nbLayersMin = 5;
    private const int nbLayersMax = 50;
    private const int nbPointsMin = 5;
    private const int nbPointsMax = 30;
    private const int brushHeightMin = 1;
    // changes with #layers 
    private int brushHeightMax = 10;
    private const int brushWidthMin = 1;
    // changes with # control points per layer
    private int brushWidthMax = 10;

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
    // 0 shapeUpdateCoilRadius
    // 1 point
    // 0 pattern
    public string manipulationType;

    private string[] brushStyleKeys = new string[] { "linear", "exponential" };
    private string[] manipulationTypeKeys = new string[] { "point", "shape", "pattern" };

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
    public TextMeshPro brushHeightTitle;
    public TextMeshPro brushWidthTitle;

    public InteractableToggleCollection manipulationShapeToggle;
    public InteractableToggleCollection brushStyleToggle;
    public PinchSlider brushHeightSlider;
    public PinchSlider brushWidthSlider;
    private generateControlPoints controller;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<generateControlPoints>();

        coilRadiusSlider.OnValueUpdated.AddListener(UpdateCoilRadius);
        nbPointsSlider.OnValueUpdated.AddListener(UpdateNbPoints);
        nbLayersSlider.OnValueUpdated.AddListener(UpdateNnbLayers);
        layerHeightSlider.OnValueUpdated.AddListener(UpdateLayerHeight);
        //coilRadiusSlider.OnInteractionEnded.AddListener(UpdateCoilRadius);
        //nbPointsSlider.OnInteractionEnded.AddListener(UpdateNbPoints);
        //nbLayersSlider.OnInteractionEnded.AddListener(UpdateNnbLayers);
        //layerHeightSlider.OnInteractionEnded.AddListener(UpdateLayerHeight);

        manipulationShapeToggle.OnSelectionEvents.AddListener(UpdateManipulationShape);
        brushStyleToggle.OnSelectionEvents.AddListener(UpdateBrushStyle);
        brushHeightSlider.OnValueUpdated.AddListener(UpdateBrushHeight);
        brushWidthSlider.OnValueUpdated.AddListener(UpdateBrushWidth);
        //brushHeightSlider.OnInteractionEnded.AddListener(UpdateBrushHeight);
        //brushWidthSlider.OnInteractionEnded.AddListener(UpdateBrushWidth);

        InitControlPoints();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void InitControlPoints()
    {

        controller.updateParams("radius", radius);
        float coilRadiusSliderValue = Remap(radius, radiusMin, radiusMax, 0f, 1f);
        coilRadiusSlider.SliderValue = coilRadiusSliderValue;
        coilRadiusTitle.text = $"{radius:F2} (cm)";

        controller.updateParams("nbPoints", nbPoints);
        nbPointsSlider.SliderStepDivisions = nbPointsMax - nbPointsMin;
        nbPointsSlider.UseSliderStepDivisions = true;
        float nbPointsSliderValue = Remap(nbPoints, nbPointsMin, nbPointsMax, 0f, 1f);
        nbPointsSlider.SliderValue = nbPointsSliderValue;
        nbPointsTitle.text = $"{nbPoints:0}";

        controller.updateParams("nbLayers", nbLayers);
        nbLayersSlider.SliderStepDivisions = nbLayersMax - nbLayersMin;
        nbLayersSlider.UseSliderStepDivisions = true;
        float nbLayersSliderValue = Remap(nbLayers, nbLayersMin, nbLayersMax, 0f, 1f);
        nbLayersSlider.SliderValue = nbLayersSliderValue;
        nbLayersTitle.text = $"{nbLayers:0}";


        controller.updateParams("layerHeight", layerHeight * .1f);
        layerHeightSlider.SliderStepDivisions = layerHeightMax - layerHeightMin;
        layerHeightSlider.UseSliderStepDivisions = true;
        float layerHeightSliderValue = Remap(layerHeight, layerHeightMin, layerHeightMax, 0f, 1f);
        layerHeightSlider.SliderValue = layerHeightSliderValue;
        layerHeightTitle.text = $"{layerHeight:0} (mm)";
     

        controller.initialToolPath();
        controller.updateParams("manipulationType", manipulationType);
        brushStyleToggle.CurrentIndex = 1;
        controller.updateParams("brushStyle", brushStyle);
        manipulationShapeToggle.CurrentIndex = 0;

        brushHeightMax = nbLayers;
        brushWidthMax = ((int)nbPoints / 4) - 1;


        controller.updateParams("brushSizeHeight", brushSizeHeight);
        brushHeightSlider.SliderStepDivisions = brushHeightMax - brushHeightMin;
        brushHeightSlider.UseSliderStepDivisions = true;
        float brushHeightSliderValue = Remap(brushSizeHeight, brushHeightMin, brushHeightMax, 0f, 1f);
        brushHeightSlider.SliderValue = brushHeightSliderValue;
        brushHeightTitle.text = $"{brushSizeHeight:0} (layers)";


        controller.updateParams("brushSizeWidth", brushSizeWidth);
        brushWidthSlider.SliderStepDivisions = brushWidthMax - brushWidthMin;
        brushWidthSlider.UseSliderStepDivisions = true;
        float brushWidthSliderValue = Remap(brushSizeWidth, brushWidthMin, brushWidthMax, 0f, 1f);
        brushWidthSlider.SliderValue = brushWidthSliderValue;
        brushWidthTitle.text = $"{brushSizeWidth:0} (points)";

        Debug.Log($"brush width{brushSizeWidth:0}");
        Debug.Log($"brush width max{brushWidthMax:0}");

    }

    public float Remap(float value, float from1, float to1, float from2, float to2)
    {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }

    private void DisplayCoilRadius(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.NewValue, 0f, 1f, radiusMin, radiusMax);
        coilRadiusTitle.text = $"{mappedValue:F2} (cm)";
    }

    private void DisplayNbPoints(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.NewValue, 0f, 1f, nbPointsMin, nbPointsMax);
        nbPointsTitle.text = $"{mappedValue:0}";
    }

    private void DisplayNbLayers(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.NewValue, 0f, 1f, nbLayersMin, nbLayersMax);
        nbLayersTitle.text = $"{mappedValue:0}";
    }

    private void DisplayLayerHeight(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.NewValue, 0f, 1f, layerHeightMin, layerHeightMax);
        layerHeightTitle.text = $"{mappedValue:0} (mm)";
    }

    private void DisplayBrushHeight(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.NewValue, 0f, 1f, brushHeightMin, brushHeightMax);
        brushHeightTitle.text = $"{mappedValue:0} (layers)";
    }

    private void DisplayBrushWidth(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.NewValue, 0f, 1f, brushWidthMin, brushWidthMax);
        brushWidthTitle.text = $"{mappedValue:0} (points)";
    }

    private void UpdateCoilRadius(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, radiusMin, radiusMax);
        coilRadiusTitle.text = $"{mappedValue:F2} (cm)";
        controller.updateParams("radius", mappedValue);
        controller.initialToolPath();
    }

    private void UpdateNbPoints(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, nbPointsMin, nbPointsMax);
        nbPointsTitle.text = $"{mappedValue:0}";
        controller.updateParams("nbPoints", (int)mappedValue);

        brushWidthMax = ((int)((int)mappedValue / 4)) - 1;
        float brushWidthSliderValue = Remap(1, brushWidthMin, brushWidthMax, 0f, 1f);
        brushWidthSlider.SliderValue = brushWidthSliderValue;
        controller.updateParams("brushSizeWidth", (int)1);

        controller.initialToolPath();
    }

    private void UpdateNnbLayers(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, nbLayersMin, nbLayersMax);
        nbLayersTitle.text = $"{mappedValue:0}";
        controller.updateParams("nbLayers", (int)mappedValue);

        brushHeightMax = (int)mappedValue;
        float brushHeighSliderValue = Remap(1, brushHeightMin, brushHeightMax, 0f, 1f);
        brushHeightSlider.SliderValue = brushHeighSliderValue;
        controller.updateParams("brushSizeHeight", (int)1);

        controller.initialToolPath();
    }

    private void UpdateLayerHeight(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, layerHeightMin, layerHeightMax);
        layerHeightTitle.text = $"{mappedValue:0} (mm)";
        controller.updateParams("layerHeight", mappedValue * .1f);
        controller.drawToolpath();
    }

    private void UpdateManipulationShape()
    {
        int selectedIndex = manipulationShapeToggle.CurrentIndex;
        controller.updateParams("manipulationType", manipulationTypeKeys[selectedIndex]);
    }
    private void UpdateBrushStyle()
    {
        int selectedIndex = brushStyleToggle.CurrentIndex;
        controller.updateParams("brushStyle", brushStyleKeys[selectedIndex]);
    }
    private void UpdateBrushHeight(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, brushHeightMin, brushHeightMax);
        brushHeightTitle.text = $"{mappedValue:0}";
        controller.updateParams("brushSizeHeight", (int)mappedValue);
    }
    private void UpdateBrushWidth(SliderEventData eventData)
    {
        float mappedValue = Remap(eventData.Slider.SliderValue, 0f, 1f, brushWidthMin, brushWidthMax);
        brushWidthTitle.text = $"{mappedValue:0}";
        controller.updateParams("brushSizeWidth", (int)mappedValue);
    }
}
