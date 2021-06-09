using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TextMesh))]
public class FlowSpeed : MonoBehaviour
{
    public Texture2D texture;
    TextMesh textMesh;
    // Start is called before the first frame update
    void Start()
    {
        this.textMesh = GetComponent<TextMesh>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Debug.Log(Physics.Raycast(transform.position, Vector3.down, out hit, 50, LayerMask.GetMask("Water")));
        Debug.Log("texturecord: " + hit.textureCoord);
        var pixuv = hit.textureCoord;
        pixuv.x *= texture.width;
        pixuv.y *= texture.height;
        var pixel = texture.GetPixel((int)pixuv.x, (int)pixuv.y);
        var x = pixel.r * 2f - 1f;
        var y = pixel.g * 2f - 1f;
        Debug.Log("x:" + pixel.r + " y:" + pixel.g + " v:" + Mathf.Sqrt(x * x + y * y) / 0.514444);

        textMesh.text = Mathf.Sqrt(x * x + y * y) + "m/s";
    }
}
