using UnityEngine;

public class PlaceTileCommand
{
    private readonly BoardManager boardManager;
    private readonly RectTransform tile;
    private readonly int x;
    private readonly int y;

    public PlaceTileCommand(
        BoardManager boardManager, RectTransform tile, int x, int y
    )
    {
        this.boardManager = boardManager;
        this.tile = tile;
        this.x = x;
        this.y = y;
    }

    public bool Execute()
    {
        return boardManager.TryPlaceTile(tile, x, y);
    }
}
