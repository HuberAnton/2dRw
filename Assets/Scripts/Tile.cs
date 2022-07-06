using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

public class Tile : IXmlSerializable
{
    public enum TileType 
    {
        Empty, 
        Floor,
        Wall
    };
    TileType _type = TileType.Empty;

    // Can store all the actions for when a tile changes.
    Action<Tile> cbTileTypeChanged;
    
    public TileType Type
    {
        get
        {
            return _type;
        }
        set
        {
            TileType oldType = _type;
            _type = value;
            // Post notification to update visual representation.
            if (cbTileTypeChanged != null && oldType != _type)
                cbTileTypeChanged(this);
        }
    }

    //Position
    int _x;
    int _y;

    public int X
    {
        get
        {
            return _x;
        }

    }

    public int Y
    {
        get
        {
            return _y;
        }

    }

    public World World { get; protected set; }

    public Job pendingJob;

    public Inventory inventory;
    public Furniture Furniture
    {
        get;protected set;
    }

    public float movementCost
    {
        get 
        { 
            if(Type == TileType.Empty)
            {
                return 0;
            }
            if (Furniture == null)
            {
                return 1;
            }

            return 1 * Furniture.movementCost;

        }
    }

    public Tile(World world, int x, int y)
    {
        this.World = world;
        this._x = x;
        this._y = y;
    }

    public bool PlaceObject(Furniture objectInstance)
    {
        //if(objectInstance == null)
        //{
        //    // Uninstall at tile
        //    Furniture = null;
        //    return true;
        //}

        if(Furniture != null)
        {
            Debug.LogError("Object already insatlled at location " + X + "," + Y + ".");
            return false;
        }

        Furniture = objectInstance;
        return true;
    }

    // Setter for aciton.
    public void RegisterTileChangedCallback(Action<Tile> callback)
    {
        cbTileTypeChanged += callback;
    }

    public bool IsNeighbour(Tile t,bool includeCornerTiles = false)
    // 0 = same tile
    // 1 = 1 tile apart
    // 2 = 2 ect
    {                                                                                                   // Diagonal should have 1 on both x and y
        return Mathf.Abs(t.X - this.X) + Mathf.Abs(t.Y - this.Y) == 1 || (includeCornerTiles && Mathf.Abs(t.X - this.X) == 1 && Mathf.Abs(t.Y - this.Y) == 1);
    }

    // Will return nulls if no valid neighour for caller to check.
    // Primarily used for pathfinding graph creation.
    public Tile[] GetNeighbours(bool includeCornerTiles = false)
    {
        Tile[] neighbours;

        // Order N, E, S, W || NE, SE, SW, NW

        if (!includeCornerTiles)
        {
            neighbours = new Tile[4];
        }
        else
        {
            neighbours = new Tile[8];
        }

        // TODO: This needs a tune up something fierce

        Tile n;

        n = World.GetTile(X, Y + 1);
        neighbours[0] = n;
        n = World.GetTile(X+1, Y);
        neighbours[1] = n;
        n = World.GetTile(X, Y - 1);
        neighbours[2] = n;
        n = World.GetTile(X-1, Y );
        neighbours[3] = n;


        if(includeCornerTiles)
        {
            n = World.GetTile(X + 1, Y + 1);
            neighbours[4] = n;
            n = World.GetTile(X + 1, Y - 1);
            neighbours[5] = n;
            n = World.GetTile(X - 1, Y - 1);
            neighbours[6] = n;
            n = World.GetTile(X - 1, Y + 1);
            neighbours[7] = n;
        }

        return neighbours;
    }

    #region Saving and Loading

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        Type = (Tile.TileType)int.Parse(reader.GetAttribute("Type"));
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("X", X.ToString());
        writer.WriteAttributeString("Y", Y.ToString());
        writer.WriteAttributeString("Type", ((int)Type).ToString());
    }

    #endregion


    #region Operator Overloading
    //public static bool operator ==(Tile lhs, Tile rhs)
    //{
    //    return ( ||lhs.X == rhs.X && lhs.Y == rhs.Y);
    //}

    //public static bool operator !=(Tile lhs, Tile rhs)
    //{
    //    if(rhs != null)
    //    return !(lhs == rhs);
    //}
    #endregion
}
