using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Texture2D texture;

    public Transform waterWindZone;

    public Transform throttleLever;
    public Transform steeringWheel;

    public LayerMask dontHitLayers;

    [Tooltip("drag force coefificient")]
    public float b_s;

    [Tooltip("ship mass")]
    public float M;

    // resistance(dragforce)
    public Vector3 R_s
    {
        get { return b_s * M * v_s.sqrMagnitude * v_s.normalized; }
    }

    [Tooltip("thrust coefficient of propeller")]
    public float K_T;


    [Tooltip("number of Propellers")]
    public int n_p;

    [Tooltip("propeller revolution")]
    public float max_f_p;

    //[Tooltip("propeller revolution speed")]
    public float throttle
    {
        get { return (throttleLever.localEulerAngles.x > 180 ? throttleLever.localEulerAngles.x - 360 : throttleLever.localEulerAngles.x) / -MAXTHROTTLE; }
    }

    [Tooltip("propeller diameter")]
    public float D;

    // water velocity
    public float v_w
    {
        get { return v_s.magnitude; }
    }

    [Tooltip("rudder coefficient")]
    public float K_rd;
    [Tooltip("rudder area")]
    public float A_rd;

    public float steering
    {
        get { return (steeringWheel.localEulerAngles.z > 180 ? steeringWheel.localEulerAngles.z - 360 : steeringWheel.localEulerAngles.z) / MAXSTEERING; }
    }

    [Tooltip("resistance coefficient for yaw")]
    public float b_y;

    [Tooltip("length of the ship")]
    public float L;

    // inertia moment of yaw
    public float I_y
    {
        get { return M * L * L / 12; }
    }
    // force of current
    public Vector3 F_c
    {
        get { return v_c * K_c; }
    }

    public Vector3 v_c
    {
        get
        {
            RaycastHit hit;
            Physics.Raycast(transform.position, Vector3.down, out hit, 50, LayerMask.GetMask("Water"));
            var pixuv = hit.textureCoord;
            pixuv.x *= texture.width;
            pixuv.y *= texture.height;
            var pixel = texture.GetPixel((int)pixuv.x, (int)pixuv.y);
            var x = pixel.r * 2f - 1f;
            var y = pixel.g * 2f - 1f;
            return new Vector3(y, 0, x);
        }
    }

    // coefficient of current
    public float K_c;

    public Vector3 F_sw
    {
        get { return F_c - (b_sw * M * v_sw.magnitude * v_sw.magnitude) * v_sw.normalized; }
    }

    [Tooltip("resistance coefficient of sway")]
    public float b_sw;

    public Vector3 a_sw
    {
        get { return F_sw / M; }
    }

    [Tooltip("velocity of sway")]
    public Vector3 v_sw;

    public Vector3 v_s;
    public float omega_y;

    public Transform world;

    public const float MAXTHROTTLE = 50;
    public const float MAXSTEERING = 120;

    BoatModel boat;
    private void Start()
    {
        boat = new SimpleBoat();
    }


    private void FixedUpdate()
    {

        var mov = boat.Update(Time.fixedDeltaTime, throttle, steering);
        Debug.Log(throttle);
        Debug.Log(steering);
        world.position -= mov.delta_pos;

        world.RotateAround(transform.position, Vector3.down, mov.delta_rot.y);
        world.RotateAround(transform.position, Vector3.back, mov.delta_rot.z);
        world.RotateAround(transform.position, Vector3.left, mov.delta_rot.x);

        var dir = v_c;
        if (dir.magnitude > 0)
        {
            waterWindZone.LookAt(waterWindZone.position + dir);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if ((dontHitLayers.value & (2 << (other.gameObject.layer - 1))) != 0)
        {
            Reset();
        }
    }

    public void Reset()
    {
        world.transform.position = Vector3.zero;
        world.rotation = Quaternion.identity;
        throttleLever.localEulerAngles = Vector3.zero;
        steeringWheel.localEulerAngles = Vector3.zero;

        v_s = Vector3.zero;
        omega_y = 0;
    }
}
