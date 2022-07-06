using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// TODO: Mouse controller should just detect mouse input and pass that out
// to whatever state or mode the game is in.
// Needs associated actions or funcs for outside scripts to listen for.

public class BuildModeController : MonoBehaviour
{
    bool buildModeIsObjects = false;
    public Tile.TileType buildTile = Tile.TileType.Floor;
    public string objectIdToBeInstalled;

    // Called by ui
    public void SetMode(int mode)
    {
        switch (mode)
        {
            case 0:
                {
                    buildTile = Tile.TileType.Empty;
                    buildModeIsObjects = false;
                    break;
                }
            case 1:
                {
                    buildTile = Tile.TileType.Floor;
                    buildModeIsObjects = false;
                    break;
                }
            default:
                {
                    buildTile = Tile.TileType.Empty;
                    break;
                }
        }
    }

    public void SetModeBuildObject(string objectName)
    {
        buildModeIsObjects = true;
        objectIdToBeInstalled = objectName;
    }

    // TODO: Allow for removing objects.
    // WorldController.OnInstalledObjectChanges should handle that once 
    // logic written in mouse controller. Placing a null should 
    // be handled as a removal.
    public void SetModeRemoveObject()
    {
        Debug.LogError("Currently not implemented.");
    }

    public void DoBuild(Tile t)
    {
        if (buildModeIsObjects)
        {
            // Instant build furniture.
            //WorldController.Instance.World.PlaceInstalledObject(objectIdToBeInstalled, t);

            if (WorldController.Instance.World.IsFurniturePlacementValid(objectIdToBeInstalled, t) && t.pendingJob == null)
            {
                string furnitureType = objectIdToBeInstalled;
                Job j = new Job(t, furnitureType, 
                    (theJob) =>
                    {
                        WorldController.Instance.World.PlaceInstalledObject(furnitureType,
                        theJob.JobLocationTile);
                        t.pendingJob = null;
                    }, 1f);
                WorldController.Instance.World.jobQueue.Enqueue(j);
                t.pendingJob = j;
                // Cancel case.
                j.RegisterJobCancelCallback((theJob) => { theJob.JobLocationTile.pendingJob = null; });
            }
        }
        else
        {
            t.Type = buildTile;
        }
    }
}