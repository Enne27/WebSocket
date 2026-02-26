using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class SendMessageToScrollView : MonoBehaviour
{
    public TMP_InputField inputField;       // Campo de texto donde se escribe el mensaje
    public RawImage drawingCanvas;          // Lienzo donde se dibuja
    public Texture2D drawingTexture;        // Textura que almacena el dibujo

    public ScrollRect scrollView;           // ScrollView donde se mostrarán los mensajes
    public GameObject messagePrefab;
    public GameObject textMessagePrefab; // Prefab para los mensajes que contienen el dibujo y el nombre de usuario
    public Transform messageContainer;      // Contenedor donde se almacenarán los mensajes dentro del ScrollView
    public UserManager userManager;         // Campo de texto para el nombre de usuario
    public DrawingAndTyping draw;           // Referencia al script DrawingAndTyping

    public float fadeDuration = 1f; // Duración del efecto de fade-in

    private AudioSource audioSource;


    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void SendMessage()
    {
        // Asegurarnos de que haya un nombre de usuario y algo escrito en el campo de texto
        if (!string.IsNullOrEmpty(inputField.text) || draw.drawingTexture != null)
        {
            if (draw.isDrawn)
            {
                GameObject newMessage = Instantiate(messagePrefab, messageContainer);
                StartCoroutine(FadeInMessage(newMessage));
                // Obtener el RawImage y el TextMeshPro del prefab
                TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                RawImage messageDrawing = newMessage.transform.GetChild(2).GetComponentInChildren<RawImage>();  // El RawImage para mostrar el dibujo
                TextMeshProUGUI messageText = newMessage.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto


                // Asignamos el dibujo a la RawImage
                if (messageDrawing != null && draw != null && draw.drawingTexture != null)
                {
                    // Hacer una copia de la textura para evitar compartir la misma referencia
                    Texture2D copiedTexture = new Texture2D(draw.drawingTexture.width, draw.drawingTexture.height);
                    copiedTexture.SetPixels(draw.drawingTexture.GetPixels());
                    copiedTexture.Apply();
                    messageDrawing.texture = copiedTexture;  // Asignamos la textura copiada al RawImage
                    messageDrawing.texture.filterMode = FilterMode.Point;
                }

                // Asignar el texto
                if (messageText != null)
                {
                    messageText.text = inputField.text;
                }

                if (usernameText != null)
                {
                    usernameText.text = userManager.loggedUser.username;
                    usernameText.color = userManager.loggedUser.favoriteColor;
                }

                draw.ClearAll(true);       // Limpiar el lienzo (el dibujo)
                inputField.text = "";  // Limpiar el campo de texto

                Canvas.ForceUpdateCanvases(); // Este paso es clave

                // Mover el ScrollView hacia abajo después de enviar el mensaje
                StartCoroutine(ScrollToBottomCoroutine());
                audioSource.Play();
            }
            else if(inputField.text!="")
            {
                GameObject newMessage = Instantiate(textMessagePrefab, messageContainer);
                StartCoroutine(FadeInMessage(newMessage));
                // Obtener el RawImage y el TextMeshPro del prefab
                TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                TextMeshProUGUI messageText = newMessage.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto
                if (messageText != null)
                {
                    messageText.text = inputField.text;
                }

                if (usernameText != null)
                {
                    usernameText.text = userManager.loggedUser.username;
                    usernameText.color = userManager.loggedUser.favoriteColor;
                }

                //draw.ClearAll(true);       // Limpiar el lienzo (el dibujo)
                //inputField.text = "";  // Limpiar el campo de texto

                Canvas.ForceUpdateCanvases(); // Este paso es clave

                // Mover el ScrollView hacia abajo después de enviar el mensaje
                StartCoroutine(ScrollToBottomCoroutine());
                audioSource.Play();
            }
            
            
        }
    }

    // Método para mover el ScrollView hacia abajo
    private IEnumerator ScrollToBottomCoroutine()
    {
        yield return new WaitForEndOfFrame();

        // Asegurarse de que el ScrollView se actualiza correctamente
        scrollView.verticalNormalizedPosition = 0f;

        // Añadir una pequeña espera adicional para asegurarse de que el desplazamiento se aplique
        yield return new WaitForEndOfFrame();

        // Asegurarse de que el ScrollView ha llegado a la parte inferior
        scrollView.verticalNormalizedPosition = 0f;
    }

    private IEnumerator FadeInMessage(GameObject message)
    {
        CanvasGroup canvasGroup = message.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = message.AddComponent<CanvasGroup>(); // Añadir CanvasGroup si no tiene uno
        }

        canvasGroup.alpha = 0f; // Comienza totalmente transparente

        // Hacer el fade-in durante un tiempo determinado
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f; // Asegurarse de que la opacidad sea 1 al final
    }

}

