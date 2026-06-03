using System;
using UnityEngine;

//adapted script to work with my input instead of the crossplatform input package
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace UnityStandardAssets.Vehicles.Car
{
    [RequireComponent(typeof (CarController))]
    public class CarUserControl : MonoBehaviour
    {
        private CarController m_Car; // the car controller we want to use
        private InputAction m_MoveAction;
        private InputAction m_HandbrakeAction;

        private void Awake()
        {
            // get the car controller
            m_Car = GetComponent<CarController>();
        }
        private void Start()
        {
            m_MoveAction = GameManager.Instance.GetInput().actions["CarMove"];
            m_HandbrakeAction = GameManager.Instance.GetInput().actions["Handbrake"];
        }

        private void FixedUpdate()
        {
            float h = 0f;
            float v = 0f;
            float handbrake = 0f;

            if (m_MoveAction != null)
            {
                Vector2 moveInput = m_MoveAction.ReadValue<Vector2>();
                h = moveInput.x;
                v = moveInput.y;
            }

            if (m_HandbrakeAction != null)
            {
                //Reading as a float so a gamepad trigger or button press maps from 0 to 1
                handbrake = m_HandbrakeAction.ReadValue<float>();
            }

            //pass the input to the car!
            m_Car.Move(h, v, v, handbrake);
        }

        private void OnEnable()
        {
            if (m_Car != null)
            {
                m_Car.IsOccupied = true;
            }
        }

        private void OnDisable()
        {
            if (m_Car != null)
            {
                m_Car.IsOccupied = false;
            }
        }
    }
}
