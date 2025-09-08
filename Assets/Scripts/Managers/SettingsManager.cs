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

    void Start()
    {
        // Laad volume (standaard 1f = 100%)
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1f);

        // Zet slider en audio gelijk
        volumeSlider.value = savedVolume;
        GameManager.Instance.audioManager.audioSource.volume = savedVolume;
    }

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
        float volume = volumeSlider.value;

        // Pas volume aan
        GameManager.Instance.audioManager.audioSource.volume = volume;

        // Sla waarde op
        PlayerPrefs.SetFloat("MasterVolume", volume);
        PlayerPrefs.Save();
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
