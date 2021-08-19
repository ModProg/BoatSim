# How to setup the AR

You can have a look at the `AR/Example/Example` Scene as an example on how to use the AR System. The main components are explained in detail below.

## AR Dependencies

To add AR to a scene, add both the `Cameras` and the `AR Controller` prefabs from `AR/Exports`.

## AR Objects

Both Text (TextMeshPro) and Mesh AR-Objects are supported. For them to function correctly it is best to add them as children of a `AR Settings` Object (e.g. the `AR/Exports/AR Controller` Prefab), but you can also use `AR Objects` in either `Padding`, `Mask` or `Global Without Parent` mode on their own.

### Mesh

Simply add the `AR Object` script and apply a Material using the `AR/Textured` Shader (e.g. `AR/Exports/AR Base`)

You can also use the `AR/Exports/Mesh` prefab.

### Text

To use `TextMeshPro` as AR you also need to add the `AR Object` Script as well as a `TMP_Font Asset` using the `AR/Text` Shader (e.g. `AR/Exports/AR Text Base`).

You can also use the `AR/Exports/Text` Prefab.

**Be aware that you need to call `ARObject.UpdateText()` after changing the text or it's attributes!**

## Tracking Delay

To apply a tracking delay to the `AR-Camera` you can change the `Delay` property of `Delayed Track Script` of `Cameras/AR Camera`.
