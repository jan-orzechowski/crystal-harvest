using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MapGenerator
{
    // Rodzaje pól:
    // - piach
    // - skała
    // - skała z kryształami
    // - skała z rudą
    // - skała z postacią (?)
    // - skała ze statkiem kosmicznym (?)

    // Na statek kosmiczny potrzeba minimum 3x5 + miejsce na 6 postaci
    // czyli powiedzmy prostokąt 5x8 i postaci w dwóch szeregach przed statkiem

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
    int sandWithCrystals = 2;
    int rockWithOre = 3;

    int startingAreaXSize;
    int startingAreaYSize;

    int startingAreaMinimumDistanceToBorder = 5;

    int checkedAreaOffset = 3;
    int minRockTilesInCheckedArea = (5*8);
    
    public int StartingAreaX { get; protected set; }
    public int StartingAreaY { get; protected set; }

    public Tile[,,] GenerateMap(int xSize, int ySize, int startingAreaXSize, int startingAreaYSize)
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

        return GetGeneratedTiles();
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
        Tile[,,] newMap = new Tile[xSize, ySize, 2];

        Tile newTile = null;
        for (int x = 0; x < xSize; x++)
        {
            for (int y = 0; y < ySize; y++)
            {
                int value = tiles[x, y];

                if (value == sand)
                {
                    newTile = new Tile(x, y, 0, TileType.Sand);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty);
                    newMap[x, y, 1] = newTile;
                }
                else if (value == rock)
                {
                    newTile = new Tile(x, y, 0, TileType.Empty);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Rock);
                    newMap[x, y, 1] = newTile;
                }                                
                else
                {
                    newTile = new Tile(x, y, 0, TileType.Empty);
                    newMap[x, y, 0] = newTile;
                    newTile = new Tile(x, y, 1, TileType.Empty);
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