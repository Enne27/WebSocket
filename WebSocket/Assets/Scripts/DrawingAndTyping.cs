using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum TarguetNum
{
    None,
    Targuet1,
    Targuet2, 
    Targuet3,
    Targuet4,
    Targuet5,
    Targuet6,
    Targuet7,
    Targuet8
}

public class DrawingAndTyping : MonoBehaviour
{
    [Header("Canvas Parameters")]
    public TMP_InputField inputField;       // Campo de texto donde se escribe
    public TMP_Text placeholderText;
    public RawImage drawingCanvas;          // Lienzo donde se dibuja
    public Texture2D drawingTexture;
    public List<Material> ObjectMaterials;// Textura que almacena el dibujo
    public Texture2D brushTexture;          // Textura del pincel
    public Color drawingColor = Color.black; // Color del trazo de dibujo
    public float drawingWidth = 10f;        // Ancho del trazo (pincel)

    [Header("PencilType")]
    public Toggle pencilToggle;             // Toggle para el lápiz
    public Toggle eraserToggle;             // Toggle para la goma
    public ToggleGroup pencilEraserGroup;  // ToggleGroup para lápiz/goma

    [Header("PencilSize")]
    public Toggle smallSizeToggle;          // Toggle para tamaño pequeño
    public Toggle mediumSizeToggle;         // Toggle para tamaño mediano
    public Toggle largeSizeToggle;          // Toggle para tamaño grande
    public ToggleGroup sizeGroup;           // ToggleGroup para tamaño

    [Header("PencilColor")]
    public Toggle blackColorToggle;         // Toggle para color negro
    public Toggle blueColorToggle;          // Toggle para color azul
    public Toggle redColorToggle;           // Toggle para color rojo
    public ToggleGroup colorGroup;          // ToggleGroup para color

    [Header("ClearCanvas")]
    public Button clearButton;              // Botón para borrar todo

    private Vector2 previousMousePosition;
    private bool isDrawing = false;
    private bool hasClickedCanvas = false;  // Controla si el usuario ya ha hecho clic en el canvas
    public bool isDrawn = false;

    private AudioSource audioSource;

    [Header("Audios")]
    public AudioClip pencilSound;
    public AudioClip eraserSound;
    public AudioClip eraseAllSound;

    public AudioClip[] typingSounds;  // Array que contendrá los 3 sonidos para el efecto de tipeo

    public int targuetRendering = 0;


    void Start()
    {
        // Inicializar el campo de texto y configurar los toggles
        if(inputField!=null)
            inputField.interactable = false; // Desactivamos el campo de texto al inicio
        drawingCanvas.raycastTarget = false; // Desactivamos interactividad del canvas hasta el primer clic

        // Asignar eventos a los Toggles
        pencilToggle.onValueChanged.AddListener(OnPencilToggleChanged);
        eraserToggle.onValueChanged.AddListener(OnEraserToggleChanged);
        smallSizeToggle.onValueChanged.AddListener(OnSizeChanged);
        mediumSizeToggle.onValueChanged.AddListener(OnSizeChanged);
        largeSizeToggle.onValueChanged.AddListener(OnSizeChanged);
        blackColorToggle.onValueChanged.AddListener(OnColorChanged);
        blueColorToggle.onValueChanged.AddListener(OnColorChanged);
        redColorToggle.onValueChanged.AddListener(OnColorChanged);

        // Asignar evento al botón de borrar
        

        // Inicializamos el lápiz por defecto
        OnPencilToggleChanged(true);
        OnSizeChanged(true);
        OnColorChanged(true);

        audioSource = GetComponent<AudioSource>();
        if (inputField != null)
            inputField.onValueChanged.AddListener(OnTextInput);
    }

    void Update()
    {
        HandleDrawing();
    }

