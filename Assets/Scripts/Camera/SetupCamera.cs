using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;

public class SetupCamera : NetworkBehaviour
{
    public override void OnNetworkSpawn()
    {
        if (!IsOwner) return;
        StartCoroutine(SetupCam());
    }

    private System.Collections.IEnumerator SetupCam()
    {
        CinemachineCamera vcam = null;

        while (vcam == null)
        {
            vcam = GameObject.FindAnyObjectByType<CinemachineCamera>();
            yield return null;
        }

        vcam.Follow = transform;
        vcam.LookAt = transform;

        Debug.Log($"[Camera System] Linked to Owner: {OwnerClientId}");
    }
}