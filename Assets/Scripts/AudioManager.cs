using UnityEngine;
using UnityEngine.UIElements;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private BoardManager board;
    [SerializeField] private GameManager game;
    [SerializeField] private TurnManager turnManager;
    [SerializeField] private AudioSource source;
    [SerializeField] private AudioClip placeClip;
    [SerializeField] private AudioClip goldClip;
    [SerializeField] private AudioClip gameOverClip;

    // suscribirse a los eventos cuando el objeto se activa
    private void OnEnable()
    {
        board.OnTilePlaced += PlayPlaceSound;
        board.OnReaction += PlayReactionSound;
        game.OnGameEnded += PlayGameOverSound;
    }

    private void OnDisable()
    {
        board.OnTilePlaced -= PlayPlaceSound;
        board.OnReaction -= PlayReactionSound;
        game.OnGameEnded -= PlayGameOverSound;
    }

    private void PlayPlaceSound(DominoTileView tile, BoardSide side)
    {
        source.PlayOneShot(placeClip);
    }

    private void PlayReactionSound(ElementType result)
    {
        if (result != ElementType.Oro) return;

        // el oro solo suena para el ganador
        bool iMadeGold = turnManager.CurrentPlayer == turnManager.LocalPlayerIndex;
        if (iMadeGold) source.PlayOneShot(goldClip);
    }

    private void PlayGameOverSound(GameResult result, int winner)
    {
        // empate: Todos lo escuchan
        if (result == GameResult.Draw)
        {
            source.PlayOneShot(gameOverClip);
            return;
        }

        // Cuando alguien si gana, solo el que pierde lo escucha
        bool iWon = winner == turnManager.LocalPlayerIndex;
        if (!iWon) source.PlayOneShot(gameOverClip);    
    }
}
