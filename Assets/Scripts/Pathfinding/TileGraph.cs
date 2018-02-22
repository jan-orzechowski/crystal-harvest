using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Diagnostics;
using System.Linq;

namespace Pathfinding
{
    public class TileGraph
    {
        Dictionary<Tile, Node> tileNodeMap;

        Queue<Tile> tilesToProcess;        

        public bool IsReady { get; protected set; }

        int tilesPerCall;
        int tilesThisCall;

        int edgesCount = 0;
        
        List<Edge> tempEdgesList;
        List<Node> tempNeighboursList;

        World world;

        Stopwatch stopwatch;

        public TileGraph(World world)
        {
            this.world = world;

            stopwatch = new Stopwatch();
            
            IsReady = false;
            
            stopwatch.Start();

            int nodesCount = world.XSize * world.YSize * world.Height;

            tileNodeMap = new Dictionary<Tile, Node>(nodesCount);
            tilesPerCall = nodesCount / 60;

            for (int height = 0; height < world.Height; height++)
            {
                for (int x = 0; x < world.XSize; x++)
                {
                    for (int y = 0; y < world.YSize; y++)
                    {
                        Tile t = world.Tiles[x, y, height];
                        Node node = new Node(t);
                        tileNodeMap.Add(t, node);
                    }
                }
            }
            tilesToProcess = new Queue<Tile>(tileNodeMap.Keys);

            tempEdgesList = new List<Edge>(16);
            tempNeighboursList = new List<Node>(16);

            stopwatch.Stop();
        }

        public void Reset()
        {
            IsReady = false;

            stopwatch.Reset();
            tilesToProcess.Clear();
            tileNodeMap.Clear();
            tempEdgesList.Clear();
            tempNeighboursList.Clear();

            for (int height = 0; height < world.Height; height++)
            {
                for (int x = 0; x < world.XSize; x++)
                {
                    for (int y = 0; y < world.YSize; y++)
                    {
                        Tile t = world.Tiles[x, y, height];
                        Node node = new Node(t);
                        tileNodeMap.Add(t, node);
                    }
                }
            }
            tilesToProcess = new Queue<Tile>(tileNodeMap.Keys);
        }

        public Dictionary<Tile, Node> GetCompleteGraph()
        {           
            if (IsReady == false)
            {
                return null;
            }        
            else
            {
                return tileNodeMap;
            }
        }

        public void ProcessTiles()
        {
            if (IsReady) return;

            stopwatch.Start();

            tilesThisCall = 0;

            while (tilesToProcess.Count > 0)
            {
                Tile t = tilesToProcess.Dequeue();

                tempNeighboursList.Clear();

                // Sprawdzanie poruszania się wzdłuż osi X, Y

                Tile n;

                // N
                n = t.GetNorthNeighbour();
                if (Tile.CheckPassability(n))
                {
                    tempNeighboursList.Add(tileNodeMap[n]);
                }

                // E
                n = t.GetEastNeighbour();
                if (Tile.CheckPassability(n))
                {
                    tempNeighboursList.Add(tileNodeMap[n]);
                }

                //S
                n = t.GetSouthNeighbour();
                if (Tile.CheckPassability(n))
                {
                    tempNeighboursList.Add(tileNodeMap[n]);
                }

                // W
                n = t.GetWestNeighbour();
                if (Tile.CheckPassability(n))
                {
                    tempNeighboursList.Add(tileNodeMap[n]);
                }

                // N-E
                if (Tile.CheckPassability(t.GetNorthEastNeighbour()))
                {
                    tempNeighboursList.Add(tileNodeMap[t.GetNorthEastNeighbour()]);
                }

                // S-E
                if (Tile.CheckPassability(t.GetSouthEastNeighbour()))
                {
                    tempNeighboursList.Add(tileNodeMap[t.GetSouthEastNeighbour()]);
                }

                // S-W
                if (Tile.CheckPassability(t.GetSouthWestNeighbour()))
                {
                    tempNeighboursList.Add(tileNodeMap[t.GetSouthWestNeighbour()]);
                }

                // N-W
                if (Tile.CheckPassability(t.GetNorthWestNeighbour()))
                {
                    tempNeighboursList.Add(tileNodeMap[t.GetNorthWestNeighbour()]);
                }                

                // Tworzenie krawędzi do sąsiadów

                tempEdgesList.Clear();

                foreach (Node neighbour in tempNeighboursList)
                {
                    Edge edge = new Edge(neighbour, neighbour.Tile.MovementCost);
                    tempEdgesList.Add(edge);
                    edgesCount++;
                }

                Node node = tileNodeMap[t];
                node.Edges = new List<Edge>(tempEdgesList);

                tilesThisCall++;
                if (tilesThisCall >= tilesPerCall)
                {
                    break;
                }                
            }
                                
            if (tilesToProcess.Count == 0)
            {
                // Sprawdzanie wszystkich schodów - z listy z World
                foreach (Building stairs in world.Stairs)
                {
                    Tile highTile = stairs.GetAccessTile();
                    Tile lowTile = stairs.GetAccessTile(getSecond: true);

                    Edge egdeUp = new Edge(tileNodeMap[highTile], 6f);
                    tileNodeMap[lowTile].Edges.Add(egdeUp);

                    Edge edgeDown = new Edge(tileNodeMap[lowTile], 6f);
                    tileNodeMap[highTile].Edges.Add(edgeDown);
                }          

                IsReady = true;
                stopwatch.Stop();
                UnityEngine.Debug.Log("Stworzono graf do szukania ścieżek. Węzły: " + tileNodeMap.Count + ". Krawędzie: " + edgesCount
                + ". Czas tworzenia: " + stopwatch.ElapsedMilliseconds + "ms.");
            }

            stopwatch.Stop();                  
        }       
    }
}