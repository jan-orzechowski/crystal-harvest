using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GoToRandomTile //: BT_GoTo 
{
    int maxDistanceFromCharacter;
    static int maxSearchesNumber = 100;

    public BT_GoToRandomTile(int maxDistanceFromCharacter) : base()
    {
        this.maxDistanceFromCharacter = maxDistanceFromCharacter;
    }

    public Tile GetDestinationTile(BT_AgentMemory am)
    {
        Tile newTile;
        TilePosition pos = am.Character.CurrentTile.Position;

        World world = GameManager.Instance.World;

        for (int i = 0; i < maxSearchesNumber; i++)
        {
            int x = UnityEngine.Random.Range(
            (Math.Max(pos.X - maxDistanceFromCharacter, 0)),
            (Math.Min(pos.X + maxDistanceFromCharacter, world.XSize)));

            int y = UnityEngine.Random.Range(
                (Math.Max(pos.Y - maxDistanceFromCharacter, 0)),
                (Math.Min(pos.Y + maxDistanceFromCharacter, world.YSize)));

            int height = UnityEngine.Random.Range(0, world.Height);

            newTile = world.GetTileFromPosition(new TilePosition(x, y, height));

            if (Tile.CheckPassability(newTile) == false)
            {
                continue;
            }
            else
            {
                return newTile;
            }
        }

        return null;
    }

    public Rotation GetDestinationTileRotation(BT_AgentMemory am)
    {
        int r = UnityEngine.Random.Range(0, 4);
        if (r == 0) return Rotation.N;
        else if (r == 1) return Rotation.E;
        else if (r == 2) return Rotation.S;
        else return Rotation.W;
    }
}
