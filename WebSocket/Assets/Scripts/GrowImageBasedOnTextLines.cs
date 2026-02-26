using UnityEngine;
using TMPro;

public class GrowImageDirectly : MonoBehaviour
{
    [Header("References")]
    public TMP_Text tmpText;        // Referencia al campo de texto (TMP_Text o TMP_InputField)
    public RectTransform imageRect; // RectTransform de la imagen que va a crecer

    [Header("Settings")]
    public float padding = 10f;     // Padding adicional para espacio entre el texto y el borde

    void Start()
    {
        // Asegúrate de que el anclaje esté en la parte superior y la parte inferior de la imagen
        imageRect.pivot = new Vector2(0.5f, 1f); // El pivot está en la parte superior, en el centro horizontal.
        imageRect.anchorMin = new Vector2(0.5f, 1f); // El anclaje mínimo (esquina inferior izquierda) está en la parte superior.
        imageRect.anchorMax = new Vector2(0.5f, 1f); // El anclaje máximo (esquina superior derecha) también está en la parte superior.
        float textHeight = tmpText.preferredHeight;

        // Añadir un poco de espacio extra para que no se pegue a los bordes (esto es opcional)
        float targetHeight = textHeight + padding;

        // Actualizar la altura del RectTransform de la imagen
        imageRect.sizeDelta = new Vector2(imageRect.sizeDelta.x, targetHeight);
    }
}

 
