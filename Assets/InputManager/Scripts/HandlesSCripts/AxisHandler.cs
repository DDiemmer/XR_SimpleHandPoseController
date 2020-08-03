using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace InputManager
{
    [CreateAssetMenu(fileName = "AxisHandler")]
    public class AxisHandler : InputHandler, ISerializationCallbackReceiver
    {

        public delegate void ValueChange(XRController controller, float value);
        public event ValueChange OnValueChange;
        public AxisType axis = AxisType.None;

        private InputFeatureUsage<float> inputFeature;
        private float previousValue = 0.0f;

        public void OnAfterDeserialize()
        {
            inputFeature = new InputFeatureUsage<float>(axis.ToString());
        }

        public void OnBeforeSerialize()
        {
            //Empty
        }
        public override void HandleState(XRController controller)
        {
            float value = GetValue(controller);
            if (value != previousValue)
            {
                previousValue = value;
                OnValueChange?.Invoke(controller, value);
            }
        }

        public float GetValue(XRController controller)
        {
            if (controller.inputDevice.TryGetFeatureValue(inputFeature, out float value))
                return value;

            return 0.0f;
        }
    }
}
