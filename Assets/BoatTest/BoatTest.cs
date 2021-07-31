using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTest : MonoBehaviour
{
    public float mass;
    public float propellerThrust;

    public float forwardResistance;

    public float rudderKoeffizient;

    public float yawResistance;

    public float inertiaFactor;

    public Vector3 velocity;
    public float yawVelocity;

    private void Start()
    {
    }


    private void FixedUpdate()
    {
        // -50 to 50
        var throttle = Input.GetAxis("Vertical") * 50 * transform.localToWorldMatrix.MultiplyVector(Vector3.forward);
        // -120 to 120 Neg is left, Pos is right
        var steering = -Input.GetAxis("Horizontal") * 35;

        var a = (propellerThrust * throttle - forwardResistance * velocity) / mass;

        velocity = a * Time.fixedDeltaTime + velocity;

        var I = inertiaFactor * mass;

        var ay = (rudderKoeffizient * velocity.magnitude * steering - yawResistance * yawVelocity) / I;

        yawVelocity = ay * Time.fixedDeltaTime;


        transform.position += velocity * Time.fixedDeltaTime;

        transform.RotateAround(transform.position, Vector2.down, yawVelocity * Time.fixedDeltaTime);
        Debug.Log(-yawVelocity * Time.fixedDeltaTime);
    }
}