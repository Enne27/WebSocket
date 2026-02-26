using System;
using UnityEngine;
using UnityEngine.UI;
using WebSocketSharp;
using SimpleJSON;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
using static NetworkMessages;
using UnityEngine.EventSystems;
using System.Collections;


[System.Serializable]
public class UserNameSettingMessage
{
    public string id;
    public string username;
    public string favoriteColor;
    public string birthDay;
    public string birthMonth;
    public string room;

    public UserNameSettingMessage(string id, string username, string favoriteColor, string birthDay, string birthMonth, string room)
    {
        this.id = id;
        this.username = username;
        this.favoriteColor = favoriteColor;
        this.birthDay = birthDay;
        this.birthMonth = birthMonth;
        this.room = room;
    }
}



public class WebSocketManager : MonoBehaviour
{
    #region Singleton
    [Header("Singleton")]
    static WebSocketManager webSocketManager;
    public static WebSocketManager instance
    {
        get
        {
            return RequestWebSocketManager();
        }
    }

    static WebSocketManager RequestWebSocketManager()
    {
        if (webSocketManager == null)
        {
            webSocketManager = FindAnyObjectByType<WebSocketManager>();
        }
        return webSocketManager;
    }
    #endregion

    [Header("Configuration")]
    [SerializeField] string serverUrl = "ws://localhost:3000";

    Queue<MessageEventArgs> messageQueue = new Queue<MessageEventArgs>();

    WebSocket ws;
    bool hasBeenWelcomed = false;

    [Header("References")]
    [SerializeField] public bool debug = true;

    [Header("Latency")]
    public int latency;


    [Header("User Info")]
    public User localUser;
    public List<User> remoteUsers = new List<User>();


    UserManager userManager;
    ConnectedUsersManager connectedUsersManager;

    [Header("UserConfig")]
    
    [Header("Events")]
    public UnityEvent onLocalUserConnected;
    public UnityEvent onLocalUserManuallyDisconnectedFromServer;
    public UnityEvent onLocalConnectionTimeOut;
    public UnityEvent onSocketDisconnection;
    public UnityEvent<string> onLocalUserWelcome;

    public UnityEvent<string> onLocalUserIDAssigned;
    public UnityEvent<UserNameSettingMessage> onUserNameSettingMessageReceived;
    public UnityEvent<string, string, bool> onReceivedChatMessage;
    public UnityEvent<string> onReceivedGameMessage;
    public UnityEvent<string> onReceivedDisconnectionMessage;
    public UnityEvent<string> onReceivedSetUserColorMessage;
    public UnityEvent<JSONNode> onReceivedOtherUsersMessage;
    

    [Header("SendButtons")]
    public Button sendRoomA;
    ButtonManager buttonManager;
    
    [Header("Id Text")]
    public TextMeshProUGUI IdText;
    [Header("InfoBox")]
    public TextMeshProUGUI InfoBox;

    private void OnEnable()
    {

        //Local UnityEvent Callbacks
         onLocalUserConnected.AddListener(LocalUserConnectedToServer);
         onLocalUserManuallyDisconnectedFromServer.AddListener(LocalUserDisconnectedFromServer);
         onLocalUserIDAssigned.AddListener(AssignLocalUserID);
         onLocalConnectionTimeOut.AddListener(ConnectionTimeOut);
         onLocalUserWelcome.AddListener(Welcome);

         //Message Handling UnityEvent Callbacks
         onUserNameSettingMessageReceived.AddListener(ProcessSetUserNameMessage);
         onReceivedChatMessage.AddListener(ProcessChatMessage);
         onReceivedDisconnectionMessage.AddListener(OnDisconnectRemoteUser);
         onReceivedOtherUsersMessage.AddListener(OnReceivedOtherUsersMessage);
         onReceivedSetUserColorMessage.AddListener(OnReceivedSetUserColorMessage);
         //onReceivedImageMessage.AddListener(ProcessImageMessage);

    }

