using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

public class InfoWindow : MasterClass
{
    public void SetUpTextFields(Building build, string buildName)
    {
        build.build.selected = true;
        transform.GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = buildName;
        UpdateView(build);
    }
    
    public void UpdateView(Building _build)
    {
        building build = _build.build;
        if (!build.constructed)
        {
            UpdateResText(build);
        }
        else
        {
            bool activate = false;
            AssignBuilding assignBuilding = _build.GetComponent<AssignBuilding>();
            Transform constructed = transform.GetChild(1).GetChild(1); 
            if (!assignBuilding)
            {
                constructed.GetChild(0).gameObject.SetActive(false);
            }
            else
            {
                if (assignBuilding.GetComponent<ProductionBuilding>())
                {
                    activate = true;
                    
                }
                else
                {
                }
                transform.GetChild(1).GetChild(1).GetChild(0).GetComponent<WorkerAssign>().FillStart(assignBuilding);
            }
            constructed.GetChild(1).gameObject.SetActive(activate);
            constructed.GetChild(1).GetComponent<Image>().fillAmount = 0.01f;
        }
        GameObject midMenu = transform.GetChild(1).gameObject;
        midMenu.transform.GetChild(0).gameObject.SetActive(!build.constructed);
        midMenu.transform.GetChild(1).gameObject.SetActive(build.constructed);
        
    }
    public void UpdateResText(building build)
    {
        transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = "";
        foreach (int i in build.cost.ammount.Where(q => q > 0))
        {
            int j = build.cost.ammount.ToList().IndexOf(i);
            transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text += $"{build.cost.names[j]} {build.localRes.ammount[j]}/{i}\n";
        }
    }
}
