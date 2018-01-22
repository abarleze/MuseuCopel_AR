using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartGameConfirmation : MonoBehaviour
{
    public Text confirmationText;
    public List<StringVariable> playerNames;
    
    private void OnEnable()
    {
        if (playerNames != null && playerNames.Count == 1)
            confirmationText.text = SinglePlayerText();
        else
            confirmationText.text = MultiPlayerText();
        
    }

    private string MultiPlayerText()
    {
        int c = 0;
        foreach (var item in playerNames)
            if (item.Value.Length == 3)
                c++;

        return "Iniciar o jogo com " + c + " jogadores?";
    }

    private string SinglePlayerText()
    {
        return "Usar as iniciais: " + playerNames[0].Value + "?";
    }
}