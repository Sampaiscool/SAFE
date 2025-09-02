using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Application", menuName = "Eye Manager/Application")]
public class ApplicationData : ScriptableObject
{
    public string applicationName;        // Name of the application
    public string dateCreated;
    public int applicationId;
    public int QuickFlashId;
    public bool isUnlocked;               // Whether the app is unlocked
    public string[] availableCommands;    // Commands available within this app

    // Save the unlocked state to PlayerPrefs (only for the session)
    public void SaveUnlockedState()
    {
        // Save the current session's unlocked state
        PlayerPrefs.SetInt(applicationName + "_Unlocked", isUnlocked ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void SaveIds()
    {
        applicationId = Random.Range(1100, 9999);
        QuickFlashId = Random.Range(10, 99);

        PlayerPrefs.SetInt(applicationName + "_ApplicationId", applicationId);
        PlayerPrefs.SetInt(applicationName + "_QuickFlashId", QuickFlashId);
        PlayerPrefs.Save();
    }

}
