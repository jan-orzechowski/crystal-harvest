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

        int edgesCount;
        
        List<Edge> tempEdgesList;
        List<Node> tempNeighboursList;

        World world;

        Stopwatch stopwatch;

        int creatingNodesLastX;
        int creatingNodesLastY;
        int creatingNodesLastHeight;
        int nodesPerCall;
        int nodesThisCall;
        bool nodesCreated;

        public TileGraph(World world)
        {
            this.world = world;

            stopwatch = new Stopwatch();
            
            IsReady = false;
            nodesCreated = false;

            stopwatch.Start();

            int nodesCount = world.XSize * world.YSize * world.Height;

            tileNodeMap = new Dictionary<Tile, Node>(nodesCount);
            tilesPerCall = nodesCount / 60;

            nodesPerCall = 1000;

            tilesToProcess = new Queue<Tile>(tileNodeMap.Keys);

            tempEdgesList = new List<Edge>(16);
            tempNeighboursList = new List<Node>(16);

            stopwatch.Stop();
        }

        public void Reset()
        {
            IsReady = false;
            nodesCreated = false;

            stopwatch.Reset();
            tilesToProcess.Clear();
            tileNodeMap.Clear();
            tempEdgesList.Clear();
            tempNeighboursList.Clear();

            edgesCount = 0;

            creatingNodesLastX = 0;
            creatingNodesLastY = 0;
            creatingNodesLastHeight = 0;            
        }

        void CreateNodes()
        {
            stopwatch.Start();

            nodesThisCall = 0;

            for (int height = creatingNodesLastHeight; height < world.Height; height++)
            {
                for (int x = creatingNodesLastX; x < world.XSize; x++)
                {
                    for (int y = creatingNodesLastY; y < world.YSize; y++)
                    {
                        Tile t = world.Tiles[x, y, height];
                        if (t.Type != TileType.Empty)
                        {
                            Node node = new Node(t);
                            tileNodeMap.Add(t, node);
                        }

                        nodesThisCall++;

                        if (nodesThisCall >= nodesPerCall)
                        {
                            creatingNodesLastHeight = height;
                            creatingNodesLastX = x;
                            creatingNodesLastY = y + 1;

                            stopwatch.Stop();

                            return;
                        }
                    }
                    creatingNodesLastY = 0;
                }
                creatingNodesLastX = 0;
            }

            nodesCreated = true;
            tilesToProcess = new Queue<Tile>(tileNodeMap.Keys);

            stopwatch.Stop();
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
            if (IsReady)
            {
                return;
            }

            if (nodesCreated == false)
            {
                CreateNodes();
                return;
            }

            stopwatch.Start();

            tilesThisCall = 0;

            while (tilesToProcess.Count > 0)
            {
                Tile t = tilesToProcess.Dequeue();

                tempNeighboursList.Clear();

                // Sprawdzanie poruszania się wzdłuż osi X, Y

                // Tile n;

                Tile n = t.GetNorthNeighbour();
                Tile e = t.GetEastNeighbour();
                Tile s = t.GetSouthNeighbour();
                Tile w = t.GetWestNeighbour();
                
                if (Tile.CheckPassability(n)) { tempNeighboursList.Add(tileNodeMap[n]); }
                if (Tile.CheckPassability(e)) { tempNeighboursList.Add(tileNodeMap[e]); }
                if (Tile.CheckPassability(s)) { tempNeighboursList.Add(tileNodeMap[s]); }
                if (Tile.CheckPassability(w)) { tempNeighboursList.Add(tileNodeMap[w]); }

                if (t.AllowDiagonal)
                {
                    Tile ne = t.GetNorthEastNeighbour();
                    Tile se = t.GetSouthEastNeighbour();
                    Tile sw = t.GetSouthWestNeighbour();
                    Tile nw = t.GetNorthWestNeighbour();

                    if (Tile.CheckDiagonalPassability(ne) && Tile.CheckDiagonalPassability(n) && Tile.CheckDiagonalPassability(e))
                    { tempNeighboursList.Add(tileNodeMap[ne]); }
                    if (Tile.CheckDiagonalPassability(se) && Tile.CheckDiagonalPassability(s) && Tile.CheckDiagonalPassability(e))
                    { tempNeighboursList.Add(tileNodeMap[se]); }
                    if (Tile.CheckDiagonalPassability(sw) && Tile.CheckDiagonalPassability(s) && Tile.CheckDiagonalPassability(w))
                    { tempNeighboursList.Add(tileNodeMap[sw]); }
                    if (Tile.CheckDiagonalPassability(nw) && Tile.CheckDiagonalPassability(n) && Tile.CheckDiagonalPassability(w))
                    { tempNeighboursList.Add(tileNodeMap[nw]); }
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