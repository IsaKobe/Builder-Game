using UnityEngine;
using UnityEngine.UI;

public class UIbuttons : MonoBehaviour
{
    public GameObject scene;
    public void ToggleAction(int i)
    {
        GridTiles gridTiles = transform.parent.GetChild(0).GetComponent<GridTiles>();
        if(gridTiles.selMode == (SelectionMode)i)
        {
            gridTiles.activeObject = null;
            gridTiles.ChangeSelMode(SelectionMode.nothing);
        }
        else
            gridTiles.ChangeSelMode((SelectionMode)i);
    }
    public void SetSpeed(Transform t)
    {
        int child = (int)Time.timeScale;
        if(child == 5)
            child = 3;
        t.parent.GetChild(child).GetComponent<Button>().interactable = true;
        t.GetComponent<Button>().interactable = false;
        scene.GetComponent<Tick>().ChangeGameSpeed(int.Parse(t.name[0].ToString()));
        
    }
}
