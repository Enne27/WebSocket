using WebSocketSharp;
using UnityEngine;

public class HelloWorld : MonoBehaviour
{
    WebSocket WS;
    bool debug = true;

    void Start()
    {
        string serverUrl = "ws://localhost:3000"; // Replace with your server's IP or hostname
        WS = new WebSocket(serverUrl);

        WS.OnOpen += (sender, e) =>
        {
            if (debug)
            {
                Debug.Log("Connected to server");

                WS.Send("Hello from Unity!");

                WS.OnMessage += (sender, e) =>
                {
                    if (debug)
                    {
                        Debug.Log("Message from server: " + e.Data);
                        WS.Send("Hello from Unioty!");                   
                    }
                };

                WS.OnMessage += (sender, e) =>
                {
                    if (debug)
                    {
                        Debug.Log("Message from server: " + e.Data);

                        HandleServerMessage(e);
                        // Handle the data received fron the server
                    }
                };

                WS.OnClose += (sender, e) =>
                {
                    if (debug)
                    {
                        Debug.Log("Disconnected from server");
                        // Start the WebSocket connection
                        WS.Connect();
                    }
                };
            }
        };
    }

    private void HandleServerMessage(MessageEventArgs e)
    {
        Debug.Log(e.Data);
    }
}