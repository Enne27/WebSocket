using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class internetImagePing : MonoBehaviour
{
    private int ping;

    [SerializeField] Sprite VeryGood;
    [SerializeField] Sprite Good;
    [SerializeField] Sprite Medium;
    [SerializeField] Sprite Bad;

    private Image image;
    private void Start()
    {
        image = GetComponent<Image>();
    }
    private void Update()
    {
        ping = WebSocketManager.instance.latency;

        if ( ping >= 1 && ping < 90)
            image.sprite = VeryGood;
        else if (ping >= 90 && ping < 150)
            image.sprite = Good;
        else if (ping >= 150 && ping < 300)
            image.sprite = Medium;
        else
            image.sprite = Bad;
    }
}