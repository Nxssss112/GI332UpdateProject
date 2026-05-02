using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu";
    [SerializeField] private AudioClip clickSound; // เพิ่มช่องสำหรับใส่ไฟล์เสียงใน Inspector

    public void BackToMenu()
    {
        // 1. เล่นเสียงทันทีก่อนจะย้ายซีน
        if (SoundManager.Instance != null && clickSound != null)
        {
            SoundManager.Instance.PlaySound(clickSound);
        }

        // 2. จัดการระบบ Network ก่อนกลับหน้าเมนู
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        // 3. โหลดซีนหน้าเมนู
        SceneManager.LoadScene(menuSceneName);
    }
}