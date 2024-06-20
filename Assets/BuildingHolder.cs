using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ResourceHolder", menuName = "ScriptableObjects/ResourceHolder", order = 1)]
public class BuildingHolder : ScriptableObject
{
    public List<ClickableObject> prefabs = new();
}
