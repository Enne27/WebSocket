using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Unity.VisualScripting;


public class ChatMessageManagerA : MonoBehaviour {
    #region Singleton
    [Header("Singleton")]
    static ChatMessageManagerA chatMessageManager;
    public static ChatMessageManagerA instance {
        get {
            return RequestChatMessageManager();
        }
    }

    static ChatMessageManagerA RequestChatMessageManager() {
        if (chatMessageManager == null) {
            chatMessageManager = FindAnyObjectByType<ChatMessageManagerA>();
        }
        return chatMessageManager;
    }
    #endregion
    [SerializeField] GameObject messageImageInstancePrefab;
    [SerializeField] GameObject messageInstancePrefab;
    public GameObject SystemMessagePrefab;
    public GameObject HappyBirthdaytMessagePrefab;
    [SerializeField] Transform messagesParentA;
    [SerializeField] Transform messagesParentB;
    [SerializeField] Transform messagesParentC;
    [SerializeField] Transform messagesParentD;
    public float fadeDuration = 1f; // Duración del efecto de fade-in
    public DrawingAndTyping draw;           // Referencia al script DrawingAndTyping
    public ScrollRect scrollView;           // ScrollView donde se mostrarán los mensajes
    public AudioSource audioSource;
    public AudioClip audioClipRecieve;
    public AudioClip audioClipSend;
    public AudioSource happyBirthday;
    public AudioSource music;
    private void OnEnable() {
        if (WebSocketManager.instance) {
            WebSocketManager.instance.onReceivedGameMessage.AddListener(DisplayGameMessage);
        }
    }

    private void OnDisable() {
        if (WebSocketManager.instance) {
            WebSocketManager.instance.onReceivedGameMessage.RemoveListener(DisplayGameMessage);
        }
    }


    public void ClearMessages() {
        //for (int i = messagesParent.transform.childCount - 1; i >= 0; i--) {
        //    Destroy(messagesParent.GetChild(i).gameObject);
        //}
    }

    public static string ReplaceSlashNWithNewline(string inputString)
    {
        // Reemplaza '/n' literal por un salto de línea real '\n'
        return inputString.Replace("/n", Environment.NewLine);
    }

    public void InstantiateMessageFromUser(User info, string message, Texture2D dibujo)
    {
        if (info.room == WebSocketManager.instance.localUser.room)
        {
            if (info.room == "roomA")
            {
                GameObject newMessage = Instantiate(messageImageInstancePrefab, messagesParentA);
                StartCoroutine(FadeInMessage(newMessage));
                // Obtener el RawImage y el TextMeshPro del prefab
                TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                RawImage messageDrawing = newMessage.transform.GetChild(2).GetComponentInChildren<RawImage>();  // El RawImage para mostrar el dibujo
                TextMeshProUGUI messageText = newMessage.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto

                messageDrawing.texture = dibujo;  // Asignamos la textura copiada al RawImage
                messageDrawing.texture.filterMode = FilterMode.Point;



                messageText.text = ReplaceSlashNWithNewline(message);

                usernameText.text = info.username;
                usernameText.color = info.favoriteColor;


                Canvas.ForceUpdateCanvases(); // Este paso es clave
                // Mover el ScrollView hacia abajo después de enviar el mensaje
                StartCoroutine(ScrollToBottomCoroutine());
            }
            else if (info.room == "roomB")
            {
                GameObject newMessage = Instantiate(messageImageInstancePrefab, messagesParentB);
                StartCoroutine(FadeInMessage(newMessage));
                // Obtener el RawImage y el TextMeshPro del prefab
                TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                RawImage messageDrawing = newMessage.transform.GetChild(2).GetComponentInChildren<RawImage>();  // El RawImage para mostrar el dibujo
                TextMeshProUGUI messageText = newMessage.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto

                messageDrawing.texture = dibujo;  // Asignamos la textura copiada al RawImage
                messageDrawing.texture.filterMode = FilterMode.Point;

                messageText.text = message;

                usernameText.text = info.username;
                usernameText.color = info.favoriteColor;

                Canvas.ForceUpdateCanvases(); // Este paso es clave

                // Mover el ScrollView hacia abajo después de enviar el mensaje
                StartCoroutine(ScrollToBottomCoroutine());
            }
            else if (info.room == "roomC")
            {
                GameObject newMessage = Instantiate(messageImageInstancePrefab, messagesParentC);
                StartCoroutine(FadeInMessage(newMessage));
                // Obtener el RawImage y el TextMeshPro del prefab
                TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                RawImage messageDrawing = newMessage.transform.GetChild(2).GetComponentInChildren<RawImage>();  // El RawImage para mostrar el dibujo
                TextMeshProUGUI messageText = newMessage.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto

                messageDrawing.texture = dibujo;  // Asignamos la textura copiada al RawImage
                messageDrawing.texture.filterMode = FilterMode.Point;

                messageText.text = message;

                usernameText.text = info.username;
                usernameText.color = info.favoriteColor;

                Canvas.ForceUpdateCanvases(); // Este paso es clave

                // Mover el ScrollView hacia abajo después de enviar el mensaje
                StartCoroutine(ScrollToBottomCoroutine());
            }
            else if (info.room == "roomD")
            {
                GameObject newMessage = Instantiate(messageImageInstancePrefab, messagesParentD);
                StartCoroutine(FadeInMessage(newMessage));
                // Obtener el RawImage y el TextMeshPro del prefab
                TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                RawImage messageDrawing = newMessage.transform.GetChild(2).GetComponentInChildren<RawImage>();  // El RawImage para mostrar el dibujo
                TextMeshProUGUI messageText = newMessage.transform.GetChild(2).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto

                messageDrawing.texture = dibujo;  // Asignamos la textura copiada al RawImage
                messageDrawing.texture.filterMode = FilterMode.Point;

                messageText.text = message;

                usernameText.text = info.username;
                usernameText.color = info.favoriteColor;

                Canvas.ForceUpdateCanvases(); // Este paso es clave

                // Mover el ScrollView hacia abajo después de enviar el mensaje
                StartCoroutine(ScrollToBottomCoroutine());
            }

            if (info.room == WebSocketManager.instance.localUser.room)
            {
                if (info.ID == WebSocketManager.instance.localUser.ID)
                    audioSource.clip = audioClipSend;
                else
                    audioSource.clip = audioClipRecieve;
                audioSource.Play();
            }

        }

        else
        {
            return;
        }
    }

