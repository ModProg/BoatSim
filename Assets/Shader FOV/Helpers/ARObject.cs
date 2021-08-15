using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;

public enum FovRestriction {
    Padding,
    Mask,
    Global
}

[System.Serializable]
public struct ShownArea {
    [MinMaxSlider(0.0f, 1.0f)]
    [Label("Horizontal (Left Eye, Right is mirrored)")]
    public Vector2 horizontal;
    [MinMaxSlider(0.0f, 1.0f)]
    public Vector2 vertical;

    public static ShownArea Full() {
        ShownArea sa;
        sa.horizontal = new Vector2(0, 1);
        sa.vertical = new Vector2(0, 1);
        return sa;
    }
}

[System.Serializable]
public struct Masks {
    [Tooltip("If omitted white is used everything is shown")]
    public Texture2D left;
    [Tooltip("If omitted white is used everything is shown")]
    public Texture2D right;
}

public class ARObject : MonoBehaviour {
    static Shader PADDING;
    static Shader MASK;

    public FovRestriction restriction_type;

    [ShowIf("restriction_type", FovRestriction.Padding)]
    public ShownArea shown_area = ShownArea.Full();
    [ShowIf("restriction_type", FovRestriction.Mask)]
    public Masks masks;

    // Start is called before the first frame update
    void Start() {
        if (!PADDING)
            PADDING = Resources.Load<Shader>("AR_Padding");

        if (masks.left == null)
            masks.left = Texture2D.whiteTexture;
        if (masks.right == null)
            masks.right = Texture2D.whiteTexture;

        var left = new GameObject("Left (" + name + ")");
        var right = new GameObject("Right (" + name + ")");

        var parent_mesh_renderer = GetComponent<MeshRenderer>();
        var parent_mesh_filter = GetComponent<MeshFilter>();

        foreach (var child in new[] { left, right }) {
            child.transform.parent = transform;
            child.transform.localPosition = Vector3.zero;
            child.transform.localRotation = Quaternion.identity;
            child.transform.localScale = Vector3.one;

            if (parent_mesh_filter) {
                var mf = child.AddComponent<MeshFilter>();
                mf.mesh = parent_mesh_filter.mesh;
            }

            if (parent_mesh_renderer) {
                var cc = child.AddComponent<MeshRenderer>();
            }
        }

        if (parent_mesh_renderer) {
            parent_mesh_renderer.enabled = false;
            var parent_material = parent_mesh_renderer.material;
            // Left
            var left_material = new Material(PADDING);
            left_material.CopyPropertiesFromMaterial(parent_material);
            left_material.SetVector("_Padding", new Vector4(shown_area.horizontal.x, shown_area.horizontal.y, shown_area.vertical.x, shown_area.vertical.y));

            var left_mesh_renderer = left.GetComponent<MeshRenderer>();
            left_mesh_renderer.material = left_material;

            // Right
            var right_material = new Material(PADDING);
            right_material.CopyPropertiesFromMaterial(parent_material);
            right_material.SetVector("_Padding", new Vector4(1 - shown_area.horizontal.y, 1 - shown_area.horizontal.x, shown_area.vertical.x, shown_area.vertical.y));

            var right_mesh_renderer = right.GetComponent<MeshRenderer>();
            right_mesh_renderer.material = right_material;
        }
    }

    // Update is called once per frame
    void Update() {

    }
}
