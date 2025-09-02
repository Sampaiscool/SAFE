using System.Collections;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public SystemDevices systemDevices;
    public Jenuve jenuve;
    public PopupManager popupManager;

    public bool isFreezeActive = false;
    private Coroutine freezeRoutine;

    public void GlitchTimedEvent()
    {
        if (!isFreezeActive)
        {
            int randomAction = Random.Range(0, 2);

            switch (randomAction)
            {
                case 0: // SystemDevices
                    popupManager.ShowPopup(AppNames.SystemDevices);
                    int amountToCorrupt = Random.Range(1, 4);
                    systemDevices.CorruptDevices(amountToCorrupt);
                    break;

                case 1: // Jenuve
                    popupManager.ShowPopup(AppNames.Jenuve);
                    jenuve.JenuveGlitch();
                    break;
            }
        }
        else
        {
            popupManager.GlitchPopupBlocked();
        }
    }

    // Called by upgrades
    public void FreezeForDuration(float duration)
    {
        if (freezeRoutine != null)
            StopCoroutine(freezeRoutine);

        freezeRoutine = StartCoroutine(FreezeRoutine(duration));
    }

    private IEnumerator FreezeRoutine(float duration)
    {
        isFreezeActive = true;
        yield return new WaitForSeconds(duration);
        isFreezeActive = false;
    }
}
