using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;

public class World : IXmlSerializable
{
    Tile[,] tiles;
    public List<Character> characters;
    // Temp
    public List<Furniture> furnitures;

    public Pathfinding.TileGraph PathGraph
    {
        get; protected set;
    }


    Dictionary<string, Furniture> installedObjectPrototypes;

    Action<Furniture> cbInstalledObjectCreated;
    Action<Character> cbCharacterCreated;
    Action<Tile> cbTileChanged;

    //TODO: Change to seperate class for customized behaviour
    public JobQueue jobQueue;

    public int Width
    {
        get { return _width; }
    }
    int _width;

    public int Height
    {
        get { return _height; }
    }
    int _height;

    
    public World(int width = 100, int height = 100)
    {
        SetupWorld(width, height);

        Character c = CreateCharacter(GetTile(Width / 2, Height / 2));
    }

    public Character CreateCharacter(Tile tile)
    {
        Character c = new Character("p1_front", tile);
        characters.Add(c);

        if (cbCharacterCreated != null)
            cbCharacterCreated(c);
        return c;
    }

    // TODO: 
    // Should cycle through all info in an xml to create prototypes instead of hard coding.
    void CreateObjectProtoTypes()
    {
        // Instantiate prototypes.
        installedObjectPrototypes = new Dictionary<string, Furniture>();
        Furniture wallPrototype = Furniture.CreatePrototype("Wall", 0, 1, 1, true);
        installedObjectPrototypes.Add(wallPrototype.ObjectId, wallPrototype);
        Furniture doorPrototype = Furniture.CreatePrototype("Door", 1, 1, 1, true);
        installedObjectPrototypes.Add(doorPrototype.ObjectId, doorPrototype);

        installedObjectPrototypes["Door"].updateActions += Furniture_Actions.Door_UpdateAction;
        installedObjectPrototypes["Door"].furnParameters["openness"] = 0;
    }
    
    // If x or y out of range returns null.
    public Tile GetTile(int x, int y)
    {
        if(x >= Width || x < 0 || y >= Height || y < 0 )
        {
            //Debug.LogError("Tile (" + x + "," + y + ") is out of range.");
            return null;
        }
        return tiles[x, y];
    }

