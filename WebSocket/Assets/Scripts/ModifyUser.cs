using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ModifyUser : MonoBehaviour
{
    [Header("UI Elements")]
    public TextMeshProUGUI usernameTitle;
    public TMP_InputField usernameInput;
    public TextMeshProUGUI colorTitle;
    public ToggleGroup colorToggleGroup;
    public TextMeshProUGUI BirthdayTitle;
    public TMP_InputField dayInput;
    public TMP_InputField monthInput;
    public Button saveButton;

    public UserManager userManager;

    private AudioSource audioSource;
    
    public AudioClip invalid;
    public AudioClip valid;

    private ButtonManager buttonManager;

    WebSocketManager websocketManager;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        buttonManager = GetComponent<ButtonManager>();
        websocketManager = GetComponent<WebSocketManager>();
        saveButton.onClick.AddListener(SaveUser);
    }

    private void Update()
    {
        bool isValid = ValidateInput();

        if (isValid)
        {
            saveButton.interactable = true;
        }
        else
        {
            saveButton.interactable = false;
        }
    }

    private void SaveUser()
    {
        bool isValid = ValidateInput();

        if (isValid)
        {
            Debug.Log("Creando Usuario");
            string username = usernameInput.text;
            Color favoriteColor = GetSelectedColor();
            int birthdayDay = int.Parse(dayInput.text);
            int birthdayMonth = int.Parse(monthInput.text);
            User newUser = new User(username, WebSocketManager.ColorToSupportedColor(favoriteColor), birthdayDay, birthdayMonth);

            // Crear y guardar el usuario en el UserManager
            userManager.ModifyUser(newUser);
            websocketManager.TryToModify(newUser);
            //websocketManager.OnConnectButtonPressed();
            audioSource.clip = valid;
            audioSource.Play();

            // Limpiar campos
            usernameInput.text = "";
            dayInput.text = "";
            monthInput.text = "";
            buttonManager.goBackModificar();
        }
    }

    private bool ValidateInput()
    {
        bool isValid = true;

        if (string.IsNullOrEmpty(usernameInput.text))
            isValid = false;

        if (colorToggleGroup.ActiveToggles().GetEnumerator().MoveNext() == false)
            isValid = false;

        if (!int.TryParse(dayInput.text, out int day) || day < 1 || day > 31)
            isValid = false;

        if (!int.TryParse(monthInput.text, out int month) || month < 1 || month > 12)
            isValid = false;

        return isValid;
    }

    private Color GetSelectedColor()
    {
        foreach (Toggle toggle in colorToggleGroup.ActiveToggles())
        {
            Image toggleImage = toggle.GetComponentInChildren<Image>();
            if (toggleImage != null)
            {
                toggle.isOn = false;
                return toggleImage.color;
            }
        }
        return Color.white;
    }



}