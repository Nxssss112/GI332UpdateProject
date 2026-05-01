using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerIconSetup : NetworkBehaviour
{
    public Image icon;

    public Color selfColor = Color.green;
    public Color enemyColor = Color.red;

    public override void OnNetworkSpawn()
    {
        if (IsOwner)
        {
            icon.color = selfColor;
        }
        else
        {
            icon.color = enemyColor;
        }
    }
}