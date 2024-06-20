using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class Resources : MonoBehaviour
{
    // fill with ALL of active resource objects!!!
    [SerializeField] public Resource[] resources;
    
    void Awake()
    {
        // Update text, or display error
        try{
            UpdateText();
        } catch (Exception e)
        {
            Debug.LogError("Idiote!, zapomel jsi zadat nejaky parametr do 'Resources'");
            Debug.LogError(e);
        }
    }
    public void UpdateText() {
        // Updates UI resource text
        foreach (Resource res in resources)
        {
            res.textField.text = res.ammount.ToString();
        }
    }

    // Changes the state of a resources
    public void UpdateResource(BuildCost cost, bool add) {
        int mod = add ? 1 : -1;

        for (int i = 0; i < cost.names.Length; i++)
        {
            for (int j = 0; j < resources.Length; j++)
            {
                if (resources[j].name == cost.names[i])
                {
                    resources[j].ammount += cost.costs[i] * mod;
                }
            }
        }
        UpdateText();
    }
    // returns the state of a resource, by it's name
    public int GetResourse(string name)
    {
        for (int i = 0; i < resources.Length; i++)
        {
            if (resources[i].name == name)
            {
                return resources[i].ammount;
            }
        }
        return -1;
    }
    public bool canAfford(BuildCost cost)
    {
        for (int i = 0; i < cost.names.Length; i++)
        {
            if (cost.costs[i] > GetResourse(cost.names[i]))
            {
                return false;
            }
        }
        return true;
    }
}
