using System;
using System.Collections.Generic;
using Improbable.Gdk.Core.Collections;
using Improbable.Gdk.DeploymentManager.Commands;
using UnityEditor;
using AuthTask =
    Improbable.Gdk.DeploymentManager.Commands.WrappedTask<Improbable.Gdk.Tools.RedirectedProcessResult, int>;

namespace Improbable.Gdk.DeploymentManager
{
    internal class TaskManager
    {
        public enum QueueMode
        {
            Normal,
            ForceNext
        }

        public bool IsActive => CurrentTask != null;
        public IWrappedTask CurrentTask { get; private set; }
        public readonly List<IWrappedTask> CompletedTasks = new List<IWrappedTask>();

        private List<Func<IWrappedTask>> queuedTasks = new List<Func<IWrappedTask>>();
        private bool isLocked;
        private Func<IWrappedTask> queuedAuthTask;

        public void ClearResults()
        {
            CompletedTasks.Clear();
        }

        public void Upload(AssemblyConfig config, QueueMode mode = QueueMode.Normal)
        {
            AddTask(mode, () => Assembly.UploadAsync(config));
        }

        public void Launch(string projectName, string assemblyName, BaseDeploymentConfig config,
            QueueMode mode = QueueMode.Normal)
        {
            AddTask(mode, () => Deployment.LaunchAsync(projectName, assemblyName, config));
        }

        public void List(string projectName, QueueMode mode = QueueMode.Normal)
        {
            AddTask(mode, () => Deployment.ListAsync(projectName));
        }

        public void Stop(DeploymentInfo info, QueueMode mode = QueueMode.Normal)
        {
            AddTask(mode, () => Deployment.StopAsync(info));
        }

        public void Auth()
        {
            queuedAuthTask = Authentication.Authenticate;
        }

        public void Update()
        {
            if (!IsActive)
            {
                // Always prefer the auth task over any queued t asks.
                if (queuedAuthTask != null)
                {
                    CurrentTask = queuedAuthTask();
                    queuedAuthTask = null;

                    if (!isLocked)
                    {
                        EditorApplication.LockReloadAssemblies();
                        isLocked = true;
                    }
                }
                else if (queuedTasks.Count > 0)
                {
                    var queuedTask = queuedTasks[0];
                    queuedTasks.RemoveAt(0);

                    CurrentTask = queuedTask();

                    if (!isLocked)
                    {
                        EditorApplication.LockReloadAssemblies();
                        isLocked = true;
                    }
                }
                else if (isLocked)
                {
                    EditorApplication.UnlockReloadAssemblies();
                    isLocked = false;
                }
            }
            else
            {
                if (CurrentTask.IsDone())
                {
                    CompletedTasks.Add(CurrentTask);
                    CurrentTask = null;
                }
            }
        }

        private void AddTask(QueueMode mode, Func<IWrappedTask> closure)
        {
            switch (mode)
            {
                case QueueMode.Normal:
                    queuedTasks.Add(closure);
                    break;
                case QueueMode.ForceNext:
                    queuedTasks.Insert(0, closure);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(mode), mode, null);
            }
        }
    }
}
