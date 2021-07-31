using UnityEngine;

public class UengBoat
{
    // Configuration
    // Table 4
    public float length = 38;
    public float width = 8;
    public float mass = 250_000;
    public float rudder_area = 1;
    public float propeller_diameter = 2;
    public float propeller_revolution = 180;

    // Table 5
    public float K_h = 0.7f;
    public float K_p = 0.8f;
    public float K_r = 0.7f;
    public float K_T = 0.9f;
    public float K_rd = 577f;
    public float K_w = 0.3f;
    public float K_c = 0.3f;

    // Surge
    Vector2 v_s;
    float b_s = 0.05f;

    public void apply_surge(float throttle = 0)
    {
        // aliases:
        var M = mass;
        var n = propeller_revolution;
        var D = propeller_diameter;

        var R_s = b_s * M * v_s.sqrMagnitude * v_s.normalized;
        // TODO propeller count
        var T = K_T * Mathf.Pow(n, 2) * Mathf.Pow(D, 4) * (Vector2)(Quaternion.Euler(0, 0, rotation * Mathf.Rad2Deg) * Vector2.up);
        T *= throttle;
        var a_s = (T - R_s) / M;
        v_s = v_s + a_s * Time.fixedDeltaTime;
        var surge = v_s * Time.fixedDeltaTime;
        position += surge;
    }

    // TODO Yaw
    float w_y;
    float b_y = 0.05f;
    float b_I = 1;

    // Input
    public void apply_yaw(float angle = 0)
    {
        // aliases:
        var M = mass;
        var L = length;
        var A_rd = rudder_area;

        var F_rd = 500 * K_rd * A_rd * v_s.sqrMagnitude * angle * 35;
        var I_y = b_I * M * L * L / 12.0f;
        var F_y = F_rd - b_y * I_y * w_y;
        var a_y = F_y / I_y;
        w_y = a_y * Time.fixedDeltaTime;
        var O_y = w_y * Time.fixedDeltaTime;
        rotation += O_y;
    }
    // Sway


    public Vector2 position;
    public float rotation;

    public Vector3 position3 { get { return new Vector3(position.x, 0, position.y); } }
}

public class NaiveBoat
{
    // State
    public float rotation;
    float rot_velocity;
    float rot_acceleration;

    Vector2 position;
    Vector2 velocity;
    Vector2 acceleration;

    // Config
    public float power;
    public float rot_power;

    // Output
    public Vector3 position3 { get { return new Vector3(position.x, 0, position.y); } }

    public void apply_surge(float throttle = 0)
    {
        var forward = (Vector2)(Quaternion.Euler(0, 0, rotation) * Vector2.up);
        acceleration += throttle * forward * power * Time.fixedDeltaTime;
        velocity += acceleration * Time.fixedDeltaTime;
        position += velocity * Time.fixedDeltaTime;
    }

    public void apply_yaw(float rudderangle = 0)
    {
        rot_acceleration += rudderangle * rot_power * Time.fixedDeltaTime * velocity.sqrMagnitude;
        rot_velocity += rot_acceleration * Time.fixedDeltaTime;
        rotation += rot_velocity * Time.fixedDeltaTime;
    }

}

public struct states_variables
{
    public float Speed_component_vx;
    public float Speed_component_vy;
    public float Rate_of_turn_r;
    public float Speed_V;
    public float Drift_angle_Beta;
    public float turning_rate_r;
    public float heading;
    public float Track_coordinate_x;
    public float Track_coordinate_y;
}

struct KinematicParametersAndTrack
{
    public float Speed_V;
    public float Drift_angle_Beta;
    public float turning_rate_r;
    public float heading;
    public float Track_coordinate_x;
    public float Track_coordinate_y;
}

public static class BenedictBoat
{
    static float L = 10;
    static float Ar = 4;
    static float m = 200_000;
    static float dt { get { return Time.fixedDeltaTime; } }
    static float cx = 30000;
    static float cyr = 30000;
    static float rho = 1000;
    static float I = m * L * L / 12f;
    static float K_T = 1;
    static float C_R = 577;
    static float D = 2;
    static float A_R = 2;
    static states_variables SubsystemKinematicParametersAndTrack(float vx, float vy, float rate_of_turn, states_variables prev)
    {
        var kpat = new states_variables();
        kpat.Speed_V = Mathf.Sqrt(vx * vx + vy * vy);
        kpat.Drift_angle_Beta = Mathf.Atan2(vy, vx) * Mathf.PI / 180f;
        kpat.turning_rate_r = rate_of_turn * L / kpat.Speed_V;
        // This is guessed
        kpat.heading = prev.heading + rate_of_turn * dt;

        var Course_over_ground_phi = kpat.heading - kpat.Drift_angle_Beta;
        kpat.Track_coordinate_x = prev.Track_coordinate_x + dt * (vx * vx + vy * vy) * Mathf.Cos(Course_over_ground_phi);
        kpat.Track_coordinate_y = prev.Track_coordinate_y + dt * (vx * vx + vy * vy) * Mathf.Sin(Course_over_ground_phi);
        return kpat;
    }

