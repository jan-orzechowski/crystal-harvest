using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator
{    
    int rockPercentage = 45;
    float minFinalRockPercentage = 0.10f;
    float maxFinalRockPercentage = 0.15f;
    
    int smoothingIterations = 4;

    // Generator liczb pseudolosowych
    System.Random prng;

    int[,] tiles;
    int xSize;
    int ySize;

    int border = 3;

    int sand = 0;
    int rock = 1;

    // Statek kosmiczny i postaci
    int startingAreaXSize;
    int startingAreaYSize;

    int startingAreaMinimumDistanceToBorder = 5;

    int checkedAreaOffset = 3;
    int minRockTilesInCheckedArea = (5*8);

    public Tile[,,] Tiles { get; protected set; }
    public int StartingAreaX { get; protected set; }
    public int StartingAreaY { get; protected set; }

    public Dictionary<TilePosition, int> Ore { get; protected set; }
    public Dictionary<TilePosition, int> Crystals { get; protected set; }

    public Tile[,,] GenerateMap(int xSize, int ySize, 
                                int startingAreaXSize, int startingAreaYSize)
    {     
        this.xSize = xSize;
        this.ySize = ySize;
        this.startingAreaXSize = startingAreaXSize;
        this.startingAreaYSize = startingAreaYSize;

        int attempts = 20;        
        for (int a = 0; a < attempts; a++)
        {
            int seed = (System.DateTime.Now.GetHashCode() + attempts).GetHashCode();
            prng = new System.Random(seed.GetHashCode());

            TryGenerateMap();

            float finalRockPercentage = GetRockPercentage();

            if (finalRockPercentage < minFinalRockPercentage
                || finalRockPercentage > maxFinalRockPercentage)
            {
                Debug.Log("Nieudana próba, nr: " + (a + 1) + ", %: " + finalRockPercentage);
                continue;
            }
            else
            {
                break;
            }
        }

        Tiles = GetGeneratedTiles();

        GenerateResources();

        return Tiles;
    }

    void TryGenerateMap()
    {
        tiles = new int[xSize, ySize];

        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (x < border || y < border
                    || x >= xSize - border || y >= ySize - border)
                {
                    tiles[x, y] = sand;
                    continue;
                }

                int randomNumber = prng.Next(0, 100);

                if (randomNumber > rockPercentage)
                {
                    tiles[x, y] = sand;
                }
                else
                {
                    tiles[x, y] = rock;
                }
            }
        }

        for (int i = 0; i < smoothingIterations; i++)
        {
            SmoothMap();
        }

        GetRandomStartingArea();
        PlaceStartingArea();
    }

    void SmoothMap()
    {
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                int neighbourRockTiles = GetRockNeighboursNumber(x, y);

                if (neighbourRockTiles > 4)
                {
                    tiles[x, y] = rock;
                }
                else if (neighbourRockTiles < 4)
                {
                    tiles[x, y] = sand;
                }
            }
        }
    }

    int GetRockNeighboursNumber(int x, int y)
    {
        int result = 0;
       
        for (int neighbourX = x - 1; neighbourX <= x + 1; neighbourX++)
        {
            for (int neighbourY = y - 1; neighbourY <= y + 1; neighbourY++)
            {
                if (neighbourX >= 0 && neighbourX < xSize 
                    && neighbourY >= 0 && neighbourY < ySize)
                {
                    if (neighbourX != x || neighbourY != y)
                    {
                        result += tiles[neighbourX, neighbourY];
                    }
                }
            }
        }

        return result;
    }

    Tile[,,] GetGeneratedTiles()
    {
        Dictionary<TileType, float> movementCosts = StaticData.LoadTilesMovementCosts();
                
        Tile[,,] newMap = new Tile[xSize, ySize, 2];

        Tile newTile = null;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                int value = tiles[x, y];

                if (value == sand)
                {
                    newTile = new Tile(x, y, 0, TileType.Sand, movementCosts[TileType.Sand]);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty, movementCosts[TileType.Empty]);
                    newMap[x, y, 1] = newTile;
                }
                else if (value == rock)
                {
                    newTile = new Tile(x, y, 0, TileType.Empty, movementCosts[TileType.Empty]);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Rock, movementCosts[TileType.Rock]);
                    newMap[x, y, 1] = newTile;
                }                                
                else
                {
                    newTile = new Tile(x, y, 0, TileType.Empty, movementCosts[TileType.Empty]);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty, movementCosts[TileType.Empty]);
                    newMap[x, y, 1] = newTile;
                }
            }
        }

        return newMap;
    }

    void GetRandomStartingArea()
    {
        int attempts = 50;
        
        int minRandomX = startingAreaMinimumDistanceToBorder;
        int maxRandomX = xSize - 1 - startingAreaMinimumDistanceToBorder - startingAreaXSize;

        int minRandomY = startingAreaMinimumDistanceToBorder;
        int maxRandomY = ySize - 1 - startingAreaMinimumDistanceToBorder - startingAreaYSize;

        for (int i = 0; i < attempts; i++)
        {            
            int randomX = prng.Next(minRandomX, maxRandomX);           
            int randomY = prng.Next(minRandomY, maxRandomY);

            int rockTilesNumber = 0;
            for (int x = randomX; x <= randomX + startingAreaXSize; x++)
            {
                for (int y = randomY; y <= randomY + startingAreaYSize; y++)
                {
                    if (tiles[x, y] == rock) rockTilesNumber++;
                }
            }

            if (rockTilesNumber >= minRockTilesInCheckedArea)
            {
                StartingAreaX = randomX;
                StartingAreaY = randomY;
                return;
            }
            else
            {
                continue;
            }
        }

        // Nie powiodło się - bierzemy środek
        StartingAreaX = (xSize - startingAreaXSize) / 2;
        StartingAreaY = (ySize - startingAreaYSize) / 2;
    }

    void PlaceStartingArea()
    {
        int endX = StartingAreaX + startingAreaXSize - 1;
        int endY = StartingAreaY + startingAreaYSize - 1;

        for (int x = StartingAreaX; x <= endX; x++)
        {
            for (int y = StartingAreaY; y <= endY; y++)
            {
                if(y == StartingAreaY && (x == StartingAreaX || x == endX)
                   || y == endY && (x == StartingAreaX || x == endX))
                {
                    continue;
                }
                else
                {
                    tiles[x, y] = 1;
                }
            }
        }
    }
    
    void GenerateResources()
    {        
        int oreAmountToPlace = prng.Next(StaticData.MinOreAmountOnMap, 
                                         StaticData.MaxOreAmountOnMap);
        int crystalsAmountToPlace = prng.Next(StaticData.MinCrystalsAmountOnMap, 
                                              StaticData.MaxCrystalsAmountOnMap);
        
        Ore = new Dictionary<TilePosition, int>();

        int orePlaced = 0;

        int attempts = 200;
        for (int i = 0; i < attempts; i++)
        {
            TilePosition randomTile = GetRandomTile();
            if (CheckTileValue(randomTile, rock) 
                && Ore.ContainsKey(randomTile) == false
                && IsTileInStartingArea(randomTile) == false)
            {
                int randomDepositSize = prng.Next(StaticData.MinOreAmountInDeposit,
                                                  StaticData.MaxOreAmountInDeposit);

                Ore.Add(randomTile, randomDepositSize);
                orePlaced += randomDepositSize;

                if (orePlaced >= oreAmountToPlace)
                {
                    break;
                }
            }
        }

        Crystals = new Dictionary<TilePosition, int>();

        int numberOfCrystalSites = 4;
        int minNumberOfCrystalsPerSite = 3;
        int maxNumberOfCrystalsPerSite = 6;
        int siteRadius = 2;

        int crystalsPlaced = 0;

        for (int s = 0; s < numberOfCrystalSites; s++)
        {
            TilePosition sitePosition;
            while (true)
            {
                sitePosition = GetRandomTile();
                if (CheckTileValue(sitePosition, sand))
                {
                    break;
                }
            }

            int numberOfCrystalsForThisSite = prng.Next(minNumberOfCrystalsPerSite, 
                                                        maxNumberOfCrystalsPerSite);

            attempts = 100;
            for (int i = 0; i < attempts; i++)
            {
                TilePosition crystalsPosition = GetRandomTileInRadius(sitePosition, siteRadius);
                if (CheckTileValue(crystalsPosition, sand)
                    && Crystals.ContainsKey(crystalsPosition) == false)
                {
                    int randomDepositSize = prng.Next(StaticData.MinCrystalsAmountInDeposit,
                                                      StaticData.MaxCrystalsAmountInDeposit);

                    Crystals.Add(crystalsPosition, randomDepositSize);
                    crystalsPlaced += randomDepositSize;

                    if (crystalsPlaced >= crystalsAmountToPlace)
                    {
                        return;
                    }

                    numberOfCrystalsForThisSite--;
                    if(numberOfCrystalsForThisSite <= 0)
                    {
                        break;
                    }
                }
            }            
        }
    }

    TilePosition GetRandomTile()
    {
        int x = prng.Next(0, xSize - 1);
        int y = prng.Next(0, ySize - 1);
        if (tiles[x,y] == rock)
        {
            return new TilePosition(x, y, 1);
        }
        else
        {
            return new TilePosition(x, y, 0);
        }
    }

    TilePosition GetRandomTileInRadius(TilePosition center, int radius)
    {
        int minX = Math.Max(0, center.X - radius);
        int maxX = Math.Min(xSize - 1, center.X + radius);
        int minY = Math.Max(0, center.Y - radius);
        int maxY = Math.Min(ySize - 1, center.Y + radius);

        int x = prng.Next(minX, maxX);
        int y = prng.Next(minY, maxY);

        if (tiles[x, y] == rock)
        {
            return new TilePosition(x, y, 1);
        }
        else
        {
            return new TilePosition(x, y, 0);
        }
    }

    bool CheckTileValue(TilePosition position, int value)
    {
        int valueAtPosition = tiles[position.X, position.Y];
        return (value == valueAtPosition);
    }

    bool IsTileInStartingArea(TilePosition position)
    {
        if (position.X >= StartingAreaX && position.X <= (StartingAreaX + startingAreaXSize - 1)
            && position.Y >= StartingAreaY && position.Y <= (StartingAreaY + startingAreaYSize - 1))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    int CountRocksOnMap()
    {
        int result = 0;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                if (tiles[x, y] == rock) result++;
            }
        }
        return result;
    }    

    float GetRockPercentage()
    {
        Debug.Log(CountRocksOnMap());
        float result = ((float)CountRocksOnMap() / (xSize * ySize));
        return result;
    }
}