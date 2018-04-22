using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.Threading
{
    /// <summary>
    /// Singleton for running action on the main threads. Unity stuff can be called only from main thread.
    /// </summary>
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
}