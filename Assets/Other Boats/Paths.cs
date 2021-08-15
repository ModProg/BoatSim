using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Paths : MonoBehaviour
{
    public float delay = 10;
    private float current;
    public Transform ships;

    public PathFollow[] pathFollows;
    private MeshFilter[] paths;
    // Start is called before the first frame update
    void Start()
    {
        paths = GetComponentsInChildren<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(current);
        current += Time.deltaTime;
        if (current >= delay)
        {
            current -= delay;
            var boat = pathFollows[Random.Range(0, pathFollows.Length)];
            var path = paths[Random.Range(0, paths.Length)];

            var instance = Instantiate(boat, transform.TransformVector(path.mesh.vertices[0]) + transform.position, transform.rotation * boat.transform.rotation, ships);
            instance.path = path.mesh.vertices;
            instance.pathT = path.transform;
        }
    }
}
