using UnityEngine;
using TMPro;

public class ResizeImageBasedOnText : MonoBehaviour
{
    [Header("References")]
    public TMP_Text tmpText;        // Referencia al campo de texto (TMP_Text o TMP_InputField)
    public RectTransform imageRect; // RectTransform de la imagen que va a crecer

    [Header("Settings")]
    public float padding = 10f;     // Padding adicional para espacio entre el texto y el borde

    void Update()
    {

        float textHeight = tmpText.preferredWidth;

        // Añadir un poco de espacio extra para que no se pegue a los bordes (esto es opcional)
        float targetHeight = textHeight + padding;

        // Actualizar la altura del RectTransform de la imagen
        imageRect.sizeDelta = new Vector2(targetHeight, imageRect.sizeDelta.y);
    }
}
