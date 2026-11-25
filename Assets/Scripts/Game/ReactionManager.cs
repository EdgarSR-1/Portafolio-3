using UnityEngine;
using System.Collections.Generic;

public class ReactionManager : MonoBehaviour
{
    private Dictionary<(ElementType, ElementType), ElementType> reactions = 
        new Dictionary<(ElementType, ElementType), ElementType>();

    private void Awake()
    {
        AddReaction(ElementType.Agua, ElementType.Fuego, ElementType.Vapor);
        AddReaction(ElementType.Roca, ElementType.Fuego, ElementType.Metal);
        AddReaction(ElementType.Planta, ElementType.Agua, ElementType.Energia);
        AddReaction(ElementType.Energia, ElementType.Vapor, ElementType.Luz);
        AddReaction(ElementType.Azufre, ElementType.Roca, ElementType.Cristal);
        AddReaction(ElementType.Luz, ElementType.Cristal, ElementType.Eter);
        AddReaction(ElementType.Eter, ElementType.Metal, ElementType.Oro); // con este se gana
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
