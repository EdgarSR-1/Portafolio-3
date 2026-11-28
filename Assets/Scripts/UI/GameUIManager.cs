using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class GameUIManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private HandManager handManager;
    [SerializeField] private GameManager gameManager;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private OpponentHUD opponentHUD;

    [Header("Botones")]
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private Button drawButton;
    [SerializeField] private Button passButton;

    [Header("Textos")]
    [SerializeField] private TextMeshProUGUI drawPileText;

    // Registro de todo en la partida
    private readonly CommandHistory history = new CommandHistory();
    public CommandHistory History => history;

    private void Start()
    {
        leftButton.onClick.AddListener(PlayLeft);
        rightButton.onClick.AddListener(PlayRight);
        drawButton.onClick.AddListener(DrawTile);
        passButton.onClick.AddListener(PassTurn);

        UpdateButtons();
    }

    // Observer todo lo que puede cambiar los botones
    private void OnEnable()
    {
        turnManager.OnTurnChanged += HandleTurnChanged;
        handManager.OnHandChanged += UpdateButtons;
        boardManager.OnTilePlaced += HandleTilePlaced;
        gameManager.StateMachine.OnStateChanged += HandleStateChanged;
        gameManager.OnGameEnded += HandleGameEnded;
    }

    private void OnDisable()
    {
        turnManager.OnTurnChanged -= HandleTurnChanged;
        handManager.OnHandChanged -= UpdateButtons;
        boardManager.OnTilePlaced -= HandleTilePlaced;
        gameManager.StateMachine.OnStateChanged -= HandleStateChanged;
        gameManager.OnGameEnded -= HandleGameEnded;
    }

    private void HandleTurnChanged(int player)
    {
        UpdateButtons();
    }
    private void HandleTilePlaced(DominoTileView tile, BoardSide side)
    {
        UpdateButtons();
    }
    private void HandleStateChanged(MatchState state)
    {
        UpdateButtons();
    }

    private void HandleGameEnded(GameResult result, int winner)
    {
        history.PrintToConsole();
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
        if (!gameManager.IsPlaying || !turnManager.IsLocalTurn) return;

        DominoTileView selectedTile = handManager.SelectedTile;

        if (selectedTile == null)
        {
            Debug.Log("Primero selecciona 1 ficha");
            return;
        }

        if (!boardManager.CanPlaceTile(selectedTile, side))
        {
            Debug.Log("No se puede colocar ficha aqui");
            return;
        }

        NetworkManager.instance.RequestTilePlacement(
            selectedTile.Data.tileID,
            (int)selectedTile.Data.leftElement,
            (int)selectedTile.Data.rightElement,
            (int)side);
    }

    private void DrawTile()
    {
        if (!gameManager.IsPlaying || !turnManager.IsLocalTurn) return;

        // solo se puede robar si no hay movimientos posibles
        if (handManager.HasPlayableTile(boardManager)) return;
        if (!handManager.DrawTile()) return;

        if (PhotonNetwork.InRoom) NetworkManager.instance.NotifyDraw();

        Debug.Log("Robaste una ficha");
    }

    private void PassTurn()
    {
        if (!gameManager.IsPlaying || !turnManager.IsLocalTurn) return;
        // no puedes pasar turno si aun puedes usar una ficha o robar
        if (handManager.HasPlayableTile(boardManager)) return;
        if (handManager.RemainingTiles > 0) return;

        if (PhotonNetwork.InRoom) NetworkManager.instance.RequestPass();
        else ApplyNetworkPass(turnManager.CurrentPlayer);
    }

    // Validación que solo va a usar el Master Client

    public bool ValidatePassRequest(int playerIndex)
    {
        return gameManager.IsPlaying && turnManager.CurrentPlayer == playerIndex;
    }

    public bool ValidateMoveRequest(
        int playerIndex, ElementType leftElement, ElementType rightElement, BoardSide side)
    {
        if (!gameManager.IsPlaying) return false;
        if (turnManager.CurrentPlayer != playerIndex) return false;

        DominoTileData data = new DominoTileData("NetworkValidation", leftElement, rightElement);
        DominoTileView temporaryTile = handManager.CreateDetachedTile(data);

        bool valid = boardManager.CanPlaceTile(temporaryTile, side);

        Destroy(temporaryTile.gameObject);
        return valid;
    }

    // Para el otro usuario

    public void ApplyNetworkPass(int playerIndex)
    {
        if (!gameManager.IsPlaying) return;

        IGameCommand command = new PassCommand(turnManager, playerIndex);
        command.Execute();
        history.Record(command);
    }

    public void ApplyNetworkMove(
        string tileID, ElementType leftElement, ElementType rightElement, BoardSide side, int playerIndex)
    {
        if (!gameManager.IsPlaying) return;

        DominoTileView tile;

        if (playerIndex == turnManager.LocalPlayerIndex)
        {
            // el mismo objeto que ya existe en la mano
            tile = handManager.FindTileByID(tileID);
            if (tile == null)
            {
                Debug.LogError("No se encontró la ficha local: " + tileID);
                return;
            }

            handManager.RemoveTile(tile);
        }
        else
        {
            // ficha del oponente, entonces no existe en tu mano
            DominoTileData data = new DominoTileData(tileID, leftElement, rightElement);
            tile = handManager.CreateDetachedTile(data);

            opponentHUD.RemoveCard();
        }

        // Command: la jugada como datos + ejecución
        MoveData move = new MoveData(
            MoveType.Place, playerIndex, tileID, leftElement, rightElement, side);

        IGameCommand command = new PlaceTileCommand(boardManager, tile, move);

        if (!command.Execute())
        {
            Debug.LogError("Error al ejecutar movimiento sincronizado");
            return;
        }

        history.Record(command);

        // Si esta jugada hizo Oro, Game Over
        if (!gameManager.IsPlaying) return;

        turnManager.RegisterSynchronizedMove();
    }

    public void ApplyOpponentDraw()
    {
        opponentHUD.AddCard();
    }

    // Los botones

    private void UpdateButtons()
    {
        if (!gameManager.IsPlaying)
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