using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonManager : MonoBehaviour
{
    public UserCreator creator;
    public UserManager manager;
    public AudioSource audioSource;
    public AudioClip soundButton;

    [Header("StartPage")]
    [SerializeField] GameObject StartPageObject;
    [SerializeField] GameObject UserConfigurationObject;
    [SerializeField] GameObject Settings;
    [SerializeField] GameObject Credits;

    [SerializeField] GameObject NotloggedUserInterface;
    [SerializeField] GameObject LoggedUserInterface;
    [Header("UserConfiguration")]
    [SerializeField] GameObject MainMenu;
    [SerializeField] GameObject AddUser;
    [SerializeField] GameObject ModifyUser;
    [Header("Rooms")]
    [SerializeField] GameObject RoomA;
    [SerializeField] GameObject RoomB;
    [SerializeField] GameObject RoomC;
    [SerializeField] GameObject RoomD;
    [Header("Exit Sprites")]
    [SerializeField] Sprite shutdown;
    [SerializeField] Sprite disconnect;
    [SerializeField] Button exitButton;
    [SerializeField] AudioSource happyBirthday;

    public float fadeDuration = 0.5f;

    private void Start()
    {
        manager = GetComponent<UserManager>();
        creator = GetComponent<UserCreator>();
        audioSource = GetComponent<AudioSource>();
        UserConfigurationObject.SetActive(false);
        Settings.SetActive(false);
        exitButton.onClick.AddListener(Exit);
    }

    public void buttonSound()
    {
        audioSource.clip = soundButton;
        audioSource.Play();
    }

    private IEnumerator FadeIn(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }
        obj.SetActive(true);
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0, 1, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = true;
    }

    private IEnumerator FadeOut(GameObject obj)
    {
        CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = obj.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1;
        canvasGroup.interactable = false;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1, 0, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0;
        obj.SetActive(false);
    }

    //-------Seccion Start Page-------//
    public void JoinChatRoomA()
    {
        
        if (manager.roomAUsers.Count < 8)
        {
            StartCoroutine(FadeOut(StartPageObject));
            StartCoroutine(FadeIn(RoomA));
            if (!manager.roomAUsers.Contains(manager.loggedUser))
                manager.JoinRoomA();
            else
            {
                buttonSound();
            }
        }
    }
    public void JoinChatRoomB()
    {
        if (manager.roomBUsers.Count < 8)
        {
            StartCoroutine(FadeOut(StartPageObject));
            StartCoroutine(FadeIn(RoomB));
            if (!manager.roomBUsers.Contains(manager.loggedUser))
                manager.JoinRoomB();
            else
            {
                buttonSound();
            }
        }
    }
    public void JoinChatRoomC()
    {
        if (manager.roomCUsers.Count < 8)
        {
            StartCoroutine(FadeOut(StartPageObject));
            StartCoroutine(FadeIn(RoomC));
            if (!manager.roomCUsers.Contains(manager.loggedUser))
                manager.JoinRoomC();
            else
            {
                buttonSound();
            }
        }
    }
    public void JoinChatRoomD()
    {
        if (manager.roomDUsers.Count < 8)
        {
            StartCoroutine(FadeOut(StartPageObject));
            StartCoroutine(FadeIn(RoomD));
            if (!manager.roomDUsers.Contains(manager.loggedUser))
                manager.JoinRoomD();
            else
            {
                buttonSound();
            }
        }
    }
    public void UserConfiguration()
    {
        
        buttonSound();
        StartCoroutine(FadeOut(StartPageObject));
        StartCoroutine(FadeIn(UserConfigurationObject));
    }
    public void Setttings()
    {

    }
    public void Exit()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    //-------Seccion Start Page-------//

    //-------Seccion User Configuration Page-------//
    public void mainMenuReturn()
    {
        creator.usernameInput.text = "";
        creator.dayInput.text = "";
        creator.monthInput.text = "";
        StartCoroutine(FadeOut(UserConfigurationObject));
        StartCoroutine(FadeIn(StartPageObject));
        buttonSound();
    }
    public void createUser()
    {
        buttonSound();
        StartCoroutine(FadeOut(MainMenu));
        StartCoroutine(FadeIn(AddUser));
    }
    public void userSelected()
    {
        buttonSound();
        if(MainMenu.activeSelf)
        {
            StartCoroutine(FadeOut(MainMenu));
        }
        else if (AddUser.activeSelf)
        {
            StartCoroutine(FadeOut(AddUser));
        }
        StartCoroutine(FadeIn(ModifyUser));

    }
    public void goBack()
    {
        buttonSound();
        creator.usernameInput.text = "";
        creator.dayInput.text = "";
        creator.monthInput.text = "";
        if (ModifyUser.activeSelf)
        {
            StartCoroutine(FadeOut(ModifyUser));
        }
        else if (AddUser.activeSelf)
        {
            StartCoroutine(FadeOut(AddUser));
        }
        StartCoroutine(FadeIn(MainMenu));
    }
    public void goBackCrear()
    {
        
        creator.usernameInput.text = "";
        creator.dayInput.text = "";
        creator.monthInput.text = "";
        exitButton.onClick.RemoveListener(Exit);
        exitButton.onClick.AddListener(DisconectUser);
        exitButton.transform.GetChild(0).GetComponent<Image>().sprite = disconnect;
        StartCoroutine(FadeOut(NotloggedUserInterface));
        StartCoroutine(FadeIn(LoggedUserInterface));
    }

    public void DisconectUser()
    {
        string escenaActual = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(escenaActual);

    }

    public void goBackModificar()
    {
        creator.usernameInput.text = "";
        creator.dayInput.text = "";
        creator.monthInput.text = "";
        StartCoroutine(FadeOut(UserConfigurationObject));
        StartCoroutine(FadeIn(StartPageObject));

    }

    public void ExitToMainMenuA()
    {
        StartCoroutine(FadeOut(RoomA));
        StartCoroutine(FadeIn(StartPageObject));
        buttonSound();
        RoomA.SetActive(false);
        StartPageObject.SetActive(true);
        happyBirthday.Stop();

    }

    public void DisconnectToMainMenuA()
    {
        StartCoroutine(FadeOut(RoomA));
        StartCoroutine(FadeIn(StartPageObject));
        RoomA.SetActive(false);
        StartPageObject.SetActive(true);
        manager.DisconnectRoomA();
        happyBirthday.Stop();
    }

    public void ExitToMainMenuB()
    {
        StartCoroutine(FadeOut(RoomB));
        StartCoroutine(FadeIn(StartPageObject));
        buttonSound();
        RoomB.SetActive(false);
        StartPageObject.SetActive(true);
        happyBirthday.Stop();
    }

    public void DisconnectToMainMenuB()
    {
        StartCoroutine(FadeOut(RoomB));
        StartCoroutine(FadeIn(StartPageObject));
        RoomB.SetActive(false);
        StartPageObject.SetActive(true);
        manager.DisconnectRoomB();
        happyBirthday.Stop();
    }

    public void ExitToMainMenuC()
    {
        StartCoroutine(FadeOut(RoomC));
        StartCoroutine(FadeIn(StartPageObject));
        buttonSound();
        RoomC.SetActive(false);
        StartPageObject.SetActive(true);
        happyBirthday.Stop();
    }

    public void DisconnectToMainMenuC()
    {
        StartCoroutine(FadeOut(RoomC));
        StartCoroutine(FadeIn(StartPageObject));
        RoomC.SetActive(false);
        StartPageObject.SetActive(true);
        manager.DisconnectRoomC();
        happyBirthday.Stop();
    }

    public void ExitToMainMenuD()
    {
        StartCoroutine(FadeOut(RoomD));
        StartCoroutine(FadeIn(StartPageObject));
        buttonSound();
        RoomD.SetActive(false);
        StartPageObject.SetActive(true);
        happyBirthday.Stop();
    }

    public void DisconnectToMainMenuD()
    {
        StartCoroutine(FadeOut(RoomD));
        StartCoroutine(FadeIn(StartPageObject));
        RoomD.SetActive(false);
        StartPageObject.SetActive(true);
        manager.DisconnectRoomD();
        happyBirthday.Stop();
    }

    public void settingsToStartMenu()
    {
        buttonSound();
        StartCoroutine(FadeOut(Settings));
        StartCoroutine(FadeIn(StartPageObject));
    }

    public void creditsToStartMenu()
    {
        buttonSound();
        StartCoroutine(FadeOut(Credits));
        StartCoroutine(FadeIn(StartPageObject));
    }

    public void ToCredits()
    {
        buttonSound();
        StartCoroutine(FadeOut(StartPageObject));
        StartCoroutine(FadeIn(Credits));
    }
    public void ToSettings()
    {
        buttonSound();
        StartCoroutine(FadeOut(StartPageObject));
        StartCoroutine(FadeIn(Settings));
    }
}
