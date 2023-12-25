using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class AsyncTaskChainProcessingHandler
{
    private Queue<Func<Task>> taskQueue = new Queue<Func<Task>>();
    private bool isProcessing = false;

    public void EnqueueTask(Func<Task> taskFunc)
    {
        taskQueue.Enqueue(taskFunc);
        if (!isProcessing)
        {
            ProcessQueue();
        }
    }
    
    public void EnqueueTasks(params Func<Task>[] taskFuncs)
    {
        foreach (var taskFunc in taskFuncs)
        {
            taskQueue.Enqueue(taskFunc);
        }
        if (!isProcessing)
        {
            ProcessQueue();
        }
    }

    private async void ProcessQueue()
    {
        isProcessing = true;

        while (taskQueue.Count > 0)
        {
            Func<Task> nextTaskFunc = taskQueue.Dequeue();
            try
            {
                Task nextTask = nextTaskFunc.Invoke();
                await nextTask;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Async task failed: {ex.Message}");
                // 可以在这里添加额外的错误处理逻辑
            }
        }

        if (taskQueue.Count > 0)
        {
            ProcessQueue();
        }
        else
        {
            isProcessing = false;
        }
    }
}