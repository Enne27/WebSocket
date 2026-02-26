using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalUserNameManager : MonoBehaviour
{

    #region Singleton
    [Header("Singleton")]
    static LocalUserNameManager localUsernameNameManager;
    public static LocalUserNameManager instance {
        get {
            return RequestChatMessageManager();
        }
    }

    static LocalUserNameManager RequestChatMessageManager() {
        if (localUsernameNameManager == null) {
            localUsernameNameManager = FindAnyObjectByType<LocalUserNameManager>();
        }
        return localUsernameNameManager;
    }
    #endregion

    [SerializeField] TMP_InputField initialNameField;
    [SerializeField] TMP_InputField initialDay;
    [SerializeField] TMP_InputField initialMonth;
    
    [SerializeField] TMP_InputField changeUserNameField;
    [SerializeField] TMP_InputField changeDay;
    [SerializeField] TMP_InputField changeMonth;
    [SerializeField] Button setUserNameButton;

    private void OnEnable() {
        if (setUserNameButton) setUserNameButton.onClick.AddListener(TryToChangeUserName);
        WebSocketManager.instance.onLocalUserConnected.AddListener(TryToSetInitialUserName);
    }

    private void OnDisable() {
        if (setUserNameButton) setUserNameButton.onClick.RemoveListener(TryToChangeUserName);
        WebSocketManager.instance.onLocalUserConnected.RemoveListener(TryToSetInitialUserName);
    }


    void TryToSetInitialUserName() {
        WebSocketManager.instance.TryToSetUsername(initialNameField.text);
    }
    void TryToChangeUserName() {
        WebSocketManager.instance.TryToSetUsername(changeUserNameField.text);
    }

}
