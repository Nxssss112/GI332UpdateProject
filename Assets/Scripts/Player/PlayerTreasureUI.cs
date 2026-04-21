using UnityEngine;
using TMPro;

public class PlayerTreasureUI : MonoBehaviour
{
    public TextMeshProUGUI text;
    private TreasureWallet myWallet;

    void Start()
    {
        FindMyPlayer();
    }

    void FindMyPlayer()
    {
        foreach (var player in FindObjectsOfType<TreasureWallet>())
        {
            if (player.IsOwner)
            {
                myWallet = player;
                break;
            }
        }
    }

    void Update()
    {
        if (myWallet == null)
        {
            FindMyPlayer();
            return;
        }

        text.text = "Treasure: " + myWallet.TotalTreasure.Value;
    }
}