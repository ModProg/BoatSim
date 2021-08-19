using System.Collections;
using System.Reflection;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;


[System.Serializable]
public struct ShownArea {
    [MinMaxSlider(0.0f, 1.0f)]
    [Label("Horizontal (left eye, right is mirrored)")]
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

public enum FovRestrictionObject {
    Global,
    // These need to match ARSettings.FovRestriction
    Padding = 1,
    Mask = 2,
}
public class ARObject : MonoBehaviour {

    static Shader PADDING;
    static Shader MASK;
    static Shader PADDING_TEXT;
    static Shader MASK_TEXT;

    public FovRestrictionObject restriction_type;
    private GameObject left, right;

    [ShowIf("restriction_type", FovRestrictionObject.Padding)]
    public ShownArea shown_area = ShownArea.Full();
    [ShowIf("restriction_type", FovRestrictionObject.Mask)]
    public Masks masks;

    // Start is called before the first frame update
    void Start() {
        if (this.restriction_type != FovRestrictionObject.Global)
            RegenerateChildren();
    }

    public void RegenerateChildren(ARSettings global_settings = null) {
        if (!PADDING)
            PADDING = Resources.Load<Shader>("AR_Padding");
        if (!MASK)
            MASK = Resources.Load<Shader>("AR_Mask");
        if (!PADDING_TEXT)
            PADDING_TEXT = Resources.Load<Shader>("AR_Text_Padding");
        if (!MASK_TEXT)
            MASK_TEXT = Resources.Load<Shader>("AR_Text_Mask");

        if (this.left)
            Destroy(this.left);
        if (this.right)
            Destroy(this.right);

        FovRestriction restriction_type;
        if (this.restriction_type == FovRestrictionObject.Global) {
            Assert.IsNotNull(global_settings, "ARObjects with using `Global` restrictions may only RegenerateChildren through ARSettings.RegenerateChildren");
            masks = global_settings.masks;
            shown_area = global_settings.shown_area;
            restriction_type = global_settings.restriction_type;
        } else {
            var masks = this.masks;
            var shown_area = this.shown_area;
            restriction_type = (FovRestriction)this.restriction_type;
        }

        if (!masks.left)
            masks.left = Texture2D.whiteTexture;
        if (!masks.right)
            masks.right = Texture2D.whiteTexture;

        this.left = new GameObject("Left (" + name + ")");
        this.left.layer = 9;
        this.right = new GameObject("Right (" + name + ")");
        this.right.layer = 10;

        var parent_mesh_renderer = GetComponent<MeshRenderer>();
        var parent_mesh_filter = GetComponent<MeshFilter>();
        var parent_text = GetComponent<TextMeshPro>();
        var parent_rect = GetComponent<RectTransform>();

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

            if (parent_text) {
                var text = child.AddComponent<TextMeshPro>();

                var fields = typeof(TextMeshPro).GetProperties();
                foreach (var field in fields) {
                    if (field.CanRead && field.CanWrite)
                        field.SetValue(text, field.GetValue(parent_text));
                }
            }

            if (parent_rect) {
                var rect = child.GetComponent<RectTransform>();
                rect.sizeDelta = parent_rect.sizeDelta;
            }

        }

        if (parent_mesh_renderer) {
            // Select correct shader
#pragma warning disable 8524
            Shader shader = (restriction_type, parent_text) switch {
                (FovRestriction.Mask, null) => MASK,
                (FovRestriction.Padding, null) => PADDING,
                (FovRestriction.Mask, _) => MASK_TEXT,
                (FovRestriction.Padding, _) => PADDING_TEXT,
            };
#pragma warning restore 8524


            parent_mesh_renderer.enabled = false;
            var parent_material = parent_mesh_renderer.material;
            // left
            var left_material = new Material(shader);
            left_material.CopyPropertiesFromMaterial(parent_material);
            left_material.SetVector("_Padding", new Vector4(shown_area.horizontal.x, shown_area.horizontal.y, shown_area.vertical.x, shown_area.vertical.y));
            left_material.SetTexture("_Mask", masks.left);

            var left_mesh_renderer = this.left.GetComponent<MeshRenderer>();
            left_mesh_renderer.material = left_material;

            // right
            var right_material = new Material(shader);
            right_material.CopyPropertiesFromMaterial(parent_material);
            right_material.SetVector("_Padding", new Vector4(1 - shown_area.horizontal.y, 1 - shown_area.horizontal.x, shown_area.vertical.x, shown_area.vertical.y));
            right_material.SetTexture("_Mask", masks.right);

            var right_mesh_renderer = this.right.GetComponent<MeshRenderer>();
            right_mesh_renderer.material = right_material;
        }
    }

    private bool hasText() {
        return GetComponent<TextMeshPro>();
    }

    [ShowIf("hasText")]
    [Button]
    public void UpdateText() {
        var parent_text = GetComponent<TextMeshPro>();
        foreach (var child in new[] { this.left, this.right }) {
            var text = child.GetComponent<TextMeshPro>();
            var fields = typeof(TextMeshPro).GetProperties();
            foreach (var field in fields) {
                if (field.CanRead && field.CanWrite)
                    field.SetValue(text, field.GetValue(parent_text));
            }
        }
    }
}
