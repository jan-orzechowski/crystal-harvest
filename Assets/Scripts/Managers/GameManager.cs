using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; protected set; }
    public World World;

    public GameObject RockTileCubePrefab;
    public GameObject RockTilePlanePrefab;

    [Space(10)]

    public GameObject MapBorderPrefab;
    public float MapBorderWidth;

    public GameObject GroundPlane;
    public float GroundPlaneWidthOutsideBorders;

    [Space(10)]

    public GameObject CharacterPrefab;
    public GameObject RobotPrefab;

    [Space(10)]

    public GameObject TilesParent;
    public GameObject BuildingsParent;
    public GameObject CharactersParent;
    public GameObject PreviewsParent;
    public GameObject ConstructionSitesParent;

    [Space(10)]

    public Tooltip Tooltip;
    public DialogBox DialogBox;

    public GameObject AccessArrowPrefab;

    public GameObject DebugBuildingPreviewPrefab;

    public float LevelHeightOffset;

    // Tablice zamiast słowników - dzięki temu można ustalić ich zawartość z poziomu edytora
    [Space(10)]
    public ResourceDisplayInfo[] resourceDisplay;
    [Space(10)]
    public NaturalDepositDisplayInfo[] depositDisplay;
    [Space(10)]
    public BuildingDisplayInfo[] buildingDisplay;
    [Space(10)]

    public PlatformTopsPrefabs PlatformTopsPrefabs;

    List<GameObject> previewObjects = new List<GameObject>();
    GameObject accessArrow;
    GameObject secondAccessArrow;

    Dictionary<string, string> strings;

    void OnEnable()
    {
        if (Instance != null) { Debug.LogError("Dwie instancje klasy GameManager!"); }
        Instance = this;

        strings = StaticData.LoadStrings();

        int startingAreaXSize = 5;
        int startingAreaYSize = 8;

        World = new World(StaticData.WorldWidth, StaticData.WorldLenght,
                          startingAreaXSize, startingAreaYSize);

        GenerateDisplayForTiles();

        for (int x = World.StartingAreaX + 1; x < World.StartingAreaX + 4; x++)
        {
            for (int y = World.StartingAreaY; y < World.StartingAreaY + 2; y++)
            {
                World.CreateNewCharacter(new TilePosition(x, y, 1), false);
            }
        }

        World.InstantBuild(new TilePosition(World.StartingAreaX + 1 , World.StartingAreaY + 2, 1), 
                          Rotation.N, World.GetBuildingPrototype("Spaceship"));

        World.PlaceNaturalResources();

        ShowMapBorders();

        ResizeGroundPlane();
    }

    void Update ()
    {
        World.UpdateModel(Time.deltaTime);
    }

    void ShowMapBorders()
    {
        // W
        ShowBorder(xPos: -MapBorderWidth,
                   yPos: -MapBorderWidth,
                   xSize: MapBorderWidth,
                   ySize: (World.YSize + (2 * MapBorderWidth))
                   );

        // S
        ShowBorder(xPos: 0f,
                   yPos: -MapBorderWidth,
                   xSize: World.XSize,
                   ySize: MapBorderWidth
                   );

        // N
        ShowBorder(xPos: 0f,
                   yPos: World.YSize,
                   xSize: World.XSize,
                   ySize: MapBorderWidth
                   );

        // E
        ShowBorder(xPos: World.XSize,
                   yPos: -MapBorderWidth,
                   xSize: MapBorderWidth,
                   ySize: (World.YSize + (2 * MapBorderWidth))
                   );
    }

    void ShowBorder(float xPos, float yPos, float xSize, float ySize)
    {
        GameObject border = Instantiate(MapBorderPrefab);

        Vector3 borderPosition = new Vector3(xPos, 0f, yPos);
        border.transform.SetPositionAndRotation(borderPosition, Quaternion.identity);

        border.transform.localScale = new Vector3(xSize, 1f, ySize);

        border.transform.SetParent(TilesParent.transform);
    }

    void ResizeGroundPlane()
    {
        float xSize = World.XSize + (2f * GroundPlaneWidthOutsideBorders);
        float ySize = World.YSize + (2f * GroundPlaneWidthOutsideBorders);
       
        float xPos = (World.XSize / 2f);
        float yPos = (World.YSize / 2f);

        GroundPlane.transform.SetPositionAndRotation(new Vector3(xPos, 0f, yPos), Quaternion.identity);

        GroundPlane.transform.localScale = new Vector3(xSize, 1f, ySize);
    }

    public CharacterDisplayObject GenerateDisplayForCharacter(Character c, bool isRobot)
    {
        GameObject gameObject = GameObject.Instantiate(
            isRobot ? RobotPrefab : CharacterPrefab,
            new Vector3(c.CurrentTile.X, 0, c.CurrentTile.Y),
            Quaternion.identity,
            CharactersParent.transform
            );

        CharacterDisplayObject displayObject = gameObject.GetComponentInChildren<CharacterDisplayObject>();

        displayObject.AssignCharacter(c);
        c.AssignDisplayObject(displayObject);

        return displayObject;
    }

    public void RemoveDisplayForCharacter(Character c)
    {
        SimplePool.Despawn(c.DisplayObject.gameObject);
        c.DisplayObject.ModelObject = null;
        c.AssignDisplayObject(null);
    }

    public SelectableDisplayObject ShowBuilding(Building building, TilePosition positionForDisplay)
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
            new Vector3(positionForDisplay.X,
                        positionForDisplay.Height * LevelHeightOffset,
                        positionForDisplay.Y),
            Quaternion.identity,
            BuildingsParent.transform
            );

        RotateGameObject(gameObject, building.Rotation);

        SelectableDisplayObject selectableDisplayObject = gameObject.GetComponentInChildren<SelectableDisplayObject>();

        selectableDisplayObject.AssignModelObject(building);
        building.AssignDisplayObject(selectableDisplayObject);        

        return selectableDisplayObject;
    }

    public ConstructionSiteDisplayObject ShowConstructionSite(ConstructionSite newSite)
    {
        Building building = newSite.Building;

        GameObject siteObjectPrefab = null;
        for (int i = 0; i < buildingDisplay.Length; i++)
        {
            if (buildingDisplay[i] != null && buildingDisplay[i].Type == building.Type)
            {
                siteObjectPrefab = buildingDisplay[i].ConstructionSiteObject;
                break;
            }
        }
        if (siteObjectPrefab == null)
        {
            Debug.Log("Nie znaleziono placu konstrukcyjnego dla budynku: " + building.Type);
            return null;
        }

        GameObject gameObject = GameObject.Instantiate(
            siteObjectPrefab,
            new Vector3(building.Tiles[0].Position.X,
                        building.Tiles[0].Position.Height * LevelHeightOffset,
                        building.Tiles[0].Position.Y),
            Quaternion.identity,
            ConstructionSitesParent.transform
            );

        RotateGameObject(gameObject, building.Rotation);

        bool builtOnSecondLevel = (building.Tiles[0].Position.Height > 0);

        ConstructionSiteDisplayObject selectableObject = gameObject.GetComponentInChildren<ConstructionSiteDisplayObject>();

        building.AssignDisplayObject(selectableObject);
        selectableObject.AssignConstructionSite(newSite, builtOnSecondLevel);

        return selectableObject;
    }

    public void ShowPreview(TilePosition position, Rotation rotation, BuildingPrototype prototype)
    {
        bool isPositionValid = World.IsValidBuildingPosition(position, rotation, prototype);

        GameObject model = null;
        for (int i = 0; i < buildingDisplay.Length; i++)
        {
            if (buildingDisplay[i] != null && buildingDisplay[i].Type == prototype.Type)
            {
                if (isPositionValid)
                {
                    model = buildingDisplay[i].PreviewModel;
                }
                else
                {
                    model = buildingDisplay[i].InvalidPreviewModel;
                }
                break;
            }
        }
        if (model == null)
        {
            model = DebugBuildingPreviewPrefab;
        }

        GameObject preview = SimplePool.Spawn(
            model,
            new Vector3(position.X, position.Height * LevelHeightOffset, position.Y),
            Quaternion.identity);

        if (isPositionValid && prototype.HasAccessTile)
        {
            TilePosition arrowPosition = prototype.NormalizedAccessTilePosition;

            accessArrow = SimplePool.Spawn(
                AccessArrowPrefab,
                new Vector3(position.X + arrowPosition.X,
                            (position.Height + arrowPosition.Height) * LevelHeightOffset,
                            position.Y + arrowPosition.Y),
                Quaternion.identity);

            RotateGameObject(accessArrow, prototype.NormalizedAccessTileRotation);

            accessArrow.transform.SetParent(preview.transform);
        }

        if (isPositionValid && prototype.HasSecondAccessTile)
        {
            TilePosition arrowPosition = prototype.NormalizedSecondAccessTilePosition;

            secondAccessArrow = SimplePool.Spawn(
                AccessArrowPrefab,
                new Vector3(position.X + arrowPosition.X,
                            (position.Height + arrowPosition.Height) * LevelHeightOffset,
                            position.Y + arrowPosition.Y),
                Quaternion.identity);

            RotateGameObject(secondAccessArrow, prototype.NormalizedSecondAccessTileRotation);

            secondAccessArrow.transform.SetParent(preview.transform);
        }

        RotateGameObject(preview, rotation);

        preview.transform.SetParent(PreviewsParent.transform);
        previewObjects.Add(preview);
    }

    public void RemoveConstructionSiteDisplay(ConstructionSite siteToRemove)
    {
        SimplePool.Despawn(siteToRemove.Building.DisplayObject.gameObject);
        siteToRemove.Building.DisplayObject.ModelObject = null;
        siteToRemove.Building.DisplayObject = null;
    }

    public void RemoveDisplayForBuilding(Building building)
    {
        SimplePool.Despawn(building.DisplayObject.gameObject);
        building.DisplayObject.ModelObject = null;
        building.DisplayObject = null;
    }

    public void HidePreviews()
    {
        for (int i = 0; i < previewObjects.Count; i++)
        {
            SimplePool.Despawn(previewObjects[i]);
        }
        if (accessArrow != null)
        {
            accessArrow.transform.parent = null;
            accessArrow.transform.localScale = new Vector3(1f, 1f, 1f);
            SimplePool.Despawn(accessArrow);
            accessArrow = null;
        }
        if (secondAccessArrow != null)
        {
            secondAccessArrow.transform.parent = null;
            secondAccessArrow.transform.localScale = new Vector3(1f, 1f, 1f);
            SimplePool.Despawn(secondAccessArrow);
            secondAccessArrow = null;
        }
        previewObjects.Clear();        
    }

    public ResourceDisplayInfo GetResourceDisplayInfo(int resourceID)
    {
        ResourceDisplayInfo result = null;
        for (int i = 0; i < resourceDisplay.Length; i++)
        {
            if (resourceDisplay[i] != null && resourceDisplay[i].ResourceID == resourceID)
            {
                result = resourceDisplay[i];
                break;
            }
        }
        return result;
    }

    void GenerateDisplayForTiles()
    {
        for (int height = 0; height < World.Height; height++)
        {
            for (int x = 0; x < World.XSize; x++)
            {
                for (int y = 0; y < World.YSize; y++)
                {
                    Tile tile = World.Tiles[x, y, height];

                    if (tile.Type == TileType.Sand)
                    {
                        continue;
                    }
                    else if (tile.Type == TileType.Rock)
                    {
                        if ((tile.GetNorthNeighbour() != null && tile.GetNorthNeighbour().Type == TileType.Rock)
                             && (tile.GetEastNeighbour() != null && tile.GetEastNeighbour().Type == TileType.Rock)
                             && (tile.GetSouthNeighbour() != null && tile.GetSouthNeighbour().Type == TileType.Rock)
                             && (tile.GetWestNeighbour() != null && tile.GetWestNeighbour().Type == TileType.Rock))
                        {
                            GameObject.Instantiate(
                                RockTilePlanePrefab,
                                new Vector3(x, height * LevelHeightOffset, y),
                                Quaternion.identity,
                                TilesParent.transform);
                        }
                        else
                        {
                            GameObject.Instantiate(
                                RockTileCubePrefab,
                                new Vector3(x, height * LevelHeightOffset, y),
                                Quaternion.identity,
                                TilesParent.transform);
                        }                        
                    }
                }   
            }
        }
    }

    void RotateGameObject(GameObject displayObject, Rotation rotation)
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

    void UpdateSinglePlatformDisplay(Tile t)
    {
        if (t != null && World.Platforms.ContainsKey(t))
        {
            ((PlatformDisplayObject)World.Platforms[t].DisplayObject).UpdatePlatformDisplay();
        }
    }

    public void UpdatePlatformDisplay(Tile t)
    {
        UpdateSinglePlatformDisplay(t);

        UpdateSinglePlatformDisplay(t.GetNorthNeighbour());
        UpdateSinglePlatformDisplay(t.GetEastNeighbour());
        UpdateSinglePlatformDisplay(t.GetSouthNeighbour());
        UpdateSinglePlatformDisplay(t.GetWestNeighbour());

        UpdateSinglePlatformDisplay(t.GetNorthEastNeighbour());
        UpdateSinglePlatformDisplay(t.GetSouthEastNeighbour());
        UpdateSinglePlatformDisplay(t.GetSouthWestNeighbour());
        UpdateSinglePlatformDisplay(t.GetNorthWestNeighbour());
    }

    public string GetText(string key)
    {
        if (strings.ContainsKey(key)) return strings[key];
        else return (key + " - NO TEXT FOUND");
    }

    public string GetResourceName(int resourceID)
    {
        if (World.ResourcesInfo.ContainsKey(resourceID))
            return GetText(World.ResourcesInfo[resourceID].Name);
        else return (resourceID + " - NO RESOURCE INFO");
    }
}