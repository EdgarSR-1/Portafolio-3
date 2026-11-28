using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DominoTileView : MonoBehaviour
{
    [Header("Imagenes de elementos")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;

    [Header("Sprites de elementos")]
    [SerializeField] private Sprite[] elementSprites;

    [Header("Selección")]
    [SerializeField] private Button tileButton;
    [SerializeField] private Image selectionBorder;
    [SerializeField] private Sprite[] selectionSprites;
    [SerializeField] private float animationSpeed = 0.2f;

    public DominoTileData Data { get; private set; }

    // Patron observer: avisa cuando se hace click en la ficha
    public event Action<DominoTileView> OnSelected;

    private Coroutine selectionAnimation;

    public void Awake()
    {
        tileButton.onClick.AddListener(SelectTile);
        selectionBorder.enabled = false;
    }

    public void SetData(DominoTileData data)
    {
        Data = data;

        leftImage.sprite = GetElementSprite(data.leftElement);

        rightImage.sprite = GetElementSprite(data.rightElement);
    }

    private Sprite GetElementSprite(ElementType element)
    {
        int index = System.Convert.ToInt32(element);

        if (index < 0 || index >= elementSprites.Length)
        {
            Debug.LogWarning($"No existe un sprite para {element}");
            return null;
        }

        return elementSprites[index];
    }

    private void SelectTile()
    {
        // if (OnSelected != null ) OnSelected(this);
        OnSelected?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        if (selectionAnimation != null)
        {
            StopCoroutine(selectionAnimation);
            selectionAnimation = null;
        }

        if (!selected)
        {
            selectionBorder.enabled = false;
            return;
        }

        selectionBorder.enabled = true;
        selectionAnimation = StartCoroutine(PlaySelectionAnimation());
    }

    private IEnumerator PlaySelectionAnimation()
    {
        int currentFrame = 0;
        
        while (true)
        {
            selectionBorder.sprite = selectionSprites[currentFrame];

            currentFrame++;

            if (currentFrame >= selectionSprites.Length) currentFrame = 0;

            yield return new WaitForSeconds(animationSpeed);
        }
    }

    public void Flip()
    {
        ElementType temp = Data.leftElement;

        Data.leftElement = Data.rightElement;
        Data.rightElement = temp;

        // actualizar imagenes
        leftImage.sprite = GetElementSprite(Data.leftElement);
        rightImage.sprite = GetElementSprite(Data.rightElement);
    }

    public void SetLeftElement(ElementType element)
    {
        Data.leftElement = element;
        leftImage.sprite = GetElementSprite(element);
    }

    public void SetRightElement(ElementType element)
    {
        Data.rightElement = element;
        rightImage.sprite = GetElementSprite(element);
    }

    private void OnDestroy()
    {
        if (tileButton != null) tileButton.onClick.RemoveListener(SelectTile);
    }
}
