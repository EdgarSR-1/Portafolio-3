using System;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    [Header("Board")]
    [SerializeField] private int columns = 28;
    [SerializeField] private int rows = 14;

    [Header("Referencias")]
    [SerializeField] private Transform boardPieces;

    private BoardCell[,] cells;

    // Patron de diseño observer
    // otras partes del codigo pueden escuchar cuando se pone una ficha
    public event Action<RectTransform, int, int> OnTilePlaced;

    public void Initialize(BoardCell[] generatedCells)
    {
        cells = new BoardCell[columns, rows];

        foreach (BoardCell cell in generatedCells)
        {
            cells[cell.X, cell.Y] = cell;
        }
        
        Debug.Log($"Board inicializado: {generatedCells.Length} celdas.");
    }

    public bool CanPlaceTile(int x, int y)
    {
        if (cells == null) return false;
        
        // int nextX;
        // if (horizontal) nextX = x + 1
        // else nextX = x;
        // int nextY;
        // if (horizontal) nextY = y;
        // else nextY = y + 1; 

        // int nextX = horizontal ? x + 1 : x;
        // int nextY = horizontal ? y : y + 1;

        int nextX = x + 1;
        int nextY = y;

        if ( !IsInsideBoard(x, y) || !IsInsideBoard(nextX, nextY) ) return false;
        
        // if (cells[x, y].IsOccupied == false)
        // {
        //     if (cells[nextX, nextY].IsOccupied == false) return true;
        // }

        // return false;

        // return !cells[x, y].IsOccupied && !cells[nextX, nextY].IsOccupied;
        
        return cells[x,y] != null &&
            cells[nextX,nextY] != null &&
            !cells[x,y].IsOccupied && !cells[nextX,nextY].IsOccupied;
        // da true solo si ambos si existen y son falsos
    }

    // public bool PlaceTile(GameObject tile, int x, int y, bool horizontal)
    // {
    //     if (!CanPlaceTile(x, y, horizontal)) return false;

    //     int nextX = horizontal ? x + 1 : x;
    //     int nextY = horizontal ? y : y + 1;

    //     cells[x, y].SetOccupied(true);
    //     cells[nextX, nextY].SetOccupied(true);

    //     tile.transform.SetParent(boardPieces, false);

    //     return true;
    // }


    public bool TryPlaceTile(RectTransform tile, int x, int y)
    {
        if (tile == null || !CanPlaceTile(x,y)) return false;
        
        BoardCell firstCell = cells[x,y];
        BoardCell secondCell = cells[x + 1, y];

        RectTransform firstRect = firstCell.GetComponent<RectTransform>();
        RectTransform secondRect = secondCell.GetComponent<RectTransform>();

        // centro de ambas celdas en coordenadas del mundo
        Vector3 firstCenter = firstRect.TransformPoint(firstRect.rect.center);
        Vector3 secondCenter = secondRect.TransformPoint(secondRect.rect.center);

        // se convierten los centros a espacio local de solo board pieces
        // Vector3 localFirst = boardPieces.InverseTransformPoint(firstCenter);
        // Vector3 localSecond = boardPieces.InverseTransformPoint(secondCenter);

        // Centro de la ficha es el punto medio entre ambas coordenadas
        //Vector3 TileCenter = (localFirst + localSecond) / 2f;
        Vector3 tileCenter = (firstCenter + secondCenter) / 2f;

        // la ficha debe de medir 2 celdas de ancho, 1 de alto
        float tileWidth = firstRect.rect.width + secondRect.rect.width;
        float tileHeight = firstRect.rect.height;

        // Colocar la ficha dentro de BoardPieces.
        tile.SetParent(boardPieces, false);
        tile.localScale = Vector3.one;
        tile.localRotation = Quaternion.identity;

        tile.anchorMin = new Vector2(0.5f, 0.5f);
        tile.anchorMax = new Vector2(0.5f, 0.5f);
        tile.pivot = new Vector2(0.5f, 0.5f);

        tile.sizeDelta = new Vector2(tileWidth, tileHeight);
        // tile.anchoredPosition = TileCenter;
        tile.position = tileCenter;

        firstCell.SetOccupied(true);
        secondCell.SetOccupied(true);

        // if (OnTilePlaced != null) OnTilePlaced.Invoke(tile, x, y);

        OnTilePlaced?.Invoke(tile, x, y);

        Debug.Log($"Celda 1: {firstCenter}");
        Debug.Log($"Celda 2: {secondCenter}");
        Debug.Log($"Centro ficha: {tileCenter}");

        return true;
    }

    private bool IsInsideBoard(int x, int y)
    {
        // solo dar true si las coordenadas son mayor a 0, pero menor a sus limites
        return x >= 0 && x < columns && y >= 0 && y < rows;
    }
}
