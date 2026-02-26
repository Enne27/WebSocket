using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Unity.VisualScripting;
using System;
using UnityEngine.Events;


[System.Serializable]
public class User
{
    public string ID;
    public string username;
    public WebSocketManager.SupportedColor favouriteColorSupported;
    public Color favoriteColor;
    public int birthdayDay;
    public int birthdayMonth;
    public string room;
    public User(string username, WebSocketManager.SupportedColor favouriteColorSupported, int birthdayDay, int birthdayMonth)
    {
        this.username = username;
        //TODO SET  WebSocketManager.SupportedColor favouriteColorSupported as favouriteColor (transofrmar el color supported a color normal)
        this.favouriteColorSupported = favouriteColorSupported;
        favoriteColor = WebSocketManager.SupportedColorToColor(favouriteColorSupported);
        this.birthdayDay = birthdayDay;
        this.birthdayMonth = birthdayMonth;
        onUserChangedName = new UnityEvent<string>();
        onUserDisconnected = new UnityEvent();
    }

    public User(string ID, string username, WebSocketManager.SupportedColor favouriteColorSupported, int birthdayDay, int birthdayMonth, string room)
    {
        this.username = username;
        this.ID = ID;
        this.favouriteColorSupported = favouriteColorSupported;
        //TODO SET  WebSocketManager.SupportedColor favouriteColorSupported as favouriteColor (transofrmar el color supported a color normal)
        favoriteColor = WebSocketManager.SupportedColorToColor(favouriteColorSupported);
        this.birthdayDay = birthdayDay;
        this.birthdayMonth = birthdayMonth;
        this.room = room;
        onUserChangedName = new UnityEvent<string>();
        
        onUserDisconnected = new UnityEvent();
        //onUserDisconnected.AddListener(ConnectedUsersManager.UpdateUserButtons);
    }

    public UnityEvent<string> onUserChangedName;
    public UnityEvent onUserDisconnected;
}

public class UserManager : MonoBehaviour
{
    [Header("User List")]
    public GameObject userButtonPrefab;
    public Transform userListContent;

    [Header("Users Online")]
    public List<User> users = new List<User>();
    public List<User> roomAUsers = new List<User>();
    public List<User> roomBUsers = new List<User>();
    public List<User> roomCUsers = new List<User>();
    public List<User> roomDUsers = new List<User>();

    // Variable que mantiene al usuario loggeado
    [SerializeField] public User loggedUser;

    [Header("User Counts Rooms")]
    public TextMeshProUGUI usersRoomACount;
    public TextMeshProUGUI usersRoomBCount;
    public TextMeshProUGUI usersRoomCCount;
    public TextMeshProUGUI usersRoomDCount;

    [Header("Chats")]
    public Transform RoomAChat;
    public Transform RoomBChat;
    public Transform RoomCChat;
    public Transform RoomDChat;

    [Header("System Messages")]
    public GameObject SystemMessagePrefab;
    public GameObject HappyBirthdaytMessagePrefab;

    [Header("User Display Elements")]
    public TMP_InputField usernameInput;
    public ToggleGroup colorToggleGroup;
    public TMP_InputField dayInput;
    public TMP_InputField monthInput;

    [Header("Dropdown")]
    public TMP_Text userName;

    private AudioSource audioSource;
    [Header("Efectos de sonido")]
    public AudioClip conectarse;
    public AudioClip desconectarse;

