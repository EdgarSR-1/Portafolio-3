using UnityEngine;
using UnityEngine.UI;


public class GameUIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private HandManager handManager;

    [Header("Botones")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    void Start()
    {
        leftButton.onClick.AddListener(PlayLeft);
        rightButton.onClick.AddListener(PlayRight);

        UpdateButtons();
    }

    private void Update()
    {
        UpdateButtons();
    }

    private void PlayLeft()
    {
        PlayTile(BoardSide.Left);
    }    

    private void PlayRight()
    {
        PlayTile(BoardSide.Right);
    }

    private void PlayTile(BoardSide side)
    {
        DominoTileView selectedTile = handManager.SelectedTile;

        if (selectedTile == null)
        {
            Debug.Log("Primero debes seleccionar una ficha");
            return;
        }

        PlaceTileCommand command =
            new PlaceTileCommand(boardManager, selectedTile, side);

        if (command.Execute())
        {
            handManager.RemoveTile(selectedTile);
            Debug.Log("Ficha colocada correctamente :D");
        } else
        {
            Debug.Log("No se puede colocar ficha aca");
        }

        UpdateButtons();
    }

    // Activa los botones solamente cuando hay una ficha seleccionada.
    private void UpdateButtons()
    {
        bool hasSelectedTile = handManager.SelectedTile != null;

        leftButton.interactable = hasSelectedTile;
        rightButton.interactable = hasSelectedTile;
    }

    private void OnDestroy()
    {
        leftButton.onClick.RemoveListener(PlayLeft);
        rightButton.onClick.RemoveListener(PlayRight);
    }

}
