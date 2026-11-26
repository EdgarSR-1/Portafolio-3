using UnityEngine;

public class BoardTest : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private BoardManager boardManager;
    [SerializeField] private DeckManager deckManager;
    [SerializeField] private DominoTileView dominoPrefab;

    private void Start()
    {
        DominoTileData data = deckManager.CreateRandomBasicTile();

        DominoTileView startingTile = Instantiate(dominoPrefab);
        startingTile.SetData(data);

        boardManager.SetStartingTile(startingTile);
    }
}
