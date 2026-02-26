using TMPro;
using UnityEngine;

public class RainbowTextEffect : MonoBehaviour
{
    public TMP_Text textMeshPro;  // Componente TMP_Text al que aplicaremos el efecto
    public float speed = 1.0f;    // Velocidad de desplazamiento del arco iris
    [SerializeField] private Color[] rainbowColors; // Colores del arco iris
    private float offset = 0f;    // Desplazamiento del gradiente

    private void Start()
    {
        // Asegurarnos de que el texto está actualizado al principio
        UpdateRainbowEffect();
    }

    private void Update()
    {
        // Decrementar el desplazamiento gradualmente con el tiempo para crear el efecto de movimiento hacia la izquierda
        offset -= Time.deltaTime * speed;

        // Asegurarnos de que el desplazamiento se mantenga dentro del rango de 0 a 1 para crear un bucle continuo
        if (offset < 0f)
        {
            offset += 1f;
        }

        // Actualizamos el efecto de arco iris en cada frame
        UpdateRainbowEffect();
    }

    private void UpdateRainbowEffect()
    {
        // Asegurarnos de que el texto no esté vacío
        if (textMeshPro.text.Length == 0) return;

        // Forzar la actualización de la malla para obtener los datos más recientes
        textMeshPro.ForceMeshUpdate();

        // Obtener la información de la malla
        TMP_MeshInfo[] meshInfo = textMeshPro.textInfo.meshInfo;

        // Recorrer todos los caracteres en el texto
        for (int i = 0; i < textMeshPro.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = textMeshPro.textInfo.characterInfo[i];

            // Verificamos si el carácter es visible (ignorar caracteres invisibles como los espacios)
            if (!charInfo.isVisible)
                continue;

            // Calcular el índice del color aplicando el desplazamiento en la dirección contraria
            int colorIndex = (int)((i + offset * rainbowColors.Length) % rainbowColors.Length);
            Color32 color = rainbowColors[colorIndex];

            // Calcular el índice de malla (puede ser más de 1 en textos con varias mallas)
            int meshIndex = charInfo.materialReferenceIndex;
            int vertexIndex = charInfo.vertexIndex;

            // Asignar el color a los 4 vértices del carácter
            meshInfo[meshIndex].colors32[vertexIndex + 0] = color; // Vértice 0
            meshInfo[meshIndex].colors32[vertexIndex + 1] = color; // Vértice 1
            meshInfo[meshIndex].colors32[vertexIndex + 2] = color; // Vértice 2
            meshInfo[meshIndex].colors32[vertexIndex + 3] = color; // Vértice 3
        }

        // Actualizar los datos de los vértices
        textMeshPro.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
    }
}
