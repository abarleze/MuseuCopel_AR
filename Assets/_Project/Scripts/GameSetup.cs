using System.Collections.Generic;
using TriviaQuizGame;
using TriviaQuizGame.Types;
using UnityEngine;
using UnityEngine.UI;

public class GameSetup : MonoBehaviour
{
    [Header("Setup")]
    public List<StringVariable> playerNames;
    public List<FloatVariable> playerScores;
    public List<Color> colors;

    [Header("References")]
    public TQGGameController gameController;
    public GameObject lifesBackground;
    public Image livesBar;

    [Header("Category Pool")]
    public GameObject categoryPrefab;
    public List<WodopoCategory> categoryList;
    public int categoryIndex;

    [Header("Fortune Wheel")]
    public float sliceAngle;
    public Transform wheel;
    public new Animation animation;

    private void Start()
    {
        int c = 0;
        int firstIndexUsed = -1;
        for (int i = 0; i < playerNames.Count; i++)
        {
            if (playerNames[i].Value.Length > 0)
            {
                gameController.players[c].name = playerNames[i];
                gameController.players[c].score = playerScores[i];
                gameController.players[c].color = colors[i];

                c++;
                if (c == 1)
                    firstIndexUsed = i;
            }
        }

        if (firstIndexUsed != -1 && c == 1)
        {
            gameController.players[firstIndexUsed].livesBar = livesBar;
        }
        else
        {
            lifesBackground.SetActive(false);
            livesBar.gameObject.SetActive(false);
        }
        
        gameController.SetNumberOfPlayers(c);

        sliceAngle = 360.0f / categoryList.Count;
        categoryIndex = UnityEngine.Random.Range(0, categoryList.Count);

        CreateWheelSlices();
        PlayWheelAnimation();

        Category cat = GetRandomCategory();
        SetCategory(cat);
    }

    private Category GetRandomCategory()
    {
        int max = categoryList[categoryIndex].categories.Count;
        int i = UnityEngine.Random.Range(0, max);

        return categoryList[categoryIndex].categories[i];
    }

    private void CreateWheelSlices()
    {
        for (int i = 0; i < categoryList.Count; i++)
        {
            Debug.Log("Creating");
            GameObject temp = Instantiate<GameObject>(categoryPrefab, wheel);
            temp.transform.localEulerAngles = Vector3.forward * i * -sliceAngle;
            temp.transform.Find("Slice").GetComponent<Image>().fillAmount = (sliceAngle / 360.0f);
            temp.transform.Find("Slice").GetComponent<Image>().color = categoryList[i].color;
            temp.transform.Find("Text").GetComponent<Text>().text = categoryList[i].categoryName;
            temp.transform.Find("Icon").GetComponent<Image>().sprite = categoryList[i].icon;
        }
        
    }

    private void PlayWheelAnimation()
    {
        // Set the rotation of the category wheel so that it aligns with the wheel arrow
        wheel.localEulerAngles = Vector3.forward * categoryIndex * -sliceAngle;

        // Play the wheel spin animation
        animation.Play();
    }

    private void SetCategory(Category category)
    {
        // If we have subcategories in this category, update them into the list of questions for this category
        if (category.subcategories != null && category.subcategories.Length > 0)
            category.UpdateSubcategories();

        gameController.SendMessage("SetQuestions", category.questions);
    }

    public void StartGame()
    {
        gameController.SendMessage("StartGame");
    }
}

[System.Serializable]
public class WodopoCategory
{
    public string categoryName;
    public Color color = Color.white;
    public Sprite icon;

    public List<Category> categories;
}