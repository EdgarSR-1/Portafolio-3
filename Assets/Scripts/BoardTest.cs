using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class BoardTest : MonoBehaviourPunCallbacks
{
    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private DominoTileView dominoPrefab;

    private const string StartingTileKey = "StartingTile";

    private bool boardInitialized = false;

    private void Start()
    {
        // por si entras sin estar en multiplayer de alguna forma
        if (!PhotonNetwork.InRoom)
        {
            InitializeOffline();
            return;
        }

        // El Master Client hace la ficha inicial.
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateStartingTile();
        }
        else
        {
            // si ya existe entonces solo recuperar.
            TryLoadStartingTile();
        }
    }

    private void GenerateStartingTile()
    {
        if (boardInitialized) return;

        DominoTileData data = deckManager.CreateRandomBasicTile();

        string serializedData =
            data.tileID + "," +
            (int)data.leftElement + "," +
            (int)data.rightElement;

        Hashtable properties = new Hashtable
        {
            { StartingTileKey, serializedData }
        };

        PhotonNetwork.CurrentRoom.SetCustomProperties(properties);

        CreateStartingTile(data);
    }

    private void TryLoadStartingTile()
    {
        if (boardInitialized) return;

        if (!PhotonNetwork.CurrentRoom.CustomProperties
            .ContainsKey(StartingTileKey))
        {
            Debug.Log("Esperando ficha inicial del Master Client...");
            return;
        }

        string serializedData =
            (string)PhotonNetwork.CurrentRoom.CustomProperties[StartingTileKey];

        string[] values = serializedData.Split(',');

        DominoTileData data = new DominoTileData(
            values[0],
            (ElementType)int.Parse(values[1]),
            (ElementType)int.Parse(values[2])
        );

        CreateStartingTile(data);
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(StartingTileKey))
        {
            TryLoadStartingTile();
        }
    }

    private void CreateStartingTile(DominoTileData data)
    {
        if (boardInitialized) return;

        DominoTileView startingTile = Instantiate(dominoPrefab);

        startingTile.SetData(data);

        boardManager.SetStartingTile(startingTile);

        boardInitialized = true;

        Debug.Log(
            $"Ficha inicial: {data.leftElement} | {data.rightElement}"
        );
    }

    private void InitializeOffline()
    {
        if (boardInitialized) return;

        DominoTileData data = deckManager.CreateRandomBasicTile();

        CreateStartingTile(data);
    }
}
