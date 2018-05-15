using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BT_SetRandomTile : BT_Node 
{
    static float minTimeBetweenSearches = 3f;
    static float maxTimeBetweenSearches = 7f;
    static int maxDistanceFromCharacter = 5;
    static int maxSearchesNumber = 100;

    public override bool CheckPrecondition(BT_AgentMemory am)
    {
        return (am.HasTimerElapsed(ID));
    }
    
    public override void Activate(BT_AgentMemory am)
    {
        am.SetTimer(ID, UnityEngine.Random.Range(minTimeBetweenSearches, maxTimeBetweenSearches));
    }

    public override BT_Result Tick(BT_AgentMemory am)
    {
        Tile newTile = null;
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

            newTile = world.GetTileFromPosition(new TilePosition(x, y, 1));

            if (Tile.CheckPassability(newTile) == false)
            {
                continue;
            }
            else
            {
                break;
            }
        }
        
        if (newTile != null)
        {
            am.RandomTile = newTile;

            Rotation randomRotation; 

            int r = UnityEngine.Random.Range(0, 4);
            if (r == 0) randomRotation = Rotation.N;
            else if (r == 1) randomRotation = Rotation.E;
            else if (r == 2) randomRotation = Rotation.S;
            else randomRotation = Rotation.W;

            am.RandomTileRotation = randomRotation;
            return BT_Result.SUCCESS;
        }
        else
        {
            am.RandomTile = null;
            return BT_Result.FAILURE;
        }      
    }    
}
