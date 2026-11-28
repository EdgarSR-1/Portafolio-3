using System.Collections.Generic;
using UnityEngine;

public enum MoveType { Place, Pass }

public readonly struct MoveData
{
    public readonly MoveType type;
    public readonly int playerIndex;
    public readonly string tileID;
    public readonly ElementType left, right;
    public readonly BoardSide side;

    public MoveData(MoveType type, int playerIndex,
        string tileID, ElementType left, ElementType right, BoardSide side)
    {
        this.type = type;
        this.playerIndex = playerIndex;
        this.tileID = tileID;
        this.left = left;
        this.right = right;
        this.side = side;
    }
}

public interface IGameCommand
{
    MoveData Data { get; }
    bool Execute();
}

public class PlaceTileCommand : IGameCommand
{
    private readonly BoardManager board;
    private readonly DominoTileView tile;
    public MoveData Data { get; }

    public PlaceTileCommand(BoardManager board, DominoTileView tile, MoveData data)
    {
        this.board = board; this.tile = tile; Data = data;
    }

    public bool Execute() => board.TryPlaceTile(tile, Data.side);    
}

public class PassCommand : IGameCommand
{
    private readonly TurnManager turns;
    public MoveData Data { get; }

    public PassCommand(TurnManager turns, int playerIndex)
    {
        this.turns = turns;
        Data = new MoveData(
            MoveType.Pass, playerIndex, null, ElementType.None, ElementType.None, BoardSide.Left);
    }

    public bool Execute()
    {
        turns.RegisterSynchronizedPass(); return true;
    }
}

public class CommandHistory
{
    private readonly List<MoveData> moves = new List<MoveData>();
    public IReadOnlyList<MoveData> Moves => moves;
    public int Count => moves.Count;

    public void Record(IGameCommand cmd) => moves.Add(cmd.Data);

    public void PrintToConsole()
    {
        for (int i = 0; i < moves.Count; i++)
        {
            MoveData m = moves[i];
            string text = m.type == MoveType.Pass
                ? "pasó turno"
                : $"colocó {m.left}|{m.right} a la {m.side}";

            Debug.Log($"Jugada {i + 1}: Jugador {m.playerIndex + 1} {text}");
        }
    }
}