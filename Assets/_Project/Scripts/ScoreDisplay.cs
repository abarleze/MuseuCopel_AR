using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    [Header("Variables")]
    public StringVariable playerName;
    public FloatVariable playerScore;
    public List<FloatVariable> otherScores;

    [Header("References")]
    public GameObject parabens;
    public Text displayLabel;
    public GameObject overlay;

    private void OnEnable()
    {
        if (otherScores != null && otherScores.Count == 0)
            ShowSinglePlayer();
        else
            ShowMultiPlayer();
        
    }

    private void ShowSinglePlayer()
    {
        displayLabel.text = playerName.Value + " + " + playerScore.Value.ToString();
    }

    private void ShowMultiPlayer()
    {
        if (playerName.Value.Length == 0)
        {
            overlay.SetActive(true);
            displayLabel.text = "";
        }
        else
        {
            bool highScore = true;
            foreach (FloatVariable other in otherScores)
                if (other.Value > playerScore.Value)
                    highScore = false;

            parabens.SetActive(highScore);

            displayLabel.text = playerName.Value + " + " + playerScore.Value.ToString();
        }
    }
}