using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Location", menuName = "Eye Manager/Location")]
public class LocationData : ScriptableObject
{
    public string locationName;                  // Main location name, e.g., "EYE_Apendor"
    public string locationDescription;           // Description of the main location
    public List<LocationData> subLocations;      // List of sub-locations, e.g., ["MainTerminal", "DataVault"]
    public List<ApplicationData> availableApplications;  // Apps available in this location

    public bool isApplication;
    public ApplicationData applicationRef;
}