     private void OnDisable()
     {
         onLocalUserConnected.RemoveListener(LocalUserConnectedToServer);
         onLocalUserIDAssigned.RemoveListener(AssignLocalUserID);
         onLocalConnectionTimeOut.RemoveListener(ConnectionTimeOut);
         onLocalUserManuallyDisconnectedFromServer.RemoveListener(LocalUserDisconnectedFromServer);
         onLocalUserWelcome.RemoveListener(Welcome);

         onUserNameSettingMessageReceived.RemoveListener(ProcessSetUserNameMessage);
         onReceivedChatMessage.RemoveListener(ProcessChatMessage);
         onReceivedDisconnectionMessage.RemoveListener(OnDisconnectRemoteUser);
         onReceivedOtherUsersMessage.RemoveListener(OnReceivedOtherUsersMessage);
         onReceivedSetUserColorMessage.RemoveListener(OnReceivedSetUserColorMessage);
        // onReceivedImageMessage.RemoveListener(ProcessImageMessage);
     }


     void Start()
     {
        SetUpLocalWebSocket();
        ws.ConnectAsync();
       // StartCoroutine(_InitialConnectionCoroutine());

         /*userManager = GetComponent<UserManager>();
         connectedUsersManager = GetComponent<ConnectedUsersManager>();
         buttonManager = GetComponent<ButtonManager>();*/
     }

     void Update()
     {
         if (messageQueue.Count > 0)
         {
             HandleServerMessage(messageQueue.Dequeue());
         }
         if(localUser != null)
         {
             IdText.text = localUser.ID;
             IdText.color = localUser.favoriteColor;
         }
     }

     void ServerConnectionTimeout()
     {
         buttonManager.DisconectUser();
         remoteUsers.Clear();
         localUser = null;
         userManager.users.Clear();
         Debug.Log("ConnectionTimeOut");
         InfoBox.text = "Connection Error: Lost Connection to server ";
         hasBeenWelcomed = false;
     }
     
     public void SetUpLocalWebSocket()
     {
         ws = new WebSocket(serverUrl);

         ws.Ping();

         ws.OnOpen += (sender, e) =>
         {
             onLocalUserConnected?.Invoke();
         };

         ws.OnMessage += (sender, e) =>
         {
             if (debug)
             {
                 Debug.Log("Message from server: " + e.Data);

             }
             if (e.Data.Contains("\"type\":\"ping\""))
                 SendPong();

             messageQueue.Enqueue(e);
         };

         ws.OnClose += (sender, e) =>
         {
             onSocketDisconnection?.Invoke();
             hasBeenWelcomed = false;
         };

         ws.OnError += (sender, e) =>
         {
             Debug.LogError("WebSocket error: " + e.Message);
         };

     }

     void SendPong()
     {
         if (ws != null && ws.IsAlive)
         {
             ws.Send("{\"type\":\"pong\"}");
             //Debug.Log("Pong enviado al servidor");
         }
     }

     private void LocalUserConnectedToServer()
     {
         if (debug)
         {
             Debug.Log("Connected to server");
         }
     }

     private void LocalUserDisconnectedFromServer()
     {

         if (debug)
         {
             Debug.Log("Disconnected from the server");
         }

         // TODO Desactivar elementos de la interfaz de chat y activar elementos de la interfaz de conexi�n
         hasBeenWelcomed = false;
     }

