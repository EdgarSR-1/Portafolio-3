using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Tilemaps;

public class BoardManager : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private RectTransform boardContent;

    [Header("Layout")]
    [SerializeField] private float tileSpacing = 0f;
    [SerializeField] private float horizontalPadding = 100f;

    private List<DominoTileView> tiles = new List<DominoTileView>();

    // Patron Observer: avisa cuando se coloca una ficha.
    public event Action<DominoTileView, BoardSide> OnTilePlaced;

    // Elementos que quedan libres en las esquinas del tablero
    public ElementType LeftEnd
    {
        get
        {
            if (tiles.Count == 0) return ElementType.None;

            return tiles[0].Data.leftElement;
        }
    }

    public ElementType RightEnd
    {
        get
        {
            if (tiles.Count == 0) return ElementType.None;

            return tiles[tiles.Count -1].Data.rightElement;
        }
    }

    // Coloca la primer ficha
    public void SetStartingTile(DominoTileView tile)
    {
        if (tile == null || tile.Data == null || tiles.Count > 0) return;

        tile.transform.SetParent(boardContent, false);
        tiles.Add(tile);

        UpdateBoardLayout();
    }

    // Comprueba siuna ficha puede colocarse en el extremo
    public bool CanPlaceTile(DominoTileView tile, BoardSide side)
    {
        if (tile == null || tile.Data == null || tiles.Count == 0) return false;

        if (side == BoardSide.Right) return tile.Data.leftElement == RightEnd;
        
        return tile.Data.rightElement == LeftEnd;
    }

    // Intenta colocar la ficha en el tablero
    public bool TryPlaceTile(DominoTileView tile, BoardSide side)
    {
        if (!CanPlaceTile(tile, side))  return false;

        tile.transform.SetParent(boardContent, false);

        if (side == BoardSide.Left) tiles.Insert(0, tile);
        else tiles.Add(tile);

        UpdateBoardLayout();

        // if (OnTilePlaced != null) OnTilePlaced(tile, side);

        OnTilePlaced?.Invoke(tile, side);

        return true;
    }

    // acomoda fichas horizontalmente
    private void UpdateBoardLayout()
    {
        if (tiles.Count == 0) return;

        RectTransform firstTile = tiles[0].GetComponent<RectTransform>();

        float tileWidth = firstTile.rect.width;
        float tileHeight = firstTile.rect.height;

        RectTransform viewport = boardContent.parent.GetComponent<RectTransform>();

        float totalWidth = horizontalPadding * 2+
            tiles.Count * tileWidth +
            (tiles.Count - 1) * tileSpacing;

        float contentWidth = Mathf.Max(totalWidth, viewport.rect.width);

        // ajusta el tamaño del scrollrect
        boardContent.anchorMin = new Vector2(0f, 0f);
        boardContent.anchorMax = new Vector2(0f, 1f);
        boardContent.pivot = new Vector2(0f, 0.5f);
        boardContent.anchoredPosition = Vector2.zero;
        boardContent.sizeDelta = new Vector2(contentWidth, 0f);

        // coloca las fichas una despues de otra
        for (int i = 0; i < tiles.Count; i++)
        {
            RectTransform tile = tiles[i].GetComponent<RectTransform>();

            tile.anchorMin = new Vector2(0f, 0.5f);
            tile.anchorMax = new Vector2(0f, 0.5f);
            tile.pivot = new Vector2(0.5f, 0.5f);

            float positionX = 
                horizontalPadding + tileWidth / 2f +
                i * (tileWidth + tileSpacing);

            tile.anchoredPosition = new Vector2(positionX, 0f);
        }
    }
}
