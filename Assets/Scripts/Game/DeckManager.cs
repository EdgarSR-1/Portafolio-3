using UnityEngine;
using System;
using System.Collections.Generic;
using NUnit.Framework.Internal.Filters;

public class DeckManager : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private int startingPoolSize = 20;
    
    // solo puede tener elementos basicos y basado en las combinaciones sale lo demas
    [SerializeField] private ElementType[] availableElements =
    {
        ElementType.Agua,
        ElementType.Fuego,
        ElementType.Roca,
        ElementType.Planta,
        ElementType.Azufre
    };

    private List<DominoTileData> deck = new List<DominoTileData>();

    // public int RemainingTiles
    // {
    //     get { return deck.Count; }
    // }
    public int RemainingTiles => deck.Count;

    private void Awake()
    {
        CreateDeck();
    }

    private void CreateDeck()
    {
        for (int i = 0; i < startingPoolSize; i++)
        {
            int leftIndex = UnityEngine.Random.Range(0, availableElements.Length);

            int rightIndex = UnityEngine.Random.Range(0, availableElements.Length);

            ElementType left = availableElements[leftIndex];
            ElementType right = availableElements[rightIndex];

            string id = Guid.NewGuid().ToString();

            deck.Add(new DominoTileData(id, left, right));
        }

        // Mezclar el mazo
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = UnityEngine.Random.Range(i, deck.Count);

            DominoTileData temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    public DominoTileData DrawTile()
    {
        if (deck.Count == 0)
        {
            Debug.Log("El mazo esta vacio");
            return null;
        }

        DominoTileData tile = deck[0];
        deck.RemoveAt(0);

        return tile;
    }

    public DominoTileData CreateRandomBasicTile()
    {
        ElementType left = availableElements[UnityEngine.Random.Range(0, availableElements.Length)];
        ElementType right = availableElements[UnityEngine.Random.Range(0, availableElements.Length)];

        return new DominoTileData(Guid.NewGuid().ToString(), left, right);
    }
}