    public void InstantiateMessageFromUser(User info, string message, bool system) {
        if (info.room == WebSocketManager.instance.localUser.room)
        {
            switch (info.room)
            {
                case "roomA":
                    if (!system)
                    {
                        GameObject newMessage = Instantiate(messageInstancePrefab, messagesParentA);
                        StartCoroutine(FadeInMessage(newMessage));
                        TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                        TextMeshProUGUI messageText = newMessage.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto
                        if (messageText != null)
                        {
                            messageText.text = message;
                        }

                        if (usernameText != null)
                        {
                            usernameText.text = info.username;
                            usernameText.color = info.favoriteColor;
                        }

                        Canvas.ForceUpdateCanvases(); // Este paso es clave

                        StartCoroutine(ScrollToBottomCoroutine());
                    }
                    else
                    {
                        if (message == "enter")
                        {
                            info.room = "roomA";
                            GameObject systemMessage = Instantiate(SystemMessagePrefab, messagesParentA);
                            TextMeshProUGUI systemMessageText = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
                            systemMessageText.text = "<color=yellow>Now Entering:</color>  <color=#" + info.favoriteColor.ToHexString() + ">" + info.username;
                            if (info.birthdayDay == DateTime.Today.Day && info.birthdayMonth == DateTime.Today.Month)
                            {
                                Debug.Log(info.birthdayDay + info.birthdayMonth);
                                GameObject systemHappyBirthdayMessage = Instantiate(HappyBirthdaytMessagePrefab, messagesParentA);
                                TextMeshProUGUI HappyMessageText = systemHappyBirthdayMessage.GetComponentInChildren<TextMeshProUGUI>();
                                HappyMessageText.text = "<color=#" + info.favoriteColor.ToHexString() + ">" + info.username;
                                happyBirthday.Play();
                                music.Pause();
                                StartCoroutine(voyAVolverAPonerLaMusica());
                            }
                            Canvas.ForceUpdateCanvases(); // Este paso es clave

                            StartCoroutine(ScrollToBottomCoroutine());
                        }
                        else if (message == "exit")
                        {
                            GameObject systemMessage = Instantiate(SystemMessagePrefab, messagesParentA);
                            TextMeshProUGUI systemMessageText = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
                            systemMessageText.text = "<color=yellow>Now Exiting:</color>  <color=#" + info.favoriteColor.ToHexString() + ">" + info.username;

                            Canvas.ForceUpdateCanvases(); // Este paso es clave

                            StartCoroutine(ScrollToBottomCoroutine());
                        }
                    }
                    break;
                case "roomB":
                    if (!system)
                    {
                        GameObject newMessage = Instantiate(messageInstancePrefab, messagesParentB);
                        StartCoroutine(FadeInMessage(newMessage));
                        TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                        TextMeshProUGUI messageText = newMessage.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto
                        if (messageText != null)
                        {
                            messageText.text = message;
                        }

                        if (usernameText != null)
                        {
                            usernameText.text = info.username;
                            usernameText.color = info.favoriteColor;
                        }

                        Canvas.ForceUpdateCanvases(); // Este paso es clave

                        StartCoroutine(ScrollToBottomCoroutine());
                    }
                    else
                    {
                        if (message == "enter")
                        {
                            info.room = "roomB";
                            GameObject systemMessage = Instantiate(SystemMessagePrefab, messagesParentB);
                            TextMeshProUGUI systemMessageText = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
                            systemMessageText.text = "<color=yellow>Now Entering:</color>  <color=#" + info.favoriteColor.ToHexString() + ">" + info.username;
                            if (info.birthdayDay == DateTime.Today.Day && info.birthdayMonth == DateTime.Today.Month)
                            {
                                Debug.Log(info.birthdayDay + info.birthdayMonth);
                                GameObject systemHappyBirthdayMessage = Instantiate(HappyBirthdaytMessagePrefab, messagesParentB);
                                TextMeshProUGUI HappyMessageText = systemHappyBirthdayMessage.GetComponentInChildren<TextMeshProUGUI>();
                                HappyMessageText.text = "<color=#" + info.favoriteColor.ToHexString() + ">" + info.username;
                                happyBirthday.Play();
                                music.Pause();
                                StartCoroutine(voyAVolverAPonerLaMusica());
                            }
                            Canvas.ForceUpdateCanvases(); // Este paso es clave

                            StartCoroutine(ScrollToBottomCoroutine());
                        }
                        else if (message == "exit")
                        {
                            GameObject systemMessage = Instantiate(SystemMessagePrefab, messagesParentB);
                            TextMeshProUGUI systemMessageText = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
                            systemMessageText.text = "<color=yellow>Now Exiting:</color>  <color=#" + info.favoriteColor.ToHexString() + ">" + info.username;

                            Canvas.ForceUpdateCanvases(); // Este paso es clave

                            StartCoroutine(ScrollToBottomCoroutine());
                        }
                    }
                    break;
                case "roomC":
                    if (!system)
                    {
                        GameObject newMessage = Instantiate(messageInstancePrefab, messagesParentC);
                        StartCoroutine(FadeInMessage(newMessage));
                        TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                        TextMeshProUGUI messageText = newMessage.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto
                        if (messageText != null)
                        {
                            messageText.text = message;
                        }

                        if (usernameText != null)
                        {
                            usernameText.text = info.username;
                            usernameText.color = info.favoriteColor;
                        }

                        Canvas.ForceUpdateCanvases(); // Este paso es clave

                        StartCoroutine(ScrollToBottomCoroutine());
                    }
                    else
                    {
                        if (message == "enter")
                        {
                            info.room = "roomC";
                            GameObject systemMessage = Instantiate(SystemMessagePrefab, messagesParentC);
                            TextMeshProUGUI systemMessageText = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
                            systemMessageText.text = "<color=yellow>Now Entering:</color>  <color=#" + info.favoriteColor.ToHexString() + ">" + info.username;
                            if (info.birthdayDay == DateTime.Today.Day && info.birthdayMonth == DateTime.Today.Month)
                            {
                                Debug.Log(info.birthdayDay + info.birthdayMonth);
                                GameObject systemHappyBirthdayMessage = Instantiate(HappyBirthdaytMessagePrefab, messagesParentC);
                                TextMeshProUGUI HappyMessageText = systemHappyBirthdayMessage.GetComponentInChildren<TextMeshProUGUI>();
                                HappyMessageText.text = "<color=#" + info.favoriteColor.ToHexString() + ">" + info.username;
                                happyBirthday.Play();
                                music.Pause();
                                StartCoroutine(voyAVolverAPonerLaMusica());
                            }
                            Canvas.ForceUpdateCanvases(); // Este paso es clave

                            StartCoroutine(ScrollToBottomCoroutine());
                        }
                        else if (message == "exit")
                        {
                            GameObject systemMessage = Instantiate(SystemMessagePrefab, messagesParentC);
                            TextMeshProUGUI systemMessageText = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
                            systemMessageText.text = "<color=yellow>Now Exiting:</color>  <color=#" + info.favoriteColor.ToHexString() + ">" + info.username;

                            Canvas.ForceUpdateCanvases(); // Este paso es clave

                            StartCoroutine(ScrollToBottomCoroutine());
                        }
                    }
                    break;
                case "roomD":
                    if (!system)
                    {
                        GameObject newMessage = Instantiate(messageInstancePrefab, messagesParentD);
                        StartCoroutine(FadeInMessage(newMessage));
                        TextMeshProUGUI usernameText = newMessage.GetComponentInChildren<TextMeshProUGUI>();
                        TextMeshProUGUI messageText = newMessage.transform.GetChild(1).GetComponentInChildren<TextMeshProUGUI>();  // El TextMeshPro para mostrar el mensaje de texto
                        if (messageText != null)
                        {
                            messageText.text = message;
                        }

                        if (usernameText != null)
                        {
                            usernameText.text = info.username;
                            usernameText.color = info.favoriteColor;
                        }

                        Canvas.ForceUpdateCanvases(); // Este paso es clave

                        StartCoroutine(ScrollToBottomCoroutine());
                    }
                    else
                    {
                        if (message == "enter")
                        {
                            WebSocketManager.instance.TryToSetRoom("");
                            GameObject systemMessage = Instantiate(SystemMessagePrefab, messagesParentD);
                            TextMeshProUGUI systemMessageText = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
                            systemMessageText.text = "<color=yellow>Now Entering:</color>  <color=#" + info.favoriteColor.ToHexString() + ">" + info.username;
                            if (info.birthdayDay == DateTime.Today.Day && info.birthdayMonth == DateTime.Today.Month)
                            {
                                Debug.Log(info.birthdayDay + info.birthdayMonth);
                                GameObject systemHappyBirthdayMessage = Instantiate(HappyBirthdaytMessagePrefab, messagesParentD);
                                TextMeshProUGUI HappyMessageText = systemHappyBirthdayMessage.GetComponentInChildren<TextMeshProUGUI>();
                                HappyMessageText.text = "<color=#" + info.favoriteColor.ToHexString() + ">" + info.username;
                                happyBirthday.Play();
                                music.Pause();
                                StartCoroutine(voyAVolverAPonerLaMusica());
                            }
                            Canvas.ForceUpdateCanvases(); // Este paso es clave

                            StartCoroutine(ScrollToBottomCoroutine());
                        }
                        else if (message == "exit")
                        {

                            GameObject systemMessage = Instantiate(SystemMessagePrefab, messagesParentD);
                            TextMeshProUGUI systemMessageText = systemMessage.GetComponentInChildren<TextMeshProUGUI>();
                            systemMessageText.text = "<color=yellow>Now Exiting:</color>  <color=#" + info.favoriteColor.ToHexString() + ">" + info.username;

                            Canvas.ForceUpdateCanvases(); // Este paso es clave

                            StartCoroutine(ScrollToBottomCoroutine());
                        }
                    }
                    break;
            }

            if (info.room == WebSocketManager.instance.localUser.room)
            {
                if (info.ID == WebSocketManager.instance.localUser.ID)
                    audioSource.clip = audioClipSend;
                else
                    audioSource.clip = audioClipRecieve;
                audioSource.Play();
            }
        }
        else
        {
            return;
        }
        
    }

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

    public void InstantiateMessageFromUser(string userName, string message) {
        //TODO
    }

    public void DisplayGameMessage(string message) {
        // Puedes personalizar c�mo muestras los mensajes de juego en Unity
        if (WebSocketManager.instance.debug) Debug.Log($"Game Message: {message}");
        InstantiateMessageFromUser("Server", message);
    }

    public IEnumerator voyAVolverAPonerLaMusica()
    {
        yield return new WaitForSecondsRealtime(11f);
        music.Play();
    }

}
