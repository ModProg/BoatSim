using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

public enum FovRestriction {
    // These need to match FovRestrictionObject
    Padding = 1,
    Mask = 2,
}

[DisallowMultipleComponent]
public class ARSettings : MonoBehaviour {

    public FovRestriction restriction_type;

    [ShowIf("restriction_type", FovRestriction.Padding)]
    public ShownArea shown_area = ShownArea.Full();
    [ShowIf("restriction_type", FovRestriction.Mask)]
    public Masks masks;

    void Start() {
        RegenerateChildren();
    }

    public void RegenerateChildren() {
        foreach (ARObject ar_object in GetComponentsInChildren<ARObject>()) {
            ar_object.RegenerateChildren(this);
        }
    }
}