    static float EquationOfMotionBalanceOfLongitudinalForces(float n, states_variables sv)
    {
        // Calculation of Resistance
        var Res = -cx * Mathf.Abs(sv.Speed_component_vx) * sv.Speed_component_vx;
        // Calculation of Thrust in a Subsystem
        var Thr = SubsystemForCalculationOfThrust(n);
        // Longitudinal force centrifugal part
        var Cen = m * sv.Speed_component_vy * sv.Rate_of_turn_r;

        return sv.Speed_component_vx + dt * (Res + Thr + Cen) / m;
    }

    static float SubsystemForCalculationOfThrust(float n)
    {
        // T = K_T * ρ * D^4 * n²
        return K_T * rho * (D * D * D * D) * (n * n);
    }

    static float EquationOfMotionBalanceOfTransverseForces(float delta, states_variables sv)
    {
        // Rudder force BST (556)
        var Rud = C_R * A_R * (sv.Speed_V * sv.Speed_V * Mathf.Sin(delta * Mathf.Deg2Rad));
        // Hull force (Polynominal)
        // TODO
        // Hull force Centrifugal part
        var Cen = m * sv.Speed_component_vx * sv.Rate_of_turn_r;

        return sv.Speed_component_vy + dt * (Rud + 0 + Cen) / m;
    }

    static float EquationOfMotionBalanceOfYawingMoments(float delta, states_variables sv)
    {
        // Hull moment (Polynominal)
        // TODO
        // Rudder torque
        var Rud = cyr * delta * (rho / 2f) * (sv.Speed_V * sv.Speed_V) * Ar * L / 2;

        return sv.turning_rate_r + dt * (0 + Rud) / I;
    }

    public static states_variables Simulate(float n, float delta, states_variables sv)
    {
        //var ret = new states_variables();
        var Speed_component_vx = EquationOfMotionBalanceOfLongitudinalForces(n, sv);
        var Speed_component_vy = EquationOfMotionBalanceOfTransverseForces(delta, sv);
        var turning_rate_r = EquationOfMotionBalanceOfTransverseForces(delta, sv);

        var ret = SubsystemKinematicParametersAndTrack(Speed_component_vx, Speed_component_vy, turning_rate_r, sv);
        ret.Speed_component_vx = Speed_component_vx;
        ret.Speed_component_vy = Speed_component_vy;
        ret.turning_rate_r = turning_rate_r;
        return ret;
    }
}

// https://scholarworks.calstate.edu/downloads/k643b207w
// public class BenedictBoat
// {
//     // state
//     float velocity = 0;
//     float position = 0;

//     // Output
//     public Vector3 position3 { get { return new Vector3(0, 0, position); } }

//     public void update(float t)
//     {
//         velocity += acceleration() * t;
//         position += velocity * t;
//     }

//     public void log()
//     {
//         Debug.Log(velocity);
//     }

//     float ship_mass = 240_000;
//     float thrust_deduction = 0;
//     float mx() { return 0; }
//     float acceleration()
//     {
//         // a = (T*(1-t) +R )/(m+m_x)
//         return (thrust() * (1f - thrust_deduction) + resistance()) / (ship_mass + mx());
//     }

//     float resistance_coefficient = 3;
//     float ship_area = 500;
//     float resistance()
//     {
//         // C_R * ρ/2 * v² * A
//         return - resistance_coefficient * density_of_water * (velocity * velocity) * ship_area;
//     }

//     float thrust_coefficient = 100;
//     float density_of_water = 1;
//     float propeller_diameter = 2;
//     float revolutions_per_second = 10;//3;
//     float thrust()
//     {
//         // T = K_T * ρ * D^4 * n²
//         return thrust_coefficient * density_of_water * Mathf.Pow(propeller_diameter, 4) * Mathf.Pow(revolutions_per_second, 2);
//     }

// }