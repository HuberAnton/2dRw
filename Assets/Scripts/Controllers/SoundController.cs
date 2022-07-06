using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    static public SoundController Instance
    {
        get
        {
            if (instance == null)
                CreateSharedInstance();
            return instance;
        }
    }

    static SoundController instance;

    float overlap = 0;
    float delayTime = 1f;

    private void Start()
    {
        //WorldController.Instance.World.RegisterInstalledObjectCreated(OnFurnitureCreated);
        //WorldController.Instance.World.RegisterTileChanged(OnTileTypeChanged);



    }

    //private void OnEnable()
    //{
    //    WorldController.Instance.World.RegisterInstalledObjectCreated(OnFurnitureCreated);
    //    WorldController.Instance.World.RegisterTileChanged(OnTileTypeChanged);
    //}

    public void AddCallBacks()
    {
        WorldController.Instance.World.RegisterInstalledObjectCreated(OnFurnitureCreated);
        WorldController.Instance.World.RegisterTileChanged(OnTileTypeChanged);
    }

    private void Update()
    {
        // TODO: Need to have a better implementation of preventing audio overlap.
        // Really doesn't need to constantly run.
        if(overlap > 0)
            overlap -= Time.deltaTime;
    }

    //TODO: These should get information out of the object to get sound.

    void OnTileTypeChanged(Tile tileData)
    {
        if (overlap > 0.1f)
        {
            return;
        }
        AudioClip ac = Resources.Load<AudioClip>("Sounds/Floor_OnCreated");
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        overlap = ac.length - (ac.length * 0.1f);
    }

    void OnFurnitureCreated(Furniture obj)
    {
        if (overlap > 0.1f)
        {
            return;
        }
        AudioClip ac = Resources.Load<AudioClip>("Sounds/Wall_OnCreated");
        AudioSource.PlayClipAtPoint(ac, Camera.main.transform.position);
        overlap = ac.length - (ac.length * 0.1f);
    }

    static void CreateSharedInstance()
    {
        GameObject obj = new GameObject("SoundConroller");
        DontDestroyOnLoad(obj);
        obj.AddComponent<SoundController>();
        instance = obj.GetComponent<SoundController>();
    }
}
