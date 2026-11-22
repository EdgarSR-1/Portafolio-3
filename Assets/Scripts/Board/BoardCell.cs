using UnityEngine;

public class BoardCell : MonoBehaviour
{
    public int X { get; private set; }
    public int Y { get; private set; }

    public bool IsOccupied { get; private set; }

    public void Initialize(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void SetOccupied(bool occupied)
    {
        IsOccupied = occupied;
    }
}
