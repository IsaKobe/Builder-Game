using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public enum jobs
{
    free,
    digging,
    building,
    demolishing,
    carrying,
    pickup,
    store,
    fullTime
}
public enum Specs
{
    worker,
    farmer,
    miner
}
public class Human : MasterClass
{
    //[SerializeField] int health = 100;
    public float speed = 0.5f;
    public jobData jData = new();
    public Resource inventory = new();
    //public List<GameObject> inv = new();
    public int maxCapacity = 10;
    public Plan planA;

    public bool nightTime = false;
    public int sleep = 0;
    public Mining m;
    public Construction c;
    public Deliveries d;
    public int id = 0;
    public Specs specialization = Specs.worker;
    public House home;

    void Awake() // initialize Classes
    {
        m = gameObject.GetComponent<Mining>();
        c = gameObject.GetComponent<Construction>();
        d = gameObject.GetComponent<Deliveries>();
        UnigueID();
        StartCoroutine(Idle());
    }
    public void UnigueID() // creates a random int
    {
        List<int> ids = transform.parent.parent.GetChild(0).GetComponentsInChildren<Human>().Select(q => q.id).ToList();
        ids.AddRange(transform.parent.parent.GetChild(1).GetComponentsInChildren<Human>().Select(q => q.id).ToList());
        do
        {
            id = UnityEngine.Random.Range(0, 2000000);
        }
        while (ids.IndexOf(id) != -1);
    }
    public IEnumerator Move(List<Vector3Int> path, IEnumerator couroutine)
    {
        if (path.Count > 0) // moves acording to the path given
        {
            if (transform.position != path.Last())
            {
                foreach (var p in path.ToList())
                {
                    transform.localPosition = p;
                    yield return new WaitForSeconds(speed);
                }
            }
        }
        StartCoroutine(couroutine);
    }

    public IEnumerator Decide() // for new Jobs or for managing fullTime jobs
    {
        switch (jData.job)
        {
            case jobs.free:
                StartCoroutine(Idle());
                break;
            case jobs.digging:
                StartCoroutine(m.Dig());
                break;
            case jobs.building:
                StartCoroutine(c.Build());
                break;
            case jobs.demolishing:
                StartCoroutine(c.Demolish());
                break;
            case jobs.carrying:
                StartCoroutine(d.Carry());
                break;
            case jobs.pickup: // after move take the resources
                yield return d.Take(true);
                Task t = d.FindStorage();
                break;
            case jobs.store:
                StartCoroutine(d.Store());
                break;
            case jobs.fullTime:
                jData.objects.building.GetComponent<ProductionBuilding>().Work(this);
                break;
            default:
                Debug.LogError("Sometnig went wrong, don't know what to do!!!");
                break;
        }
        yield break;
    }
    public IEnumerator Idle()
    {
        jData = new jobData();
        planA = new();
        Vector3Int v = ToInt(GameObject.Find("Elevator").transform.localPosition);
        if (v != ToInt(transform.localPosition)) // if not standing on the elevator
        {
            planA = gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), new() { GameObject.Find("Elevator") }, this).Result;
            StartCoroutine(Move(planA.path, LookForNew())); //  go to the elevator and look for a new Job 
        }
        else
        {
            yield return new WaitForSeconds(5); // Decrease to have better reactions
            StartCoroutine(LookForNew());
        }
        yield return null;
    }
    public IEnumerator LookForNew()
    {
        planA = new();
        jData = new();
        StopC();
        List<jobData> j = GetComponentInParent<JobQueue>().GetActiveJobs(); // Gets all active jobs
        if (j.Count > 0)
        {
            jobData jD = new();
            try
            {
                jD = gameObject.GetComponent<PathFinder>().FindJob(ToInt(transform.position), j).Result; // Finds the one closest
                if (jD.ID != -1)
                {
                    jData = j.Where(g => g.ID == jD.ID).Single(); // assigns all the needed data
                    j.Where(g => g.ID == jD.ID).Single().human = this;
                    jData.path = jD.path;
                    if (!nightTime) // checks if it's day
                    {
                        switch (jData.job)
                        {
                            case jobs.digging:
                                StartCoroutine(Move(jData.path, m.Dig()));
                                break;
                            case jobs.demolishing:
                                StartCoroutine(Move(jData.path, c.Demolish()));
                                break;
                            case jobs.pickup:
                                planA.interest = jData.objects.building.gameObject;
                                StartCoroutine(Move(jData.path, d.Take(true)));
                                //StartCoroutine(LookForNew());
                                break;
                            case jobs.building:
                            case jobs.carrying:
                            case jobs.store:
                                c.FindResources();
                                break;
                        }
                    }
                }
                else
                {
                    if (!nightTime)
                    {
                        Task t = d.Gather(); // if no found try to gather leftovers
                    }

                    yield break;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"LookForNew(), {gameObject.name} :  {e}");
            }
        }
        else
        {
            if (!nightTime)
            {
                Task t = d.Gather(); // if no found try to gather leftovers
            }
        }
        yield break;
    }
    public void StopC()
    {
        StopCoroutine("Move");
        StopAllCoroutines();
        m.StopAllCoroutines();
        d.StopAllCoroutines();
        c.StopAllCoroutines();

        if(jData.job == jobs.fullTime) // if working stop working
        {
            if(jData.objects.building.GetComponent<ProductionBuilding>() != null)
            {
                jData.objects.building.GetComponent<ProductionBuilding>().working.RemoveAll(q => this);
            }
        }
    }
    public void Day()
    {
        nightTime = false; // stop sleeping
        StopC();
        switch (jData.job)
        {
            case jobs.free:
                StartCoroutine(LookForNew());
                print("I'm free");
                break;
            case jobs.building:
                c.FindResources();
                break;
            case jobs.carrying:
                StartCoroutine(d.Carry());
                break;
            case jobs.demolishing:
            case jobs.digging:
            case jobs.fullTime:
                StartCoroutine(Move(gameObject.GetComponent<PathFinder>().FindJob(ToInt(transform.position), new() { jData }).Result.path, Decide()));
                break;
            case jobs.store:
            case jobs.pickup:
                StartCoroutine(Move(gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), new() { planA.interest }, this).Result.path, Decide()));
                break;
            default:
                StartCoroutine(Move(gameObject.GetComponent<PathFinder>().FindJob(ToInt(transform.position), new() { jData }).Result.path, Decide()));
                break;
        }
    }
    public void Night()
    {
        sleep = 0;
        nightTime = true;
        if(jData.job == jobs.fullTime)
        {
            jData.objects.building.GetComponent<ProductionBuilding>().working.Remove(this); // removes from working
        }
        StopC();
        if (home != null)
        {
            if (home.transform.localPosition != transform.localPosition) // sends worker home to sleep
            {
                StartCoroutine(Move(gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.localPosition), new() { home.gameObject }, this).Result.path, Sleep()));
                return;
            }
            StartCoroutine(Sleep()); // worker starts sleeping
        }
        else // if no home, move to elevator
        {
            GameObject el = GameObject.Find("Elevator");
            if (ToInt(el.transform.localPosition) != transform.localPosition)
            {
                StartCoroutine(Move(gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.localPosition), new() { el }, this).Result.path, Sleep()));
                return;
            }

        }

    }
    IEnumerator Sleep()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            sleep++;
        }
    }
}