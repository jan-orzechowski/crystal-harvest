using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Diagnostics;
using Priority_Queue;

namespace Pathfinding
{
    public class AStar
    {
        Queue<Tile> path;

        public bool IsReady { get; protected set; }
        public bool IsImpossible { get { return (GetLength() == 0); } }

        public bool IsInitialized { get; protected set; }

        public Tile Start { get; protected set; }
        public Tile Goal { get; protected set; }

        int nodesPerCall = 200;
        int nodesThisCall;

        Node startNode;
        Node goalNode;
        Node currentNode;

        HashSet<Node> closedSet;
        SimplePriorityQueue<Node> openSet;
        Dictionary<Node, Node> cameFrom;
        Dictionary<Node, float> g_score;
        Dictionary<Node, float> f_score;

        Stopwatch stopwatch;

        public AStar(World world)
        {
            IsInitialized = false;
            IsReady = false;

            stopwatch = new Stopwatch();
           
            // Węzły, które zostały już sprawdzone
            closedSet = new HashSet<Node>();

            // Węzły do sprawdzenia
            openSet = new SimplePriorityQueue<Node>();

            int size = world.XSize * world.YSize * world.Height;

            // Aby móc odtworzyć ścieżkę
            cameFrom = new Dictionary<Node, Node>(size);

            // g_score - rzeczywista odległość
            g_score = new Dictionary<Node, float>(size);
           
            // f_score - szacowana odległość
            f_score = new Dictionary<Node, float>(size);            
        }

        public void NewPath(TileGraph tileGraph, Tile start, Tile goal)
        {            
            if (IsInitialized == true) Reset();

            Start = start;
            Goal = goal;

            Dictionary<Tile, Node> tileNodeMap = tileGraph.GetCompleteGraph();

            if (tileNodeMap == null)
            {
                UnityEngine.Debug.LogError("Pathfinding bez istniejącego grafu!");
                return;
            }

            startNode = tileNodeMap[start];
            goalNode = tileNodeMap[goal];

            if (startNode == null || goalNode == null)
            {
                UnityEngine.Debug.LogError("Zażądano ścieżki pomiędzy nieistniejącymi polami!");
                return;
            }

            // Najpierw sprawdzamy pole startowe
            openSet.Enqueue(startNode, 0);

            // Pole startowe (i pola już sprawdzone) mają g_score równe zeru
            g_score[startNode] = 0;

            // Szacowanie odległości, tj. po prostu obliczanie dystansu w linii prostej
            f_score[startNode] = HeuristicCostEstimate(startNode, goalNode);

            IsInitialized = true;
        }

        public void Reset()
        {
            stopwatch.Reset();

            Start = null;
            Goal = null;

            startNode = null;
            goalNode = null;
            currentNode = null;

            nodesThisCall = 0;

            closedSet.Clear();
            openSet.Clear();
            cameFrom.Clear();
            g_score.Clear();
            f_score.Clear();
            IsInitialized = false;
            IsReady = false;
        }    

