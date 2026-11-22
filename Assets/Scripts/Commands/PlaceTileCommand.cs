public class PlaceTileCommand
{
    private readonly BoardManager boardManager;
    private readonly DominoTileView tile;
    private readonly BoardSide side;

    public PlaceTileCommand(
        BoardManager boardManager, DominoTileView tile, BoardSide side
    )
    {
        this.boardManager = boardManager;
        this.tile = tile;
        this.side = side;
    }

    public bool Execute()
    {
        return boardManager.TryPlaceTile(tile, side);
    }
}
