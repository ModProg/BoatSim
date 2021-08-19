using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class RandomText : MonoBehaviour {
    [MinMaxSlider(0, 100)]
    public Vector2Int Length;
    private float progress;

    // Update is called once per frame
    void Update() {

        progress += Time.deltaTime;

        if (progress > 0.5) {
            progress = 0;
            var text = GetComponent<TextMeshPro>();
            var len = Random.Range(Length[0], Length[1]);
            var s = "";
            var chars = "                  ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            for (int i = 0; i < len; i++) {
                s += chars[Random.Range(0, chars.Length)];
            }
            text.text = s;
            GetComponent<ARObject>().UpdateText();
        }
    }
}