        public void Process()
        {
            if (IsReady == true) return;
            if (IsInitialized == false) return;

            //stopwatch.Start();
            nodesThisCall = 0;

            // Póki w zbiorze pól do sprawdzenia coś jeszcze jest, wykonujemy to             
            while (openSet.Count > 0)
            {               
                // Bierzemy następne pole do sprawdzenia
                currentNode = openSet.Dequeue();

                // Jeśli to cel, rekonstruujemy ścieżkę i przerywamy
                if (currentNode == goalNode)
                {
                    ReconstructPath(cameFrom, currentNode);
                    //stopwatch.Stop();
                    IsReady = true;
                    //UnityEngine.Debug.Log("Znaleziono ścieżkę pomiędzy: " 
                    //    + Start.Position.ToString() + " a " 
                    //    + Goal.Position.ToString() 
                    //    + ". Dystans: " + GetLength()
                    //    + ". Czas wyszukiwania: " + stopwatch.ElapsedMilliseconds + "ms.");
                    return;
                }

                // Odhaczamy to pole jako już sprawdzone
                closedSet.Add(currentNode);

                // Sprawdzamy po kolei możliwe przejścia do sąsiednich pól
                foreach (Edge edge in currentNode.Edges)
                {
                    Node neighbour = edge.Goal;

                    if (closedSet.Contains(neighbour) == true)
                    {
                        // Jeśli już sprawdziliśmy tego sąsiada, pomijamy go
                        continue;
                    }

                    float movementCostToNeighbor = edge.Cost * DistanceBetween(currentNode, neighbour);

                    float tentative_g_score = g_score[currentNode] + movementCostToNeighbor;

                    if (openSet.Contains(neighbour) && tentative_g_score >= g_score[neighbour])
                    {
                        continue;
                    }

                    cameFrom[neighbour] = currentNode;
                    g_score[neighbour] = tentative_g_score;
                    f_score[neighbour] = g_score[neighbour] + HeuristicCostEstimate(neighbour, goalNode);

                    if (openSet.Contains(neighbour) == false)
                    {
                        openSet.Enqueue(neighbour, f_score[neighbour]);
                    }
                    else
                    {
                        openSet.UpdatePriority(neighbour, f_score[neighbour]);
                    }
                }

                nodesThisCall++;
                if (nodesThisCall >= nodesPerCall)
                {
                    //stopwatch.Stop();
                    return;
                }
            }

            // Jeśli skończyły się pola do sprawdzenia i wykonywanie tej metody nie zostało przerwane znalezieniem celu
            IsReady = true;
            //UnityEngine.Debug.Log("Pathfinding: nie można znależć ścieżki do celu!");
        }

        void ReconstructPath(Dictionary<Node, Node> cameFrom, Node currentNode)
        {
            // W tym momencie mamy pole docelowe oraz mapę z każdego pola do jego poprzedniego pola (słownik cameFrom).
            // Dzięki temu możemy zrekonstruować ścieżkę (na razie w odwróconej kolejności).
            Queue<Tile> reversePath = new Queue<Tile>();

            // Zaczynamy od pola docelowego i będziemy wracać do pola startowego
            Tile currentTile = currentNode.Tile;
            reversePath.Enqueue(currentTile);

            // Pętla zakończy się na polu startowym, które nie jest wpisane do słownika
            while (cameFrom.ContainsKey(currentNode))
            {
                currentNode = cameFrom[currentNode];
                currentTile = currentNode.Tile;
                reversePath.Enqueue(currentTile);                
            }

            // W tym momencie mamy gotową ścieżkę
            path = new Queue<Tile>(reversePath.Reverse());
        }

        float DistanceBetween(Node a, Node b)
        {
            // Czy pola sąsiadują wzdłuż osi?
            if (Mathf.Abs(a.X - b.X) + Mathf.Abs(a.Y - b.Y) == 1)
            {
                return 1f;
            }

            //Czy pola sąsiadują po przekątnych?
            if (Mathf.Abs(a.X - b.X) == 1 && Mathf.Abs(a.Y - b.Y) == 1)
            {
                return 1.41421356237f;
            }

            // Gdy pola nie sąsiadują
            return Mathf.Sqrt(
                   Mathf.Pow(a.X - b.X, 2) +
                   Mathf.Pow(a.Y - b.Y, 2)
            );
        }

        float HeuristicCostEstimate(Node a, Node b)
        {
            return Mathf.Sqrt(
                Mathf.Pow(a.X - b.X, 2) + Mathf.Pow(a.Y - b.Y, 2));
        }

        public Tile Dequeue()
        {
            if (path == null) return null;
            else { return path.Dequeue(); }
        }

        public Tile Peek()
        {
            if (path == null) return null;
            else { return path.Peek(); }
        }

        public bool Contains(Tile t)
        {
            if (path == null || t == null) return false;
            else { return path.Contains(t); }
        }

        public int GetLength()
        {
            if (path == null) return 0;
            else { return path.Count; }
        }
    }
}
