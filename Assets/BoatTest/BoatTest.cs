using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoatTest : MonoBehaviour
{
    //UengBoat boat;
    //NaiveBoat boat;
    states_variables sv;
    // public float rot;
    // public float surge;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // boat.rot_power=rot;
        // boat.power=surge;
        var n = 10 * Input.GetAxis("Vertical");
        var delta = 35 * Input.GetAxis("Horizontal");
        sv = BenedictBoat.Simulate(n, delta, sv);
        transform.position = new Vector3(sv.Track_coordinate_x, 0, sv.Track_coordinate_y);

        transform.eulerAngles = new Vector3(0, -sv.heading, 0);
    }
}
