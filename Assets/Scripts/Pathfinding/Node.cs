using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pathfinding
{
    public class Node<T>
    {
        // Could be x,y  instead
        public T data;
        // Only outward edges.
        public Edge<T>[] edges;
    }
}