     private void HandleServerMessage(MessageEventArgs e)
     {

         if (string.IsNullOrEmpty(e.Data))
         {
             return;
         }

         try
         {
             JSONNode messageJSON = JSON.Parse(e.Data);
             string messageType = messageJSON[Network_MessageKey_Type].Value;



                 string username = messageJSON[Network_Content_UserName].Value;
                 string color = messageJSON[Network_Content_Color].Value;
                 string birthDay = messageJSON[NetWork_Content_BirthDay].Value;
                 string birthMonth = messageJSON[NetWork_Content_BirthMonth].Value;
                 string room = messageJSON[Network_Content_Room].Value;
                 string messageContent = messageJSON[Network_Content_Message].Value;
                 string userId = messageJSON[Network_Content_UserID].Value;
                 string system = messageJSON[Network_SystemMessage].Value;
                 string image = messageJSON[Network_Content_Image].Value;

                 switch (messageType)
                 {
                     case Network_MessageKey_Chat:
                         bool systembool;
                         if (system == "True")
                         {
                             systembool = true;
                         }
                         else
                             systembool = false;
                         onReceivedChatMessage?.Invoke(userId, messageContent, systembool);
                         break;

                     case Network_MessageKey_UserID:
                         // Pensado para establecer que el usuario ha recibido su id
                         onLocalUserIDAssigned?.Invoke(userId);
                         break;
                     case Network_MessageKey_SetUserName:
                         // Pensado para mostrar mensajes de usuarios en Unity
                         onUserNameSettingMessageReceived?.Invoke(new UserNameSettingMessage(userId, username, color, birthDay, birthMonth, room));
                         break;
                     case Network_MessageKey_GameMessage:
                         //Pensado para mostrar mensajes del servidor en Unity
                         onReceivedGameMessage?.Invoke(messageContent);
                         break;

                     case Network_MessageKey_Disconnection:
                         // Pensado para establecer que un usuario se ha desconectado
                         onReceivedDisconnectionMessage?.Invoke(userId);
                         break;

                     case Network_MessageKey_OtherUsers:
                         // Pensado para recibir la información de los usuarios conectados
                         onReceivedOtherUsersMessage?.Invoke(messageJSON);
                         break;
                     case Network_MessageKey_SetUserColor:
                         onReceivedSetUserColorMessage?.Invoke(color);
                         break;
                     case Network_MessageKey_Image:
                         //onReceivedImageMessage?.Invoke(new ImageMessage(userId, messageContent, image));
                         break;
                     case "latency":
                         SetPing(messageJSON["latency"].Value);
                         break;
                     case "ping":
                         break;

                     default:
                         Debug.LogWarning($"Unknown message type: {messageType}");
                         Debug.Log(messageContent);
                         break;
                 }


             }
         catch (Exception ex)
         {
             Debug.LogError($"Error al parsear el mensaje JSON: {ex.Message}");
         }
     }

     private void ProcessImageMessage(ImageMessage image)
     {


         //Debug.Log(GetUserWithID(image.userId).username +" "+ image.message +" "+ image.image);
         string base64Image = image.image;


         User info = GetUserWithID(image.userId);

         if (info != null)
         {
             // Convertir de Base64 a bytes
             byte[] imageBytes = Convert.FromBase64String(base64Image);

             // Crear una textura desde los bytes
             Texture2D tex = new Texture2D(2, 2);
             tex.LoadImage(imageBytes);
             tex.Apply();

             if (debug) Debug.Log($"Received chat message from {info.username} : {image.message}");

             Debug.Log(info.room);
             ChatMessageManagerA.instance.InstantiateMessageFromUser(info, image.message, tex);
         }

     }

     private void AssignLocalUserID(string userId)
     {
         //Se asigna un ID
         if (debug) Debug.Log($"Assigned Local User ID: {userId} ");
         userManager.loggedUser.ID = userId;
     }


     //Método llamado cuando se recibe un mensaje de tipo NetworkMessages.Network_MessageKey_SetUserName
     public void ProcessSetUserNameMessage(UserNameSettingMessage user)
     {
         if (debug) Debug.Log($"Received setUserName message: {user.room}");

         if (user.id != localUser.ID)
         {
             // Assign a random color to the remote user's image
             User remoteUser = GetUserWithID(user.id);
             if (remoteUser != null)
             {
                 ChangeRemoteUserName(remoteUser, user.username, StringToSupportedColor(user.favoriteColor), user.birthDay, user.birthMonth, user.room);
             }
         }
         else
         {
             if (hasBeenWelcomed)
             {
                 ChatMessageManagerA.instance.DisplayGameMessage(localUser.username + " changed his room to " + user.room);
                 localUser.onUserChangedName?.Invoke(user.room);

             }
             else
             {
                 onLocalUserWelcome?.Invoke(user.room);
             }
             localUser.room = user.room;
         }
     }

