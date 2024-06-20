using UnityEngine;

public class UIbuttons : MonoBehaviour
{
    [SerializeField] GridTiles gridTiles;
    public void toggleAction(int i)
    {
        gridTiles.ChangeSelMode((selectionMode)i);
    }
}
