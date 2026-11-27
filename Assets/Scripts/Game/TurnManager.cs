using UnityEngine;
using System;
using Photon.Pun;
using Photon.Realtime;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private int playerCount = 2;

    private int currentPlayer = 0;
    private int consecutivePasses = 0;

    public bool GameOver { get; private set; }

    public event Action<int> OnTurnChanged;
    public event Action OnDrawGame;

    // public int CurrentPlayer
    // {
    //     get { return currentPlayer; }
    // }
    public int CurrentPlayer => currentPlayer;

    // identidad del jugador
    public int LocalPlayerIndex
    {
        get
        {
            if (PhotonNetwork.InRoom) return PhotonNetwork.LocalPlayer.ActorNumber - 1;

            return 0;
        }
    }

    // checar si es tu turno
    public bool IsLocalTurn => CurrentPlayer == LocalPlayerIndex;

    public void PassTurn()
    {
        if (GameOver || !IsLocalTurn) return;

        consecutivePasses++;

        Debug.Log("Jugador " + currentPlayer + " paso su turno");

        if (consecutivePasses >= playerCount)
        {
            Debug.Log("Empate: todos los jugadores pasaron turno");
            EndGame();
            // if (OnDrawGame != null) OnDrawGame();
            OnDrawGame?.Invoke();
            return;
        }

        AdvanceTurn();
    }

    public void RegisterSynchronizedPass()
    {
        if (GameOver) return;

        consecutivePasses++;
        Debug.Log("Jugador " + currentPlayer + " pasó turno");

        if (consecutivePasses >= playerCount)
        {
            Debug.Log("Empate; todos los jugadores pasaron turno");
            EndGame();
            OnDrawGame?.Invoke();
            return;
        }

        AdvanceTurn();
    }

    // public void RegisterMove()
    // {
    //     if (GameOver || !IsLocalTurn) return;

    //     consecutivePasses = 0;
    //     AdvanceTurn();
    // }

    public void RegisterSynchronizedMove()
    {
        if (GameOver) return;

        consecutivePasses = 0;
        AdvanceTurn();
    }

    private void AdvanceTurn()
    {
        currentPlayer++;

        if (currentPlayer >= playerCount) currentPlayer = 0;

        Debug.Log("Turno del jugador: " + currentPlayer);

        OnTurnChanged?.Invoke(currentPlayer);
    }

    public void EndGame()
    {
        GameOver = true;
    }
}
