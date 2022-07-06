using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Edge<T>
    {
        // Cost to leave tile.
        public float cost;
        // Node connector
        public Node<T> node;
    }

}