    void OnTextInput(string text)
    {
        if (typingSounds != null && typingSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, typingSounds.Length);  // Seleccionar un índice aleatorio
            audioSource.PlayOneShot(typingSounds[randomIndex]);      // Reproducir el sonido seleccionado
        }
    }

    // Cambiar a lápiz
    void OnPencilToggleChanged(bool isOn)
    {
        if (isOn)
        {
            if (blackColorToggle.isOn)
            {
                drawingColor = Color.black; // Lápiz es de color negro
            }
            else if (redColorToggle.isOn)
            {
                drawingColor = Color.red;
            }
            else if (blueColorToggle.isOn)
            {
                drawingColor = Color.blue;
            }
        
        }
    }

    // Cambiar a goma de borrar
    void OnEraserToggleChanged(bool isOn)
    {
        if (isOn)
        {
            drawingColor = Color.white; // Goma es de color blanco
        }
    }

    // Cambiar el tamaño del trazo
    void OnSizeChanged(bool isOn)
    {
        if (smallSizeToggle.isOn)
        {
            drawingWidth = 1f;
        }
        else if (mediumSizeToggle.isOn)
        {
            drawingWidth = 3f;
        }
        else if (largeSizeToggle.isOn)
        {
            drawingWidth = 5f;
        }
    }

    // Cambiar el color del trazo
    void OnColorChanged(bool isOn)
    {
        if (blackColorToggle.isOn && drawingColor != Color.white)
        {
            drawingColor = Color.black;
        }
        else if (blueColorToggle.isOn && drawingColor != Color.white)
        {
            drawingColor = Color.blue;
        }
        else if (redColorToggle.isOn && drawingColor != Color.white)
        {
            drawingColor = Color.red;
        }
    }

    void HandleDrawing()
    {
        if (Input.GetMouseButtonDown(0) && RectTransformUtility.RectangleContainsScreenPoint(drawingCanvas.rectTransform, Input.mousePosition, null))
        {
            if (!hasClickedCanvas)  // Detectamos el primer clic en el canvas
            {
                hasClickedCanvas = true;
                if (inputField != null)
                    inputField.interactable = true; // Activamos el campo de texto
                drawingCanvas.raycastTarget = true; // Activamos el canvas para permitir dibujar
                if (placeholderText!=null)
                    placeholderText.text = ""; // Limpiar el texto de marcador de posición
            }
        }

        // Si ya se ha hecho el primer clic, permitimos el dibujo
        if (hasClickedCanvas && Input.GetMouseButton(0) && RectTransformUtility.RectangleContainsScreenPoint(drawingCanvas.rectTransform, Input.mousePosition, null))
        {
            if (!isDrawing)
            {
                CreateDrawingTextureIfNeeded(); // Solo se crea la textura después del primer clic
                if (pencilToggle.isOn && pencilSound != null && !audioSource.isPlaying)
                {
                    audioSource.clip = pencilSound;
                    audioSource.loop = true; // Configura para repetir el sonido mientras dibujas
                    audioSource.Play();
                }
                else if(eraserToggle.isOn && pencilSound != null && !audioSource.isPlaying)
                {
                    audioSource.clip = eraserSound;
                    audioSource.loop = true; // Configura para repetir el sonido mientras dibujas
                    audioSource.Play();
                }
            }
            

            Vector2 mousePos = Input.mousePosition;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(drawingCanvas.rectTransform, mousePos, null, out mousePos);

            // Normalizamos las coordenadas de dibujo para que no se salgan de la textura
            mousePos.x += drawingCanvas.rectTransform.rect.width / 2;
            mousePos.y += drawingCanvas.rectTransform.rect.height / 2;

            if (isDrawing)
            {
                DrawBrush(previousMousePosition, mousePos); // Dibuja con la textura de pincel

            }

            previousMousePosition = mousePos;
            isDrawing = true;
            
        }
        else
        {
            if (isDrawing && audioSource.isPlaying)
            {
                // Detener el sonido del lápiz cuando se deja de dibujar
                audioSource.Stop();
            }
            isDrawing = false;
        }
    }
    /// <summary>
    /// 
    /// </summary>
    void CreateDrawingTextureIfNeeded()
    {
        if (drawingTexture == null)
        {
            drawingTexture = new Texture2D((int)drawingCanvas.rectTransform.rect.width, (int)drawingCanvas.rectTransform.rect.height);
            drawingTexture.filterMode = FilterMode.Point;
            drawingTexture.wrapMode = TextureWrapMode.Repeat;
            drawingCanvas.texture = drawingTexture;
            if (drawingTexture != null)
            {
                for (int x = 0; x < drawingTexture.width; x++)
                {
                    for (int y = 0; y < drawingTexture.height; y++)
                    {
                        drawingTexture.SetPixel(x, y, Color.white); // Fondo blanco
                    }
                }
                drawingTexture.Apply();
                
                if (ObjectMaterials[targuetRendering] != null)
                {
                    ObjectMaterials[targuetRendering].mainTexture = drawingTexture;
                }
            }
            
        }
    }

    void DrawBrush(Vector2 start, Vector2 end)
    {
        float distance = Vector2.Distance(start, end);
        Vector2 direction = (end - start).normalized;

        for (float i = 0; i < distance; i += drawingWidth)
        {
            Vector2 point = start + direction * i;
            ApplyBrushToCanvas(point, drawingWidth);
        }

        drawingTexture.Apply();
        
    }

    void ApplyBrushToCanvas(Vector2 position, float width)
    {
        int x = Mathf.FloorToInt(position.x);
        int y = Mathf.FloorToInt(position.y);

        int brushSize = Mathf.CeilToInt(width);
        for (int dx = -brushSize; dx <= brushSize; dx++)
        {
            for (int dy = -brushSize; dy <= brushSize; dy++)
            {
                if (x + dx >= 0 && x + dx < drawingTexture.width && y + dy >= 0 && y + dy < drawingTexture.height)
                {
                    drawingTexture.SetPixel(x + dx, y + dy, drawingColor);
                    isDrawn = true;
                }
            }
        }
    }

    public void ClearAll(bool isSending)
    {
        if (drawingTexture != null)
        {
            StartCoroutine(ClearCanvasWithAnimation());
        }
        if (!isSending)
        {
            audioSource.clip = eraseAllSound;
            audioSource.loop = false;
            audioSource.Play();
        }
        if(inputField!=null)
            inputField.text = "";
        if(placeholderText!=null)
            placeholderText.text = "<b>Write</b> or <b>Draw</b> something ...";
        hasClickedCanvas = false; // Reiniciar el estado para requerir un nuevo clic
        if (inputField != null) 
            inputField.interactable = false; // Desactivar interactividad del campo de texto
        drawingCanvas.raycastTarget = false; // Desactivar interactividad del canvas
    }

    IEnumerator ClearCanvasWithAnimation()
    {
        int height = drawingTexture.height;
        int width = drawingTexture.width;
        int stepSize = 2;

        for (int y = 0; y < height; y += stepSize)
        {
            for (int x = 0; x < width; x++)
            {
                for (int stepY = 0; stepY < stepSize && (y + stepY) < height; stepY++)
                {
                    drawingTexture.SetPixel(x, y + stepY, Color.white);
                }
            }
            drawingTexture.Apply();
            yield return null;
        }
        drawingTexture = null;
        isDrawn=false;
    }
}
