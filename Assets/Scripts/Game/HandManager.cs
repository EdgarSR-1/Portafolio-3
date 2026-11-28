using UnityEngine;
using System.Collections.Generic;


public class HandManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform handContent;
    [SerializeField] private DominoTileView dominoPrefab;

    [Header("Configuración")]
    [SerializeField] private int startingHandSize = 7;
    // [SerializeField] private ElementType[] availableElements;

    [SerializeField] private DeckManager deckManager;

    private DominoTileView selectedTile;

    private PlayerHand playerHand = new PlayerHand();
    
    private List<DominoTileView> tileViews = new List<DominoTileView>();

    public DominoTileView SelectedTile
    {
        get { return selectedTile; }
    }

    public int RemainingTiles => deckManager.RemainingTiles;

    private void Start()
    {
        CreateStartingHand();
    }

    private void CreateStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            if (!DrawTile()) break;
        }
    }

    public bool DrawTile()
    {
        DominoTileData data = deckManager.DrawTile();

        if (data == null) return false;
        
        AddTile(data);
        return true;
    }

    // parte visual de la ficha
    private void CreateTileView(DominoTileData data)
    {
        DominoTileView tile = Instantiate(dominoPrefab, handContent);
        tile.SetData(data);
        // escuchar cunado el jugador selecciona esta ficha
        tile.OnSelected += SelectTile;
        tileViews.Add(tile);
    }

    // retira una ficha de la mano
    public void RemoveTile(DominoTileView tile)
    {
        if (tile == null) return;

        if (selectedTile == tile)
        {
            selectedTile.SetSelected(false);
            selectedTile = null;
        }

        tile.OnSelected -= SelectTile;

        playerHand.RemoveTile(tile.Data);
        tileViews.Remove(tile);

        // Destroy(tile.gameObject);
    }

    private void SelectTile(DominoTileView tile)
    {
        // quitar la selección actual
        if (selectedTile != null) selectedTile.SetSelected(false);
        
        // seleccionar la nueva
        selectedTile = tile;

        selectedTile.SetSelected(true);
    }

    public void AddTile(DominoTileData data)
    {
        if (data == null) return;

        playerHand.AddTile(data);
        CreateTileView(data);
    }

    public bool HasPlayableTile(BoardManager boardManager)
    {
        foreach (DominoTileView tile in tileViews)
        {
            if (boardManager.CanPlaceTile(tile, BoardSide.Left) || 
                boardManager.CanPlaceTile(tile, BoardSide.Right))
            {
                return true;
            }
        }

        return false;
    }

    public DominoTileView FindTileByID(string tileID)
    {
        foreach (DominoTileView tile in tileViews)
        {
            if (tile.Data.tileID == tileID) return tile;
        }

        return null;
    }

    public DominoTileView CreateDetachedTile(DominoTileData data)
    {
        DominoTileView tile = Instantiate(dominoPrefab, handContent);
        tile.SetData(data);

        return tile;
    }
}
