using UnityEngine;
using System;
using Photon.Pun;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private int playerCount = 2;

    private int currentPlayer = 0;
    private int consecutivePasses = 0;

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

    public void RegisterSynchronizedPass()
    {
        consecutivePasses++;
        Debug.Log("Jugador " + currentPlayer + " pasó turno");

        if (consecutivePasses >= playerCount)
        {
            Debug.Log("Empate: todos los jugadores pasaron turno");
            OnDrawGame?.Invoke();
            return;
        }

        AdvanceTurn();
    }


    public void RegisterSynchronizedMove()
    {
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
}
