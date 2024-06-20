using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class BuildButtons : MonoBehaviour
{
    [SerializeField] BuildCost cost; // fill only for building

    // selects category that is supposed to be opened
    public void OpenCategory(string category)
    {
        GameObject categ = GameObject.Find(category).transform.GetChild(0).gameObject;
        categ.SetActive(!(categ.activeSelf));
    }
    // selects tile to build
    public void selPrefab(GameObject g)
    {
        GridTiles sel = Camera.main.transform.parent.GetChild(1).GetChild(0).GetComponent<GridTiles>();
        sel.buildingPrefab = g;
        sel.ChangeSelMode(selectionMode.build);
    }

}
