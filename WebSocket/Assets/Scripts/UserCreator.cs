using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;

public class UserCreator : MonoBehaviour
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

    public void SaveUser()
    {
        bool isValid = ValidateInput();

        if (isValid)
        {
            string username = usernameInput.text;
            Color favoriteColor = GetSelectedColor();
            int birthdayDay = int.Parse(dayInput.text);
            int birthdayMonth = int.Parse(monthInput.text);
            User newUser = new User(username, WebSocketManager.ColorToSupportedColor(favoriteColor), birthdayDay, birthdayMonth);

            // Crear y guardar el usuario en el UserManager
            userManager.AddUser(newUser);
            //websocketManager.TryToSetUsername(usernameInput.text);
            //websocketManager.OnConnectButtonPressed();
            audioSource.clip = valid;
            audioSource.Play();

            // Limpiar campos
            usernameInput.text = "";
            dayInput.text = "";
            monthInput.text = "";
            //buttonManager.goBackCrear();
        }
    }

    private bool ValidateInput()
    {
        bool isValid = true;

        // Validar que el nombre de usuario no esté vacío
        if (string.IsNullOrEmpty(usernameInput.text))
            isValid = false;

        // Validar que haya un toggle seleccionado en el ToggleGroup
        if (colorToggleGroup.ActiveToggles().GetEnumerator().MoveNext() == false)
            isValid = false;

        // Validar día
        if (!int.TryParse(dayInput.text, out int day) || day < 1 || day > 31)
            isValid = false;

        // Validar mes
        if (!int.TryParse(monthInput.text, out int month) || month < 1 || month > 12)
            isValid = false;
        else
        {
            // Validación de los días según el mes
            if (!IsValidDayForMonth(day, month))
                isValid = false;
        }

        return isValid;
    }

    // Método para validar si el día es válido para un mes dado
    private bool IsValidDayForMonth(int day, int month)
    {
        // Definir los días máximos para cada mes
        int[] daysInMonth = new int[]
        {
        31,  // Enero
        29,  // Febrero (siempre tiene 29 días)
        31,  // Marzo
        30,  // Abril
        31,  // Mayo
        30,  // Junio
        31,  // Julio
        31,  // Agosto
        30,  // Septiembre
        31,  // Octubre
        30,  // Noviembre
        31   // Diciembre
        };

        // Comprobar si el día es válido para el mes dado
        return day <= daysInMonth[month - 1];
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

    private Toggle GetSelectedToggle()
    {
        foreach (Toggle toggle in colorToggleGroup.ActiveToggles())
        {
            Image toggleImage = toggle.GetComponentInChildren<Image>();
            if (toggleImage != null)
            {
                return toggle;
            }
        }
        return null;
    }

    public WebSocketManager.SupportedColor GetSelectedSupportedColor()
    {
        return GetSelectedToggle().GetComponent<SupportedColorReference>().colorReference;
    }
}