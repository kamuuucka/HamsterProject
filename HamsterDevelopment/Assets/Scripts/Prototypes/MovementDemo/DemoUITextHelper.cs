using TMPro;
using UnityEngine;

public class DemoUITextHelper : MonoBehaviour
{
    [Header("General")]
    [SerializeField] private Transform objToObserve;

    [SerializeField] private bool isDynamic;

    enum Type
    {
        Neither, Slope, Stairs
    }
    [SerializeField] private Type objectType;

    [Header("Slope")]
    [Tooltip("This script can only provide positive rotation. \n Set to true to instead show (360 - Rotation)")]
    [SerializeField] private bool reverseRotation;

    [Header("Stairs")]
    [SerializeField] private bool showStepHeight;
    [SerializeField] private bool showStepDepth;


    [Header("Other")]
    [SerializeField] private TextMeshProUGUI displayText;



    private void Start()
    {
        LoadText();
    }

    private void Update()
    {
        if (isDynamic && objToObserve.hasChanged)
        {
            LoadText();
            objToObserve.hasChanged = false;
        }
    }

    private void LoadText()
    {
        string text = "";
        switch (objectType)
        {
            case Type.Slope:
                float rotation = objToObserve.eulerAngles.x;
                if (reverseRotation) { rotation = 360 - rotation; }
                text = "Degrees:\n" + Mathf.RoundToInt(rotation).ToString();

                SetText(text);
                break;
            case Type.Stairs:
                if (showStepHeight)
                {
                    float yScale = objToObserve.lossyScale.y;
                    text += "Step height: " + yScale.ToString("N2") + "\n";
                }
                if (showStepDepth)
                {
                    float zScale = objToObserve.lossyScale.z;
                    text += "Step depth: " + zScale.ToString("N1");
                }

                SetText(text);
                break;
            default:
                break;
        }
    }


    private void SetText(string pText)
    {
        displayText.text = pText;
    }



}
