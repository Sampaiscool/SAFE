using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "ItemData", menuName = "Eye Manager/Item", order = 3)]
public class ItemSO : ScriptableObject
{
    public string itemName;

    [TextArea(1, 10)] // (min lines, max lines)
    public string itemDescription;

    public bool isLoreItem;
}
