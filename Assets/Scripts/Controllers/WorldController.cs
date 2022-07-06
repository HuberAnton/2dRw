using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Xml.Serialization;
using System.IO;


public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; protected set; }
    public World World { get; protected set; }

    // Debug Will be removed.
    static bool loadWorld = false;

    // TODO: Probably not a good idea to use on enable.
    void OnEnable()
    {
        // TODO: Sound controller should be called by something to store sounds in a dicitonary.
        var test = SoundController.Instance;
        if (Instance != null)
        {
            Debug.Log("Too instances of world controller.");
        }
        Instance = this;

        if(loadWorld)
        {
            loadWorld = false;
            CreateWorldFromSaveFile();
        }
        else
        CreateEmptyWorld();
    }

    public void Update()
    {
        // Calls the data to update.
        // Allows to create states within
        // the game that halts the data
        World.Update(Time.deltaTime);
    }

    void CreateEmptyWorld()
    {
        World = new World();

        //TODO: Move all camera related controls to their own class.
        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
    }

    public void NewWorldButton()
    {
        //CreateEmptyWorld();

        // TODO: This is not a good idea. Should clear all its memory.
        SceneManager.LoadScene(0);
    }

    public void SaveWorldButton()
    {
        XmlSerializer seri = new XmlSerializer(typeof(World));
        TextWriter writer = new StringWriter();
        seri.Serialize(writer, World);
        writer.Close();

        //Debug.Log(writer.ToString());

        PlayerPrefs.SetString("SaveGame00",writer.ToString());
    }

    public void LoadWorldButton()
    {
        loadWorld = true;
        SceneManager.LoadScene(0);
    }

    void CreateWorldFromSaveFile()
    {
        string game = PlayerPrefs.GetString("SaveGame00");

        XmlSerializer seri = new XmlSerializer(typeof(World));
        StringReader reader = new StringReader(game);
        World = (World)seri.Deserialize(reader);

        SoundController.Instance.AddCallBacks();

        //TODO: Move all camera related controls to their own class.
        Camera.main.transform.position = new Vector3(World.Width / 2, World.Height / 2, Camera.main.transform.position.z);
    }

    public Tile GetTileAtWorldCoordinate(Vector3 coord)
    {
        int x = (int)coord.x;
        int y = (int)coord.y;

        return World.GetTile(x, y);
    }

    //Debug and to be removed.
    public void CreateTestMap()
    {
        World.CreateTestMap();
        World.RegenerateNewTileGraph();
    }
}