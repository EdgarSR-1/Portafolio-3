using System;
using UnityEngine;
using UnityEngine.UI;

public class DominoTileView : MonoBehaviour
{
    [Header("Imagenes de elementos")]
    [SerializeField] private Image leftImage;
    [SerializeField] private Image rightImage;

    [Header("Sprites de elementos")]
    [SerializeField] private Sprite[] elementSprites;

    public DominoTileData Data { get; private set; }

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
}
