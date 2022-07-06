using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JobSpriteController : MonoBehaviour
{
    public Dictionary<Job, Sprite> jobSpriteController;

    // TODO: Need to remove dependancy.
    FurnitureSpriteController fsc;

    Dictionary<Job, GameObject> JobVisualMap;

    // Start is called before the first frame update
    void Start()
    {
        jobSpriteController = new Dictionary<Job, Sprite>();
        
        fsc = GameObject.FindObjectOfType<FurnitureSpriteController>();

        JobVisualMap = new Dictionary<Job, GameObject>();
        WorldController.Instance.World.jobQueue.RegisterJobCreationCallback(OnJobCreated);
    }

    void OnJobCreated(Job j)
    {
        Tile tileData = j.JobLocationTile;

        if(JobVisualMap.ContainsKey(j))
        {
            Debug.LogError("On job created for a job that already exists. Need a proper cancel.");
            //JobVisualMap.Remove(j);
            return;
        }
        GameObject tileJobVisual = new GameObject();
        JobVisualMap.Add(j, tileJobVisual);

        tileJobVisual.name = "Job_" + tileData.X + "," + tileData.Y;
        tileJobVisual.transform.position = new Vector3(tileData.X, tileData.Y, 0);
        tileJobVisual.transform.SetParent(this.transform, true);

        SpriteRenderer render = tileJobVisual.gameObject.AddComponent<SpriteRenderer>();
        render.sortingLayerName = "InstalledObject";
        render.sprite = fsc.GetSpriteForInstalledObject(j.JobObjectType);

        render.color = new Color(render.color.r, 0,0, 0.50f);
        j.RegisterJobCompleteCallback(OnJobEnded);
        j.RegisterJobCancelCallback(OnJobEnded);
    }

    void OnJobEnded(Job j)
    {
        // Remove sprite

        if (!JobVisualMap.ContainsKey(j))
        {
            Debug.Log("Job building " + j.JobObjectType + " at location " + j.JobLocationTile.X + "," + j.JobLocationTile.Y +
                " has completed but there was no associated sprite to remove. Check if being correctly created or if another" +
                " job is removing sprite.");
            return;
        }
        GameObject jobVisual = JobVisualMap[j];


        j.UnRegisterJobCompleteCallback(OnJobEnded);
        j.UnregistercbJobCancelCallback(OnJobEnded);


        // TODO: Allow for pooling of visual sprites instead of destruction.
        Destroy(jobVisual);
    }
}
