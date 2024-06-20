using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class building
{
    public int sizeX;
    public int sizeZ;
    public int heigth = 0;
    public int capacity = 50;
    public Resource cost;
    public Resource localRes;
    public bool constructed = false;
    public building(int _sizeX, int _sizeZ, Resource _cost)
    {
        this.sizeX = _sizeX;
        this.sizeZ = _sizeZ;
        this.cost = _cost;
        this.localRes = _cost;
    }
    public building()
    {

    }
}
[System.Serializable]
public class Plan
{
    public List<Vector3Int> path = new();
    public List<GameObject> interests = new();
}

[System.Serializable]
public class JobObjects
{
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
public class jobData
{
    public jobs job;
    public Vector3Int jobPos;
    public List<Vector3Int> path;
    public Human human;
    public JobObjects objects;
    public int ID;
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
    public jobData(List<Vector3Int> _path, int _ID, jobs _job)
    {
        this.path = _path;
        this.ID = _ID;
        this.job = _job;
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

// Struct for Costs
[System.Serializable]
public class Resource
{
    public string[] names = { "metal", "coal", "stone" };
    public int[] ammount = { 0, 0, 0 };
    public Resource()
    {

    }
    public Resource(int[] _am)
    {
        this.ammount = _am;
    }
}

[System.Serializable]
public struct rock
{
    public string name;
    public int hardness;
    [SerializeField] public GameObject chunk;
}
