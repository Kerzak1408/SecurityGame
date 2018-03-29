using System;
using System.Collections.Generic;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
    private static readonly Queue<Action> taskQueue = new Queue<Action>();

    public static TaskManager Instance { get; private set; }

    public void RunOnMainThread(Action action)
    {
        lock (taskQueue)
        {
            taskQueue.Enqueue(action);
        }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        lock (taskQueue)
        {
            while (taskQueue.Count > 0)
            {
                taskQueue.Dequeue().Invoke();
            }
        }
    }

}