    private WebSocketManager wsManager;
    private ChatMessageManagerA chatMessageManager;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        wsManager = GetComponent<WebSocketManager>();
        chatMessageManager = GetComponent<ChatMessageManagerA>();
    }

    public void AddUser(User newUser)
    {
        users.Add(newUser);
        loggedUser = newUser;
        userName.text = newUser.username;
        userName.color = newUser.favoriteColor;
        CreateUserButton(newUser);
        wsManager.remoteUsers.Add(newUser);
        wsManager.localUser = loggedUser;
    }

    public void ModifyUser(User userModified)
    {
        userModified.ID = loggedUser.ID;
        foreach (User user in users)
        {
            if (user.ID == loggedUser.ID)
            {

                user.username = userModified.username;
                user.favouriteColorSupported = userModified.favouriteColorSupported;
                user.favoriteColor = userModified.favoriteColor;
                user.birthdayDay = userModified.birthdayDay;
                user.birthdayMonth = userModified.birthdayMonth;
            }
        }
        loggedUser = userModified;
        userName.text = userModified.username;
        userName.color = userModified.favoriteColor;
        UpdateUserButtons();
    }

    public void DeleteUser()
    {
        if (loggedUser == null)
        {
            Debug.LogError("No se puede eliminar un usuario nulo.");
            return;
        }

        // Verificar si el usuario está en la lista antes de intentar eliminarlo
        if (!users.Contains(loggedUser))
        {
            Debug.LogWarning("El usuario no se encuentra en la lista.");
            return;
        }

        // Eliminar el usuario de la lista
        users.Remove(loggedUser);

        // Si el usuario eliminado es el seleccionado, actualizamos la selección
        Debug.Log($"Usuario '{loggedUser.username}' eliminado con éxito.");
        loggedUser = null;
        // Actualizar la interfaz de usuario: botones y dropdown
        UpdateUserButtons(); // Re-crea los botones para los usuarios restantes

        // Opcional: Log para depuración
    }


    private void CreateUserButton(User user)
    {
        GameObject userButton = Instantiate(userButtonPrefab, userListContent);
        Button button = userButton.GetComponent<Button>();
        TMP_Text buttonText = userButton.GetComponentInChildren<TMP_Text>();

        buttonText.text = user.username;
        buttonText.color = user.favoriteColor;


        // Forzar actualización del layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(userListContent.GetComponent<RectTransform>());
    }
    public void UpdateUserButtons()
    {
        // Limpiar los botones actuales
        foreach (Transform child in userListContent)
        {
            Destroy(child.gameObject);
        }

        // Crear los botones actualizados con los usuarios modificados
        foreach (User user in users)
        {
            CreateUserButton(user);
        }
    }

    public void JoinRoomA()
    {
        if (!roomAUsers.Contains(loggedUser))
        {
            audioSource.clip = conectarse;
            audioSource.Play();
            roomAUsers.Add(loggedUser);
            
            wsManager.localUser.room = "roomA";
            wsManager.TryToSetRoom("roomA");
            wsManager.SendMessageToServer("enter",true);
        }
    }

    public void JoinRoomB()
    {
        if (!roomBUsers.Contains(loggedUser))
        {
            audioSource.clip = conectarse;
            audioSource.Play();
            roomBUsers.Add(loggedUser);

            wsManager.localUser.room = "roomB";
            wsManager.TryToSetRoom("roomB");
            wsManager.SendMessageToServer("enter", true);
        }
    }

    public void JoinRoomC()
    {
        if (!roomCUsers.Contains(loggedUser))
        {
            audioSource.clip = conectarse;
            audioSource.Play();
            roomCUsers.Add(loggedUser);

            wsManager.localUser.room = "roomC";
            wsManager.TryToSetRoom("roomC");
            wsManager.SendMessageToServer("enter", true);
        }
    }

    public void JoinRoomD()
    {
        if (!roomDUsers.Contains(loggedUser))
        {
            audioSource.clip = conectarse;
            audioSource.Play();
            roomDUsers.Add(loggedUser);

            wsManager.localUser.room = "roomD";
            wsManager.TryToSetRoom("roomD");
            wsManager.SendMessageToServer("enter", true);
        }
    }

    public void DisconnectRoomA()
    {
        audioSource.clip = desconectarse;
        audioSource.Play();
        roomAUsers.Remove(loggedUser);
        wsManager.SendMessageToServer("exit", true);
        WebSocketManager.instance.TryToSetRoom("");
    }

    public void DisconnectRoomB()
    {
        audioSource.clip = desconectarse;
        audioSource.Play();
        roomBUsers.Remove(loggedUser);
        wsManager.SendMessageToServer("exit", true);
        WebSocketManager.instance.TryToSetRoom("");
    }

    public void DisconnectRoomC()
    {
        audioSource.clip = desconectarse;
        audioSource.Play();
        roomCUsers.Remove(loggedUser);
        wsManager.SendMessageToServer("exit", true);
        WebSocketManager.instance.TryToSetRoom("");
    }

    public void DisconnectRoomD()
    {
        audioSource.clip = desconectarse;
        audioSource.Play();
        roomDUsers.Remove(loggedUser);
        wsManager.SendMessageToServer("exit", true);
        WebSocketManager.instance.TryToSetRoom("");
    }
}