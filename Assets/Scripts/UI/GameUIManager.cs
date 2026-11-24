using UnityEditor.Build;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


public class GameUIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private HandManager handManager;
    [SerializeField] private DeckManager deckManager;

    [Header("Botones")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button passButton;

    [SerializeField] private TurnManager turnManager;


    void Start()
    {
        leftButton.onClick.AddListener(PlayLeft);
        rightButton.onClick.AddListener(PlayRight);
        drawButton.onClick.AddListener(DrawTile);
        passButton.onClick.AddListener(PassTurn);

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
            turnManager.RegisterMove();
        } else
        {
            Debug.Log("No se puede colocar ficha aca");
        }

        UpdateButtons();
    }

    private void DrawTile()
    {
        // solo se puede robar si no hay movimientos posibles
        if (handManager.HasPlayableTile(boardManager)) return;

        DominoTileData data = deckManager.DrawTile();

        if (data == null) return;

        handManager.AddTile(data);

        Debug.Log("Robaste una ficha");
        UpdateButtons();
    }

    private void PassTurn()
    {
        // no puedes pasar turno si aun puedes usar una ficha
        if (handManager.HasPlayableTile(boardManager)) return;
        if (deckManager.RemainingTiles > 0) return;

        turnManager.PassTurn();

        UpdateButtons();
    }

    // Activa los botones solamente cuando hay una ficha seleccionada.
    private void UpdateButtons()
    {
        // bool hasSelectedTile = handManager.SelectedTile != null;

        // leftButton.interactable = hasSelectedTile;
        // rightButton.interactable = hasSelectedTile;

        DominoTileView selectedTile = handManager.SelectedTile;

        bool canPlayLeft = selectedTile != null &&
            boardManager.CanPlaceTile(selectedTile, BoardSide.Left);

        bool canPlayRight = selectedTile != null &&
            boardManager.CanPlaceTile(selectedTile, BoardSide.Right);

        bool hasPlayableTile = handManager.HasPlayableTile(boardManager);

        bool canDraw = !hasPlayableTile && deckManager.RemainingTiles > 0;
        bool canPass = !hasPlayableTile && deckManager.RemainingTiles == 0;

        leftButton.interactable = canPlayLeft;
        rightButton.interactable = canPlayRight;

        drawButton.interactable = canDraw;
        passButton.interactable = canPass;
    }

    private void OnDestroy()
    {
        leftButton.onClick.RemoveListener(PlayLeft);
        rightButton.onClick.RemoveListener(PlayRight);
        drawButton.onClick.RemoveListener(DrawTile);
        passButton.onClick.RemoveListener(PassTurn);
    }

}
