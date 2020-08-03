using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ExemploInputHandlers : MonoBehaviour
{
    public TextMeshProUGUI textButton;
    public TextMeshProUGUI textAxis;
    public TextMeshProUGUI textAxis2D;
    public TextMeshProUGUI textSecAxis2D;
    public InputManager.ButtonHandler primaryClickHandler = null;
    public InputManager.AxisHandler2D primaryAxisHandler = null;
    public InputManager.AxisHandler2D secundaryAxisHandler = null;
    public InputManager.AxisHandler triggerHandler = null;
    public InputManager.ButtonHandler AxisClickHandler = null;

    public void OnEnable()
    {
        primaryClickHandler.OnButtonDown += PrintPrimaryButtonDown;
        primaryClickHandler.OnButtonUp += PrintPrimaryButtonUp;
        primaryAxisHandler.OnValueChange += PrintPrimaryAxis;
        secundaryAxisHandler.OnValueChange += PrintSecundaryAxis;
        triggerHandler.OnValueChange += PrintTrigger;
        AxisClickHandler.OnButtonDown += PrintAxisClickDown;
        AxisClickHandler.OnButtonUp += PrintAxisClickUp;
    }

    public void OnDisable()
    {
        primaryClickHandler.OnButtonDown -= PrintPrimaryButtonDown;
        primaryClickHandler.OnButtonUp -= PrintPrimaryButtonUp;
        primaryAxisHandler.OnValueChange -= PrintPrimaryAxis;
        triggerHandler.OnValueChange -= PrintTrigger;
        AxisClickHandler.OnButtonDown -= PrintAxisClickDown;
        AxisClickHandler.OnButtonUp -= PrintAxisClickUp;
    }
    private void PrintPrimaryButtonDown(XRController controller)
    {
        print("Primary button down");
        Debug.Log("Primary button down");
        textButton.text = "Primary button down";
    }

    private void PrintPrimaryButtonUp(XRController controller)
    {
        print("Primary button up");
        Debug.Log("Primary button Up");
        textButton.text = "Primary button up";
    }
    private void PrintAxisClickDown(XRController controller)
    {
        print("Axis click down");
        Debug.Log("Axis click down");
        textButton.text = "Axis click down";
    }

    private void PrintAxisClickUp(XRController controller)
    {
        print("Axis click up");
        Debug.Log("Axis click Up");
        textButton.text = "Axis click up";
    }
    private void PrintPrimaryAxis(XRController controller, Vector2 value)
    {
        print("Left axis 2d:" + value);
        Debug.Log("Left axis:" + value);
        textAxis2D.text = "Left axis 2d" + value.ToString();
    }
    private void PrintSecundaryAxis(XRController controller, Vector2 value)
    {
        print("Right axis 2d:" + value);
        Debug.Log("Right axis:" + value);
        textSecAxis2D.text = "Right axis 2d" + value.ToString();
    }

    private void PrintTrigger(XRController controller, float value)
    {
        if (value >= 0.001f)
        {
            print("trigger :" + value);
            Debug.Log("trigger :" + value);
            textAxis.text = "trigger" + value.ToString();
        }
    }
}
