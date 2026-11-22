using UnityEngine;
using System;

[Serializable]
// usar monobehaviour estaba dandome errores
public class DominoTileData
{
    public string tileID;

    public ElementType leftElement;
    public ElementType rightElement;

    public DominoTileData(
        string tileId, ElementType leftElement, ElementType rightElement
    )
    {
        this.tileID = tileId;
        this.leftElement = leftElement;
        this.rightElement = rightElement;
    }
}
