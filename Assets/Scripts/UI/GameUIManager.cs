using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class GameUIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private HandManager handManager;
    [SerializeField] private GameManager gameManager;

    [Header("Botones")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button passButton;

    [SerializeField] private TurnManager turnManager;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI drawPileText;


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
        if (!turnManager.IsLocalTurn) return;
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
        if (!turnManager.IsLocalTurn) return;

        // solo se puede robar si no hay movimientos posibles
        if (handManager.HasPlayableTile(boardManager)) return;
        if (!handManager.DrawTile()) return;

        Debug.Log("Robaste una ficha");
        UpdateButtons();
    }

    private void PassTurn()
    {
        // no puedes pasar turno si aun puedes usar una ficha
        if (handManager.HasPlayableTile(boardManager)) return;
        if (handManager.RemainingTiles > 0) return;

        turnManager.PassTurn();
        UpdateButtons();
    }

    // Activa los botones solamente cuando hay una ficha seleccionada.
    private void UpdateButtons()
    {
        if (gameManager.CurrentState != GameState.Playing)
        {
            leftButton.interactable = false;
            rightButton.interactable = false;
            drawButton.interactable = false;
            passButton.interactable = false;
            return;
        }

        bool isMyTurn = turnManager.IsLocalTurn;

        DominoTileView selectedTile = handManager.SelectedTile;

        bool canPlayLeft = selectedTile != null &&
            boardManager.CanPlaceTile(selectedTile, BoardSide.Left);

        bool canPlayRight = selectedTile != null &&
            boardManager.CanPlaceTile(selectedTile, BoardSide.Right);

        bool hasPlayableTile = handManager.HasPlayableTile(boardManager);

        bool canDraw = !hasPlayableTile && handManager.RemainingTiles > 0;
        bool canPass = !hasPlayableTile && handManager.RemainingTiles == 0;

        leftButton.interactable = isMyTurn && canPlayLeft;
        rightButton.interactable = isMyTurn && canPlayRight;

        drawButton.interactable = isMyTurn && canDraw;
        passButton.interactable = isMyTurn && canPass;

        drawPileText.text = handManager.RemainingTiles.ToString();
    }

    private void OnDestroy()
    {
        leftButton.onClick.RemoveListener(PlayLeft);
        rightButton.onClick.RemoveListener(PlayRight);
        drawButton.onClick.RemoveListener(DrawTile);
        passButton.onClick.RemoveListener(PassTurn);
    }

}
