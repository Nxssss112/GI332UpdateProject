using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class ReturnToMenu : MonoBehaviour
{
    [SerializeField] private string menuSceneName = "Menu"; 

    public void BackToMenu()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.Shutdown();
        }

        SceneManager.LoadScene(menuSceneName);
    }
}