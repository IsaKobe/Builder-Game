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
        Build build = _build.build;
        if (!build.constructed)
        {
            UpdateResText(_build);
        }
        else
        {
            bool productionButton = false;
            bool generalText = false;
            AssignBuilding assignBuilding = _build.GetComponent<AssignBuilding>();
            Transform constructed = transform.GetChild(1).GetChild(1); // gameObject "Constructed"
            if (!assignBuilding)
            {
                constructed.GetChild(0).gameObject.SetActive(false); // workers
                constructed.GetChild(1).gameObject.SetActive(true); // storage
                constructed.GetChild(1).GetComponent<StorageAssign>().building = _build;
                StorageAssign SA = constructed.GetChild(1).GetComponent<StorageAssign>();
                setStorageButton(_build.GetComponent<Storage>().canStore, constructed);
            }
            else
            {
                string s = "";
                ProductionBuilding productionBuilding = assignBuilding.GetComponent<ProductionBuilding>();
                if (productionBuilding)
                {
                    productionButton = true;
                    s = "Workers";
                    string _s = "";
                    Resource r = build.localRes;
                    for (int i = 0; i < r.ammount.Length; i++)
                    {
                        if(r.ammount[i] > 0)
                        {
                            _s += $"{r.names[i]}: {r.ammount[i]}";
                        }
                    }
                    constructed.GetChild(2).GetChild(3).GetComponent<TMP_Text>().text = _s;
                    float prog = productionBuilding.currentTime / productionBuilding.prodTime;
                    constructed.GetChild(2).GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = prog == 0 ? 0.01f : prog;
                }
                else // active the text
                {
                    generalText = true;
                    s = "Residents";
                }
                constructed.GetChild(0).GetChild(2).GetChild(0).GetChild(0).GetComponent<TMP_Text>().text = s; // worker table
                constructed.GetChild(0).GetComponent<WorkerAssign>().FillStart(assignBuilding);
                setAssignButton(true, constructed.GetChild(0).GetChild(2));
                constructed.GetChild(1).gameObject.SetActive(false); // storage Table
            }
            constructed.GetChild(2).gameObject.SetActive(productionButton); // the "ProductionButton" gameObject
            constructed.GetChild(3).gameObject.SetActive(generalText); // general text
        }
        GameObject midMenu = transform.GetChild(1).gameObject;
        midMenu.transform.GetChild(0).gameObject.SetActive(!build.constructed);
        midMenu.transform.GetChild(1).gameObject.SetActive(build.constructed);
        
    }
    public void UpdateResText(Building _build)
    {
        Transform constructed = transform.GetChild(1).GetChild(1); // gameObject "Constructed"
        Build build = _build.build;
        if (!build.constructed)
        {
            string s = "";
            foreach (int i in build.cost.ammount.Where(q => q > 0))
            {
                int j = build.cost.ammount.ToList().IndexOf(i);
                s += $"{build.cost.names[j]} {build.localRes.ammount[j]}/{i}\n";
            }
            transform.GetChild(1).GetChild(0).GetChild(1).GetComponent<TMP_Text>().text = s;
        }
        else
        {
            Storage storage = _build.GetComponent<Storage>();
            if (storage)
            {
                constructed.GetChild(1).GetComponent<StorageAssign>().UpdateAmmounts();
            }
            ProductionBuilding pB = _build.GetComponent<ProductionBuilding>();
            if (pB)
            {
                string s = "";
                for (int i = 0; i < pB.productionCost.ammount.Length; i++)
                {
                    if(pB.productionCost.ammount[i] > 0)
                    {
                        s += $"{pB.inputResource.ammount[i]}/{pB.productionCost.ammount[i]} {pB.productionCost.names[i]} \n";
                    }
                }
                constructed.GetChild(2).GetChild(1).GetComponent<TMP_Text>().text = s;
                s = "";
                for (int i = 0; i < pB.production.ammount.Length; i++)
                {
                    if (pB.production.ammount[i] > 0)
                    {
                        s += $"{pB.production.ammount[i]} {pB.production.names[i]} \n";
                    }
                }
                constructed.GetChild(2).GetChild(2).GetComponent<TMP_Text>().text = s;
                return;
            }

            House h = _build.GetComponent<House>();
            if (h)
            {
                string s = $"Can house up to {h.limit} workers \n";
                s += $"Occupancy {h.assigned.Count}/{h.limit}";
                constructed.GetChild(3).GetComponent<TMP_Text>().text = s;
                return;
            }
        }
        
    }
    public void setAssignButton(bool assign, Transform buttons) // toggles info worker to assigned and back
    {
        buttons.parent.GetChild(0).gameObject.SetActive(assign);
        buttons.GetChild(0).GetComponent<Button>().interactable = !assign;
        buttons.GetChild(0).GetChild(0).GetComponent<TMP_Text>().color = !assign ? Color.black : Color.white;
        buttons.GetChild(0).GetChild(0).GetComponent<TMP_Text>().fontStyle = !assign ? FontStyles.Normal : FontStyles.Bold;

        buttons.parent.GetChild(1).gameObject.SetActive(!assign);
        buttons.GetChild(1).GetComponent<Button>().interactable = assign;
        buttons.GetChild(1).GetChild(0).GetComponent<TMP_Text>().color = assign ? Color.black : Color.white;
        buttons.GetChild(1).GetChild(0).GetComponent<TMP_Text>().fontStyle = assign ? FontStyles.Normal : FontStyles.Bold;
    }
    void setStorageButton(List<bool> canStore, Transform button)
    {
        button = button.GetChild(1).GetChild(0);
        int i = 0;
        foreach(bool active in canStore)
        {
            button.GetChild(i).GetChild(1).gameObject.GetComponent<Button>().interactable = !active;
            button.GetChild(i).GetChild(2).gameObject.GetComponent<Button>().interactable = active;
            i++;
        }
    }
}