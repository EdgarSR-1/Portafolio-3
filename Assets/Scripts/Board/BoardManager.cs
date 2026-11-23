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
    [SerializeField] private float tileWidth = 240f;
    [SerializeField] private float tileHeight = 120f;
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

        // no estaba guardando el tamaño en el lado izquierdo :/
        // tile.transform.SetParent(boardContent, false);
        MoveTileToBoard(tile);
        tiles.Add(tile);

        UpdateBoardLayout();
    }

    // Comprueba siuna ficha puede colocarse en el extremo
    public bool CanPlaceTile(DominoTileView tile, BoardSide side)
    {
        if (tile == null || tile.Data == null || tiles.Count == 0) return false;

        // antes de poder hacer flip:
        // if (side == BoardSide.Right) return tile.Data.leftElement == RightEnd;

        if (side == BoardSide.Right)
            return tile.Data.leftElement == RightEnd || tile.Data.rightElement == RightEnd;
        
        // return tile.Data.rightElement == LeftEnd;
        return tile.Data.rightElement == LeftEnd || tile.Data.leftElement == LeftEnd;
    }

    // Intenta colocar la ficha en el tablero
    public bool TryPlaceTile(DominoTileView tile, BoardSide side)
    {
        if (!CanPlaceTile(tile, side))  return false;

        // comprobar si necesita rotar la ficha
        if (side == BoardSide.Right) if (tile.Data.leftElement != RightEnd) tile.Flip();
        else if (tile.Data.rightElement != LeftEnd) tile.Flip();

        // tile.transform.SetParent(boardContent, false);
        MoveTileToBoard(tile);

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

        // RectTransform firstTile = tiles[0].GetComponent<RectTransform>();

        // float tileWidth = firstTile.rect.width;
        // float tileHeight = firstTile.rect.height;
        // Debug.Log("Ancho de ficha: " + tileWidth);
        // Debug.Log("Cantidad de fichas: " + tiles.Count);

        RectTransform viewport = boardContent.parent.GetComponent<RectTransform>();

        // para centrar las fichas:
        // ancho que ocupa todas las fichas que ya se pusieron
        float chainWidth = 
            tiles.Count * tileWidth +
            (tiles.Count - 1) * tileSpacing;

        // espacio total necesario para todas las fichas
        float totalWidth = 
            horizontalPadding * 2 +
            tiles.Count * tileWidth +
            (tiles.Count - 1) * tileSpacing;

        float contentWidth = Mathf.Max(totalWidth, viewport.rect.width);

        // cerrar la cadena de fichas dentro del contenido
        float startX = (contentWidth - chainWidth) / 2f;

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

            tile.sizeDelta = new Vector2(tileWidth, tileHeight);

            // float positionX = 
            //     horizontalPadding + tileWidth / 2f +
            //     i * (tileWidth + tileSpacing);

            float positionX =
                startX + tileWidth / 2f +
                i * (tileWidth + tileSpacing);
            
            tile.anchoredPosition = new Vector2(positionX, 0f);

            Debug.Log("Ficha " + i + " posicion X: " + tile.anchoredPosition.x);
        }
    }

    // me estaba dando al mover una ficha a la izquierda que su posicion se vuelve 100 con tamaño 0
    private void MoveTileToBoard(DominoTileView tile)
    {
        RectTransform tileRect = tile.GetComponent<RectTransform>();

        // no me daba el tamaño correcto del lado izquierdo....

        // guardar el tamaño actual antes de cambiar el contenedor
        // Vector2 tileSize = tileRect.rect.size;

        // mover la ficha al tablero
        tileRect.SetParent(boardContent, false);

        // configurar los anchors
        tileRect.anchorMin = new Vector2(0.5f, 0.5f);
        tileRect.anchorMax = new Vector2(0.5f, 0.5f);
        tileRect.pivot = new Vector2(0.5f, 0.5f);

        // recuperar el tamaño
        // tileRect.sizeDelta = tileSize;

        // asignar el tamaño manualmente
        tileRect.sizeDelta = new Vector2(tileWidth, tileHeight);
    }
}
