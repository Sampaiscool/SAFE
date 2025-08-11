using UnityEngine;
using UnityEngine.UI;

public class SystemDevices : MonoBehaviour
{
    // Directe verwijzingen naar de drie apparaten
    public GameObject device1;
    public GameObject device2;
    public GameObject device3;

    // Afbeeldingen voor corruptie en originele staat
    public Sprite errorImage;
    public Sprite originalImage;

    // Individuele corruptiestatus voor elk apparaat
    private bool isDevice1Corrupted;
    private bool isDevice2Corrupted;
    private bool isDevice3Corrupted;

    void Awake()
    {
        // Initialiseer de corruptiestatus als false
        isDevice1Corrupted = false;
        isDevice2Corrupted = false;
        isDevice3Corrupted = false;

        // Stel de initiële afbeeldingen in
        SetDeviceImage(device1, originalImage);
        SetDeviceImage(device2, originalImage);
        SetDeviceImage(device3, originalImage);
    }

    /// <summary>
    /// Methode om de corruptie voor device 1 te herstellen.
    /// Deze methode wordt direct aan de knop van device 1 gekoppeld in de Unity Editor.
    /// </summary>
    public void ResetDevice1()
    {
        if (isDevice1Corrupted)
        {
            SetDeviceImage(device1, originalImage);
            isDevice1Corrupted = false;
            Debug.Log("Device 1 hersteld.");
        }
    }

    /// <summary>
    /// Methode om de corruptie voor device 2 te herstellen.
    /// Deze methode wordt direct aan de knop van device 2 gekoppeld in de Unity Editor.
    /// </summary>
    public void ResetDevice2()
    {
        if (isDevice2Corrupted)
        {
            SetDeviceImage(device2, originalImage);
            isDevice2Corrupted = false;
            Debug.Log("Device 2 hersteld.");
        }
    }

    /// <summary>
    /// Methode om de corruptie voor device 3 te herstellen.
    /// Deze methode wordt direct aan de knop van device 3 gekoppeld in de Unity Editor.
    /// </summary>
    public void ResetDevice3()
    {
        if (isDevice3Corrupted)
        {
            SetDeviceImage(device3, originalImage);
            isDevice3Corrupted = false;
            Debug.Log("Device 3 hersteld.");
        }
    }

    // Methode om willekeurige apparaten te corrumperen
    public void CorruptDevices(int amount)
    {
        // Alle apparaten terugzetten naar de originele staat voor een nieuwe corruptie
        isDevice1Corrupted = false;
        isDevice2Corrupted = false;
        isDevice3Corrupted = false;
        SetDeviceImage(device1, originalImage);
        SetDeviceImage(device2, originalImage);
        SetDeviceImage(device3, originalImage);

        int corruptedCount = 0;
        while (corruptedCount < amount)
        {
            int randomDeviceIndex = Random.Range(1, 4);

            switch (randomDeviceIndex)
            {
                case 1:
                    if (!isDevice1Corrupted)
                    {
                        isDevice1Corrupted = true;
                        SetDeviceImage(device1, errorImage);
                        corruptedCount++;
                    }
                    break;

                case 2:
                    if (!isDevice2Corrupted)
                    {
                        isDevice2Corrupted = true;
                        SetDeviceImage(device2, errorImage);
                        corruptedCount++;
                    }
                    break;

                case 3:
                    if (!isDevice3Corrupted)
                    {
                        isDevice3Corrupted = true;
                        SetDeviceImage(device3, errorImage);
                        corruptedCount++;
                    }
                    break;
            }
        }
    }

    // Hulp-methode om de afbeelding van een apparaat in te stellen
    private void SetDeviceImage(GameObject device, Sprite image)
    {
        if (device == null) return;
        var deviceImage = device.GetComponent<Image>();
        if (deviceImage != null)
        {
            deviceImage.sprite = image;
        }
    }

    // De HeatAndCorruptionCheck methode blijft hetzelfde
    public void HeatAndCorruptionCheck()
    {
        if (isDevice1Corrupted || isDevice2Corrupted || isDevice3Corrupted)
        {
            Debug.Log("Speler heeft verloren!");
            // Je kunt hier verdere game over logica toevoegen
        }
    }
}