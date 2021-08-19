using UnityEngine;

public class FPSRestrictor : MonoBehaviour {
    public float fps = 20;
    float elapsed;
    Camera[] cams;

    void Start() {
        cams = GetComponentsInChildren<Camera>();
        foreach (var cam in cams)
                cam.enabled = false;
    }

    void Update() {
        elapsed += Time.deltaTime;
        if (elapsed > 1 / fps) {
            elapsed = 0;
            foreach (var cam in cams)
                cam.Render();
        }
    }
}