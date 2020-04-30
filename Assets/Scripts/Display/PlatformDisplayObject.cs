using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class PlatformDisplayObject : SelectableDisplayObject
{
    PlatformTopsPrefabs prefabs;
    Tile tile;

    World world;

    List<GameObject> models;

    void Awake()
    {
        prefabs = GameManager.Instance.PlatformTopsPrefabs;
        world = GameManager.Instance.World;

        models = new List<GameObject>();
    }

    public void UpdatePlatformDisplay()
    {
        HidePlatform();

        bool n = HasPlatform(tile.GetNorthNeighbour());
        bool e = HasPlatform(tile.GetEastNeighbour());
        bool s = HasPlatform(tile.GetSouthNeighbour());
        bool w = HasPlatform(tile.GetWestNeighbour());

        bool ne = HasPlatform(tile.GetNorthEastNeighbour());
        bool se = HasPlatform(tile.GetSouthEastNeighbour());
        bool sw = HasPlatform(tile.GetSouthWestNeighbour());
        bool nw = HasPlatform(tile.GetNorthWestNeighbour());

        // 4 krawędzie
        // bez dodatkowych narożników
        if (!n && !e && !s && !w)
        {
            Spawn(prefabs.Top_4Sides, 0);
        } 
        else

        // 0 krawędzi
        // cztery możliwe narożniki
        if (n && e && s && w)
        {
            Spawn(prefabs.Top_0Sides, 0);
            
            if (!ne) { SpawnNECorner(); }
            if (!se) { SpawnSECorner(); }
            if (!sw) { SpawnSWCorner(); }
            if (!nw) { SpawnNWCorner(); }
        }
        else

        // 2 krawędzie równoległe
        // bez dodatkowych narożników
        if (n && !e && s && !w)
        {
            // N - S
            Spawn(prefabs.Top_2Sides, 90);
        }
        else 
        if (!n && e && !s && w)
        {
            // W - E
            Spawn(prefabs.Top_2Sides, 0);
        }
        else
     
        // 2 krawędzie prostopadłe
        // jeden możliwy narożnik
        if (n && !e && !s && w)
        {
            Spawn(prefabs.Top_Corner, 180);

            if (!nw) { SpawnNWCorner(); }
        }
        else
        if (n && e && !s && !w)
        {
            Spawn(prefabs.Top_Corner, -90);

            if (!ne) { SpawnNECorner(); }
        }
        else
        if (!n && e && s && !w)
        {
            Spawn(prefabs.Top_Corner, 0);

            if (!se) { SpawnSECorner(); }
        }
        else
        if (!n && !e && s && w)
        {
            Spawn(prefabs.Top_Corner, 90);

            if (!sw) { SpawnSWCorner(); }
        }
        else

        // 3 krawędzie
        // bez dodatkowych narożników
        if (n && !e && !s && !w)
        {
            Spawn(prefabs.Top_3Sides, -90);
        }
        else
        if (!n && e && !s && !w)
        {
            Spawn(prefabs.Top_3Sides, 0);
        }
        else
        if (!n && !e && s && !w)
        {
            Spawn(prefabs.Top_3Sides, 90);
        }
        else
        if (!n && !e && !s && w)
        {
            Spawn(prefabs.Top_3Sides, 180);
        }
        else

        // 1 krawędź
        // dwa możliwe narożniki
        if (!n && e && s && w)
        {
            Spawn(prefabs.Top_1Side, 0);

            if (!se) { SpawnSECorner(); }
            if (!sw) { SpawnSWCorner(); }
        }
        else
        if (n && !e && s && w)
        {
            Spawn(prefabs.Top_1Side, 90);

            if (!nw) { SpawnNWCorner(); }
            if (!sw) { SpawnSWCorner(); }
        }
        else
        if (n && e && !s && w)
        {
            Spawn(prefabs.Top_1Side, 180);

            if (!nw) { SpawnNWCorner(); }
            if (!ne) { SpawnNECorner(); }
        }
        else
        if (n && e && s && !w)
        {
            Spawn(prefabs.Top_1Side, -90);
            if (!ne) { SpawnNECorner(); }
            if (!se) { SpawnSECorner(); }
        }        
    }

    void HidePlatform()
    {
        for (int i = 0; i < models.Count; i++)
        {
            SimplePool.Despawn(models[i]);
        }
        models.Clear();
    }

    bool HasPlatform(Tile t)
    {
        return (t != null && world.Platforms.ContainsKey(t));
    }

    GameObject GetRotationObject(GameObject go)
    {
        return go.transform.GetChild(0).gameObject;
    }

    void SpawnNECorner()
    {
        Spawn(prefabs.Top_SingleCorner, -90);
    }

    void SpawnSECorner()
    {
        Spawn(prefabs.Top_SingleCorner, 0);
    }

    void SpawnSWCorner()
    {
        Spawn(prefabs.Top_SingleCorner, 90);
    }

    void SpawnNWCorner()
    {
        Spawn(prefabs.Top_SingleCorner, 180);
    }

    void Spawn(GameObject prefab, float rotation)
    {
        GameObject cornerObject = SimplePool.Spawn(prefab, 
                                                   this.transform.position, 
                                                   Quaternion.identity);

        GameObject rotationObject = GetRotationObject(cornerObject);
        rotationObject.transform.SetPositionAndRotation(rotationObject.transform.position, 
                                                        Quaternion.Euler(0, rotation, 0));

        cornerObject.transform.SetParent(this.transform);
        models.Add(cornerObject);
    }

    public override void AssignModelObject(ISelectable modelObject)
    {
        base.AssignModelObject(modelObject);

        if (modelObject != null && modelObject is Building)
        {
            Building building = (Building)modelObject;
            building.AssignDisplayObject(this);
            if (building.Type == "Platform")
            {
                tile = building.Tiles[0];
                GameManager.Instance.UpdatePlatformDisplay(building.Tiles[0]);
                return;
            }
        }
    }
}

