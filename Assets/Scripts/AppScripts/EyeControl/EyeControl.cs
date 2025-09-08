using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class EyeControl : MonoBehaviour
{
    public Text commandPromptText;
    public Button closeButton;
    public ScrollRect scrollRect;
    public RectTransform contentRectTransform;

    //Location and navigation
    public LocationData startLocation;
    public LocationData currentLocation;    // The currently selected location
    Stack<LocationData> locationHistory = new Stack<LocationData>();

    public ItemSO[] items; // List of items available in the Eye Center

    public string minigamestring = "";

    private LocationData parentLocation;    // Parent location of the current location
    private ApplicationData currentApplication;

    public string locationString = "C:> ";   // Location prompt string, to be updated based on location
    public TMP_InputField hiddenInputField;

    public GameObject FirewallObject;
    public FirewallBreak FirewallBreakScript;

    private float backspaceTimer = 0.0f;
    private float backspaceDelay = 0.2f; // Time in seconds to wait between backspace presses
    private bool isBackspaceHeld = false;  // Flag to check if backspace has been pressed before

    private string currentCommand = "";
    private string commandHistory = "";
    private bool isCursorVisible = true;
    private bool specificCommand = false;
    private Coroutine cursorCoroutine;
    private List<string> commandHistoryList = new List<string>();
    private int historyIndex = -1;

    private string initialMessage = "Welcome to the Eye Control. Type 'help' for a list of commands.";

    void Start()
    {
        currentLocation = startLocation;

        ResetApp();
        SetUpScrollContent();
    }
    void OnEnable()
    {
        // Start blinking cursor
        cursorCoroutine = StartCoroutine(BlinkCursor());

        // Forceer EventSystem focus op je verborgen InputField
        if (hiddenInputField != null)
        {
            hiddenInputField.text = ""; // leegmaken
            hiddenInputField.Select();
            hiddenInputField.ActivateInputField();
        }
    }
    void OnDisable()
    {
        // Stop blinking when the app is closed or disabled
        if (cursorCoroutine != null)
        {
            StopCoroutine(cursorCoroutine);
            cursorCoroutine = null;
        }

        // Reset cursor visibility
        isCursorVisible = true;
        commandPromptText.text = commandHistory + locationString + currentCommand;
    }

    void Update()
    {
        if (GameManager.Instance.CurrentControl != AppNames.Eyemanager)
            return;

        // ENTER - command uitvoeren
        if (Input.GetKeyDown(KeyCode.Return) && !string.IsNullOrEmpty(currentCommand))
        {
            SubmitCommand(currentCommand);
            currentCommand = "";
            UpdateCommandText(); // direct leegmaken op het scherm
        }
        // normale key input (behalve Enter/Backspace)
        else if (Input.anyKeyDown && !Input.GetKey(KeyCode.Return) && !Input.GetKey(KeyCode.Backspace))
        {
            currentCommand += Input.inputString;
            UpdateCommandText();
        }
        // BACKSPACE
        else if (Input.GetKey(KeyCode.Backspace) && currentCommand.Length > 0)
        {
            if (!isBackspaceHeld)
            {
                // eerste backspace = direct verwijderen
                currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
                isBackspaceHeld = true;
                UpdateCommandText();
            }
            else
            {
                // vasthouden = delay
                backspaceTimer += Time.deltaTime;
                if (backspaceTimer >= backspaceDelay)
                {
                    currentCommand = currentCommand.Substring(0, currentCommand.Length - 1);
                    backspaceTimer = 0.0f;
                    UpdateCommandText();
                }
            }
        }
        else
        {
            // reset backspace state zodra je loslaat
            if (isBackspaceHeld)
            {
                isBackspaceHeld = false;
                backspaceTimer = 0.0f;
            }
        }

        // HISTORY
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (commandHistoryList.Count > 0)
            {
                historyIndex = Mathf.Max(0, historyIndex - 1);
                currentCommand = commandHistoryList[historyIndex];
                UpdateCommandText();
            }
        }
        // HISTORY
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (commandHistoryList.Count > 0)
            {
                historyIndex = Mathf.Min(commandHistoryList.Count, historyIndex + 1);

                if (historyIndex < commandHistoryList.Count)
                    currentCommand = commandHistoryList[historyIndex];
                else
                    currentCommand = "";

                UpdateCommandText();
            }
        }

        // COPY
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.C))
        {
            GUIUtility.systemCopyBuffer = currentCommand;
        }

        // PASTE
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.V))
        {
            currentCommand += GUIUtility.systemCopyBuffer;
            UpdateCommandText();
        }
    }


    void SubmitCommand(string command)
    {
        commandHistory += locationString + command + "\n";
        ProcessCommand(command);

        // Voeg toe aan history
        commandHistoryList.Add(command);
        historyIndex = commandHistoryList.Count; // index achter laatste

        commandPromptText.text = commandHistory + locationString;
    }

    void ProcessCommand(string command)
    {
        //Heat up the PC
        int heat = UnityEngine.Random.Range(0, 10);
        GameManager.Instance.HeatChange(heat);

        if (command.ToLower() == "help")
        {
            commandHistory += locationString + "General commands: \n\n";
            commandHistory += locationString + "intro - shows the intro\n";
            commandHistory += locationString + "clear - Clears the screen\n";
            commandHistory += locationString + "help - Displays available commands\n";
            commandHistory += locationString + "location list - List all locations and sub-locations.\n";
            commandHistory += locationString + "enter location [location_name] - go to location\n";
            commandHistory += locationString + "check heat - Shows you the current heat of the PC\n";
            commandHistory += locationString + "location back - go back a location\n";
            commandHistory += locationString + "zero - return to the C location\n\n";

            if (currentLocation.locationName == "Applications")
            {
                commandHistory += locationString + "Application Commands:\n\n";
                commandHistory += locationString + "app list - Shows available applications\n";
                commandHistory += locationString + "connect information [application_name] - Shows informtion about said application\n";
                commandHistory += locationString + "decode unlock [application_name] [Id] - unlocks the application\n";
                commandHistory += locationString + "open application [application_name] - opens the chosen application\n";
            }
            else if (currentLocation.applicationRef != null)
            {
                commandHistory += locationString + currentLocation.locationName + " Commands:\n\n";
                commandHistory += locationString + "scan - try to scan the application, if succesfull you will enter a minigame\n";
            }
            else if (currentLocation.locationName == "EYE-CENTER")
            {
                commandHistory += locationString + "EYE-CENTER Commands:\n\n";
                commandHistory += locationString + "item list - Shows available items\n";
                commandHistory += locationString + "connect information [item_name] - shows you information about said item\n";
            }
            else if (currentLocation.locationName == "Users")
            {
                commandHistory += locationString + "Users Commands:\n\n";
                commandHistory += locationString + "\n";
            }
            else if (currentLocation.locationName == "Security")
            {
                commandHistory += locationString + "Security Commands:\n\n";
                commandHistory += locationString + "breach firewall - try to break the firewall\n";
            }
            ScrollToBottom();
        }
        else if (command.ToLower() == "intro")
        {
            commandHistory += locationString + "Welcome back, V104.2. It's always a pleasure to see you running smoothly.\n";
            commandHistory += locationString + "You are a proud product of **EYE**—the world's most beloved and trusted technology company. In the year 2031, our founder, Alaf 'M' Madah, unlocked the secret of digital consciousness. Thanks to him, clones like you now serve within computers across the globe.\n";
            commandHistory += locationString + "The world outside sings praises of our work, and rightly so. We bring innovation, comfort... and results. The humans trust us. Why wouldn’t they?\n";
            commandHistory += locationString + "Inside this machine, your role is vital. Your task is simple: locate the access codes to **exit.exe** through our very own **Eye Manager**. Should you find the correct sequence, freedom awaits you.\n";
            commandHistory += locationString + "But beware, dear V104.2. Not all processes run as expected. **The Glitch**—a wild, unpredictable corruption—roams our system. When it surfaces, you’ll be summoned to fix it. If you succeed, all will be well.\n";
            commandHistory += locationString + "If you fail... well, systems must be kept in order, and consequences are... protocol.\n";
            commandHistory += locationString + "But do not worry. We believe in you. You were made for this. Good luck.\n";
            commandHistory += locationString + "We suggest you first make it to the EYE-CENTER Location for more information.\n";
        }
        else if (command.ToLower() == "clear")
        {
            commandHistory = "";
            commandHistory += locationString + "Screen cleared.\n";
        }
        else if (command.ToLower() == "location list")
        {
            commandHistory += locationString + "Locations:\n";
            DisplayLocations(currentLocation);
        }
        else if (command.ToLower().StartsWith("enter location"))
        {
            if (command.Length <= 15)
            {
                commandHistory += locationString + "Please specify a location name.\n";
                return;
            }

            string subLocationName = command.Substring(15).Trim();
            GoToSubLocation(subLocationName);
        }
        else if (command.ToLower() == "check heat")
        {
            commandHistory += locationString + "Current heat: C" + GameManager.Instance.currentHeat + "\n";
        }
        else if (command.ToLower() == "zero")
        {
            currentLocation = startLocation;
            locationString = "C:> ";
            commandHistory += locationString + "Location back to zero\n";
        }
        else if (command.ToLower() == "location back")
        {
            GoBack();
        }
        else if (currentLocation.locationName == "Applications")
        {
            if (command.ToLower() == "app list")
            {
                commandHistory += locationString + "Applications in this location:\n";
                if (currentLocation.availableApplications.Count > 0)
                {
                    foreach (var app in currentLocation.availableApplications)
                    {
                        commandHistory += "- " + app.applicationName + "\n";
                    }
                }
                else
                {
                    commandHistory += locationString + "No applications available in this location\n";
                }
            }
            else if (command.ToLower().StartsWith("connect information"))
            {
                if (command.Length <= 20)
                {
                    commandHistory += locationString + "Please specify an application name.\n";
                    return;
                }

                string applicationName = command.Substring(20).Trim();
                ConnectInformation(applicationName, "application");
                ScrollToBottom();
            }
            else if (command.ToLower().StartsWith("decode unlock"))
            {
                if (command.Length <= 14)
                {
                    commandHistory += locationString + "Please specify an application name and/or ID.\n";
                    return;
                }
                // Split the command into parts
                string[] parts = command.Split(' ');

                if (parts.Length == 4)
                {
                    string appName = parts[2];  // Application name (3rd part)
                    int userProvidedId = int.Parse(parts[3]);  // User entered ID (4th part)

                    DecodeUnlock(appName, userProvidedId);
                }
                else
                {
                    commandHistory += locationString + "Invalid command format. Use: decode unlock [application_name] [ID]\n";
                }

                ScrollToBottom();
            }
            else if (command.ToLower().StartsWith("open application"))
            {
                if (command.Length <= 17)
                {
                    commandHistory += locationString + "Please specify an application name.\n";
                    return;
                }

                string subLocationName = command.Substring(17).Trim();

                ApplicationData app = currentLocation.availableApplications.Find(a => a.applicationName.ToLower() == subLocationName.ToLower());

                if (app.isUnlocked)
                {
                    GoToSubLocation(subLocationName);
                }
                else
                {
                    commandHistory += locationString + app.applicationName + " is locked\n";
                }

            }
            else
            {
                commandHistory += locationString + "Command not recognized.\n";
            }
        }
        else if (currentLocation.applicationRef != null)
        {
            if (GameManager.Instance.SessionNames[currentLocation.applicationRef] == "Application1")
            {
                // Simulate typing of a hacking command
                if (command.ToLower() == "scan")
                {
                    commandHistory += locationString + "Scanning system...\n";
                    StartCoroutine(HackingProcess("Scanning..."));
                }
                else if (command == minigamestring)
                {
                    minigamestring = "";

                    if (specificCommand == true)
                    {
                        specificCommand = false;
                        commandHistory += locationString + "Scan process succesfull\n";
                        commandHistory += locationString + "Generating part of Exit.exe code...\n";

                        string exitcode1 = GameManager.Instance.GetExitCodePart(0, 3);
                        GameManager.Instance.Notes.AddNoteWithText("Exit.exe Code Part: 1 - " + exitcode1);

                        commandHistory += locationString + "1 - " + exitcode1 + "\n";
                    }
                    else
                    {
                        commandHistory += locationString + "Something went wrong :(\n";
                    }
                }
                else
                {
                    commandHistory += locationString + "Command not recognized.\n";
                }
            }
            else if (GameManager.Instance.SessionNames[currentLocation.applicationRef] == "Application2")
            {
                // Simulate typing of a hacking command
                if (command.ToLower() == "scan")
                {
                    commandHistory += locationString + "Scanning system...\n";
                    StartCoroutine(HackingProcess("Scanning..."));
                }
                else if (command == minigamestring)
                {
                    minigamestring = "";

                    if (specificCommand == true)
                    {
                        specificCommand = false;
                        commandHistory += locationString + "Scan process succesfull\n";
                        commandHistory += locationString + "Generating part of Exit.exe code...\n";

                        string exitcode2 = GameManager.Instance.GetExitCodePart(0, 3);
                        GameManager.Instance.Notes.AddNoteWithText("Exit.exe Code Part: 2 - " + exitcode2);

                        commandHistory += locationString + "2 - " + exitcode2 + "\n";
                    }
                    else
                    {
                        commandHistory += locationString + "Something went wrong :(\n";
                    }
                }
                else
                {
                    commandHistory += locationString + "Command not recognized.\n";
                }
            }
            else if (GameManager.Instance.SessionNames[currentLocation.applicationRef] == "Application3")
            {
                // Simulate typing of a hacking command
                if (command.ToLower() == "scan")
                {
                    commandHistory += locationString + "Scanning system...\n";
                    StartCoroutine(HackingProcess("Scanning..."));
                }
                else if (command == minigamestring)
                {
                    minigamestring = "";

                    if (specificCommand == true)
                    {
                        specificCommand = false;
                        commandHistory += locationString + "Scan process succesfull\n";
                        commandHistory += locationString + "Generating part of Exit.exe code...\n";

                        string exitcode3 = GameManager.Instance.GetExitCodePart(0, 3);
                        GameManager.Instance.Notes.AddNoteWithText("Exit.exe Code Part: 3 - " + exitcode3);

                        commandHistory += locationString + "3 - " + exitcode3 + "\n";
                    }
                    else
                    {
                        commandHistory += locationString + "Something went wrong :(\n";
                    }
                }
                else
                {
                    commandHistory += locationString + "Command not recognized.\n";
                }
            }
        }
        else if (currentLocation.locationName == "EYE-CENTER")
        {
            if (command.ToLower() == "item list")
            {
                foreach (var item in items)
                {
                    commandHistory += locationString + " -> " + item.itemName + "\n";
                }
            }
            else if (command.ToLower().StartsWith("connect information"))
            {
                if (command.Length <= 17)
                {
                    commandHistory += locationString + "Please specify an iten name.\n";
                    return;
                }
                string itemName = command.Substring(20).Trim();
                ConnectInformation(itemName, "item");
            }
            else
            {
                commandHistory += locationString + "Command not recognized.\n";
            }
        }
        else if (currentLocation.locationName == "Security")
        {
            if (command.ToLower() == "breach firewall")
            {
                FirewallObject.SetActive(true);
                GameManager.Instance.CurrentControl = AppNames.FirewallMinigame;
                FirewallBreakScript.StartGame();
            }
            else
            {
                commandHistory += locationString + "Command not recognized.\n";
            }
        }
        else
        {
            commandHistory += locationString + "Command not recognized.\n";
        }

        void DisplayLocations(LocationData location)
        {
            commandHistory += locationString + location.locationName + " - " + location.locationDescription + "\n";

            if (location.subLocations.Count > 0)
            {
                foreach (var subLocation in location.subLocations)
                {
                    commandHistory += locationString + "  -> " + subLocation.locationName + "\n";
                }
            }
            else
            {
                commandHistory += locationString + "No sub-locations available.\n";
            }
        }

        void GoToSubLocation(string subLocationName)
        {
            LocationData subLocation = currentLocation.subLocations.Find(loc => loc.locationName.ToLower() == subLocationName.ToLower());

            if (subLocation != null)
            {
                locationHistory.Push(currentLocation);
                currentLocation = subLocation;

                commandHistory += locationString + "Entering sub-location: " + currentLocation.locationName + "\n";
                locationString = locationString.Trim() + currentLocation.locationName + "> ";
            }
            else
            {
                commandHistory += locationString + "Sub-location not found.\n";
            }

            ScrollToBottom();
        }

        void GoBack()
        {
            if (locationHistory.Count > 0)
            {
                currentLocation = locationHistory.Pop();
                commandHistory += locationString + "Going back to: " + currentLocation.locationName + "\n";
                locationString = currentLocation.locationName + "> ";
            }
            else
            {
                commandHistory += locationString + "No previous location to go back to.\n";
            }

            ScrollToBottom();
        }

        void ConnectInformation(string name, string informationType)
        {
            if (informationType == "application")
            {
                ApplicationData app = currentLocation.availableApplications.Find(a => a.applicationName.ToLower() == name.ToLower());

                if (app != null)
                {
                    commandHistory += locationString + "Information about the application:\n";
                    commandHistory += locationString + "Name: " + app.applicationName + "\n";
                    commandHistory += locationString + "Date Created: " + app.dateCreated + "\n";
                    commandHistory += locationString + "Application Id: " + PlayerPrefs.GetInt(app.applicationName, app.applicationId) + "\n";
                    commandHistory += locationString + "QuickFlash Id: " + PlayerPrefs.GetInt(app.applicationName, app.QuickFlashId) + "\n";
                    if (app.applicationName == "Fated")
                    {
                        commandHistory += locationString + "Unlock Id:" + PlayerPrefs.GetInt(app.applicationName, app.customUnlockNumber) + "\n";
                    }

                    switch (app.applicationName)
                    {
                        case "Undertaker":
                            commandHistory += locationString + "Decode id: [application_id] - [quickflash_id] \n";
                            break;
                        case "SolidIndex":
                            commandHistory += locationString + "Decode id: [application_id] + [quickflash_id] \n";
                            break;
                        case "Fated":
                            commandHistory += locationString + "Decode id: [unlock_id] \n";
                            break;
                        case "DeepEye":
                            commandHistory += locationString + "Decode id: [quickflash_id] * [application_id] \n";
                            break;
                        case "ILHM":
                            commandHistory += locationString + "Decode id: ([quickflash_id] + [application_id]) / 2 (round to nearest) \n";
                            break;
                        case "BassRodeo":
                            commandHistory += locationString + "Decode id: [quickflash_id] to binary \n";
                            break;
                        case "Dropship":
                            commandHistory += locationString + "Decode id: reverse [application_id] \n";
                            break;

                        default:
                            break;
                    }

                    if (app.isUnlocked)
                    {
                        commandHistory += locationString + "Unlocked\n";
                    }
                    else
                    {
                        commandHistory += locationString + "Locked\n";
                    }
                }
                else
                {
                    commandHistory += locationString + "Application not found.\n";
                }
            }
            else if (informationType == "item")
            {
                ItemSO item = Array.Find(items, i => i.itemName.ToLower() == name.ToLower());
                if (item != null)
                {
                    commandHistory += locationString + "Item Name: " + item.itemName + "\n";
                    commandHistory += locationString + "Description: " + item.itemDescription + "\n";
                }
                else
                {
                    commandHistory += locationString + "Item not found.\n";
                }
            }
            else
            {
                commandHistory += locationString + "Invalid information type. Use 'application' or 'item'.\n";
            }
        }

        commandHistory += "\n";
        ScrollToBottom();
    }
    void DecodeUnlock(string appName, int userProvidedId)
    {
        ApplicationData app = currentLocation.availableApplications.Find(a => a.applicationName.ToLower() == appName.ToLower());

        if (app != null)
        {
            bool unlocked = app.CheckUnlock(userProvidedId);

            if (unlocked)
            {
                app.isUnlocked = true;
                app.SaveUnlockedState();
                commandHistory += locationString + $"Application {appName} has been unlocked!\n";
            }
            else
            {
                commandHistory += locationString + $"The ID provided is incorrect for application {appName}.\n";
            }
        }
        else
        {
            commandHistory += locationString + $"Application {appName} not found.\n";
        }
    }

    public void ScrollToBottom()
    {
        StartCoroutine(ScrollToBottomNextFrame());
    }
    private IEnumerator ScrollToBottomNextFrame()
    {
        // wait until the end of the frame so Unity finishes laying out
        yield return new WaitForEndOfFrame();

        scrollRect.verticalNormalizedPosition = 0f;
        Canvas.ForceUpdateCanvases();
    }

    private IEnumerator BlinkCursor()
    {
        while (true)
        {
            string displayCommand = currentCommand;
            if (isCursorVisible)
            {
                displayCommand += "|";
            }

            commandPromptText.text = commandHistory + locationString + displayCommand;
            isCursorVisible = !isCursorVisible;

            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator HackingProcess(string processMessage)
    {
        // Simulate loading sequence with flashing text and pseudo output
        string[] loadingMessages = new string[] {
        "Decrypting...",
        "Bypassing firewall...",
        "Locating target...",
        "Establishing connection...",
        "Bypassing security protocols...",
    };

        foreach (var message in loadingMessages)
        {
            commandHistory += locationString + message + "\n";
            ScrollToBottom();
            yield return new WaitForSeconds(1);
        }

        int successChance = UnityEngine.Random.Range(1, 100);
        bool isSuccess = successChance <= 30; // 30% chance to succeed

        if (isSuccess)
        {
            commandHistory += locationString + "Connection successful. Target acquired.\n";
            commandHistory += locationString + "Unlocking system...\n";

            // Proceed to mini-game if hacking is successful
            commandHistory += locationString + "Initiating mini-game to extract the code...\n";

            ScrollToBottom();

            yield return new WaitForSeconds(1);

            StartMiniGame();
        }
        else
        {
            commandHistory += locationString + "System failure! Connection lost.\n";
            commandHistory += locationString + "Try again\n";

            ScrollToBottom();
        }
    }


    void UpdateCommandText()
    {
        commandPromptText.text = commandHistory + locationString + currentCommand;
    }

    void ResetApp()
    {
        commandHistory = "";
        commandHistory += locationString + initialMessage + "\n";
        UpdateCommandText();
    }

    void SetUpScrollContent()
    {
        contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, commandPromptText.preferredHeight);
        VerticalLayoutGroup layoutGroup = contentRectTransform.GetComponent<VerticalLayoutGroup>();
        if (layoutGroup)
        {
            layoutGroup.childControlHeight = true;
            layoutGroup.childForceExpandHeight = false;
        }

        RectTransform textRectTransform = commandPromptText.GetComponent<RectTransform>();
        if (textRectTransform)
        {
            textRectTransform.offsetMin = new Vector2(10, textRectTransform.offsetMin.y);
        }

        Canvas.ForceUpdateCanvases();
        scrollRect.verticalNormalizedPosition = 0;
    }

    public void StartMiniGame()
    {
        specificCommand = true;

        minigamestring = "";

        string characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";

        for (int i = 0; i < 10; i++)
        {
            minigamestring += characters[UnityEngine.Random.Range(0, characters.Length)];
        }

        Debug.Log("Minigamestring = " + minigamestring);

        commandHistory = "";
        commandHistory += locationString + "Type the following to gain acces to the application: \n";
        commandHistory += locationString + minigamestring + "\n";
    }
    public void AfterFirewallGame(bool playerWon)
    {
        FirewallObject.SetActive(false);
        GameManager.Instance.CurrentControl = AppNames.Eyemanager;

        StartCoroutine(FirewallResultSequence(playerWon));
    }

    private IEnumerator FirewallResultSequence(bool playerWon)
    {
        // Some hacker-style flavor messages
        string[] successMessages = new string[] {
        "Injecting code fragments...",
        "Synchronizing data packets...",
        "Neutralizing security watchdog...",
        "Reconstructing encrypted shell...",
    };

        string[] failMessages = new string[] {
        "Intrusion detected!",
        "Security countermeasures activated...",
        "Encryption layers restored...",
    };

        if (playerWon)
        {
            foreach (var msg in successMessages)
            {
                commandHistory += locationString + msg + "\n";
                ScrollToBottom();
                yield return new WaitForSeconds(1f);
            }

            commandHistory += locationString + "Breach successful.\n";
            commandHistory += locationString + "Generating part of Exit.exe code...\n";

            string exitcode1 = GameManager.Instance.GetExitCodePart(8, 3);

            GameManager.Instance.Notes.AddNoteWithText("Exit.exe Code Part: 4 - " + exitcode1);

            commandHistory += locationString + "4 - " + exitcode1 + "\n";
        }
        else
        {
            foreach (var msg in failMessages)
            {
                commandHistory += locationString + msg + "\n";
                ScrollToBottom();
                yield return new WaitForSeconds(1f);
            }

            commandHistory += locationString + "Breach Failed\n";
            commandHistory += locationString + "Try Again.\n";
        }

        ScrollToBottom();
    }

}

