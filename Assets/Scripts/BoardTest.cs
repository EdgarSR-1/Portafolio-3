using UnityEngine;

public class BoardTest : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private DominoTileView dominoPrefab;

    private void Start()
    {
        // TEST ANTIGUO - BORRAR
        // Ficha inicial: Agua Fuego
        // DominoTileView startingTile =
        //     CreateTile("tile_001", ElementType.Agua, ElementType.Fuego);

        // boardManager.SetStartingTile(startingTile);

        // // Ficha a la derecha: Fuego Vapor
        // DominoTileView rightTile =
        //     CreateTile("tile_002", ElementType.Fuego, ElementType.Vapor);

        // PlaceTileCommand rightCommand =
        //     new PlaceTileCommand(boardManager, rightTile, BoardSide.Right);

        // Debug.Log("Colocar derecha: " + rightCommand.Execute());

        // // Ficha a la izquierda: Cristal Agua
        // DominoTileView leftTile =
        //     CreateTile("tile_003", ElementType.Cristal, ElementType.Agua);

        // PlaceTileCommand leftCommand =
        //     new PlaceTileCommand(boardManager, leftTile, BoardSide.Left);

        // Debug.Log("Colocar izquierda: " + leftCommand.Execute());

        // crear solamente 1 ficha inicial
        DominoTileView startingTile = CreateTile("tile_001", ElementType.Agua, ElementType.Fuego);

        boardManager.SetStartingTile(startingTile);
    }

    private DominoTileView CreateTile(
        string id, ElementType left, ElementType right)
    {
        DominoTileView tile = Instantiate(dominoPrefab);

        DominoTileData data = new DominoTileData(id, left, right);

        tile.SetData(data);

        return tile;
    }
}
