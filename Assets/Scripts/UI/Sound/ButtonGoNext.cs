using UnityEngine;

public class ButtonGoNext : MonoBehaviour
{
    public AudioClip clickSound;

    public void GoNext()
    {
        SoundManager.Instance.PlaySound(clickSound);
    }
}