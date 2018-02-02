using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; protected set; }
    public World World;

    public GameObject DebugGroundTilePrefab;
    public GameObject DebugRockTilePrefab;
    public GameObject CharacterPrefab;

    public GameObject TilesParent;
    public GameObject BuildingsParent;
    public GameObject CharactersParent;
    public GameObject PreviewsParent;

    public GameObject DebugBuildingPreviewPrefab;

    public float LevelHeightOffset;

    // Tablica zamiast słownika - dzięki temu można ustalić jej zawartość z poziomu edytora
    public BuildingDisplayInfo[] buildingDisplay;

    List<GameObject> PreviewObjects = new List<GameObject>();

    void OnEnable()
    {
        if (Instance != null) { Debug.LogError("Dwie instancje klasy GameManager!"); }
        Instance = this;

        int worldSize = 50;

        World = new World(worldSize, worldSize);

        GenerateDisplayForTiles();

        for (int x = 0; x < 6; x++)
        {
            for (int y = 0; y < 5; y++)
            {
                World.CreateNewCharacter(new TilePosition(x, y, 0));
            }
        }                
    }

    void Update ()
    {
        World.UpdateModel(Time.deltaTime);
    }
    public SelectableDisplayObject GenerateDisplayForCharacter(Character c)
    {
        GameObject gameObject = GameObject.Instantiate(
            CharacterPrefab,
            new Vector3(c.CurrentTile.X, 0, c.CurrentTile.Y),
            Quaternion.identity,
            BuildingsParent.transform
            );

        SelectableDisplayObject displayObject = gameObject.GetComponentInChildren<SelectableDisplayObject>();

        displayObject.AssignModelObject(c);
        c.AssignDisplayObject(displayObject);

        return displayObject;
    }

    public SelectableDisplayObject ShowBuilding(Building building, Rotation rotation)
    {
        GameObject model = null;
        for (int i = 0; i < buildingDisplay.Length; i++)
        {
            if (buildingDisplay[i] != null && buildingDisplay[i].Type == building.Type)
            {
                model = buildingDisplay[i].Model;
                break;
            }
        }
        if (model == null)
        {
            Debug.Log("Nie znaleziono modelu dla budynku: " + building.Type);
            model = DebugBuildingPreviewPrefab;
        }

        GameObject gameObject = GameObject.Instantiate(
            model,
            new Vector3(building.Tiles[0].Position.X, 
                        building.Tiles[0].Position.Height * LevelHeightOffset, 
                        building.Tiles[0].Position.Y),
            Quaternion.identity,
            BuildingsParent.transform
            );

        GetRotationForDisplayObject(gameObject, rotation);

        SelectableDisplayObject selectableDisplayObject = gameObject.GetComponentInChildren<SelectableDisplayObject>();

        selectableDisplayObject.AssignModelObject(building);
        building.AssignDisplayObject(selectableDisplayObject);

        return selectableDisplayObject;
    }

    public void ShowPreview(TilePosition position, Rotation rotation, BuildingPrototype prototype)
    {
        GameObject model = null;
        for (int i = 0; i < buildingDisplay.Length; i++)
        {
            if (buildingDisplay[i] != null && buildingDisplay[i].Type == prototype.ModelName)
            {
                model = buildingDisplay[i].PreviewModel;
                break;
            }
        }
        if (model == null)
        {
            model = DebugBuildingPreviewPrefab;
        }

        GameObject preview = SimplePool.Spawn(model, 
            new Vector3(position.X, position.Height * LevelHeightOffset, position.Y), 
            Quaternion.identity);

        GetRotationForDisplayObject(preview, rotation);

        preview.transform.SetParent(PreviewsParent.transform);
        PreviewObjects.Add(preview);
    }

    public void HidePreviews()
    {
        for (int i = 0; i < PreviewObjects.Count; i++)
        {
            SimplePool.Despawn(PreviewObjects[i]);
        }
        PreviewObjects.Clear();
    }

    public void RemoveDisplayForBuilding(Building building)
    {
        building.DisplayObject.transform.gameObject.SetActive(false);
        building.DisplayObject.ModelObject = null;
        building.DisplayObject = null;
    }

    void GenerateDisplayForTiles()
    {
        for (int height = 0; height < World.Height; height++)
        {
            for (int x = 0; x < World.XSize; x++)
            {
                for (int y = 0; y < World.YSize; y++)
                {
                    if (World.Tiles[x, y, height].Type == TileType.Dirt)
                    {
                        GameObject.Instantiate(
                            DebugGroundTilePrefab,
                            new Vector3(x, height * LevelHeightOffset, y),
                            Quaternion.identity,
                            TilesParent.transform);
                    }
                    else if (World.Tiles[x, y, height].Type == TileType.Rock)
                    {
                        GameObject.Instantiate(
                            DebugRockTilePrefab,
                            new Vector3(x, height * LevelHeightOffset, y),
                            Quaternion.identity,
                            TilesParent.transform);
                    }
                }   
            }
        }
    }

    void GetRotationForDisplayObject(GameObject displayObject, Rotation rotation)
    {
        if (rotation == Rotation.N)
        {
            return;
        }
        else if (rotation == Rotation.E)
        {
            displayObject.transform.SetPositionAndRotation(
                displayObject.transform.position + new Vector3(0, 0, 1),
                Quaternion.Euler(new Vector3(0, 90, 0))
                );
        }
        else if (rotation == Rotation.S)
        {
            displayObject.transform.SetPositionAndRotation(
                displayObject.transform.position + new Vector3(1, 0, 1),
                Quaternion.Euler(new Vector3(0, 180, 0))
                );
        }
        else
        {
            displayObject.transform.SetPositionAndRotation(
                displayObject.transform.position + new Vector3(1, 0, 0),
                Quaternion.Euler(new Vector3(0, 270, 0))
                );
        }
    }
}