using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class GlitchManager : MonoBehaviour
{
    public SystemDevices systemDevices;
    public Jenuve jenuve;
    public Ghost ghost;

    public PopupManager popupManager;

    public bool isFreezeActive = false;
    private Coroutine freezeRoutine;

    public void GlitchTimedEvent()
    {
        systemDevices = GameManager.Instance.systemDevices;
        jenuve = GameManager.Instance.jenuve;
        ghost = GameManager.Instance.ghost;


        if (!isFreezeActive)
        {
            int randomAction = Random.Range(0, 3);

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

                case 2: // Ghost
                    popupManager.ShowPopup(AppNames.Ghost);
                    ghost.GhostGlitch();
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
