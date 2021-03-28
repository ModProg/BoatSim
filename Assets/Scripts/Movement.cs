using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{

    public Transform throttleLever;
    public Transform steeringWheel;

    public float mass;
    public float propellerThrust;

    public float forwardResistance;

    public float rudderKoeffizient;

    public float yawResistance;

    public float inertiaFactor;

    public Vector3 velocity;
    public float yawVelocity;

    public Transform world;

    private void Start()
    {
    }


    private void FixedUpdate()
    {
        // -50 to 50
        var throttle = (throttleLever.localEulerAngles.x > 180 ? throttleLever.localEulerAngles.x - 360 : throttleLever.localEulerAngles.x) * transform.localToWorldMatrix.MultiplyVector(Vector3.forward);
        // -120 to 120 Neg is left, Pos is right
        var steering = (steeringWheel.localEulerAngles.z > 180 ? steeringWheel.localEulerAngles.z - 360 : steeringWheel.localEulerAngles.z) * 35 / 120;

        var a = (propellerThrust * throttle - forwardResistance * velocity) / mass;

        velocity = a * Time.fixedDeltaTime + velocity;

        var I = inertiaFactor * mass;

        var ay = (rudderKoeffizient * velocity.magnitude * steering - yawResistance * yawVelocity) / I;

        yawVelocity = ay * Time.fixedDeltaTime;


        world.position -= velocity * Time.fixedDeltaTime;

        world.RotateAround(transform.position, Vector2.down, yawVelocity * Time.fixedDeltaTime);
        Debug.Log(-yawVelocity * Time.fixedDeltaTime);
    }
}
