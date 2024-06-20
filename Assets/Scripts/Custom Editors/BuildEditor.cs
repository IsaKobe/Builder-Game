using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BuildingGrid))]
public class BuildEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BuildingGrid building = ((Building)target).build.blueprint;
        base.OnInspectorGUI();
        
    }
}
