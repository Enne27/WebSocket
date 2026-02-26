using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkMessages : MonoBehaviour {
    public const string Network_MessageKey_Type = "type";
    public const string Network_MessageKey_UserID = "userId";
    public const string Network_MessageKey_SetUserName = "setUserName";
    public const string Network_MessageKey_SetUserRoom = "setUserRoom";
    public const string Network_MessageKey_SetUserColor= "setUserColor";
    public const string Network_MessageKey_Chat = "chat";
    public const string Network_MessageKey_GameMessage = "gameMessage";
    public const string Network_MessageKey_Disconnection = "disconnect";
    public const string Network_MessageKey_OtherUsers = "otherUsers";
    public const string Network_MessageKey_Image = "image";

    public const string Network_Content_Message = "message";
    public const string Network_Content_UserName = "userName";
    public const string Network_Content_Color = "favouriteColor";
    public const string Network_Content_UserID = "userId";
    public const string NetWork_Content_BirthDay = "birthDay";
    public const string NetWork_Content_BirthMonth = "birthMonth";
    public const string Network_Content_Users = "users";
    public const string Network_SystemMessage = "system";
    public const string Network_Content_Room = "room";
    public const string Network_Content_Image = "image";
}
