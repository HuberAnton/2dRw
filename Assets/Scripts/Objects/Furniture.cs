using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;


// Placed in a tile.
// May be interacted with.
// May be walked over.
// Only 1 per tile max

public class Furniture : IXmlSerializable
{
    // To be read in from lua files?
    public Dictionary<string, object> furnParameters;
    public Action<Furniture, float> updateActions;

    // Base tile.
    public Tile Tile
    { get; protected set; }

    // Used for graphic.
    public string ObjectId
    {
        get; protected set;
    }

    // Lower means quicker movement.
    // eg 2f = twice as long to move through.
    // Also takes in values from tiles and objects
    // occupying the same space.
    // If 0 is not walkable.
    public float movementCost
    {
        get; protected set;
    }

    // Installed object dimensions.
    int width;
    int height;

    // For multi-tile objects.
    public bool LinksToNeighbours
    {
        get; protected set;
    }

    Action<Furniture> cbOnChanged;

    Func<Tile, bool> funcPositionValidation; 

    // TODO: Implement large objects.
    // TODO: Implement object rotation.

    protected Furniture()
    {

    }

    public Furniture(Furniture other)
    {
        this.movementCost = other.movementCost;
        this.ObjectId = other.ObjectId;
        this.width = other.width;
        this.height = other.height;
        this.LinksToNeighbours = other.LinksToNeighbours;

        this.furnParameters = new Dictionary<string, object>(other.furnParameters);
        if(other.updateActions !=null)
            this.updateActions = (Action<Furniture,float>)other.updateActions.Clone();
    }

    // Should be called by the factory at load.
    static public Furniture CreatePrototype(string objectId, float movementCost = 1f, int width = 1, int height = 1, bool linksToNeighbours = false)
    {
        Furniture obj = new Furniture();
        obj.movementCost = movementCost;
        obj.ObjectId = objectId;
        obj.width = width;
        obj.height = height;
        obj.LinksToNeighbours = linksToNeighbours;

        obj.funcPositionValidation += obj.__IsValidPosition;
        obj.furnParameters = new Dictionary<string, object>();
        return obj;
    }

    // Placement in world.
    static public Furniture PlaceInstance(Furniture proto, Tile tile)
    {
        if(!proto.funcPositionValidation(tile))
        {
            Debug.LogError("Can not place object at position " +tile.X+","+tile.Y);
            return null;
        }

        Furniture obj = new Furniture(proto);


        // Redundant as above check occurs
        if (!tile.PlaceObject(obj))
        {
            // Failed to place object in tile.
            return null;
        }
        obj.Tile = tile;

        // Data has already changed so telling the 
        // tiles around this tile to update will work.
        if(obj.LinksToNeighbours)
        {
            Tile t;
            World wrld = obj.Tile.World;

            // Needs to be condensed.
            t = wrld.GetTile(obj.Tile.X, obj.Tile.Y + 1);
            if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.ObjectId == obj.ObjectId)
            {
                t.Furniture.cbOnChanged(t.Furniture);
            }
            t = wrld.GetTile(obj.Tile.X + 1, obj.Tile.Y);
            if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.ObjectId == obj.ObjectId)
            {
                t.Furniture.cbOnChanged(t.Furniture);
            }
            t = wrld.GetTile(obj.Tile.X, obj.Tile.Y - 1);
            if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.ObjectId == obj.ObjectId)
            {
                t.Furniture.cbOnChanged(t.Furniture);
            }
            t = wrld.GetTile(obj.Tile.X - 1, obj.Tile.Y);
            if (t != null && t.Furniture != null && t.Furniture.cbOnChanged != null && t.Furniture.ObjectId == obj.ObjectId)
            {
                t.Furniture.cbOnChanged(t.Furniture);
            }
        }

        return obj;
    }

    public void RegisterOnChangeCallback(Action<Furniture> cbFunction)
    {
        cbOnChanged += cbFunction;
    }

    public void UnregisterOnChangeCallback(Action<Furniture> cbFunction)
    {
        cbOnChanged -= cbFunction;
    }
    
    bool __IsValidPosition(Tile tile)
    {
        // Check tile is a placeable type.
        if(tile.Type != Tile.TileType.Floor)
        {
            return false;
        }

        // Check tile does not contain an installed object already.
        if(tile.Furniture != null)
        {
            return false;
        }

        return true;
    }

    public bool IsValidPosition(Tile t)
    {
        return funcPositionValidation(t);
    }


    #region Saving and Loading

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        movementCost = float.Parse(reader.GetAttribute("movementCost"));
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("X", Tile.X.ToString());
        writer.WriteAttributeString("Y", Tile.Y.ToString());
        writer.WriteAttributeString("ObjectId", ObjectId);
        writer.WriteAttributeString("movementCost", movementCost.ToString());
    }
    #endregion
}