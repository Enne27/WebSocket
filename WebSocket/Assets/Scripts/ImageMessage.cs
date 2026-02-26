using System;

[Serializable]
public class ImageMessage
{
    public string type;
    public string userId;
    public string message;
    public string image; // Imagen en Base64
    public string targetNumber;

    public ImageMessage(string userId, string message, string image)
    {
        this.userId = userId;
        this.message = message;
        this.image = image;
    }
    public ImageMessage(string userId, string message, string image, string targetNum)
    {
        this.userId = userId;
        this.message = message;
        this.image = image;
        this.targetNumber = targetNum;
    }
    
}