using UnityEngine;
using UnityEngine.UI;

public class RecipeRowView : MonoBehaviour
{
    [SerializeField] private Image firstImage;
    [SerializeField] private Image secondImage;
    [SerializeField] private Image resultImage;

    public void Show(Sprite first, Sprite second, Sprite result)
    {
        firstImage.sprite = first;
        secondImage.sprite = second;
        resultImage.sprite = result;
    }
}
