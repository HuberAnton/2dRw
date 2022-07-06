using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Job
{
    public Tile JobLocationTile
    {
        get; protected set;
    }
    float jobTime;

    float jobCompletedPercentage = 0;

    Action<Job> cbJobComplete;
    Action<Job> cbJobCancel;
    Action<Job> cbJobCreated;

    // Not good. Too specific.
    public string JobObjectType
    {
         get; protected set;
    }

    public Job(Tile location, string jobObjectType, Action<Job> cbJobComplete, float jobTime = 1f)
    {
        this.JobLocationTile = location;
        this.JobObjectType = jobObjectType;
        this.cbJobComplete = cbJobComplete;
        this.jobTime = jobTime;
    }

    public void RegisterJobCompleteCallback(Action<Job> cb)
    {
        this.cbJobComplete += cb;
    }
    public void UnRegisterJobCompleteCallback(Action<Job> cb)
    {
        this.cbJobComplete -= cb;
    }

    public void RegisterJobCancelCallback(Action<Job> cb)
    {
        this.cbJobCancel += cb;
    }

    public void UnregistercbJobCancelCallback(Action<Job> cb)
    {
        this.cbJobCancel -= cb;
    }

    public void RegisterJobCreatedCallback(Action<Job> cb)
    {
        this.cbJobCreated += cb;
    }

    public void UnregisterJobCreatedCallback(Action<Job> cb)
    {
        this.cbJobCreated -= cb;
    }

    public void DoWork(float workTime)
    {
        jobTime -= workTime;

        if(jobTime <= 0)
        {
            if(cbJobComplete != null)
                cbJobComplete(this);
        }
    }

    public void CancelJob()
    {
        if (cbJobCancel != null)
            cbJobCancel(this);
    }
}
