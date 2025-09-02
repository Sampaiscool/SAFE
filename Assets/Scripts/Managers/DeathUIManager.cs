using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeathUIManager : MonoBehaviour
{
    public Button tryAgainButton;
    public TMP_Text DeathReasonText;

    void Start()
    {
        if (GameManager.Instance.LostDueTo == DeathReason.SystemDevices_CorruptionAndHeat)
        {
            DeathReasonText.text = "Player lost due to heat and corruption.";
        }
        else if (GameManager.Instance.LostDueTo == DeathReason.Jenuve_IncorrectCell)
        {
            DeathReasonText.text = "Player lost due to being on the incorrect cell inside Jenuva.";
        }
        else if (GameManager.Instance.LostDueTo == DeathReason.Jenuve_GlitchMiniGameFailed)
        {
            DeathReasonText.text = "Player lost due to failing to fix the glitch in time inside Jenuva.";
        }
        else if (GameManager.Instance.LostDueTo == DeathReason.KFlipped_NotFinished)
        {
            DeathReasonText.text = "Player lost due to not finishing the KFlipped minigame.";
        }
    }

    public void OnRetryButtonClicked()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MenuScene");
    }
}
