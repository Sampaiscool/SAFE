using UnityEngine;
using UnityEngine.UI;

public class SystemDevices : MonoBehaviour
{
    // Direct references to the three devices
    public GameObject device1;
    public GameObject device2;
    public GameObject device3;

    // Corruption image and original image for each device
    public Sprite errorImage;
    public Sprite originalImage;

    // Individual corruption status for each device
    private bool isDevice1Corrupted;
    private bool isDevice2Corrupted;
    private bool isDevice3Corrupted;

    void Start()
    {
        // Initialize the corruption status as false
        isDevice1Corrupted = false;
        isDevice2Corrupted = false;
        isDevice3Corrupted = false;

        // Optionally, you can set the initial device states to the original images
        SetDeviceImage(device1, originalImage);
        SetDeviceImage(device2, originalImage);
        SetDeviceImage(device3, originalImage);
    }

    // Method to corrupt a random device (1 to 3 devices)
    public void CorruptDevices(int amount)
    {
        // Reset corruption status
        isDevice1Corrupted = false;
        isDevice2Corrupted = false;
        isDevice3Corrupted = false;

        // Corrupt devices
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
                        AttachResetButton(device1, ResetDevice1);
                        corruptedCount++;
                    }
                    break;

                case 2:
                    if (!isDevice2Corrupted)
                    {
                        isDevice2Corrupted = true;
                        SetDeviceImage(device2, errorImage);
                        AttachResetButton(device2, ResetDevice2);
                        corruptedCount++;
                    }
                    break;

                case 3:
                    if (!isDevice3Corrupted)
                    {
                        isDevice3Corrupted = true;
                        SetDeviceImage(device3, errorImage);
                        AttachResetButton(device3, ResetDevice3);
                        corruptedCount++;
                    }
                    break;
            }
        }
    }

    // Method to set the device image (original or error image)
    private void SetDeviceImage(GameObject device, Sprite image)
    {
        if (device == null) return;

        var deviceImage = device.GetComponent<Image>();
        if (deviceImage != null)
        {
            deviceImage.sprite = image;
        }
    }

    // Method to attach a reset button listener
    private void AttachResetButton(GameObject device, UnityEngine.Events.UnityAction action)
    {
        var deviceButton = device.GetComponentInChildren<Button>();
        if (deviceButton != null)
        {
            deviceButton.onClick.RemoveAllListeners(); // Remove any existing listeners
            deviceButton.onClick.AddListener(action); // Add the reset action
        }
        else
        {
            Debug.LogError("No Button component found on device: " + device.name);
        }
    }

    // Reset the corruption for device 1
    private void ResetDevice1()
    {
        SetDeviceImage(device1, originalImage);
        isDevice1Corrupted = false;
    }

    // Reset the corruption for device 2
    private void ResetDevice2()
    {
        SetDeviceImage(device2, originalImage);
        isDevice2Corrupted = false;
    }

    // Reset the corruption for device 3
    private void ResetDevice3()
    {
        SetDeviceImage(device3, originalImage);
        isDevice3Corrupted = false;
    }

    // Method to check if all devices are corrupted and handle the game over state
    public void HeatAndCorruptionCheck()
    {
        if (isDevice1Corrupted || isDevice2Corrupted || isDevice3Corrupted)
        {
            Debug.Log("Player Lost the game!");
        }
    }
}
