using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoButtons : MasterClass
{
    public int id;
    public void CloseInfoMenu()
    {
        transform.parent.parent.gameObject.SetActive(false);
        GameObject.Find("Grid").GetComponent<GridTiles>().activeBuild = null;
    }
    public void SwitchViews(bool state)
    {
        transform.parent.parent.GetChild(0).gameObject.SetActive(state);
        transform.parent.parent.GetChild(1).gameObject.SetActive(!state);
    }
    public async void ManageWorkers(bool add)
    {
        await transform.parent.parent.parent.parent.GetComponent<WorkerAssign>().ManageHuman(id, add);
    }
}
