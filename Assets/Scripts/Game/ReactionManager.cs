using UnityEngine;
using System.Collections.Generic;

public class ReactionManager : MonoBehaviour
{
    private Dictionary<(ElementType, ElementType), ElementType> reactions = 
        new Dictionary<(ElementType, ElementType), ElementType>();

    private void Awake()
    {
        AddReaction(
            ElementType.Agua,
            ElementType.Fuego,
            ElementType.Vapor
        );

        AddReaction(
            ElementType.Vapor,
            ElementType.Planta,
            ElementType.Energia
        );

        AddReaction(
            ElementType.Energia,
            ElementType.Agua,
            ElementType.Luz
        );

        AddReaction(
            ElementType.Roca,
            ElementType.Fuego,
            ElementType.Metal
        );

        AddReaction(
            ElementType.Metal,
            ElementType.Agua,
            ElementType.Cristal
        );

        AddReaction(
            ElementType.Cristal,
            ElementType.Azufre,
            ElementType.Eter
        );

        AddReaction(
            ElementType.Luz,
            ElementType.Roca,
            ElementType.Eter
        );

        AddReaction(
            ElementType.Eter,
            ElementType.Fuego,
            ElementType.Oro
        );

        AddReaction(
            ElementType.Luz,
            ElementType.Azufre,
            ElementType.Oro
        );
    }

    private void AddReaction(ElementType first, ElementType second, ElementType result)
    {
        reactions[(first, second)] = result;
        reactions[(second, first)] = result;
    }

    public bool TryGetResult(ElementType first, ElementType second, out ElementType result)
    {
        return reactions.TryGetValue((first, second), out result);
    }
}
