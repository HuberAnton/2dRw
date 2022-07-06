using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Xml;
public class Character : IXmlSerializable
{    public string CharacterId
    {
        get; protected set;
    }


// Position - Neccessary as pure data class.
public float X
    {
        // Placeholder.
        get
        {
            return Mathf.Lerp(Tile.X, nextTile.X, movementPercentage);
        }
    }
    public float Y
    {
        // Placeholder.
        get
        {
            return Mathf.Lerp(Tile.Y, nextTile.Y, movementPercentage);
        }
    }

    // Movement
    public Tile Tile
    {
        get; protected set;
    }

    public Tile DestinationTile
    {
        get; protected set;
    }

    public Tile nextTile;

    public Pathfinding.AStar path;

    // Unsure if this is a good idea.
    // Mostly an internal value.
    float movementPercentage = 0f;

    // Not sure what this speed is exactley.
    // Should be tiles per second but it is slightly slower.
    float speed = 2f;

    Action<Character> cbOnChanged;

    Job myJob;


    public Character(string characterId, Tile tile)
    {
        this.CharacterId = characterId;
        this.Tile = this.DestinationTile = this.nextTile = tile;
    }

    // Update controlled by worldcontroller
    public void Update(float deltaTime)
    {
        // If false doing job.
        DoJob(deltaTime);
        DoMovement(deltaTime);

        if (cbOnChanged != null)
            cbOnChanged(this);
    }

    void DoJob(float deltaTime)
    {
        // If no current job look for one.
        if (myJob == null)
        {
            // Check queue
            myJob = Tile.World.jobQueue.Dequeue();

            if (myJob != null)
            {
                // TODO: Need to check if job is reachable.


                myJob.RegisterJobCancelCallback(OnJobEnded);
                myJob.RegisterJobCompleteCallback(OnJobEnded);
                // Move towards job.
                DestinationTile = myJob.JobLocationTile;
            }
        }

        // TODO: Make character states
        // This is getting messy with multiple behavious.
        if (Tile == DestinationTile)
        {
            if (myJob != null)
            {
                myJob.DoWork(deltaTime);
            }
        }
    }

    
    public void AbandonJob()
    {
        nextTile = DestinationTile = Tile;
        path = null;
        // TODO: Should call cancel on job which should handle this.
        Tile.World.jobQueue.Enqueue(myJob);
        myJob = null;
    }

    void DoMovement(float deltaTime)
    {
        
        if(Tile == DestinationTile)
        {
            path = null;
            return;
        }

        if(nextTile == null || nextTile == Tile)
        {
            if(path == null || path.Length() == 0)
            {
                path = new Pathfinding.AStar(Tile.World, Tile, DestinationTile);
                if(path.Length() == 0)
                {
                    Debug.LogError("Returned no path to destination. Cancelling job.");
                    //TODO: Job needs to go back on queue if incomplete and cancelled.
                    // Consider having a progress till complete for the job.
                    AbandonJob();
                    return;
                }
            }

            // Get next tile in path system
            nextTile = path.Dequeue();
            if (nextTile == Tile)
                Debug.LogError("Next tile is current tile.");

        }


        // Total distance
        float distToTravel = Mathf.Sqrt(Mathf.Pow(Tile.X - nextTile.X, 2) +
            Mathf.Pow(Tile.Y - nextTile.Y, 2));
        // How much of that can be traveled this frame
        float distThisFrame = speed * deltaTime;
        // Overall how much is this to the destination.
        float percThisFrame = distThisFrame / distToTravel;
        // Add to current distance
        movementPercentage += percThisFrame;

        if (movementPercentage >= 1)
        {
            //TODO: Get the next destination from algorithm
            // It should be a neighbour.

            Tile = nextTile;
            movementPercentage = 0;
        }
    }

    public void RegisterOnChangeCallback(Action<Character> cbFunction)
    {
        cbOnChanged += cbFunction;
    }

    public void UnregisterOnChangeCallback(Action<Character> cbFunction)
    {
        cbOnChanged -= cbFunction;
    }

    public void SetDestination(Tile tile)

    {
        if (!Tile.IsNeighbour(tile))
        {
            Debug.LogError("Character is attempting to set destination that is either itself or greater than a 1 tile range.");
        }
        DestinationTile = tile;
    }

    void OnJobEnded(Job j)
    {
        if(j != myJob)
        {
            Debug.LogError("Job eneded that wasn't characters job. Check character has unregistered from job correctly.");
            return;
        }

        myJob = null;
        
        // Job should go back to queue if not completed.

        // If job completed should be added back to object pool of jobs.

    }

    public XmlSchema GetSchema()
    {
        return null;
    }

    public void ReadXml(XmlReader reader)
    {
        // No read as of yet.
    }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("X", Tile.X.ToString());
        writer.WriteAttributeString("Y", Tile.Y.ToString());
        //writer.WriteAttributeString("CharacterId", CharacterId);
    }
}
