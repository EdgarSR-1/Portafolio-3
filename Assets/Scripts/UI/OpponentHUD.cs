using UnityEngine;
using TMPro;
using Photon.Pun;
using System.Collections.Generic;
using UnityEditor.Search;

public class OpponentHUD : MonoBehaviour
{
    [SerializeField] private HandManager handManager;
    [SerializeField] private TextMeshProUGUI opponentNameText;
    [SerializeField] private TextMeshProUGUI cardCountText;
    [SerializeField] private Transform cardsContent;
    [SerializeField] private GameObject cardBackPrefab;

    private readonly List<GameObject> cardBacks = new List<GameObject>();

    private void Start()
    {
        ShowOpponentName();

        for (int i = 0; i < handManager.StartingHandSize; i++) AddCard();
    }

    private void ShowOpponentName()
    {
        string opponentName = "Jugador 2";

        if (PhotonNetwork.InRoom && PhotonNetwork.PlayerListOthers.Length > 0)
        {
            Photon.Realtime.Player other = PhotonNetwork.PlayerListOthers[0];

            opponentName = string.IsNullOrEmpty(other.NickName)
            ? "Jugador: " + other.ActorNumber : other.NickName;
        }

        opponentNameText.text = opponentName;
    }

    public void AddCard()
    {
        cardBacks.Add(Instantiate(cardBackPrefab, cardsContent));
        UpdateCount();
    }

    public void RemoveCard()
    {
        if (cardBacks.Count == 0) return;

        int last = cardBacks.Count - 1;
        Destroy(cardBacks[last]);
        cardBacks.RemoveAt(last);
        UpdateCount();
    }

    public void UpdateCount()
    {
        cardCountText.text = cardBacks.Count.ToString();
    }
}
