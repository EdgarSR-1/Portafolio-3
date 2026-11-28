using System;
using UnityEngine;

public enum GameResult { Won, Draw }

public abstract class MatchState
{
    public virtual void Enter() { }
    public virtual void Exit() { }
}

// Espera a que el tgablero tenga su primer ficha
public class DealingState : MatchState
{
    public override void Enter() => Debug.Log("Estado: Repartiendo");
}

// Hay 1 jugador activo, se puede jugar
public class TurnState : MatchState
{
    public readonly int Player;
    public TurnState(int player) { Player = player; }

    public override void Enter() { Debug.Log("Estado: Turno del jugador " + Player); }
}

public class GameOverState : MatchState
{
    public readonly GameResult Result;
    public readonly int Winner;

    public GameOverState(GameResult result, int winner)
    {
        Result = result;
        Winner = winner;
    }

    public override void Enter() { Debug.Log("Estado: Fin de partida (" + Result + ")"); }
}

public class MatchStateMachine
{
    public MatchState Current { get; private set; }
    public event Action<MatchState> OnStateChanged;

    public void ChangeState(MatchState next)
    {
        Current?.Exit();
        Current = next;
        Current.Enter();
        OnStateChanged?.Invoke(next);
    }
}