     //Método llamado cuando el usuario local entra al chat
     private void Welcome(string userName)
     {
         ChatMessageManagerA.instance.ClearMessages();
         ChatMessageManagerA.instance.DisplayGameMessage("Welcome, " + userName + "!");
         hasBeenWelcomed = true;
     }


     private void OnDisconnectRemoteUser(string disconnectedUserID)
     {
         User remoteUser = remoteUsers.Find((m) => m.ID == disconnectedUserID);
         if (remoteUser != null)
         {
             if (debug) Debug.Log($"Disconnecting {remoteUser.username}");
             remoteUser.onUserDisconnected?.Invoke();
             remoteUsers.Remove(remoteUser);
             connectedUsersManager.UpdateUserButtons();

         }
         else
         {
             Debug.Log("Trying to disconnect non existing user");
         }
     }

     // M�todo para enviar mensajes al servidor
     public void SendMessageToServer(string message, bool system)
     {
         if (ws != null && ws.IsAlive)
         {
             // Asumiendo que este mensaje es un mensaje de chat
             ChatMessage chatMessage = new ChatMessage
             {
                 type = Network_MessageKey_Chat,
                 userId = localUser.ID,
                 message = message,
                 system = system,
                 room = localUser.room,
             };

             // Utilizando JsonUtility para convertir el objeto a JSON directamente
             string jsonMessage = JsonUtility.ToJson(chatMessage);

             ws.Send(jsonMessage);
         }
         else
         {
             buttonManager.DisconectUser();
             InfoBox.text = "Connection Error: Lost Connection to server ";
             Debug.LogWarning("WebSocket connection is not alive. Unable to send message.");
         }
     }

     public void TryToSetUsername(TextMeshProUGUI name)
     {
         TryToSetUsername(name.text);
     }

     public void TryToModify(User user)
     {
         if (ws != null && ws.IsAlive)
         {
             // Crear el mensaje de chat para establecer el nombre de usuario
             ChatMessage setUsernameMessage = new ChatMessage
             {
                 type = Network_MessageKey_SetUserName,
                 userName = user.username,
                 message = SupportedColorToString(ColorToSupportedColor(user.favoriteColor)),
                 birthDay = user.birthdayDay,
                 birthMonth = user.birthdayMonth
             };

             // Convertir el mensaje en formato JSON
             string jsonMessage = JsonUtility.ToJson(setUsernameMessage);

             // Enviar el mensaje al servidor
             ws.Send(jsonMessage);

             // Ahora, actualizamos la lista de usuarios remotos con la nueva información
             User remoteUser = remoteUsers.Find(u => u.ID == user.ID);
             if (remoteUser != null)
             {
                 // Actualizamos la información del usuario
                 remoteUser.username = user.username;
                 remoteUser.favouriteColorSupported = user.favouriteColorSupported;
                 remoteUser.favoriteColor = user.favoriteColor;
                 remoteUser.birthdayDay = user.birthdayDay;
                 remoteUser.birthdayMonth = user.birthdayMonth;

                 // Aquí puedes actualizar la interfaz si es necesario, por ejemplo:
                 ConnectedUsersManager.instance.SetUpUserRepresentation();
             }
         }
     }

     // M�todo para establecer el nombre de usuario
     public void TryToSetUsername(string username)
     {

         UserCreator userCreator = GetComponent<UserCreator>();
         if (ws != null && ws.IsAlive)
         {
             ChatMessage setUsernameMessage = new ChatMessage
             {
                 type = Network_MessageKey_SetUserName,
                 userName = username,
                 message = SupportedColorToString(userCreator.GetSelectedSupportedColor()),
                 birthDay = int.Parse(userCreator.dayInput.text),
                 birthMonth = int.Parse(userCreator.monthInput.text)
             };

             string jsonMessage = JsonUtility.ToJson(setUsernameMessage);
             ws.Send(jsonMessage);
             userCreator.SaveUser();
             buttonManager.goBackCrear();
         }
     }

