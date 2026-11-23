using System.Collections.Generic;

public class PlayerHand
{
    private List<DominoTileData> tiles = new List<DominoTileData>();

    public int Count
    {
        get { return tiles.Count; }
    }

    public List<DominoTileData> GetTiles()
    {
        return tiles;
    }

    public void AddTile(DominoTileData tile)
    {
        tiles.Add(tile);
    }

    public bool RemoveTile(DominoTileData tile)
    {
        return tiles.Remove(tile);
    }
}
