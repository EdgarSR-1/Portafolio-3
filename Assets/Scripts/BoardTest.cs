using UnityEngine;

public class BoardTest : MonoBehaviour
{
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private RectTransform dominoPrefab;
    [SerializeField] private RectTransform boardPieces;


    private void Start()
    {
        // Instanciar dentro del Canvas y de BoardPieces.
        RectTransform tile = Instantiate(
            dominoPrefab,
            boardPieces,
            false
        );

        PlaceTileCommand command = new PlaceTileCommand(
            boardManager,
            tile,
            5,
            4
        );

        bool success = command.Execute();

        Debug.Log(success
            ? "Ficha colocada correctamente."
            : "No se pudo colocar la ficha."
        );
    }

    private void OnEnable()
    {
        boardManager.OnTilePlaced += HandleTilePlaced;
    }

    private void OnDisable()
    {
        boardManager.OnTilePlaced -= HandleTilePlaced;
    }

    private void HandleTilePlaced(RectTransform tile, int x, int y)
    {
        Debug.Log($"Ficha colocada en ({x}, {y})");
    }
}