     public void TryToSetRoom(string room)
     {

         if (ws != null && ws.IsAlive)
         {

             ChatMessage setUsernameMessage = new ChatMessage
             {
                 type = Network_MessageKey_SetUserName,
                 userName = localUser.username,
                 message = SupportedColorToString(localUser.favouriteColorSupported),
                 birthDay = localUser.birthdayDay,
                 birthMonth = localUser.birthdayMonth,
                 room = room
             };

             string jsonMessage = JsonUtility.ToJson(setUsernameMessage);
             ws.Send(jsonMessage);
         }
     }

     private void SetupRemoteUser(string id, string username, SupportedColor favoriteColor, int birthDay, int birthMonth, string room)
     {
         User userInfo = GetUserWithID(id);

         if (userInfo == null)
         {
             Debug.Log(room);
             userInfo = new User(id, username, favoriteColor, birthDay, birthMonth, room);
             remoteUsers.Add(userInfo);
             ChatMessageManagerA.instance.DisplayGameMessage(username + " has connected to the server");
         }

         ConnectedUsersManager.instance.SetUpUserRepresentation();
     }


     public User GetUserWithID(string anId)
     {
         if (anId != localUser.ID) return remoteUsers.Find((User info) => info.ID == anId);
         else return localUser;
     }

     public void ProcessChatMessage(string userID, string messageContent, bool system)
     {
         User info = GetUserWithID(userID);
         if (info != null)
         {
             if (debug) Debug.Log($"Received chat message from {info.username} : {messageContent}");
             //Debug.Log(info.room);
             ChatMessageManagerA.instance.InstantiateMessageFromUser(info, messageContent, system);

         }
         else
         {
             Debug.Log("Message from unkown user");
         }
     }

     public void OnConnectButtonPressed()
     {
         ws.Connect();
         onSocketDisconnection.AddListener(ServerConnectionTimeout);
         StartCoroutine(_InitialConnectionCoroutine());
     }

     public void OnDisconnectButtonPressed()
     {
         if (ws != null && ws.IsAlive)
         {
             onSocketDisconnection.RemoveListener(ServerConnectionTimeout);
             ws.Close();
             onLocalUserManuallyDisconnectedFromServer?.Invoke();
         }
     }

     private void OnApplicationQuit()
     {
         ws.Close();
     }

     IEnumerator _InitialConnectionCoroutine(float maxTimeOut = 1f)
     {
         float timeOutTimer = 0;
         while ((ws == null || !ws.IsAlive) && timeOutTimer < maxTimeOut)
         {
             timeOutTimer += Time.deltaTime;
             yield return null;
         }

         if (ws != null && ws.IsAlive)
         {
             if (debug) Debug.Log($"Connection was successfull after {timeOutTimer} seconds");

         }
         else
         {
             onLocalConnectionTimeOut?.Invoke();
         }
     }

     public void OnSendMessageButtonClicked(TMP_InputField messageInputField, DrawingAndTyping draw)
     {

         if (draw.drawingTexture != null && (messageInputField.text != "" || messageInputField.text != null))
         {
             if (draw.isDrawn)
             {
                 string message = ReplaceEnterWithNewline(messageInputField.text);
                 if (!IsWhite(draw.drawingTexture))
                 {
                         SendImage(message, draw.drawingTexture);
                         messageInputField.text = "";
                         EventSystem.current.SetSelectedGameObject(null);
                 }
                 else if (IsWhite(draw.drawingTexture) && !message.IsNullOrEmpty() )
                 {
                     SendMessageToServer(message, false);
                     messageInputField.text = "";
                     EventSystem.current.SetSelectedGameObject(null);
                 }
                     draw.ClearAll(true);
             }
             else
             {
                 string message = messageInputField.text;
                 if(!message.IsNullOrEmpty())
                 {
                     SendMessageToServer(message, false);
                     messageInputField.text = "";
                     EventSystem.current.SetSelectedGameObject(null);
                 }
                 draw.ClearAll(true);
             }
         }
     }

