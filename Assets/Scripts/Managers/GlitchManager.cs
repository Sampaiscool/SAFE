using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public SystemDevices systemDevices;
    public PopupManager popupManager;

    public void GlitchTimedEvent()
    {
        // Call a random action (for now we have CorruptDevices)
        int randomAction = Random.Range(0, 1); // Can be expanded to more actions later

        switch (randomAction)
        {
            case 0: //SystemDevices
                popupManager.ShowPopup(AppNames.SystemDevices);
                int amountToCorrupt = Random.Range(1, 4);  // Randomly choose between 1 to 3 devices to corrupt
                systemDevices.CorruptDevices(amountToCorrupt);  // Corrupt the devices
                break;
            default:
                break;
        }
    }
}
