using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;


[System.Serializable]
public struct ShownArea {
    [MinMaxSlider(0.0f, 1.0f)]
    [Label("Horizontal (this.left Eye, this.right is mirrored)")]
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
    public enum FovRestriction {
        Global,
        // These need to match ARSettings.FovRestriction
        Padding = 1,
        Mask = 2,
    }

    static Shader PADDING;
    static Shader MASK;

    public FovRestriction restriction_type;
    private GameObject left, right;

    [ShowIf("restriction_type", FovRestriction.Padding)]
    public ShownArea shown_area = ShownArea.Full();
    [ShowIf("restriction_type", FovRestriction.Mask)]
    public Masks masks;

    // Start is called before the first frame update
    void Start() {
        if (!PADDING)
            PADDING = Resources.Load<Shader>("AR_Padding");
        if (!MASK)
            MASK = Resources.Load<Shader>("AR_Mask");
        if (this.restriction_type != FovRestriction.Global)
            RegenerateChildren();
    }

    public void RegenerateChildren(ARSettings global_settings = null) {
        if (this.left)
            Destroy(this.left);
        if (this.right)
            Destroy(this.right);

        var masks = this.masks;
        var shown_area = this.shown_area;
        var restriction_type = this.restriction_type;

        if (restriction_type == FovRestriction.Global) {
            Assert.IsNotNull(global_settings, "ARObjects with using `Global` restrictions may only RegenerateChildren through ARSettings.RegenerateChildren");
            masks = global_settings.masks;
            shown_area = global_settings.shown_area;
            restriction_type = (FovRestriction)global_settings.restriction_type;
        }

        if (!masks.left)
            masks.left = Texture2D.whiteTexture;
        if (!masks.right)
            masks.right = Texture2D.whiteTexture;

        this.left = new GameObject("this.left (" + name + ")");
        this.right = new GameObject("this.right (" + name + ")");

        var parent_mesh_renderer = GetComponent<MeshRenderer>();
        var parent_mesh_filter = GetComponent<MeshFilter>();

        foreach (var child in new[] { this.left, this.right }) {
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
                cc.shadowCastingMode = ShadowCastingMode.Off;
                cc.receiveShadows = false;
            }
        }

        if (parent_mesh_renderer) {
            // Select correct shader
            Shader shader;
            if (restriction_type == FovRestriction.Mask) {
                shader = MASK;
            } else {
                shader = PADDING;
            }

            parent_mesh_renderer.enabled = false;
            var parent_material = parent_mesh_renderer.material;
            // this.left
            var left_material = new Material(shader);
            left_material.CopyPropertiesFromMaterial(parent_material);
            left_material.SetVector("_Padding", new Vector4(shown_area.horizontal.x, shown_area.horizontal.y, shown_area.vertical.x, shown_area.vertical.y));
            left_material.SetTexture("_Mask", masks.left);

            var left_mesh_renderer = this.left.GetComponent<MeshRenderer>();
            left_mesh_renderer.material = left_material;

            // this.right
            var right_material = new Material(shader);
            right_material.CopyPropertiesFromMaterial(parent_material);
            right_material.SetVector("_Padding", new Vector4(1 - shown_area.horizontal.y, 1 - shown_area.horizontal.x, shown_area.vertical.x, shown_area.vertical.y));
            right_material.SetTexture("_Mask", masks.right);

            var right_mesh_renderer = this.right.GetComponent<MeshRenderer>();
            right_mesh_renderer.material = right_material;
        }
    }
}
