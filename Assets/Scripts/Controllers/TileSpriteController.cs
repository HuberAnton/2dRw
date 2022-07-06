using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class TileSpriteController : MonoBehaviour
{
    public Sprite nothingSprite;
    public Sprite floorSprite;

    //TODO: Change this to a file for easier uniform access.
    public string tileSpriteFolderLocation = "Images/Tiles";

    // Data to it's visual components
    Dictionary<Tile, GameObject> tileGameObjectMap;             // Ground on tile

    World World
    {
        get {return WorldController.Instance.World; }
    }

    // Start is called before the first frame update
    void Start()
    {
        //LoadSprites();
        tileGameObjectMap = new Dictionary<Tile, GameObject>();

        for (int x = 0; x < World.Width; x++)
        {
            for (int y = 0; y < World.Height; y++)
            {
                Tile tileData = World.GetTile(x, y);
                GameObject tileVisual = new GameObject();

                tileGameObjectMap.Add(tileData, tileVisual);

                tileVisual.name = "Tile_" + x + "," + y;
                tileVisual.transform.position = new Vector3(tileData.X, tileData.Y, 0);
                tileVisual.transform.SetParent(this.transform, true);

                SpriteRenderer render = tileVisual.gameObject.AddComponent<SpriteRenderer>();
                render.sortingLayerName = "Ground";
                render.sprite = nothingSprite;


                OnTileChanged(tileData);
            }
        }
        // Callbacks
        World.RegisterTileChanged(OnTileChanged);
    }

    // Not too cash money.
    //void LoadSprites()
    //{
    //    objectIdSpritesMap = new Dictionary<string, Sprite>();
    //    Sprite[] sprites = Resources.LoadAll<Sprite>(tileSpriteFolderLocation);
    //    foreach (Sprite s in sprites)
    //    {
    //        objectIdSpritesMap[s.name] = s;
    //    }
    //}

    // Passed to the tiles.
    void OnTileChanged(Tile tileData)
    {
        if(!tileGameObjectMap.ContainsKey(tileData))
        {
            Debug.LogError("Tile data not found in world controller tilegameobjectmap. Check factory class.");
        }

        GameObject tileVisual = tileGameObjectMap[tileData];
        // Should be a switch in this circumstance 
        if (tileData.Type == Tile.TileType.Floor)
        {
            tileVisual.GetComponent<SpriteRenderer>().sprite = floorSprite;
        }
        else if (tileData.Type == Tile.TileType.Empty)
        {
            tileVisual.GetComponent<SpriteRenderer>().sprite = nothingSprite;
        }
        else
        {
            Debug.LogError("OnTileTypeChanged: erro unknown type.");
        }
    }

    public Tile GetTileAtWorldCoordinate(Vector3 coord)
    {
        int x = (int)coord.x;
        int y = (int)coord.y;

        return World.GetTile(x, y);
    }
}