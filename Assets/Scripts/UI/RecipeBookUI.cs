using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class RecipeBookUI : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardManager board;
    [SerializeField] private ReactionManager reactionManager;

    [Header("UI")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Transform rowsContent;
    [SerializeField] private RecipeRowView rowPrefab;

    [Header("Sprites")]
    // mismo orden que el enum ElementType
    // igual que en DominoTileView
    [SerializeField] private Sprite[] elementSprites;
    [SerializeField] private Sprite questionSprite;

    private readonly List<RecipeRowView> rows = new List<RecipeRowView>();

    private void Start()
    {
        // una fila por receta
        // se tienen que ocultar antes de que el jugador las cheque
        foreach (Recipe recipe in reactionManager.Recipes)
        {
            RecipeRowView row = Instantiate(rowPrefab, rowsContent);
            row.Show(questionSprite, questionSprite, questionSprite);
            rows.Add(row);
        }

        panel.SetActive(false);
    }

    private void OnEnable()
    {
        board.OnReactionMade += RevealRecipe;
    }

    private void OnDisable()
    {
        board.OnReactionMade -= RevealRecipe;
    }

    public void TogglePanel()
    {
        panel.SetActive(!panel.activeSelf);
    }

    private void RevealRecipe(ElementType a, ElementType b, ElementType result)
    {
        for (int i = 0; i < reactionManager.Recipes.Count; i++)
        {
            Recipe r = reactionManager.Recipes[i];

            // la receta es la misma sin importar el orden
            bool sameRecipe = (r.first == a && r.second == b) ||
                (r.first == b && r.second == a);

            if (sameRecipe)
            {
                rows[i].Show(
                    elementSprites[(int)r.first],
                    elementSprites[(int)r.second],
                    elementSprites[(int)r.result]);
                return;
            }
        }
    }
}
