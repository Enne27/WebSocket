using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ConnectedUsersManager : MonoBehaviour {

    #region Singleton
    [Header("Singleton")]
    static ConnectedUsersManager connectedUsersManager;
    public static ConnectedUsersManager instance {
        get {
            return RequestConnectedUsersManager();
        }
    }

    static ConnectedUsersManager RequestConnectedUsersManager() {
        if (connectedUsersManager == null) {
            connectedUsersManager = FindAnyObjectByType<ConnectedUsersManager>();
        }
        return connectedUsersManager;
    }
    #endregion

    [SerializeField]  GameObject remoteUserRepresentationPrefab;
    [SerializeField]  Transform remoteUserRepresentationsParent;
    //private List<RemoteUserInfoRepresentation> remoteUserRepresentations = new List<RemoteUserInfoRepresentation>();
      WebSocketManager wsManager;

    private void OnEnable() {
        
        if (WebSocketManager.instance) {
            
        }
        wsManager = GetComponent<WebSocketManager>();
    }

    private void OnDisable() {
        if (WebSocketManager.instance) {

        }
    }
    public void ClearRemoteUsersRepresentation() {
        UpdateUserButtons();
    }

    public void SetUpUserRepresentation() {
        
        UpdateUserButtons();
    }

    public void UpdateUserButtons()
    {
        // Limpiar los botones actuales
        foreach (Transform child in remoteUserRepresentationsParent)
        {
            Destroy(child.gameObject);
        }

        // Crear los botones actualizados con los usuarios modificados
        foreach (User user in wsManager.remoteUsers)
        {
            CreateUserButton(user);
        }
    }

    private void CreateUserButton(User user)
    {
        GameObject userButton = Instantiate(remoteUserRepresentationPrefab, remoteUserRepresentationsParent);
        Button button = userButton.GetComponent<Button>();
        TMP_Text buttonText = userButton.GetComponentInChildren<TMP_Text>();

        buttonText.text = user.username;
        buttonText.color = user.favoriteColor;


        // Forzar actualizaci�n del layout
        LayoutRebuilder.ForceRebuildLayoutImmediate(remoteUserRepresentationsParent.GetComponent<RectTransform>());
    }
}
