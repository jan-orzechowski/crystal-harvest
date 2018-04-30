using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class BT_AgentMemory
{
    Dictionary<int, Dictionary<string, float>> floats;
    Dictionary<int, Dictionary<string, int>> ints;
    HashSet<int> runningNodes;

    public IWorkplace Workplace;
    public IWorkplace PotentialWorkplace;
    public bool UseWorkplaceSecondAccessTile;

    public Service Service;
    public Service PotentialService;
    public bool UseServiceSecondAccessTile;

    // public IAccessible Destination { get; protected set; }
    // public bool UseDestinationSecondAccessTile { get; protected set; }

    public Character Character { get; protected set; }

    public float DeltaTime;
    public World World;

    public BT_AgentMemory(Character character)
    {
        floats = new Dictionary<int, Dictionary<string, float>>();
        ints = new Dictionary<int, Dictionary<string, int>>();
        runningNodes = new HashSet<int>();
        World = GameManager.Instance.World;
        Character = character;
    }

    public float GetFloat(int id, string key, float notFoundValue = 0f)
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
    
    public int GetInt(int id, string key, int notFoundValue = 0)
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
   
    public void StartRunning(int id)
    {
        if (runningNodes.Contains(id) == false) runningNodes.Add(id);
    }

    public void StopRunning(int id)
    {
        if (runningNodes.Contains(id)) runningNodes.Remove(id);
    }
    
    public bool IsRunning(int id)
    {
        return (runningNodes.Contains(id));
    }
}