using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boat : MonoBehaviour
{
    // Start is called before the first frame update
    public Vector3[] path;
    public Transform pathT;
    public float speed = 3;
    public float progress = 0;
    public Vector3 startRot;


    // Update is called once per frame
    void Update()
    {
        var lastI = Mathf.FloorToInt(progress);
        if (lastI >= path.Length - 1)
        {
            Debug.Log("DIE");
            Destroy(this.gameObject, 0);
            return;
        }
        var last = pathT.TransformVector(path[lastI]) + pathT.position;
        var next = pathT.TransformVector(path[Mathf.Max(Mathf.CeilToInt(progress), lastI + 1)]) + pathT.position;
        progress += speed * Time.deltaTime / Vector3.Distance(next, last);

        transform.rotation = Quaternion.LookRotation(next - last);
        transform.localEulerAngles += startRot;
        transform.position = Vector3.Lerp(last, next, progress - lastI);
    }
}
