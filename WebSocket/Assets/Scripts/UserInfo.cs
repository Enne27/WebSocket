using UnityEngine.Events;

[System.Serializable]
public class UserInfo {
    public string userName;
    public string userID;

    public UserInfo(string userID, string userName) {
        this.userID = userID;
        this.userName = userName;
        onUserChangedName = new UnityEvent<string>();
        onUserDisconnected = new UnityEvent();
    }

    public UnityEvent<string> onUserChangedName;
    public UnityEvent onUserDisconnected;

}