     public bool IsWhite(Texture2D textura)
     {
         // Asegúrate de que la textura no sea null
         if (textura == null)
         {
             Debug.LogWarning("La textura es null");
             return false;
         }

         // Obtener todos los píxeles de la textura
         Color[] pixeles = textura.GetPixels();

         // Comprobar si todos los píxeles son blancos
         foreach (Color pixel in pixeles)
         {
             if (pixel != Color.white) // Si encontramos un píxel que no es blanco
             {
                 return false;
             }
         }

         // Si todos los píxeles son blancos
         return true;
     }

     public static string ReplaceEnterWithNewline(string inputString)
     {
         // Reemplaza los saltos de línea por '/n'
         return inputString.Replace("\r\n", "/n").Replace("\n", "/n").Replace("\r", "/n");
     }

     public void SendImage(string message, Texture2D texture)
     {
         byte[] imageBytes = texture.EncodeToPNG();
         string base64Image = Convert.ToBase64String(imageBytes);

         var json = "{" +
             "\"type\": \"image\", " +
             "\"userId\": \"" + localUser.ID + "\"," +
             "\"message\": \"" + message + "\"," +
             "\"image\": \"" + base64Image + "\"}";
         ws.Send(json);

     }

     private void OnReceivedOtherUsersMessage(JSONNode messageJSON)
     {
         //Recibimos información de usuarios conectados, por tanto destruímos los actuales y regeneramos 
         ConnectedUsersManager.instance.ClearRemoteUsersRepresentation();
         JSONArray users = messageJSON[Network_Content_Users].AsArray;
         for (int i = 0; i < users.Count; i++)
         {
             SetupRemoteUser(users[i][Network_Content_UserID], users[i][Network_Content_UserName], StringToSupportedColor(users[i][Network_Content_Color]), users[i][NetWork_Content_BirthDay], users[i][NetWork_Content_BirthMonth], users[i][Network_Content_Room]);
         }
     }

     private void ChangeRemoteUserName(User info, string name, SupportedColor color, string birthDay, string birthMonth, string room)
     {
         //Recibimos un mensaje informando de que un usuario quiere cambiarse su nombre de usuario
         ChatMessageManagerA.instance.DisplayGameMessage(info.username + " has changed his room to " + info.room);
         info.username = name;
         info.favouriteColorSupported = color;
         info.favoriteColor = SupportedColorToColor(color);
         info.birthdayDay = int.Parse(birthDay);
         info.birthdayMonth = int.Parse(birthMonth);
         info.room = room;
         info.onUserChangedName?.Invoke(room);
     }

     private void ConnectionTimeOut()
     {
         remoteUsers.Clear();
         localUser = null;
         userManager.users.Clear();
         Debug.Log("ConnectionTimeOut");
         hasBeenWelcomed = false;
         InfoBox.text = "Connection Error: Time Out. Unable to connect to server.";
     }

     private void OnReceivedSetUserColorMessage(string newColor)
     {
         Debug.Log(newColor);
         //Código de establecemos color
         switch (StringToSupportedColor(newColor))
         {
             case SupportedColor.RED:

                 break;
             case SupportedColor.ORANGE:
                 break;
             case SupportedColor.YELLOW:
                 break;
             case SupportedColor.GREEN:
                 break;
             case SupportedColor.L_BLUE:
                 break;
             case SupportedColor.D_BLUE:
                 break;
             case SupportedColor.PURPLE:
                 break;
             case SupportedColor.PINK:
                 break;
             case SupportedColor.WHITE:
                 break;
             case SupportedColor.COLORS_LENGTH:
                 break;
         }
     }

     public enum SupportedColor { RED, ORANGE, YELLOW, GREEN, L_BLUE, D_BLUE, PURPLE, PINK, WHITE, COLORS_LENGTH };

     string SupportedColorToString(SupportedColor supportedColor)
     {
         switch (supportedColor)
         {
             case SupportedColor.RED:
                 return "red";
             case SupportedColor.ORANGE:
                 return "orange";
             case SupportedColor.YELLOW:
                 return "yellow";
             case SupportedColor.GREEN:
                 return "green";
             case SupportedColor.L_BLUE:
                 return "l_blue";
             case SupportedColor.D_BLUE:
                 return "d_blue";
             case SupportedColor.PURPLE:
                 return "purple";
             case SupportedColor.PINK:
                 return "pink";
         }
         return "white";
     }

