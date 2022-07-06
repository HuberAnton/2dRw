using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Consider inheriting from queue instead overwriting funcitons.
public class JobQueue
{
    Queue<Job> jobQueue;

    Action<Job> cbJobCreated;

    public JobQueue()
    {
        jobQueue = new Queue<Job>();
    }

    public void Enqueue(Job j)
    {
        jobQueue.Enqueue(j);

        if (cbJobCreated != null)
            cbJobCreated(j);

    }

    public Job Dequeue()
    {
        if(jobQueue.Count == 0)
        {
            //Debug.LogError("Job queue empty. Returning Null. " +
            //    "If expecting job check it is added to jobqueue on creation. " +
            //    "If not enjoy your down time.");
            return null;
        }
        return jobQueue.Dequeue();
    }


    public void RegisterJobCreationCallback(Action<Job> cb)
    {
        cbJobCreated += cb;
    }

    public void UnegisterJobCreationCallback(Action<Job> cb)
    {
        cbJobCreated -= cb;
    }
}
