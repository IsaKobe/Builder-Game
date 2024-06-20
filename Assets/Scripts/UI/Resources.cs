using System;
using System.Linq;
using TMPro;
using UnityEngine;

public class Resources : MasterClass
{
    // fill with ALL of active resource objects!!!
    public Resource resources;
    public TMP_Text[] textFields;

    void Awake()
    {
        // Update text, or display error
        try
        {
            Storage[] storage = GameObject.Find("Buildings").GetComponentsInChildren<Storage>();
            resources = new();
            foreach(Storage _s in storage)
            {
                for(int i = 0; i < _s.build.localRes.ammount.Length; i++)
                {
                    resources.ammount[i] += _s.build.localRes.ammount[i];
                }
            }
            UpdateResText();
        }
        catch (Exception e)
        {
            Debug.LogError("Idiote!, zapomel jsi zadat nejaky parametr do 'Resources'");
            Debug.LogError(e);
        }
    }
    public void UpdateResText()
    {
        // Updates UI resource text
        for (int i = 0; i < textFields.Length; i++)
        {
            textFields[i].text = resources.ammount[i].ToString();
        }
    }
    // Chnges global counter of a resource
    public void UpdateResourceAmmount(int resource_type, int ammount)
    {
        resources.ammount[resource_type] += ammount;
        UpdateResText();
    }

    // Changes the state of a resources
    public void UpdateResource(Resource cost, bool add)
    {
        int mod = add ? 1 : -1;

        for (int i = 0; i < cost.names.Length; i++)
        {
            for (int j = 0; j < resources.ammount.Length; j++)
            {
                if (resources.names[j] == cost.names[i])
                {
                    resources.ammount[j] += cost.ammount[i] * mod;
                }
            }
        }
        UpdateResText();
    }
    // returns the state of a resource, by it's name
    public int GetResourse(string name)
    {
        for (int i = 0; i < resources.ammount.Length; i++)
        {
            if (resources.names[i] == name)
            {
                return resources.ammount[i];
            }
        }
        return -1;
    }
    public bool CanAfford(Resource cost)
    {
        for (int i = 0; i < cost.names.Length; i++)
        {
            if (cost.ammount[i] > resources.ammount[i])
            {
                return false;
            }
        }
        return true;
    }
}
