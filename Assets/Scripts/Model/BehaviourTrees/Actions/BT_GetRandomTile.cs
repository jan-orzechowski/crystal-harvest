using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_GetRandomTile : BT_Node
{
    string tileVarName;
    int maxDistanceFromCharacter;
    int maxHeightDifference;
    int maxSearchesNumber = 100;

    public BT_GetRandomTile(string tileVarName, int maxDistanceFromCharacter, int maxHeightDifference = 2) : base()
    {
        this.tileVarName = tileVarName;
        this.maxDistanceFromCharacter = maxDistanceFromCharacter;
        this.maxHeightDifference = maxHeightDifference;
    }

    public override BT_Result Tick(BT_AgentMemory am)
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

            int height = UnityEngine.Random.Range(
                (Math.Max(pos.Height - maxHeightDifference, 0)),
                (Math.Min(pos.Height + maxHeightDifference, world.Height)));

            newTile = world.GetTileFromPosition(new TilePosition(x, y, height));

            if (newTile == null || newTile.MovementCost == 0 || newTile.Type == TileType.Empty)
            {
                continue;
            }
            else
            {
                am.SetGlobalTile(tileVarName, newTile);
                return BT_Result.SUCCESS;
            }
        }
        return BT_Result.FAILURE;
    }
}
