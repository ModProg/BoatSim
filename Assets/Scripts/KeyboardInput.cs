using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardInput : MonoBehaviour
{
    public Transform throttleLever;
    public Transform steeringWheel;

    public float SteeringSpeed = 1;
    public float ThrottleSpeed = 1;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        var old_s = steeringWheel.rotation;
        var old_t = throttleLever.rotation;

        float d = Time.deltaTime;
        float h = Input.GetAxis("Horizontal");
        steeringWheel.Rotate(new Vector3(0, 0, h * d * SteeringSpeed));
        float v = Input.GetAxis("Vertical");
        throttleLever.Rotate(new Vector3(v * d * ThrottleSpeed, 0, 0));

        if (Input.GetButtonDown("Reset"))
        {
            throttleLever.localEulerAngles = Vector3.zero;
            steeringWheel.localEulerAngles = Vector3.zero;
        }

        // -50 to 50
        var throttle = (throttleLever.localEulerAngles.x > 180 ? throttleLever.localEulerAngles.x - 360 : throttleLever.localEulerAngles.x);
        if (Mathf.Abs(throttle) > Movement.MAXTHROTTLE)
            throttleLever.rotation = old_t;
        // -120 to 120 Neg is left, Pos is right
        var steering = (steeringWheel.localEulerAngles.z > 180 ? steeringWheel.localEulerAngles.z - 360 : steeringWheel.localEulerAngles.z);
        if (Mathf.Abs(steering) > Movement.MAXSTEERING)
            steeringWheel.rotation = old_s;
    }
}
