using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public List<ApplicationData> allApplications;

    private const string SessionKey = "CurrentSessionGenerated";

    public void StartNewSession()
    {
        // Check if we already generated IDs for this session
        if (PlayerPrefs.GetInt(SessionKey, 0) == 1)
        {
            Debug.Log("Session already has IDs. Skipping reset.");
            return;
        }

        ResetAllApplicationIds();

        // Mark that we’ve generated IDs for this session
        PlayerPrefs.SetInt(SessionKey, 1);
        PlayerPrefs.Save();
    }

    public void ResetAllApplicationIds()
    {
        // Optional: clear old IDs if you want
        foreach (var app in allApplications)
        {
            app.SaveIds();
        }

        Debug.Log("New IDs generated for all applications.");
    }
}
