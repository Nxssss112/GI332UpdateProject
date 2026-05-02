using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    [SerializeField] private Image cooldownImage;

    private float cooldownTime;
    private float timer;
    private bool cooling;

    // เริ่มคูลดาวน์
    public void StartCooldown(float time)
    {
        cooldownTime = time;
        timer = time;
        cooling = true;

        cooldownImage.fillAmount = 1f; // เริ่มเต็ม
    }

    private void Update()
    {
        if (!cooling) return;

        timer -= Time.deltaTime;

        cooldownImage.fillAmount = timer / cooldownTime;

        if (timer <= 0f)
        {
            cooling = false;
            cooldownImage.fillAmount = 0f; // พร้อมยิง
        }
    }

    // เช็คว่ายิงได้ไหม (ยังคูลดาวน์อยู่หรือเปล่า)
    public bool IsReady()
    {
        return !cooling;
    }
}