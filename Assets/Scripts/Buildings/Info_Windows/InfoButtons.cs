using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InfoButtons : MasterClass
{
    public int id;
    public void CloseInfoMenu() // closes the info window
    {
        transform.parent.parent.gameObject.SetActive(false);
        GameObject.Find("Grid").GetComponent<GridTiles>().activeBuild = null;
    }
    public void SwitchViews(bool state) // changes assign to unassign and back
    {
        GameObject.Find("Info Window").GetComponent<InfoWindow>().setAssignButton(state, transform.parent);
    }
    public async void ManageWorkers(bool add) // assigns or unassigns worker
    {
        await transform.parent.parent.parent.parent.GetComponent<WorkerAssign>().ManageHuman(id, add);
    }
    public void ManageStorage(bool status)
    {
        gameObject.GetComponent<Button>().interactable = false;
        transform.parent.GetChild(status ? 2 : 1).GetComponent<Button>().interactable = true;
        Storage storage = transform.parent.parent.parent.GetComponent<StorageAssign>().building.GetComponent<Storage>();
        storage.canStore[id] = status;
    }
}
