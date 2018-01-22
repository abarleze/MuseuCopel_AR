using UnityEngine;
using UnityEngine.UI;

public class ButtonEnabler : MonoBehaviour
{
    public Button button;
    public StringVariable[] playerNames;
    public int minPlayers = 4;

    private void OnEnable()
    {
        button = GetComponent<Button>();
        button.interactable = false;

        foreach (var item in playerNames)
        {
            item.Value = "";
            item.onValueChange += Validate;
        }
    }

    private void OnDisable()
    {
        foreach (var item in playerNames)
        {
            item.onValueChange -= Validate;
        }
    }

    private void Validate()
    {
        int c = 0;
        foreach (var item in playerNames)
        {
            if (item.Value.Length > 0)
                c++;
        }

        bool conflict = CheckForConflict();

        button.interactable = c >= minPlayers && !conflict;
    }

    private bool CheckForConflict()
    {
        for (int i = 0; i < playerNames.Length - 1; i++)
        {
            if (playerNames[i].Value.Length == 0)
                continue;

            for (int j = i + 1; j < playerNames.Length; j++)
            {
                if (playerNames[j].Value.Length > 0)
                    if (playerNames[i].Value == playerNames[j].Value)
                        return true;
            }
        }

        return false;
    }
}