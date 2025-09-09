using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SessionManager : MonoBehaviour
{
    public List<ApplicationData> allApplications;
    public LocationData ApplicationLocation;

    private const string SessionKey = "CurrentSessionGenerated";

    public List<string> SessionPorts = new List<string>();
    public string CorrectPort;

    private const string PortsKey = "SessionPorts";
    private const string CorrectPortKey = "CorrectPort";

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
    public void InitializeApplicationsForGame()
    {
        // Vind de Applications-locatie
        LocationData applicationsLocation = this.ApplicationLocation;

        // Maak eerst zeker dat hij leeg is
        applicationsLocation.availableApplications.Clear();

        // Kopieer de lijst van alle apps zodat we kunnen shufflen
        List<ApplicationData> copy = new List<ApplicationData>(allApplications);

        // Shuffle
        for (int i = 0; i < copy.Count; i++)
        {
            int randomIndex = Random.Range(i, copy.Count);
            var temp = copy[i];
            copy[i] = copy[randomIndex];
            copy[randomIndex] = temp;
        }

        // Voeg de eerste 3 toe en geef ze een sessionName
        for (int i = 0; i < 3 && i < copy.Count; i++)
        {
            var app = copy[i];

            // Add the app to the Applications location
            applicationsLocation.availableApplications.Add(app);

            // Assign a session name for this playthrough
            app.sessionName = "Application" + (i + 1);
            GameManager.Instance.SessionNames[app] = app.sessionName;

        }
    }
    public void GeneratePorts()
    {
        // Als er al ports bestaan, laad die in plaats van nieuwe te maken
        if (PlayerPrefs.HasKey(PortsKey) && PlayerPrefs.HasKey(CorrectPortKey))
        {
            string json = PlayerPrefs.GetString(PortsKey, "[]");
            SessionPorts = JsonUtility.FromJson<PortListWrapper>(json).ports;
            CorrectPort = PlayerPrefs.GetString(CorrectPortKey, "");
            return;
        }

        // Nieuwe ports maken
        System.Random rand = new System.Random();
        SessionPorts.Clear();
        for (int i = 0; i < 5; i++)
        {
            string ip = $"{rand.Next(0, 256)}.{rand.Next(0, 256)}.{rand.Next(0, 256)}.{rand.Next(0, 256)}";
            SessionPorts.Add(ip);
        }

        CorrectPort = SessionPorts[rand.Next(SessionPorts.Count)];

        // Opslaan in PlayerPrefs
        PortListWrapper wrapper = new PortListWrapper { ports = SessionPorts };
        string saveJson = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString(PortsKey, saveJson);
        PlayerPrefs.SetString(CorrectPortKey, CorrectPort);
        PlayerPrefs.Save();

        Debug.Log($"Generated Ports: {string.Join(", ", SessionPorts)}");
        Debug.Log($"Correct Port: {CorrectPort}");
    }

    // Handige functie om te resetten tijdens development
    public void ResetPorts()
    {
        PlayerPrefs.DeleteKey(PortsKey);
        PlayerPrefs.DeleteKey(CorrectPortKey);
        PlayerPrefs.Save();
        SessionPorts.Clear();
        CorrectPort = "";
    }
}

[System.Serializable]
public class PortListWrapper
{
    public List<string> ports;
}
