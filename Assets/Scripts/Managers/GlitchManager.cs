using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public SystemDevices systemDevices;
    public Jenuve jenuve;
    public PopupManager popupManager;

    public void GlitchTimedEvent()
    {
        int randomAction = Random.Range(0, 2);

        switch (randomAction)
        {
            case 0: //SystemDevices
                popupManager.ShowPopup(AppNames.SystemDevices);
                int amountToCorrupt = Random.Range(1, 4);
                systemDevices.CorruptDevices(amountToCorrupt);
                break;
            case 1: //Jenuve
                popupManager.ShowPopup(AppNames.Jenuve);
                jenuve.JenuveGlitch();
                break;
            default:
                break;
        }
    }
}
