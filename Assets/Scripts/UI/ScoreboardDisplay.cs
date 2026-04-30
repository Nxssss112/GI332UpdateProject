using UnityEngine;
using TMPro;
using System.Text;

public class ScoreboardDisplay : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI scoreboardText;
    [SerializeField] private TextMeshProUGUI winnerText;

    void Start()
    {
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;                  

        DisplayScores();
    }

    private void DisplayScores()
    {
        if (GameManager.FinalResults == null || GameManager.FinalResults.Count == 0)
        {
            scoreboardText.text = "No scores found.";
            return;
        }

        StringBuilder sb = new StringBuilder();
        sb.AppendLine("RANK - NAME - SCORE");

        for (int i = 0; i < GameManager.FinalResults.Count; i++)
        {
            var data = GameManager.FinalResults[i];
            sb.AppendLine($"{i + 1}. {data.Name} : {data.Score}");
        }

        scoreboardText.text = sb.ToString();

        if (winnerText != null && GameManager.FinalResults.Count > 0)
        {
            winnerText.text = $"Winner: {GameManager.FinalResults[0].Name}!";
        }
    }
}