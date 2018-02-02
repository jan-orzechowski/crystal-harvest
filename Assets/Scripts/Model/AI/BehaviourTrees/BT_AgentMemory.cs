using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_AgentMemory
{
    Dictionary<int, Dictionary<string, float>> floats;
    Dictionary<int, Dictionary<string, int>> ints;
    Dictionary<int, Dictionary<string, Tile>> tiles;

    public Character Character { get; protected set; }

    public float DeltaTime;
    public World World;

    public BT_AgentMemory(Character character)
    {
        floats = new Dictionary<int, Dictionary<string, float>>();
        ints = new Dictionary<int, Dictionary<string, int>>();
        tiles = new Dictionary<int, Dictionary<string, Tile>>();
        World = GameManager.Instance.World;
        Character = character;
    }

    public float GetFloat(int id, string key, float notFoundValue)
    {
        if (floats.ContainsKey(id) && floats[id].ContainsKey(key))
        {
            return floats[id][key];
        }
        else
        {
            return notFoundValue; 
        }        
    }

    public void SetFloat(int id, string key, float value)
    {
        if(floats.ContainsKey(id) == false)
        {
            floats.Add(id, new Dictionary<string, float>());
        }
        floats[id][key] = value;
    }

    public float GetGlobalFloat(string key, float notFoundValue)
    {
        return GetFloat(0, key, notFoundValue);
    }

    public void SetGlobalFloat(string key, float value)
    {
        SetFloat(0, key, value);
    }

    public int GetInt(int id, string key, int notFoundValue)
    {
        if (ints.ContainsKey(id) && ints[id].ContainsKey(key))
        {
            return ints[id][key];
        }
        else
        {
            return notFoundValue;
        }
    }

    public void SetInt(int id, string key, int value)
    {
        if (ints.ContainsKey(id) == false)
        {
            ints.Add(id, new Dictionary<string, int>());
        }
        ints[id][key] = value;
    }

    public int GetGlobalInt(string key, int notFoundValue)
    {
        return GetInt(0, key, notFoundValue);
    }

    public void SetGlobalInt(string key, int value)
    {
        SetInt(0, key, value);
    }
    
    public Tile GetTile(int id, string key)
    {
        if (tiles.ContainsKey(id) && tiles[id].ContainsKey(key))
        {
            return tiles[id][key];
        }
        else
        {
            return null;
        }
    }

    public void SetTile(int id, string key, Tile tile)
    {
        if (tiles.ContainsKey(id) == false)
        {
            tiles.Add(id, new Dictionary<string, Tile>());
        }
        tiles[id][key] = tile;
    }

    public Tile GetGlobalTile(string key)
    {
        return GetTile(0, key);
    }

    public void SetGlobalTile(string key, Tile tile)
    {
        SetTile(0, key, tile);
    }







    public void SetRunning(int id, bool running)
    {
        if (running)
        {
            SetInt(id, "_running", 1);
        }
        else
        {
            SetInt(id, "_running", 0);
        }        
    }

    public bool IsRunning(int id)
    {
        int i = GetInt(id, "_running", 0);
        if (i == 1)
        {
            return true;
        }
        else
        {
            return false;
        }        
    }
}