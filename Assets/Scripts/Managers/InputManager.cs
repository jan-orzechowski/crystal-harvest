using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class InputManager : MonoBehaviour 
{
    public Tile CurrentTile { get; private set; }

    Camera mainCamera;

    Ray ray;
    RaycastHit[] raycastResults;
    int maxRaycastResultsNumber = 20;
    Dictionary<Character, RaycastHit> charactersHit = new Dictionary<Character, RaycastHit>();
    Dictionary<Building, RaycastHit> buildingsHit = new Dictionary<Building, RaycastHit>();

    int rayLayerMask = -1;
    float rayMaxDistance = 100f;

    public GameObject SelectionBoxPrefab;
    GameObject selectionBox;
    MeshRenderer selectionBoxMeshRenderer;

    BuildModeManager buildModeManager;
    bool buildMode;

    public ISelectable SelectedObject;

    public SelectionPanel SelectionPanel;
    public SidePanel SidePanel;

    public GameObject debugPreviewPrefab;
    GameObject debugPreview;

    bool showingHighlight;

    bool isDragging;
    TilePosition dragStartPosition;
    TilePosition dragEndPosition;

    World world;

    void Start () 
    {
        mainCamera = Camera.main;

        raycastResults = new RaycastHit[maxRaycastResultsNumber];

        selectionBox = Instantiate(SelectionBoxPrefab);
        selectionBoxMeshRenderer = selectionBox.transform.GetComponentInChildren<MeshRenderer>();
        debugPreview = Instantiate(debugPreviewPrefab);
        debugPreview.SetActive(false);

        world = GameManager.Instance.World;
        
        buildModeManager = FindObjectOfType<BuildModeManager>();
        SetBuildMode(false);
    }

    void Update () 
    {
        GetTileUnderMouse();
        HandleUserInput();

        UpdateBuildingPreviews();
        UpdateSelection();
        // UpdateCurrentTileHighlight();
    }

    void GetTileUnderMouse()
    {
        CurrentTile = null;

        Vector3 cameraPosition = mainCamera.transform.position;
        Vector3 mousePosition = mainCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100.0f));        

        for (int height = world.Height - 1; height >= 0; height--)
        {
            TilePosition tp = GetTilePositionFromMousePosition(
                cameraPosition, mousePosition,
                height, GameManager.Instance.LevelHeightOffset);

            Tile t = world.GetTileFromPosition(tp);
            if (t == null || t.Type == TileType.Empty)
            {
                continue;
            }
            else
            {
                CurrentTile = t;
                break;
            }
        }

        if (CurrentTile == null)
        {
            // Sprawdzanie, czy nie jest to sytuacja, w której nie natrafiliśmy na żadne niepuste pole, ponieważ natrafiliśmy na ścianę
            // W takim razie bierzemy odpowiadające najwyższe pole

            TilePosition tp = GetTilePositionFromMousePosition(cameraPosition, mousePosition, 0, 0);
            if (world.CheckTilePosition(tp))
            {
                for (int i = world.Height - 1; i >= 0; i--)
                {
                    Tile t = world.GetTileFromPosition(new TilePosition(tp.X, tp.Y, i));
                    if (t == null || t.Type == TileType.Empty)
                    {
                        continue;
                    }
                    else
                    {
                        CurrentTile = t;
                        break;
                    }
                }
            }
        }
    }

    void HandleUserInput()
    {
        World world = GameManager.Instance.World;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (world.CannotUnpause == false)
            {
                world.Paused = !world.Paused;
            }
        }

        if (Input.GetMouseButtonDown(0)) // LPM
        {            
            if (EventSystem.current.IsPointerOverGameObject())
            {
                // Kursor jest nad elementem UI
                return;
            }
            else
            {
                SelectionPanel.HidePanels();
                SidePanel.HidePanels();
            }

            if (buildMode)
            {
                if (CurrentTile != null)
                {
                    if (buildModeManager.BuildMode == BuildMode.Multiple)
                    {
                        if (isDragging == false)
                        {
                            // Początek przeciągania
                            dragStartPosition = CurrentTile.Position;
                            isDragging = true;
                        }

                    }
                    else // if (buildModeManager.BuildMode == BuildMode.Single)
                    {
                        buildModeManager.Build(CurrentTile.Position, CurrentTile.Position);
                    }
                }
            }
            else
            {
                DoRaycast();
            }
        }

        if (Input.GetMouseButton(0)) // LPM
        {
            if (isDragging)
            {
                if (CurrentTile != null)
                    dragEndPosition = CurrentTile.Position;
            }
        }

        if (Input.GetMouseButtonUp(0)) // LPM
        {
            // Koniec przeciągania
            if (isDragging && buildMode)
            {
                isDragging = false;
                if (CurrentTile != null)
                {
                    dragEndPosition = CurrentTile.Position;
                    buildModeManager.Build(dragStartPosition, dragEndPosition);
                }
            }
        }

        if (Input.GetMouseButtonDown(1) // PPM
        || Input.GetKeyDown(KeyCode.Escape))
        {
            if (SidePanel.PanelVisible) SidePanel.HidePanels();
            if (SelectionPanel.PanelVisible) SelectionPanel.HidePanels();

            SetBuildMode(false);
            RemoveSelection();
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (buildMode)
            {
                buildModeManager.Rotate();
            }
        }

        if (Input.GetMouseButton(2)) // ŚPM
        {
            
        }
    }

    TilePosition GetTilePositionFromMousePosition(Vector3 cameraPosition, Vector3 mousePosition, int height, float heightOffset)
    {
        if (mousePosition.y == cameraPosition.y) // linia równoległa, brak przecięcia
        {
            return new TilePosition(-1, -1, -1);
        }
        else
        {
            float t;

            t = ((height * heightOffset) - cameraPosition.y) / (mousePosition.y - cameraPosition.y);

            float x = cameraPosition.x + (mousePosition.x - cameraPosition.x) * t;
            float z = cameraPosition.z + (mousePosition.z - cameraPosition.z) * t;

            //Debug.Log("Punkt przecięcia dla " + heightOffset + ": " + x + ", " + z);

            int xCoord = Mathf.FloorToInt(x);
            int zCoord = Mathf.FloorToInt(z);

            //Debug.Log("Pole na poziomie " + heightOffset + ": " + xCoord + ", " + zCoord);

            return new TilePosition(xCoord, zCoord, height);
        }
    }

    void DoRaycast()
    {
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        charactersHit.Clear();
        buildingsHit.Clear();
        
        // To zwraca wszystkie collidery
        int hitNumber = Physics.RaycastNonAlloc(ray, raycastResults, rayMaxDistance, rayLayerMask, QueryTriggerInteraction.Ignore);
        
        for (int i = 0; i < hitNumber; i++)
        {
            SelectableDisplayObject hitDisplayObject = raycastResults[i].transform.GetComponentInParent<SelectableDisplayObject>();

            if (hitDisplayObject.ModelObject is Character)
            {
                charactersHit.Add((Character)hitDisplayObject.ModelObject, raycastResults[i]);
            }
            else if (hitDisplayObject.ModelObject is ConstructionSite)
            {
                buildingsHit.Add(((ConstructionSite)hitDisplayObject.ModelObject).Building, raycastResults[i]);
            }
            else if (hitDisplayObject.ModelObject is Building)
            {
                if(buildingsHit.ContainsKey((Building)hitDisplayObject.ModelObject) == false)
                    buildingsHit.Add((Building)hitDisplayObject.ModelObject, raycastResults[i]);
            }
        }

        if (charactersHit.Count > 0)
        {
            Character selectedCharacter = null;
            float distance = rayMaxDistance;
            foreach (Character character in charactersHit.Keys)
            {
                float newDistance = Mathf.Abs(Vector3.Magnitude(
                    Camera.main.transform.position - charactersHit[character].point));

                if (newDistance < distance)
                {
                    distance = newDistance;
                    selectedCharacter = character;
                }
            }
            
            // Mamy postać!
            Select(selectedCharacter);
        }
        else if (buildingsHit.Count > 0)
        {
            Building selectedBuilding = null;
            float distance = rayMaxDistance;
            
            foreach (Building building in buildingsHit.Keys)
            {
                float newDistance = Mathf.Abs(Vector3.Magnitude(
                    Camera.main.transform.position - buildingsHit[building].point));

                if (newDistance < distance)
                {
                    distance = newDistance;
                    selectedBuilding = building;
                }
            }

            // Mamy budynek!
            Select(selectedBuilding);
        }
        else
        {
            // Nie trafiliśmy nic
            RemoveSelection();
        }
    }

    void UpdateCurrentTileHighlight()
    {
        if (showingHighlight && CurrentTile != null)
        {
            debugPreview.SetActive(true);
            debugPreview.transform.position = new Vector3(
                CurrentTile.Position.X + 0.5f, 0f, CurrentTile.Position.Y + 0.5f);
        }
        else
        {
            debugPreview.SetActive(false);
        }
    }

    void UpdateBuildingPreviews()
    {
        GameManager.Instance.HidePreviews();

        if (buildMode)
        {
            if (isDragging)
            {
                buildModeManager.ShowBuildPreview(dragStartPosition, dragEndPosition);
            }            
            else if (CurrentTile != null)
            {
                buildModeManager.ShowBuildPreview(CurrentTile.Position, CurrentTile.Position);
            }
        }        
    }

    void UpdateSelection()
    {
        if (SelectedObject != null && SelectedObject.GetDisplayObject() != null)
        {
            BoxCollider selectedObjectCollider = SelectedObject.GetDisplayObject().Collider;

            if (selectedObjectCollider != null && selectedObjectCollider.gameObject.activeSelf)
            {
                SidePanel.HidePanels();

                if (selectionBoxMeshRenderer.enabled == false)
                    selectionBoxMeshRenderer.enabled = true;

                selectionBox.transform.SetPositionAndRotation(
                    selectedObjectCollider.transform.position + selectedObjectCollider.center,
                    selectedObjectCollider.transform.rotation);

                selectionBox.transform.localScale = new Vector3(
                    selectedObjectCollider.size.x * selectedObjectCollider.transform.localScale.x,
                    selectedObjectCollider.size.y * selectedObjectCollider.transform.localScale.y,
                    selectedObjectCollider.size.z * selectedObjectCollider.transform.localScale.z);
            }
            else
            {
                selectionBoxMeshRenderer.enabled = false;
            }
        }
        else
        {
            RemoveSelection();
        }
    }

    void Select(ISelectable selectedObject)
    {        
        if (selectedObject != null)
        {
            SelectedObject = selectedObject;
            SelectionPanel.gameObject.SetActive(true);
            SelectionPanel.AssignSelectedObject(selectedObject);
        }
        else
        {
            RemoveSelection();
        }
    }

    public void RemoveSelection()
    {
        SelectedObject = null;
        SelectionPanel.gameObject.SetActive(false);
        SelectionPanel.HidePanels();
        selectionBoxMeshRenderer.enabled = false;
    }

    public void SetBuildMode(bool buildMode)
    {
        this.buildMode = buildMode;
        if (buildMode)
        {
            SidePanel.HidePanels();
            showingHighlight = false;
            RemoveSelection();                        
        }
        else
        {
            showingHighlight = true;
        }
    }
}