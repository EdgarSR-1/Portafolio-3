using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Build;
using UnityEngine.TextCore;
using Photon.Pun;


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
            (int)selectedTile.Data.leftElement,(int)selectedTile.Data.rightElement,
            (int)side);

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
        if (!turnManager.IsLocalTurn) return;
        // no puedes pasar turno si aun puedes usar una ficha
        if (handManager.HasPlayableTile(boardManager)) return;
        if (handManager.RemainingTiles > 0) return;

        if (PhotonNetwork.InRoom) NetworkManager.instance.RequestPass();
        else turnManager.RegisterSynchronizedPass();

        // turnManager.PassTurn();
        // UpdateButtons();
    }

    public bool ValidatePassRequest(int playerIndex)
    {
        return gameManager.CurrentState == GameState.Playing
            && turnManager.CurrentPlayer == playerIndex;
    }

    public void ApplyNetworkPass(int playerIndex)
    {
        turnManager.RegisterSynchronizedPass();
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

    public bool ValidateMoveRequest(
        int playerIndex, ElementType leftElement, ElementType rightElement, BoardSide side)
    {
        if (gameManager.CurrentState != GameState.Playing) return false;
        if (turnManager.CurrentPlayer != playerIndex) return false;

        DominoTileData data = new DominoTileData("NetworkValidation", leftElement, rightElement);

        DominoTileView temporaryTile = handManager.CreateDetachedTile(data);

        bool valid = boardManager.CanPlaceTile(temporaryTile, side);

        Destroy(temporaryTile.gameObject);

        return valid;
    }

    public void ApplyNetworkMove(
        string tileID, ElementType leftElement, ElementType rightElement, BoardSide side, int playerIndex)
    {
        DominoTileView tile;

        // si la ficha pertenece a la mano del jugador
        // se vuelve a utilizar el GameObject que ya existe
        if (playerIndex == turnManager.LocalPlayerIndex)
        {
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
            // la ficha del oponente no existe en la mano
            DominoTileData data = new DominoTileData(tileID, leftElement, rightElement);

            tile = handManager.CreateDetachedTile(data);
        }

        PlaceTileCommand command = new PlaceTileCommand(boardManager, tile, side);

        if (!command.Execute())
        {
            Debug.LogError("Error al ejecutar movimiento sincronizado");
            return;
        }

        Debug.Log("Movimiento multiplayer aplicado");

        // avanzar el turno en ambos clientes
        turnManager.RegisterSynchronizedMove();

        UpdateButtons();
    }
}
