using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BoardGridGenerator : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField] private Transform gridParent;
    [SerializeField] private GameObject cellPrefab;

    [SerializeField] private int columns = 28;
    [SerializeField] private int rows = 14;

    [Header("Colores")]
    [SerializeField] private Color colorA = Color.white;
    [SerializeField] private Color colorB = Color.black;

    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;

    private void Awake()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        int totalCells = columns * rows;
        List<BoardCell> generatedCells = new();

        for (int i = 0; i < totalCells; i++)
        {
            GameObject cell = Instantiate(cellPrefab, gridParent);

            int x = i % columns; // columna
            int y = i / columns; // fila

            Image background = cell.GetComponent<Image>();
            // Image background = cell.transform.Find("Background").GetComponent<Image>();

            // if ((x + y) % 2 == 0) image.color = colorA;
            // else image.color = colorB;

            background.color = (x + y) % 2 == 0 ? colorA : colorB;

            cell.name = $"Cell_{x}_{y}";
            
            // Para saber las coordenadas y poder identificar si se puede agregar una ficha
            BoardCell boardCell = cell.GetComponent<BoardCell>();
            boardCell.Initialize(x, y);

            generatedCells.Add(boardCell);
        }

        // Forzar la actualización del sistema de UI.
        Canvas.ForceUpdateCanvases();

        LayoutRebuilder.ForceRebuildLayoutImmediate(
            (RectTransform)gridParent
        );

        Debug.Log(
            $"Celda 0: {generatedCells[0].transform.position}"
        );

        Debug.Log(
            $"Celda 1: {generatedCells[1].transform.position}"
        );

        Debug.Log(
            $"Celda 2: {generatedCells[2].transform.position}"
        );
        
        boardManager.Initialize(generatedCells.ToArray());
    }
}
