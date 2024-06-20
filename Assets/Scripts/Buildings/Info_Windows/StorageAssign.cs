using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class StorageAssign : MonoBehaviour
{
    public Building building;
    public void UpdateAmmounts()
    {
        int j = transform.GetChild(0).childCount - 1; // get number of resource items
        Transform tran = transform.GetChild(0);
        for(int i = 0; i < j; i++) // for each resource item in content
        {
            tran.GetChild(i).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = building.build.localRes.ammount[i].ToString(); // set the text to the count in storage
        }
        tran.GetChild(j).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = $"{building.build.localRes.ammount.Sum()}/{building.build.capacity}";
    }
}