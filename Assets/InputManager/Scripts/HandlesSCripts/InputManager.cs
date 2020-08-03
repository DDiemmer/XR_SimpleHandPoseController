using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

namespace InputManager
{
    public class InputManager : MonoBehaviour
    {
        public List<ButtonHandler> allButtonsHandlers = new List<ButtonHandler>();
        public List<AxisHandler> allAxisHandlers = new List<AxisHandler>();
        public List<AxisHandler2D> allAxisHandlers2D = new List<AxisHandler2D>();

        private XRController controller;

        private void Awake()
        {
            controller = GetComponent<XRController>();
        }

        // Update is called once per frame
        void Update()
        {
            HandleButtonEvents();
            HandleAxis2dEvents();
            HandleAxisEvents();
        }

        private void HandleButtonEvents()
        {
            foreach (ButtonHandler handler in allButtonsHandlers)
            {
                handler.HandleState(controller);
            }
        }

        private void HandleAxis2dEvents()
        {
            foreach (AxisHandler2D handler in allAxisHandlers2D)
            {
                handler.HandleState(controller);
            }
        }

        private void HandleAxisEvents()
        {
            foreach (AxisHandler handler in allAxisHandlers)
            {
                handler.HandleState(controller);
            }
        }
    }
}