using System.Collections.Generic;
using UnityEngine;

public class KillNotificationProvider : MonoBehaviour
{
    public GameObject KillNotificationPrefab;


    private readonly Queue<KillNotification> notificationQueue = new Queue<KillNotification>();

    public int MaxActiveNotifications = 3;

    public void AddKill(string name)
    {
        if (notificationQueue.Count == MaxActiveNotifications)
        {
            notificationQueue.Dequeue().Despawn();
        }

        foreach (var notification in notificationQueue)
        {
            notification.Relegate();
        }

        // Would be better pooled but this is probably OK
        var killFeedToken = Instantiate(KillNotificationPrefab).GetComponent<KillNotification>();
        killFeedToken.SetName(name);
        killFeedToken.transform.SetParent(transform, false);
        killFeedToken.OnDespawn += OnNotificationDespawned;
        notificationQueue.Enqueue(killFeedToken);
    }

    private void OnNotificationDespawned()
    {
        notificationQueue.Dequeue();
    }
}
