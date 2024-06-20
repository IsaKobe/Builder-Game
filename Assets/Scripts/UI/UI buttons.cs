using UnityEngine;

public class UIbuttons : MasterClass
{
    [SerializeField] GridTiles gridTiles;
    public void ToggleAction(int i)
    {
        gridTiles.ChangeSelMode((SelectionMode)i);
    }
}
