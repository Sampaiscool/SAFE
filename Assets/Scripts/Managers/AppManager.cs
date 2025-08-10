using UnityEngine;

public class AppManager : MonoBehaviour
{
    public GameObject eyemanagerPanel;
    public GameObject ClockPanel;
    public GameObject SystemDevicesPanel;

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
}
