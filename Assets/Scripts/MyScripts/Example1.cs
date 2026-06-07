using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Example1 : MonoBehaviour
{
    public float maxSteeringAngle;
    public float maxMotorTorque;
    public float maxBrakeTorque;

    public WheelCollider leftWheel;
    public WheelCollider rightWheel;

    public GameObject visualLeftWheel;
    public GameObject visualRightWheel;


    public void FixedUpdate()
    {
        // Recogemos del Input los valores de aceleración,
        // giro y freno. 
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        float brake = maxBrakeTorque * Input.GetAxis("Jump");

        // Aplicamos a cada rueda estos valores 
        leftWheel.steerAngle = steering;
        leftWheel.motorTorque = motor;
        leftWheel.brakeTorque = brake;

        rightWheel.steerAngle = steering;
        rightWheel.motorTorque = motor;
        rightWheel.brakeTorque = brake;

        Vector3 position;
        Quaternion rotation;

        // Asignamos a la mesh de la rueda la posición
        // y el giro del collider 
        // Estos valores de posicidn y rotacitin pertenecen 
        // a los calculos hechos en el ciclo anterior, por lo que 
        // siempre ink) una iteraci6n por detras. 
        leftWheel.GetWorldPose(out position, out rotation);
        visualLeftWheel.transform.position = position;
        visualLeftWheel.transform.rotation = rotation;

        rightWheel.GetWorldPose(out position, out rotation);
        visualRightWheel.transform.position = position;
        visualRightWheel.transform.rotation = rotation;

    }
}

