using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class TileGraph
    {

        public Dictionary<Tile, Node<Tile>> Nodes
        {
            get; protected set;
        }

        // Creates a graph out of the world.
        // Need to consider the possibility that I might need another
        // graph for tiles that are unfloored or tiles that are specially
        // traversable
        public TileGraph(World world)
        {
            int w_max = world.Width;
            int h_max = world.Height;

            Nodes = new Dictionary<Tile, Node<Tile>>();

            // Nodes created on tiles that are walkable.

            for (int x = 0; x < w_max; x++)
            {
                for (int y = 0; y < h_max; y++)
                {
                    Tile t = world.GetTile(x, y);

                    // If 0 untravelable
                    //if (t.movementCost > 0)
                    //{
                    // TODO: Consider adding a constructor for node.
                    Node<Tile> n = new Node<Tile>();
                    n.data = t;
                    Nodes.Add(t, n);
                    //}
                }
            }

            // Edges created when tile is walkable.
            // An edge from a tile is movement to that tile only, not returning.
            foreach (Tile t in Nodes.Keys)
            {
                Node<Tile> n = Nodes[t];

                List<Edge<Tile>> edges = new List<Edge<Tile>>();

                Tile[] neighbours = t.GetNeighbours(true);

                for (int i = 0; i < neighbours.Length; i++)
                {
                    if (neighbours[i] != null && neighbours[i].movementCost > 0)
                    {
                        // Diagonal clip rule

                        if (IsClippingCorner(t, neighbours[i]))
                        {
                            // Skip as would clip
                            continue;
                        }

                        Edge<Tile> e = new Edge<Tile>();
                        e.cost = neighbours[i].movementCost;
                        e.node = Nodes[neighbours[i]];

                        edges.Add(e);
                    }
                }
                // Node now has associated edges.
                n.edges = edges.ToArray();
            }
        }
        
        // Stops Edge creation 
        bool IsClippingCorner(Tile curr, Tile neighbour)
        {
            int dX = curr.X - neighbour.X;
            int dY = curr.Y - neighbour.Y;

            // If tiles are diagonal.
            if (Mathf.Abs(dX) + Mathf.Abs(dY) == 2)
            {
                // North South
                if(curr.World.GetTile(curr.X - dX, curr.Y).movementCost == 0)
                    return true;
                // East West
                if (curr.World.GetTile(curr.X, curr.Y - dY).movementCost == 0)
                    return true;
            }
            // Will create edge.
            return false;
        }

    }
}
