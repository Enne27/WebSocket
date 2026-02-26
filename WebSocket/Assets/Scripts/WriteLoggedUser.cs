using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WriteLoggedUser : MonoBehaviour
{
    public UserManager userManager;
    private TextMeshProUGUI username;

    // Start is called before the first frame update
    void Start()
    {
        username = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        username.text = userManager.loggedUser.username;
        username.color = userManager.loggedUser.favoriteColor;
    }
}
