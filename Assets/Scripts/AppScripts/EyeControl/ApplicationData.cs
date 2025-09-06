using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Application", menuName = "Eye Manager/Application")]
public class ApplicationData : ScriptableObject
{
    public string applicationName;        // Name of the application
    public string sessionName;            // Name used internally for logic
    public string dateCreated;
    public int applicationId;
    public int QuickFlashId;
    public bool isUnlocked;               // Whether the app is unlocked
    public string[] availableCommands;    // Commands available within this app

    public int customUnlockNumber;

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
        customUnlockNumber = Random.Range(1, 9999);
        PlayerPrefs.SetInt(applicationName + "_ApplicationId", applicationId);
        PlayerPrefs.SetInt(applicationName + "_QuickFlashId", QuickFlashId);
        PlayerPrefs.SetInt(applicationName + "_CustomUnlock", customUnlockNumber);
        PlayerPrefs.Save();
    }
    // Controleer unlock op basis van userInput
    public bool CheckUnlock(int userInput)
    {
        Debug.Log($"Checking unlock for {applicationName} with user input: {userInput}");

        bool unlocked = false;

        switch (applicationName)
        {
            case "Undertaker":
                unlocked = (userInput == (applicationId - QuickFlashId));
                break;

            case "SolidIndex":
                unlocked = (userInput == (applicationId + QuickFlashId));
                break;

            case "Fated":
                unlocked = (userInput == customUnlockNumber);
                break;

            case "DeepEye":
                unlocked = (userInput == (applicationId * QuickFlashId));
                break;

            case "ILHM":
                int correctId = Mathf.RoundToInt((applicationId + QuickFlashId) / 2f);

                Debug.Log($"CorrectId: {correctId}");

                unlocked = (userInput == correctId);
                break;
            case "BassRodeo":
                int quickFlashBinary = int.Parse(System.Convert.ToString(QuickFlashId, 2));

                Debug.Log($"quickFlashBinary: {quickFlashBinary}");

                unlocked = (userInput == (applicationId + quickFlashBinary));
                break;
            case "Dropship":
                string appIdString = applicationId.ToString();
                char[] charArray = appIdString.ToCharArray();
                System.Array.Reverse(charArray);
                string reversedAppIdString = new string(charArray);
                int reversedAppId = int.Parse(reversedAppIdString);

                Debug.Log($"reversedAppId: {reversedAppId}");

                unlocked = (userInput == reversedAppId);
                break;
            default:
                unlocked = false;
                break;
        }

        return unlocked;
    }

}
