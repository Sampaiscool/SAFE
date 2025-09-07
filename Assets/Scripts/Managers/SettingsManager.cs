using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    public GameObject SettingsObject;
    public Slider volumeSlider;

    public int glitchIndexSpawn;

    public bool IsSettingsOpen = false;

    public void ToggleSettings()
    {
        if (IsSettingsOpen)
        {
            CloseSettings();
        }
        else
        {
            OpenSettings();
        }
    }
    public void OpenSettings()
    {
        IsSettingsOpen = true;
        SettingsObject.SetActive(true);
        Time.timeScale = 0f;
    }
    public void CloseSettings()
    {
        IsSettingsOpen = false;
        SettingsObject.SetActive(false);
        Time.timeScale = 1f;
    }
    public void ChangeVolume()
    {
        GameManager.Instance.audioManager.audioSource.volume = volumeSlider.value;
    }

    public void DebugDeath()
    {
        CloseSettings();
        GameManager.Instance.PlayerLost(DeathReason.Debug_Death);
    }

    public void SpawnGlitch()
    {
        GameManager.Instance.glitchManager.GlitchTimedEvent(glitchIndexSpawn);
    }

    public void GiveCoins()
    {
        GameManager.Instance.CoinsChange(100000, false);
    }
}
