using UnityEngine;
using System;
using System.Collections.Generic;


public class HandManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform handContent;
    [SerializeField] private DominoTileView dominoPrefab;

    [Header("Configuración")]
    [SerializeField] private int startingHandSize = 7;
    [SerializeField] private ElementType[] availableElements;

    private DominoTileView selectedTile;

    private PlayerHand playerHand = new PlayerHand();
    
    private List<DominoTileView> tileViews = new List<DominoTileView>();

    public DominoTileView SelectedTile
    {
        get { return selectedTile; }
    }

    private void Start()
    {
        CreateStartingHand();
    }

    private void CreateStartingHand()
    {
        for (int i = 0; i < startingHandSize; i++)
        {
            DominoTileData data = CreateRandomTile();

            playerHand.AddTile(data);

            CreateTileView(data);
        }
    }

    private DominoTileData CreateRandomTile()
    {
        int leftIndex = UnityEngine.Random.Range(0, availableElements.Length);
        int rightIndex = UnityEngine.Random.Range(0, availableElements.Length);

        ElementType left = availableElements[leftIndex];
        ElementType right = availableElements[rightIndex];

        string id = Guid.NewGuid().ToString();

        return new DominoTileData(id, left, right);
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
}
