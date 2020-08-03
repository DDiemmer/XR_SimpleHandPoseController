
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InputManager
{
    public class InputHandler : ScriptableObject
    {
        public virtual void HandleState(XRController controller)
        {
            // Empty
        }
    }
}