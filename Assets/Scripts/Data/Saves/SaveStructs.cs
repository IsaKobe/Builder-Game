using JetBrains.Annotations;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class JobSave
{
    public JobState job;
    public List<GridPos> path;
    public int objectId;
    public Type objectType = null; // -1 – unassigned; 0 – nothing; 1 – building; 2 – rock; 3 – chunk
    public JobSave()
    {

    }
    public JobSave(JobData jobData)
    {
        job = jobData.job;
        path = jobData.path;
        // interest assign 
        if (jobData.interest)
            jobData.interest.GetComponent<ClickableObject>().GetID(this);
        else
        {
            objectId = -1;
        }
        
    }
}
////////////////////////////////////////////////////////////
//--------------------------Grid--------------------------//
////////////////////////////////////////////////////////////
[Serializable]
public class GridSave
{
    public int width;
    public int height;
    public List<int> toBeDug;
    public List<BSave> buildings;
    public List<ChunkSave> chunks;
    public ClickableObjectSave[,] gridItems;
}

[Serializable]
public class ClickableObjectSave
{
    public int id;
}
[Serializable]
public class RockSave : ClickableObjectSave
{
    public int integrity;
    public string oreName;
    public bool toBeDug;
}
[Serializable]
public class WaterSave : ClickableObjectSave
{
    public int ammount;
}
////////////////////////////////////////////////////////////
//---------------------Grid Buildings---------------------//
////////////////////////////////////////////////////////////
[Serializable]
public class StorageObjectSave : ClickableObjectSave
{
    public StorageResSave resSave;
}
[Serializable]
public class BSave : StorageObjectSave
{
    public Build build;
    public string prefabName;
    public float rotationY;
    public GridPos gridPos;
}

[Serializable]
public class StorageBSave : BSave
{
    public List<bool> canStore;
    public bool main;
}

[Serializable]
public class AssignBSave : BSave
{
    public List<int> assigned;
    public int limit;
}

[Serializable]
public class ProductionBSave : AssignBSave
{
    public StorageResSave inputRes;
    public ProductionTime pTime;
    public ProductionStates pStates;
    public ProductionBSave() { }
}
//////////////////////////////////////////////////////////////////
//--------------------------Grid Items--------------------------//
//////////////////////////////////////////////////////////////////
/*
[Serializable]
public class GridItemSave
{
    public GridPos gridPos;
    public int itemId;
    public int type;
    public GridItemSave(int x, int z, GridItem _gridItem)
    {
        gridPos = new(x, z);
        gridItem = _gridItem;
    }
}*/
[Serializable]
public class ChunkSave
{
    public int id;
    public GridPos gridPos;
    public StorageResSave localRes;
    public int humanId;
    public ChunkSave()
    {

    }
    public ChunkSave(Chunk chunk)
    {
        id = chunk.id;
        gridPos = new(chunk.transform.position);
        localRes = new(chunk.localRes);
    }
}
//////////////////////////////////////////////////////////////////
//-----------------------------Humans---------------------------//
//////////////////////////////////////////////////////////////////

public class HumanSave
{
    // Data mainly for loading
    public int id = -1;
    public string name;
    public MyColor color;
    public GridPos gridPos;
    // Job use
    public JobSave jobSave;
    public Resource inventory;
    // statuses
    public int sleep;
    public bool hasEaten;
    // specializations
    public Specs specs;
    public int houseID;
    public int workplaceId;


    public HumanSave()
    {

    }
    public HumanSave(Human h)
    {
        id = h.id;
        name = h.name;
        color = new(h.transform.GetChild(1).GetComponent<MeshRenderer>().material.color); // saves color of hat
        gridPos = new(h.transform.position);
        jobSave = new(h.jData);
        inventory = h.inventory;
        sleep = h.sleep;
        hasEaten = h.hasEaten;
        specs = h.specialization;
        houseID = h.home ? h.home.id : -1;
        workplaceId = h.workplace ? h.workplace.id : -1;
    }
}
[Serializable]
public class StorageResSave
{
    public Resource stored;
    public List<Resource> requests;
    public List<int> carriers;
    public List<int> mod;

    public StorageResSave(StorageResource storageResource)
    {
        stored = storageResource.stored;
        requests = storageResource.requests;
        carriers = storageResource.carriers.Select(q => q.id).ToList();
        mod = storageResource.mods;
    }
    public StorageResSave()
    {

    }
}
[Serializable]
public class MyColor{
    public float r;
    public float g;
    public float b;
    public float a;
    public MyColor(Color color)
    {
        r = Mathf.RoundToInt(color.r * 255);
        g = Mathf.RoundToInt(color.g * 255);
        b = Mathf.RoundToInt(color.b * 255);
        a = Mathf.RoundToInt(color.a * 255);
    }
    public Color ConvertColor()
    {
        return new(r / 255, g / 255, b / 255, a / 255);
    }
}
