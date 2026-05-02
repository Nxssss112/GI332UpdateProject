using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private TMP_InputField joinCodeField;
    [SerializeField] private Toggle privateToggle;
    [SerializeField] private AudioClip clickSound; // เพิ่มช่องสำหรับใส่ไฟล์เสียงใน Inspector

    private void Start()
    {
        if (ClientSingleton.Instance == null)
        {
            return;
        }
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }

    public async void StartHost()
    {
        // เล่นเสียงทันทีที่กด (ใช้ SoundManager ที่เป็น DontDestroyOnLoad)
        if (SoundManager.Instance != null && clickSound != null)
        {
            SoundManager.Instance.PlaySound(clickSound);
        }

        await HostSingleton.Instance.GameManager.StartHostAsync(privateToggle.isOn);
    }

    public async void StartClient()
    {
        // เล่นเสียงทันทีที่กด
        if (SoundManager.Instance != null && clickSound != null)
        {
            SoundManager.Instance.PlaySound(clickSound);
        }

        await ClientSingleton.Instance.GameManager.StartClientAsync(joinCodeField.text);
    }
}