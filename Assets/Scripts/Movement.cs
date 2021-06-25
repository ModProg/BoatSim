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
    public float f_p
    {
        get { return (throttleLever.localEulerAngles.x > 180 ? throttleLever.localEulerAngles.x - 360 : throttleLever.localEulerAngles.x) * max_f_p; }
    }

    [Tooltip("propeller diameter")]
    public float D;

    // Propeller Force
    public Vector3 T
    {
        get { return Mathf.Sign(f_p) * (K_T * f_p * f_p * D * D * D * D * n_p * transform.localToWorldMatrix.MultiplyVector(Vector3.forward)); }
    }

    // acceleration
    public Vector3 a_s
    {
        get { return (T - R_s) / M; }
    }

    // rudder force
    public float F_rd
    {
        get { return K_rd * A_rd * v_w * v_w * Theta_rd; }
    }

    // water velocity
    public float v_w
    {
        get { return v_s.magnitude; }
    }

    [Tooltip("rudder coefficient")]
    public float K_rd;
    [Tooltip("rudder area")]
    public float A_rd;

    public float Theta_rd
    {
        get { return Mathf.Deg2Rad * (steeringWheel.localEulerAngles.z > 180 ? steeringWheel.localEulerAngles.z - 360 : steeringWheel.localEulerAngles.z) * 35 / 120; }
    }

    // yaw net force
    public float F_y
    {
        get { return F_rd - b_y * I_y * omega_y; }
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

    // angular acceleration for yaw
    public float a_y
    {
        get { return F_y / I_y; }
    }

    // force of current
    public Vector3 F_c
    {
        get { return v_c* K_c; }
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

    private void Start()
    {
    }


    private void FixedUpdate()
    {
        v_s += a_s * Time.fixedDeltaTime;
        Debug.LogError(F_c);

        omega_y += a_y * Time.fixedDeltaTime;

        v_sw += a_sw * Time.deltaTime;

        world.position -= v_s * Time.fixedDeltaTime + v_sw * Time.deltaTime;

        world.RotateAround(transform.position, Vector2.down, omega_y * Time.fixedDeltaTime);

        var dir = v_c;
        if (dir.magnitude > 0){
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
