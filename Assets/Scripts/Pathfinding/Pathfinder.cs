using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Pathfinding
{
    public class Pathfinder
    {        
        Dictionary<Character, AStar> currentPaths = new Dictionary<Character, AStar>();
        Dictionary<Character, AStar> newPaths = new Dictionary<Character, AStar>();

        World world;
        TileGraph graph;

        public Pathfinder(World world)
        {
            this.world = world;
        }

        public void Process()
        {
            if (graph == null)
            {
                graph = new TileGraph(world);
            }
            else if (graph.IsReady == false)
            {
                graph.ProcessTiles();
            }
            else 
            {                
               foreach (AStar path in newPaths.Values)
                {
                    // Sprawdzamy, czy wszystkie ścieżki są obliczone            
                    if (path.IsInitialized && path.IsReady == false)
                    {
                        path.Process();
                        return;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
        }

        public void InvalidateGraph(List<Tile> modifiedTiles)
        {
            if (graph == null)
            {
                graph = new TileGraph(world);
            }
            else
            {
                graph.Reset();
            }

            foreach (Character character in newPaths.Keys)
            {
                bool recalculate = false;

                // Optymalizacja: nie każe liczyć nowej ścieżki, 
                // jeśli zmiana dotyczy bliskiego pola i nie blokuje poprzedniej ścieżki
                for (int i = 0; i < modifiedTiles.Count; i++)
                {
                    Tile t = modifiedTiles[i];

                    if (Math.Abs(character.CurrentTile.X - t.X) <= 4
                        && Math.Abs(character.CurrentTile.Y - t.Y) <= 4
                        && newPaths[character].Contains(t) == false)
                    {

                        continue;
                    }
                    else
                    {
                        recalculate = true;
                        break;
                    }
                }
                if (recalculate)
                {
                    character.PathNeedsReplacement = true;
                }  
            }

            newPaths.Clear();
        }

        public AStar GetPath(Character character, Tile start, Tile goal)
        {
            // Jeśli nie było wcześniej tej postaci, to dodajemy ją
             if (newPaths.ContainsKey(character) == false || currentPaths.ContainsKey(character) == false)
            {
                newPaths[character] = new AStar(world);
                currentPaths[character] = new AStar(world);
            }

            // Jeśli stara ścieżka jest właściwa
            if (currentPaths[character].Start == start && currentPaths[character].Goal == goal
                && (currentPaths[character].GetLength() > 0 && currentPaths[character].Peek() == character.CurrentTile)
                && currentPaths[character].IsInitialized == true && currentPaths[character].IsReady)
            {               
                return currentPaths[character];
            }

            // Jeśli nie ma nowej ścieżki
            if (newPaths[character].IsInitialized == false)
            {
                if (graph != null && graph.IsReady)
                {
                    newPaths[character].NewPath(graph, start, goal);
                }
                return null;
            }

            // Jeśli nowa ścieżka jest niewłaściwa
            if (newPaths[character].Start != start || newPaths[character].Goal != goal)
            {
                if (graph != null && graph.IsReady)
                {
                    newPaths[character].NewPath(graph, start, goal);
                }
                return null;
            }

            // Jeśli nowa ścieżka jest właściwa i gotowa
            if (newPaths[character].IsReady)
            {              
                SwitchAndResetOldPath(character);
                return currentPaths[character];              
            }
            else
            {
                return null;
            }            
        }

        void SwitchAndResetOldPath(Character character)
        {
            if (currentPaths.ContainsKey(character) == false ||
                newPaths.ContainsKey(character) == false)
            {
                //Debug.Log("Próbowano wymienić ścieżki dla postaci spoza listy!");
                return;
            }

            AStar oldPath;
            oldPath = currentPaths[character];
            currentPaths[character] = newPaths[character];
            oldPath.Reset();
            newPaths[character] = oldPath;            
        }

        bool CheckPathTiles(Character character, Tile start, Tile goal, bool checkNewPath)
        {
            if (checkNewPath)
            {
                return (newPaths.ContainsKey(character) && newPaths[character] != null
                && newPaths[character].Start == start && newPaths[character].Goal == goal);
            }
            else
            {
                return (currentPaths.ContainsKey(character) && currentPaths[character] != null
                && currentPaths[character].Start == start && currentPaths[character].Goal == goal);
            }            
        }

        public void RemoveCharacter(Character c)
        {
            if (newPaths.ContainsKey(c)) newPaths.Remove(c);
            if (currentPaths.ContainsKey(c)) currentPaths.Remove(c);
        }
    }
}
