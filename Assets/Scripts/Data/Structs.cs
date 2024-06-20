using System.Collections.Generic;
using TMPro;
using UnityEngine;


[System.Serializable]
public class building
{
    public int sizeX;
    public int sizeZ;
    int heigth = 0;
    public BuildCost cost;
    public building(int _sizeX, int _sizeZ, BuildCost _cost)
    {
        this.sizeX = _sizeX;
        this.sizeZ = _sizeZ;
        this.cost = _cost;
    }
    public building()
    {

    }
}

[System.Serializable]
public class JobObjects{
    public Rock r;
    public Building building;
    public JobObjects(Rock _r)
    {
        this.r = _r;
    }
    public JobObjects(Building _building)
    {
        this.building = _building;
    }
}

[System.Serializable]
public class jobData{
    public jobs job;
    public Vector3Int jobPos;
    public List<Vector3Int> path;
    public Human human;
    public JobObjects objects;
    public int ID;
    public jobData(jobs _job, Vector3Int _jobPos, List<Vector3Int> _path, Human _human, JobObjects _objects, int _ID)
    {
        this.job = _job;
        this.jobPos = _jobPos;
        this.path = _path;
        this.human = _human;
        this.objects = _objects;
        this.ID = _ID;
    }
    public jobData(jobs _job, Vector3Int _jobPos, JobObjects _objects)
    {
        this.job = _job;
        this.jobPos = _jobPos;
        this.objects = _objects;
    }
    public jobData(List<Vector3Int> _path, Human _human)
    {
        this.path = _path;
        this.human = _human;
    }
    public jobData(List<Vector3Int> _path, int _ID)
    {
        this.path = _path; 
        this.ID = _ID;
    }
    public jobData(Vector3Int _jobPos)
    {
        this.jobPos = _jobPos;
    }
    public jobData()
    {

    }
}

// Struct of a Resource object
[System.Serializable]
public struct Resource
{
    public string name;
    public int ammount;
    public TMP_Text textField;
}

// Struct for Costs
[System.Serializable]
public struct BuildCost
{
    public string[] names;
    public int[] costs;
}

[System.Serializable]
public struct rock
{
    public string name;
    public int hardness;
    [SerializeField] public List<BuildCost> resources;
}
