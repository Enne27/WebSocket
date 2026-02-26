using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class localUserIdShow : MonoBehaviour
{
    TextMeshProUGUI text;
    WebSocketManager ws;

    private void Start()
    {
        ws = WebSocketManager.instance;
    }
    void Update()
    {
        text.text = ws.localUser.ID;
    }
}