     public SupportedColor StringToSupportedColor(string color)
     {
         switch (color)
         {
             case "red":
                 return SupportedColor.RED;
             case "orange":
                 return SupportedColor.ORANGE;
             case "yellow":
                 return SupportedColor.YELLOW;
             case "green":
                 return SupportedColor.GREEN;
             case "l_blue":
                 return SupportedColor.L_BLUE;
             case "d_blue":
                 return SupportedColor.D_BLUE;
             case "purple":
                 return SupportedColor.PURPLE;
             case "pink":
                 return SupportedColor.PINK;
         }
         return SupportedColor.WHITE;
     }



     public static Color SupportedColorToColor(SupportedColor supportedColor)
     {
         switch (supportedColor)
         {
             case SupportedColor.RED:
                 return HexToColorMethod("#FF4949");
             case SupportedColor.ORANGE:
                 return HexToColorMethod("#FF9549");
             case SupportedColor.YELLOW:
                 return HexToColorMethod("#FFEB49");
             case SupportedColor.GREEN:
                 return HexToColorMethod("#A0FF49");
             case SupportedColor.L_BLUE:
                 return HexToColorMethod("#49FFFD");
             case SupportedColor.D_BLUE:
                 return HexToColorMethod("#495DFF");
             case SupportedColor.PURPLE:
                 return HexToColorMethod("#D949FF");
             case SupportedColor.PINK:
                 return HexToColorMethod("#FF49C8");
         }
         return Color.white;
     }
     //TODO
     public static SupportedColor ColorToSupportedColor(Color color)
     {
         // Definir los colores aproximados en formato hexadecimal
         if (AreColorsApproximatelyEqual(color, HexToColorMethod("#FF4949")))
             return SupportedColor.RED;
         if (AreColorsApproximatelyEqual(color, HexToColorMethod("#FF9549")))
             return SupportedColor.ORANGE;
         if (AreColorsApproximatelyEqual(color, HexToColorMethod("#FFEB49")))
             return SupportedColor.YELLOW;
         if (AreColorsApproximatelyEqual(color, HexToColorMethod("#A0FF49")))
             return SupportedColor.GREEN;
         if (AreColorsApproximatelyEqual(color, HexToColorMethod("#49FFFD")))
             return SupportedColor.L_BLUE;
         if (AreColorsApproximatelyEqual(color, HexToColorMethod("#495DFF")))
             return SupportedColor.D_BLUE;
         if (AreColorsApproximatelyEqual(color, HexToColorMethod("#D949FF")))
             return SupportedColor.PURPLE;
         if (AreColorsApproximatelyEqual(color, HexToColorMethod("#FF49C8")))
             return SupportedColor.PINK;

         return SupportedColor.RED; // Valor por defecto si no coincide
     }

     // Función para comparar dos colores con un margen de error (para evitar problemas con valores de color flotantes)
     private static bool AreColorsApproximatelyEqual(Color color1, Color color2, float tolerance = 0.05f)
     {
         return Mathf.Abs(color1.r - color2.r) < tolerance &&
                Mathf.Abs(color1.g - color2.g) < tolerance &&
                Mathf.Abs(color1.b - color2.b) < tolerance;
     }

     public static Color HexToColorMethod(string hex)
     {
         hex = hex.Replace("#", "");

         if (hex.Length != 6)
         {
             return Color.black;
         }

         float r = Mathf.Clamp01(int.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber) / 255f);
         float g = Mathf.Clamp01(int.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber) / 255f);
         float b = Mathf.Clamp01(int.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber) / 255f);

         return new Color(r, g, b);
     }

     public void SetCurrentColor()
     {
         ChatMessage colorMessage = new ChatMessage
         {
             type = Network_MessageKey_SetUserColor,
             userId = localUser.ID,
             message = SupportedColorToString(localUser.favouriteColorSupported)
         };

     }

     public void SetPing(string latency)
     {
         this.latency = int.Parse(latency);
     }
}