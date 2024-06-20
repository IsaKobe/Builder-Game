using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIbuttons : MonoBehaviour
{
    [SerializeField] GridTiles gridTiles;
    public void toggleAction(int i)
    {
       gridTiles.ChangeSelMode((selectionMode)i);
    }
}
