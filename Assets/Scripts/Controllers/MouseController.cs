using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Mouse controller should just detect mouse input and pass that out
// to whatever state or mode the game is in.
// Needs associated actions or funcs for outside scripts to listen for.

public class MouseController : MonoBehaviour
{
    public Camera camera;

    Vector3 lastPos;
    Vector3 currentFramePos;
    Vector3 dragStartPos;

    public GameObject testCursorPrefab;
    public List<Poolable> activeMarkers = new List<Poolable>();

    // TODO: RemoveDependecy
    BuildModeController bmc;


    private void Start()
    {
        GameObjectPoolController.AddEntry(testCursorPrefab.name, testCursorPrefab, 50, 1000);
        bmc = GameObject.FindObjectOfType<BuildModeController>();
    }

    // Update is called once per frame
    void Update()
    {
        currentFramePos = camera.ScreenToWorldPoint(Input.mousePosition);
        currentFramePos.z = 0;

        UpdateDragging();
        UpdateCameraMovement();

        // sets to 0. For reference https://answers.unity.com/questions/1460269/orthographic-camera-with-rotation-and-screentoworl.html
        lastPos = /*currentFramePos;*/camera.ScreenToWorldPoint(Input.mousePosition);
        lastPos.z = 0;
    }


    void UpdateDragging()
    {
        if (EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        // Start drag - Click
        if (Input.GetMouseButtonDown(0))
        {
            dragStartPos = currentFramePos;
        }

        int startX = (int)dragStartPos.x;
        int endX = (int)currentFramePos.x;

        if (endX < startX)
        {
            int temp = endX;
            endX = startX;
            startX = temp;
        }

        int startY = (int)dragStartPos.y;
        int endY = (int)currentFramePos.y;

        if (endY < startY)
        {
            int temp = endY;
            endY = startY;
            startY = temp;
        }

        // No point updating held conditions if same pos.
        if (currentFramePos != lastPos)
        {
            ClearMarkers();
            // Hold
            if (Input.GetMouseButton(0))
            {
                for (int x = startX; x <= endX; ++x)
                {
                    for (int y = startY; y <= endY; y++)
                    {
                        Tile t = WorldController.Instance.World.GetTile(x, y);
                        if (t != null)
                        {
                            var go = GameObjectPoolController.Dequeue(testCursorPrefab.name);
                            go.gameObject.SetActive(true);
                            //go.GetComponent<SpriteRenderer>().sprite; Just needs to get the current sprite from bmc.
                            go.transform.position = new Vector3(x, y, 0); // = (GameObject)Instantiate(testCursorPrefab, new Vector3(x, y, 0), Quaternion.identity);
                            go.transform.rotation = Quaternion.identity;
                            activeMarkers.Add(go);
                        }
                    }
                }
            }
        }

        // End drag - Release
        if (Input.GetMouseButtonUp(0))
        {
            for (int x = startX; x <= endX; ++x)
            {
                for (int y = startY; y <= endY; y++)
                {
                    Tile t = WorldController.Instance.World.GetTile(x, y);
                    if (t != null)
                    {
                        bmc.DoBuild(t);
                    }
                }
            }
            ClearMarkers();
        }
    }






    void UpdateCameraMovement()
    {
        // Camera movement
        if (Input.GetMouseButton(2) || Input.GetMouseButton(1))
        {
            Vector3 diff = lastPos - currentFramePos;
            camera.transform.Translate(diff);
        }
        //else
        //{
        // Remember that with ortho this will not work. You need a z position which mouse position
        // sets to 0. For reference https://answers.unity.com/questions/1460269/orthographic-camera-with-rotation-and-screentoworl.html
        //lastPos = /*currentFramePos;*/camera.ScreenToWorldPoint(Input.mousePosition);
        //lastPos.z = 0;
        //}

        // Zoom
        camera.orthographicSize -= camera.orthographicSize * Input.GetAxis("Mouse ScrollWheel") * 0.7f;

        camera.orthographicSize = Mathf.Clamp(camera.orthographicSize, 5f, 15f);
    }

    void ClearMarkers()
    {
        while (activeMarkers.Count > 0)
        {
            for (int i = activeMarkers.Count - 1; i >= 0; --i)
            {
                GameObjectPoolController.Enqueue(activeMarkers[i]);
            }
            activeMarkers.Clear();
        }
    }
}