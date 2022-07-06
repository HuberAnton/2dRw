using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class FurnitureSpriteController : MonoBehaviour
{
    //TODO: Change this to a file for easier uniform access.
    public string installedObjectSpriteFolderLocation = "Images/Furniture";

    // Data to it's visual components
  
    Dictionary<Furniture, GameObject> furnitureGameObjectMap;   // Furniture on tile including walls
    Dictionary<string, Sprite> objectIdSpritesMap;              // Stored sprites for prototypes
    World World
    {
        get {return WorldController.Instance.World; }
    }

    // Start is called before the first frame update
    void Start()
    {
        LoadSprites();
        furnitureGameObjectMap = new Dictionary<Furniture, GameObject>();

        World.RegisterInstalledObjectCreated(OnFurnitureCreated);

        foreach(Furniture furn in World.furnitures)
        {
            OnFurnitureCreated(furn);
        }
    }

    // Not too cash money.
    void LoadSprites()
    {
        objectIdSpritesMap = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>(installedObjectSpriteFolderLocation);
        foreach (Sprite s in sprites)
        {
            objectIdSpritesMap[s.name] = s;
        }
    }

    // Visual creation functions called by changes to tiles.
    //TODO: Needs to consider multitile objects and rotation.
    public void OnFurnitureCreated(Furniture obj)
    {
        GameObject furnitureVisual = new GameObject();

        furnitureGameObjectMap.Add(obj, furnitureVisual);

        furnitureVisual.name = obj.ObjectId+"_"+obj.Tile.X+","+obj.Tile.Y;
        furnitureVisual.transform.position = new Vector3(obj.Tile.X, obj.Tile.Y, 0);
        furnitureVisual.transform.SetParent(this.transform, true);

        // TODO: Need to assaign sorting layer somewhere. Should be both for tiles and installed objects.
        // Should create 2 different classes.
        SpriteRenderer render = furnitureVisual.gameObject.AddComponent<SpriteRenderer>();
        render.sprite = GetSpriteForInstalledObject(obj);

        render.sortingLayerName = "InstalledObject";
        obj.RegisterOnChangeCallback(OnInstalledObjectChange);
    }

    // If object already exists on tile to swap sprite.
    // TODO: Should have a case handling object removal.
    void OnInstalledObjectChange(Furniture obj)
    {
        if(!furnitureGameObjectMap.ContainsKey(obj))
        {
            Debug.LogError("Can not change visual " + obj.ObjectId + " as it has no current visual.");
            return;
        }
        GameObject furnitureVisual = furnitureGameObjectMap[obj];
        SpriteRenderer render = furnitureVisual.gameObject.GetComponent<SpriteRenderer>();
        render.sprite = GetSpriteForInstalledObject(obj);
    }

    // Both for single and joined sprites
    // Single name: "example"
    // Joined name: "example_NESW" in that order.
    public Sprite GetSpriteForInstalledObject(Furniture obj)
    {
        // If not linked will return just the furnitures id.
        if(!obj.LinksToNeighbours)
        {
            return objectIdSpritesMap[obj.ObjectId];
        }

        string spriteName = obj.ObjectId + "_";
        Tile t;
        t = World.GetTile(obj.Tile.X, obj.Tile.Y + 1);
        if(t != null && t.Furniture != null && t.Furniture.ObjectId == obj.ObjectId)
        {
            spriteName += "N";
        }
        t = World.GetTile(obj.Tile.X + 1, obj.Tile.Y);
        if (t != null && t.Furniture != null && t.Furniture.ObjectId == obj.ObjectId)
        {
            spriteName += "E";
        }
        t = World.GetTile(obj.Tile.X, obj.Tile.Y - 1);
        if (t != null && t.Furniture != null && t.Furniture.ObjectId == obj.ObjectId)
        {
            spriteName += "S";
        }
        t = World.GetTile(obj.Tile.X - 1, obj.Tile.Y);
        if (t != null && t.Furniture != null && t.Furniture.ObjectId == obj.ObjectId)
        {
            spriteName += "W";
        }

        // Sanity checking.
        if(!objectIdSpritesMap.ContainsKey(spriteName))
        {
            Debug.Log("Installed object sprite " + spriteName + " does not exist. Check spritemap or factory " +
                "for object " + obj.ObjectId+".");
        }
        return objectIdSpritesMap[spriteName];
    }

    public Sprite GetSpriteForInstalledObject(string objectId)
    {
        if (objectIdSpritesMap.ContainsKey(objectId))
        {
            return objectIdSpritesMap[objectId];
        }
        if (objectIdSpritesMap.ContainsKey(objectId + "_"))
        {
            return objectIdSpritesMap[objectId+"_"];
        }
            Debug.LogError(objectId + " does not have a sprite. Check name is correct of both job and object at creation");
        return null;
    }


}