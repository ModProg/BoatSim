using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[ExecuteInEditMode]
public class SetUpPaths : MonoBehaviour
{
    public Material material;
    public float width = 30;

    void Update()
    {
        foreach (var mf in GetComponentsInChildren<MeshFilter>())
        {
            var lr = mf.GetComponent<LineRenderer>();
            if (lr == null)
            {
                lr = mf.gameObject.AddComponent<LineRenderer>();
                lr.startColor = lr.endColor = Random.ColorHSV(hueMin: 0, hueMax: .5f, saturationMin: 1, saturationMax: 1, valueMin: 1, valueMax: 1, alphaMin: 1, alphaMax: 1);
            }
            lr.startWidth = lr.endWidth = width;
            lr.gameObject.layer = this.gameObject.layer;
            lr.material = material;
            lr.positionCount = mf.sharedMesh.vertexCount;
            lr.SetPositions(mf.sharedMesh.vertices.Select(x => transform.TransformVector(x) + transform.position).ToArray());

        }
    }

}
