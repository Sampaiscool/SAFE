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

    // Reset unlocked state for the new session (this would be called on a new game start)
    public void ResetUnlockedState()
    {
        // Randomly assign unlocked state for the new playthrough
        isUnlocked = UnityEngine.Random.Range(0, 2) == 1;  // Randomly true or false
        PlayerPrefs.SetInt(applicationName + "_Unlocked", isUnlocked ? 1 : 0);  // Save it
        PlayerPrefs.Save();
    }
}
