using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
// Shorthand
using TileNode = Pathfinding.Node<Tile>;
using Priority_Queue;
using System.Linq;


namespace Pathfinding
{
    public class AStar
    {
        Queue<Tile> path;

        // Generates the path.
        // I'd say this is unideal as a constructor
        // as this implies destruction of old and creation of new.
        public AStar(World world, Tile startTile, Tile endTile)
        {
            if (world.PathGraph == null)
            {
                world.RegenerateNewTileGraph();
            }
            Dictionary<Tile, TileNode> nodes = world.PathGraph.Nodes;
            // Shouldn't hit here but just in case.
            // Check nodes exist
            if (!nodes.ContainsKey(startTile))
            {
                Debug.LogError("Tile " + startTile.X + "," + startTile.Y + " does not have a node in dictionary." +
                    " Check creation of tile graph is correct.");
                return;
            }

            TileNode start = nodes[startTile];
            TileNode end = nodes[endTile];


            if (!nodes.ContainsKey(endTile))
            {
                Debug.LogError("Tile " + endTile.X + "," + endTile.Y + " does not have a node in dictionary." +
                    " Check creation of tile graph is correct.");
            }
            List<TileNode> closedSet = new List<TileNode>();
            SimplePriorityQueue<TileNode> openSet = new SimplePriorityQueue<TileNode>();
            // Value irellivant as only item in queue.
            openSet.Enqueue(start, 0);

            // Nodes already traveled
            Dictionary<TileNode, TileNode> cameFrom = new Dictionary<TileNode, TileNode>();

            Dictionary<TileNode, float> gScore = new Dictionary<TileNode, float>();
            // Assume nodes not investigated are infinite.
            foreach (TileNode n in nodes.Values)
            {
                gScore[n] = Mathf.Infinity;
            }
            // Start node is effortless to travel.
            gScore[start] = 0;

            Dictionary<TileNode, float> fScore = new Dictionary<TileNode, float>();
            // Assume nodes not investigated are infinite.
            foreach (TileNode n in nodes.Values)
            {
                fScore[n] = Mathf.Infinity;
            }
            fScore[start] = HeuristicCostEstimateAsCrowFlys(start, end);

            while (openSet.Count > 0)
            {
                TileNode current = openSet.Dequeue();
                // Found path to end.
                if (current == end)
                {
                    ReconstructPath(cameFrom, end);
                    return;
                }

                // Add to list of searched nodes
                closedSet.Add(current);
                foreach (Edge<Tile> neighbour in current.edges)
                {
                    // Node already searched.
                    if (closedSet.Contains(neighbour.node))
                        continue;

                    float movementCostToNeighbour = neighbour.node.data.movementCost * DistBetween(current, neighbour.node);

                    float gScoreNew = gScore[current] + movementCostToNeighbour;

                    if (openSet.Contains(neighbour.node) && gScoreNew >= gScore[neighbour.node])
                    {
                        continue;
                    }
                    cameFrom[neighbour.node] = current;
                    gScore[neighbour.node] = gScoreNew;
                    fScore[neighbour.node] = gScore[neighbour.node] + HeuristicCostEstimateAsCrowFlys(neighbour.node, end);

                    if (!openSet.Contains(neighbour.node))
                    {
                        openSet.Enqueue(neighbour.node, fScore[neighbour.node]);
                    }
                }
            }
            // Failure to get to goal.
            // No path found.

        }

        // Make path to goal to destination using search.                       
        void ReconstructPath(Dictionary<TileNode, TileNode> cameFrom, TileNode current) // Goal
        {
            Queue<Tile> totalPath = new Queue<Tile>();
            totalPath.Enqueue(current.data);
            while (cameFrom.ContainsKey(current))
            {
                current = cameFrom[current];
                totalPath.Enqueue(current.data);
            }
            path = new Queue<Tile>(totalPath.Reverse());
        }


        // This might be pointless.
        float DistBetween(TileNode a, TileNode b)
        {
            // This will cause issues if an edge has a special path through it. Eg a portal. 
            //return Mathf.Abs(a.data.X - b.data.X) + Mathf.Abs(a.data.Y - b.data.Y) == 1 ? 1 : 1.41421356237f;


            if (Mathf.Abs(a.data.X - b.data.X) + Mathf.Abs(a.data.Y - b.data.Y) == 1)
            {
                return 1f;
            }

            // Diagonal case
            if (Mathf.Abs(a.data.X - b.data.X) == 1 && Mathf.Abs(a.data.Y - b.data.Y) == 1)
            {
                return 1.41421356237f;
            }

            // Should just do this
            return Mathf.Sqrt(
                Mathf.Pow(a.data.X - b.data.X, 2) +
                Mathf.Pow(a.data.Y - b.data.Y, 2));

        }

        // Might change this to an interface implementation as name is shit. 
        float HeuristicCostEstimateAsCrowFlys(TileNode start, TileNode end)
        {
            return Mathf.Sqrt(
                Mathf.Pow(start.data.X - end.data.X, 2) +
                Mathf.Pow(start.data.Y - end.data.Y, 2)
                );
        }

        public Tile Dequeue()
        {
            return path.Dequeue();
        }


        public int Length()
        {
            if (path == null)
                return 0;

            return path.Count;
        }
    }
}