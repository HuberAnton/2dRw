using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSpriteController : MonoBehaviour
{
    Dictionary<Character, GameObject> characterGameObjectMap;
    Dictionary<string, Sprite> characterIdSpriteMap;

    public string characterSpriteFolderLocation;

    World World
    {
        get { return WorldController.Instance.World; }
    }

    void Start()
    {
        LoadSprites();
        characterGameObjectMap = new Dictionary<Character, GameObject>();
        World.RegisterCharacterCreated(OnCharacterCreated);

        foreach(Character c in World.characters)
        {
            OnCharacterCreated(c);
        }
        // TODO: Spawn characters elsewhere.
        //Character c = World.CreateCharacter(World.GetTile(World.Width / 2, World.Height / 2));
    }

    // Not too cash money.
    void LoadSprites()
    {
        characterIdSpriteMap = new Dictionary<string, Sprite>();
        Sprite[] sprites = Resources.LoadAll<Sprite>(characterSpriteFolderLocation);
        foreach (Sprite s in sprites)
        {
            characterIdSpriteMap[s.name] = s;
        }
    }


    // Visual creation functions called by changes to tiles.
    //TODO: Needs to consider multitile objects and rotation.
    public void OnCharacterCreated(Character ch)
    {
        GameObject characterVisual = new GameObject();

        characterGameObjectMap.Add(ch, characterVisual);

        characterVisual.name = ch.CharacterId;
        characterVisual.transform.position = new Vector3(ch.X, ch.Y, 0);
        characterVisual.transform.SetParent(this.transform, true);

        // TODO: Need to assaign sorting layer somewhere. Should be both for tiles and installed objects.
        // Should create 2 different classes.
        SpriteRenderer render = characterVisual.gameObject.AddComponent<SpriteRenderer>();
        render.sprite = GetSpriteForCharacter(ch);

        render.sortingLayerName = "Default";
        ch.RegisterOnChangeCallback(OnCharacterChange);
    }


    void OnCharacterChange(Character ch)
    {
        if (!characterGameObjectMap.ContainsKey(ch))
        {
            Debug.LogError("Can not change visual " + ch.CharacterId + " as it has no current visual.");
            return;
        }
        GameObject characterVisual = characterGameObjectMap[ch];
        SpriteRenderer render = characterVisual.gameObject.GetComponent<SpriteRenderer>();
                                                     // TODO: Should not be creating a new vector.
                                                    // Consider moving it the distance
        characterVisual.transform.position = new Vector3(ch.X, ch.Y, characterVisual.transform.position.z);
        render.sprite = GetSpriteForCharacter(ch);
        // TODO: Change sprite depending on direciton moved ect.
    }



    // Name: "example_N".
    public Sprite GetSpriteForCharacter(Character ch)
    {

        // Get facing
        string spriteName = ch.CharacterId;// + "_";
        //Tile t;
        //t = World.GetTile(obj.Tile.X, obj.Tile.Y + 1);
        //if (t != null && t.Furniture != null && t.Furniture.ObjectId == ch.ObjectId)
        //{
        //    spriteName += "N";
        //}
        //t = World.GetTile(obj.Tile.X + 1, obj.Tile.Y);
        //if (t != null && t.Furniture != null && t.Furniture.ObjectId == ch.ObjectId)
        //{
        //    spriteName += "E";
        //}
        //t = World.GetTile(obj.Tile.X, obj.Tile.Y - 1);
        //if (t != null && t.Furniture != null && t.Furniture.ObjectId == ch.ObjectId)
        //{
        //    spriteName += "S";
        //}
        //t = World.GetTile(obj.Tile.X - 1, obj.Tile.Y);
        //if (t != null && t.Furniture != null && t.Furniture.ObjectId == obj.ObjectId)
        //{
        //    spriteName += "W";
        //}

        // Sanity checking.
        if (!characterIdSpriteMap.ContainsKey(spriteName))
        {
            Debug.Log("Installed object sprite " + spriteName + " does not exist. Check spritemap or factory" +
                "for object " + ch.CharacterId + ".");
        }
        return characterIdSpriteMap[spriteName];
    }

    public Sprite GetSpriteForCharacter(string characterId)
    {
        if (characterIdSpriteMap.ContainsKey(characterId))
        {
            return characterIdSpriteMap[characterId];
        }
        if (characterIdSpriteMap.ContainsKey(characterId + "_"))
        {
            return characterIdSpriteMap[characterId + "_"];
        }
        Debug.LogError(characterId + " does not have a sprite. Check name is correct of both job and object at creation");
        return null;
    }
}
