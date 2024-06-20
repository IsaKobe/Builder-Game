using UnityEngine;

public class BuildButtons : MasterClass
{
    // selects category that is supposed to be opened
    public void OpenCategory(string category)
    {
        GameObject categ = GameObject.Find(category).transform.GetChild(0).gameObject;
        categ.SetActive(!(categ.activeSelf));
    }
    // selects tile to build
    public void SelPrefab(GameObject g)
    {
        GridTiles sel = GameObject.Find("Grid").GetComponent<GridTiles>();
        sel.buildingPrefab = g;
        sel.ChangeSelMode(SelectionMode.build);
    }

}
