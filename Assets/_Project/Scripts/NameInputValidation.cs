using UnityEngine;
using UnityEngine.UI;

public class NameInputValidation : MonoBehaviour
{
    private InputField inputField;

    private void Awake()
    {
        inputField = GetComponent<InputField>();
        inputField.onValidateInput += Validate;
    }

    private char Validate(string text, int charIndex, char addedChar)
    {
        if (addedChar.IsEnglishLetter())
            return char.ToUpper(addedChar);
        else
            return '\0';
    }
}

public static class CharExtention
{
    public static bool IsEnglishLetter(this char c)
    {
        return (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z');
    }
}