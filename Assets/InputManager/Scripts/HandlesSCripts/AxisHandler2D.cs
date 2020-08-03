using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace InputManager
{
    [CreateAssetMenu(fileName = "AxisHandler2D")]
    public class AxisHandler2D : InputHandler, ISerializationCallbackReceiver
    {
        public delegate void ValueChange(XRController controller, Vector2 value);
        public event ValueChange OnValueChange;
        public Axis2DType axis = Axis2DType.None;

        private InputFeatureUsage<Vector2> inputFeature;
        private Vector2 previousValue = Vector2.zero;

        public void OnAfterDeserialize()
        {
            inputFeature = new InputFeatureUsage<Vector2>(axis.ToString());
        }

        public void OnBeforeSerialize()
        {
            //Empty
        }
        public override void HandleState(XRController controller)
        {
            Vector2 value = GetValue(controller);
            if (value != previousValue)
            {
                previousValue = value;
                OnValueChange?.Invoke(controller, value);
            }
        }

        public Vector2 GetValue(XRController controller)
        {
            if (controller.inputDevice.TryGetFeatureValue(inputFeature, out Vector2 value))
                return value;

            return Vector2.zero;
        }
    }
}