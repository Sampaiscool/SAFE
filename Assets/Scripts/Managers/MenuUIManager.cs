using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUIManager : MonoBehaviour
{
    public GameObject difficultyButtons;
    public GameObject startButton;

    public void OnStartClicked()
    {
        difficultyButtons.SetActive(true);
        startButton.SetActive(false);
    }

    public void OnDifficultySelected(int difficultyIndex)
    {
        PlayerSettings.chosenDifficulty = (Difficulties)difficultyIndex;

        // Load your game scene after selecting
        UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
    }
}
