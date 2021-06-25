using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rendering : MonoBehaviour
{
    public int width, heigth;

    public Camera ar;
    public Camera vr;


    RenderTexture depthAR;
    RenderTexture depthVR;
    RenderTexture colorAR;

    void Start()
    {
        depthAR = new RenderTexture(width, heigth, 24, RenderTextureFormat.Depth);
        colorAR = new RenderTexture(width, heigth, 0, RenderTextureFormat.Default);

        depthVR = new RenderTexture(width, heigth, 24, RenderTextureFormat.Depth);

        // To fix unnecesary confusion
        ar.stereoTargetEye = StereoTargetEyeMask.None;
        ar.stereoTargetEye = StereoTargetEyeMask.None;

        ar.SetTargetBuffers(colorAR.colorBuffer, depthAR.depthBuffer);
        vr.SetTargetBuffers(depthVR.colorBuffer, depthVR.depthBuffer);

        var material = GetComponent<Renderer>();
        material.material.SetTexture("_Color", colorAR);
        material.material.SetTexture("_DepthFG", depthAR);
        material.material.SetTexture("_DepthBG", depthVR);
    }
}
