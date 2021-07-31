using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BoatTest : MonoBehaviour
{
    BoatModel boat;

    private void Start()
    {
        boat = new SimpleBoat();
    }


    private void FixedUpdate()
    {
        var throttle = Input.GetAxis("Vertical");
        var steering = Input.GetAxis("Horizontal");

        var mov = boat.Update(Time.fixedDeltaTime, throttle, steering);
        transform.position += mov.delta_pos;

        transform.RotateAround(transform.position, Vector3.up, mov.delta_rot.y);
        transform.RotateAround(transform.position, Vector3.forward, mov.delta_rot.z);
        transform.RotateAround(transform.position, Vector3.right, mov.delta_rot.x);
    }
}