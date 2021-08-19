using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedTrack : MonoBehaviour
{
    public Transform tracked;
    public float delay;
    public float progress;

    void Update()
    {
        progress += Time.deltaTime;

        if (progress > delay)
        {
            this.transform.SetPositionAndRotation(tracked.position, tracked.rotation);
            progress = 0;
        }
    }
}