    public void RandomizeTile()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _width; y++)
            {
                if (UnityEngine.Random.Range(0, 2) == 0)
                {
                    tiles[x, y].Type = Tile.TileType.Empty;
                }
                else
                {
                    tiles[x, y].Type = Tile.TileType.Floor;
                }
            }
        }
    }

    public Furniture PlaceInstalledObject(string objectType, Tile t)
    {
        if (!installedObjectPrototypes.ContainsKey(objectType))
        {
            Debug.LogError("Prototype for object " + objectType + " not found. Check if init at load.");
            return null;
        }

        Furniture obj = Furniture.PlaceInstance(installedObjectPrototypes[objectType], t);
        if (obj == null)
        {
            // Do not place an object at location as something already there.
            return null;
        }

        furnitures.Add(obj);

        if (cbInstalledObjectCreated != null)
        {
            cbInstalledObjectCreated(obj);
            RegenerateNewTileGraph();
        }
        return obj;
    }

    // May be unneeded.
    public Furniture GetFurniturePrototype(string objectId)
    {
        if (!installedObjectPrototypes.ContainsKey(objectId))
        {
            Debug.LogError(objectId + " does not have a prototype. Check name is correct of both job and object at creation");
            return null;
        }
        return installedObjectPrototypes[objectId];
    }

    public void OnTileChanged(Tile tile)
    {
        if (cbTileChanged == null)
            return;
        cbTileChanged(tile);
    }

    public bool IsFurniturePlacementValid(string objectId, Tile t)
    {
        return installedObjectPrototypes[objectId].IsValidPosition(t);
    }

    public void Update(float deltaTime)
    {
        // TODO: Foreach is a no.
        foreach(Character c in characters)
        {
            c.Update(deltaTime);
        }

    }



    // Callbacks assaigment
    // May beed a better soloution as more cb will be added.
    public void RegisterInstalledObjectCreated(Action<Furniture> cbFunc)
    {
        cbInstalledObjectCreated += cbFunc;
    }

    public void UnregisterInstalledObjectCreated(Action<Furniture> cbFunc)
    {
        cbInstalledObjectCreated -= cbFunc;
    }  
    public void RegisterTileChanged(Action<Tile> cbFunc)
    {
        cbTileChanged += cbFunc;
    }

    public void UnregisterTileChanged(Action<Tile> cbFunc)
    {
        cbTileChanged -= cbFunc;
    }

    public void RegisterCharacterCreated(Action<Character> cbfunc)
    {
        cbCharacterCreated += cbfunc;
    }

    public void UnregisterCharacterCreated(Action<Character> cbfunc)
    {
        cbCharacterCreated -= cbfunc;
    }



    // For debugging
    public void CreateTestMap()
    {
        int l = Width / 2 - 5;
        int b = Height / 2 - 5;

        for(int x = l-5; x < l + 15; ++x)
        {
            for(int y = b - 5; y < b + 15; ++y)
            {
                tiles[x, y].Type = Tile.TileType.Floor;

                // Wall placing
                if (x == l || x == (l + 9) || y == b || y == (b + 9))
                {
                    if(x != (l + 9) && y != (b + 4))
                    {
                        PlaceInstalledObject("Wall", tiles[x, y]);
                    }
                }
            }
        }
    }

    public void RegenerateNewTileGraph()
    {
        PathGraph = new Pathfinding.TileGraph(this);
    }

    void SetupWorld(int width = 100, int height = 100)
    {
        jobQueue = new JobQueue();

        this._width = width;
        this._height = height;

        tiles = new Tile[width, height];

        // Gameworld
        for (int x = 0; x < width; ++x)
        {
            for (int y = 0; y < height; ++y)
            {
                tiles[x, y] = new Tile(this, x, y);
                tiles[x, y].RegisterTileChangedCallback(OnTileChanged);
            }
        }
        CreateObjectProtoTypes();

        characters = new List<Character>();
        furnitures = new List<Furniture>();
    }


    //***********************************
    // Saving and loading.
    // To change
    //***********************************

    public World() //: this(100, 100)
    {
        SetupWorld();
        Character c = CreateCharacter(GetTile(Width / 2, Height / 2));
    }
    public XmlSchema GetSchema()
    {
        return null;
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("Width", Width.ToString());
        writer.WriteAttributeString("Height", Height.ToString());

        writer.WriteStartElement("Tiles");

        for (int x = 0; x < Width; ++x)
            for (int y = 0; y < Height; ++y)
            {
                writer.WriteStartElement("Tile");
                tiles[x, y].WriteXml(writer);
                writer.WriteEndElement();
            }
        
        writer.WriteEndElement();

        writer.WriteStartElement("Furnitures");
        {
            foreach (Furniture furn in furnitures)
            {
                writer.WriteStartElement("Furniture");
                furn.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        writer.WriteEndElement();

        writer.WriteStartElement("Characters");
        {
            foreach (Character chs in characters)
            {
                writer.WriteStartElement("Character");
                chs.WriteXml(writer);
                writer.WriteEndElement();
            }
        }
        writer.WriteEndElement();

    }

    public void ReadXml(XmlReader reader)
    {
        _width = int.Parse(reader.GetAttribute("Width"));
        _height = int.Parse(reader.GetAttribute("Height"));

        // Creates all tiles in dimension.
        SetupWorld(_width, _height);

        while(reader.Read())
        {
            switch(reader.Name)
            {
                case "Tiles":
                    ReadXml_Tiles(reader);
                    break;
                case "Furnitures":
                    ReadXml_Furnitures(reader);
                    break;
                case "Characters":
                    ReadXml_Characters(reader);
                    break;

            }
        }
    }
    void ReadXml_Tiles(XmlReader reader)
    {
        while(reader.Read())
        {
            if (reader.Name != "Tile")
                return;

            // Tiles to access.
            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));

            tiles[x, y].ReadXml(reader);
        }
    }
    void ReadXml_Furnitures(XmlReader reader)
    {
        while(reader.Read())
        {
            if (reader.Name != "Furniture")
                return;

            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));

            // Places object at correct location
            Furniture furn = PlaceInstalledObject(reader.GetAttribute("ObjectId"), tiles[x,y]);
            // Read out any other information that varies from the prototype(damage, special cases)
            furn.ReadXml(reader);
        }
    }
    void ReadXml_Characters(XmlReader reader)
    {
        while (reader.Read())
        {
            if (reader.Name != "Character")
                return;

            int x = int.Parse(reader.GetAttribute("X"));
            int y = int.Parse(reader.GetAttribute("Y"));

            Character ch = CreateCharacter(GetTile(x, y));
            ch.ReadXml(reader);
        }

    }
}