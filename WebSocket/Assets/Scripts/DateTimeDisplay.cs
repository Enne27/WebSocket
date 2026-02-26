using UnityEngine;
using TMPro;

public class DateTimeDisplay : MonoBehaviour
{
    public TextMeshProUGUI dateText;  // Asigna el TextMeshPro para la fecha
    public TextMeshProUGUI timeText;  // Asigna el TextMeshPro para la hora

    void Update()
    {
        // Obtener la fecha y hora actuales
        System.DateTime currentTime = System.DateTime.Now;

        // Formatear la fecha en el formato 00/00/00 (día/mes/año)
        string formattedDate = currentTime.ToString("dd/MM/yy");

        // Formatear la hora en el formato 00:00 (hora:minuto)
        string formattedTime = currentTime.ToString("HH:mm");

        // Actualizar el texto de los TextMeshPro
        dateText.text = formattedDate;
        timeText.text = formattedTime;
    }
}
