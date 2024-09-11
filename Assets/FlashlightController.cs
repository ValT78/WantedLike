using UnityEngine;

public class FlashlightController : MonoBehaviour
{
    public Material flashlightMaterial;
    public RectTransform flashlight;

    void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            (RectTransform)transform, Input.mousePosition, null, out Vector2 pos);
        flashlight.localPosition = pos;

        // Mettre à jour la position et la taille du masque dans le shader
        Vector4 maskPos = new(pos.x, pos.y, flashlight.rect.width, flashlight.rect.height);
        flashlightMaterial.SetVector("_MaskPos", maskPos);
    }
}
