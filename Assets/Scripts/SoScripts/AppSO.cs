using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AppData", menuName = "New App", order = 1)]
public class AppSO : ScriptableObject
{
    [SerializeField]
    public AppNames appName;
    public Sprite appIcon;
}
