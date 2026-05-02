using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;

public class ProjectileLauncher : NetworkBehaviour
{
    [Header("References")]
    [SerializeField] private Transform projectileSpawnPoint;
    [SerializeField] private GameObject serverProjectilePrefab;
    [SerializeField] private GameObject clientProjectilePrefab;
    [SerializeField] private CooldownUI cooldownUI;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip fireSound;

    [Header("Settings")]
    [SerializeField] private float projectileSpeed = 30f;
    [SerializeField] private float fireRate = 0.5f;

    private float lastFireTime;

    private void Update()
    {
        if (!IsOwner) return;

        if (Mouse.current.leftButton.isPressed && Time.time >= lastFireTime + fireRate)
        {
            if (cooldownUI == null || cooldownUI.IsReady())
            {
                HandleFire();
                lastFireTime = Time.time;

                if (cooldownUI != null)
                    cooldownUI.StartCooldown(fireRate);
            }
        }
    }

    private void HandleFire()
    {
        // ?? เล่นเสียงยิง (Local)
        PlayFireSound();

        Vector3 pos = projectileSpawnPoint.position;
        Vector3 dir = projectileSpawnPoint.forward;

        // Visual ฝั่งตัวเอง
        SpawnDummy(pos, dir);

        // ส่งไป Server
        FireServerRpc(pos, dir);
    }

    private void PlayFireSound()
    {
        if (audioSource != null && fireSound != null)
            audioSource.PlayOneShot(fireSound);
    }

    [ServerRpc]
    private void FireServerRpc(Vector3 pos, Vector3 dir)
    {
        // ?? ให้ทุกคนได้ยินเสียงยิง
        PlayFireSoundClientRpc();

        GameObject obj = Instantiate(serverProjectilePrefab, pos, Quaternion.identity);
        obj.transform.forward = dir;

        if (obj.TryGetComponent<NetworkObject>(out var netObj))
            netObj.Spawn();

        // กันยิงโดนตัวเอง
        if (obj.TryGetComponent<DestroySelfOnContact>(out var contactScript))
            contactScript.SetOwner(OwnerClientId);

        // ให้คนอื่นเห็นกระสุน
        SpawnDummyClientRpc(pos, dir);
    }

    [ClientRpc]
    private void PlayFireSoundClientRpc()
    {
        PlayFireSound();
    }

    [ClientRpc]
    private void SpawnDummyClientRpc(Vector3 pos, Vector3 dir)
    {
        if (IsOwner) return;
        SpawnDummy(pos, dir);
    }

    private void SpawnDummy(Vector3 pos, Vector3 dir)
    {
        if (clientProjectilePrefab == null) return;

        GameObject obj = Instantiate(clientProjectilePrefab, pos, Quaternion.identity);
        obj.transform.forward = dir;

        if (obj.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = dir * projectileSpeed;
        }

        Destroy(obj, 3f);
    }
}