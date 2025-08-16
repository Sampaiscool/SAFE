using UnityEngine;

public class AppManager : MonoBehaviour
{
    public GameObject eyemanagerPanel;
    public GameObject ClockPanel;
    public GameObject SystemDevicesPanel;
    public GameObject JenuvePanel;
    public GameObject NotesPanel;

    // This method is called to open the app
    public void OpenApp(AppSO appData, AppNames AppName)
    {
        switch (AppName)
        {
            case AppNames.Eyemanager:
                OpenEyemanager();
                break;
            case AppNames.Clock:
                OpenClock();
                break;
            case AppNames.SystemDevices:
                OpenSystemDevices();
                break;
            case AppNames.Jenuve:
                OpenJenuve();
                break;
            case AppNames.Notes:
                OpenNotes();
                break;
            default:
                Debug.LogWarning("AppManager Could not find the App Name");
                break;
        }
    }
    private void CloseOtherApps()
    {
        eyemanagerPanel.SetActive(false);
        ClockPanel.SetActive(false);
        SystemDevicesPanel.SetActive(false);
        JenuvePanel.SetActive(false);
    }

    public void OpenEyemanager()
    {
        Debug.Log("Opening Eyemanager");
        CloseOtherApps();
        // Enable the Eyemanager panel
        eyemanagerPanel.SetActive(true);
    }

    public void OpenClock()
    {
        Debug.Log("Opening Clock");
        CloseOtherApps();
        // Enable the Eyemanager panel
        ClockPanel.SetActive(true);
    }

    public void OpenSystemDevices()
    {
        Debug.Log("Opening System Devices");
        CloseOtherApps();
        // Enable the Eyemanager panel
        SystemDevicesPanel.SetActive(true);
    }

    public void CloseEyemanager()
    {
        Debug.Log("Closing Eyemanager");
        CloseOtherApps();
        // Disable the Eyemanager panel
        eyemanagerPanel.SetActive(false);
    }
    public void CloseClock()
    {
        Debug.Log("Closing Clock");
        CloseOtherApps();
        // Enable the Eyemanager panel
        ClockPanel.SetActive(false);
    }
    public void CloseSystemDevices()
    {
        Debug.Log("Closing System Devices");
        CloseOtherApps();
        // Enable the Eyemanager panel
        SystemDevicesPanel.SetActive(false);
    }

    public void OpenJenuve()
    {
        GameManager.Instance.CurrentControl = AppNames.Jenuve;
        Debug.Log("Opening Jenuve");
        CloseOtherApps();
        // Enable the Jenuve panel
        JenuvePanel.SetActive(true);
    }
    public void CloseJenuve()
    {
        GameManager.Instance.CurrentControl = AppNames.None;
        Debug.Log("Closing Jenuve");
        CloseOtherApps();
        // Disable the Jenuve panel
        JenuvePanel.SetActive(false);
    }
    public void OpenNotes()
    {
        GameManager.Instance.CurrentControl = AppNames.Notes;
        Debug.Log("Opening Notes");
        CloseOtherApps();
        // Enable the Notes panel
        NotesPanel.SetActive(true);
    }
    public void CloseNotes()
    {
        GameManager.Instance.CurrentControl = AppNames.None;
        Debug.Log("Closing Notes");
        CloseOtherApps();
        // Disable the Notes panel
        NotesPanel.SetActive(false);
    }
}
