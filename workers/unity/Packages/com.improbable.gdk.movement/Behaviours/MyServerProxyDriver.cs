using System.Collections.Generic;
using System.Linq;
using Improbable.Gdk.GameObjectRepresentation;
using Improbable.Gdk.Movement;
using Improbable.Worker.CInterop;
using UnityEngine;

public class MyServerProxyDriver : MonoBehaviour
{
    [Require] private ClientMovement.Requirable.Reader clientInput;
    [Require] private ServerMovement.Requirable.Reader server;

    private Dictionary<int, ClientRequest> inputs = new Dictionary<int, ClientRequest>();

    private int workerIndex;

    private void OnEnable()
    {
        server.LatestUpdated += ServerOnLatestUpdated;
        server.AuthorityChanged += ServerOnAuthorityChanged;

        workerIndex = GetComponent<MyServerMovementDriver>().workerIndex;
    }

    private void ServerOnLatestUpdated(ServerResponse response)
    {
        var keys = inputs.Keys.ToArray();
        foreach (var key in keys)
        {
            if (key <= response.Timestamp)
            {
                // Debug.Log($"[Server-{workerIndex}] Proxy removing input {key} < {response.Timestamp}");
                inputs.Remove(key);
            }
        }
    }

    private void ServerOnAuthorityChanged(Authority auth)
    {
        if (auth == Authority.Authoritative || auth == Authority.NotAuthoritative)
        {
            Debug.Log($"[Server-{workerIndex}] Proxy auth changed to {auth}");
            Debug.Log($"[Server-{workerIndex}] {inputs.Count} pending inputs." +
                $"Confirm Frame: {server.Data.Latest.Timestamp}");
            // inputs.Clear();
        }
    }

    public void GetInitialRequests(List<ClientRequest> inputBuffer)
    {
        var confirmFrame = server.Data.Latest.Timestamp;
        Debug.Log($"[Server-{workerIndex}] Current Server data frame: {confirmFrame}");

        var keys = inputs.Keys.OrderBy(i => i);
        foreach (var key in keys)
        {
            if (key <= confirmFrame)
            {
                Debug.Log($"[Server-{workerIndex}] Skipping old frame: {key}");
            }
            else
            {
                Debug.Log($"[Server-{workerIndex}] Adding Unacked input frame {key}");
                inputBuffer.Add(inputs[key]);
            }
        }
    }
}
