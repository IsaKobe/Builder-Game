using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum jobs
{
    free,
    digging,
    building,
    demolishing,
    carrying,
    pickup,
    store
}
public class Human : MonoBehaviour
{
    //[SerializeField] int health = 100;
    public float speed = 0.5f;
    public jobData jData = new jobData();
    public Resource inventory = new Resource();
    //public List<GameObject> inv = new();
    public int maxCapacity = 10;
    public Plan planA;

    public Mining m;
    public Construction c;
    public Deliveries d;
    public List<String> names = new List<string>();
    public List<String> order = new List<string>();
    public int debugIndex = 0;
    void Awake()
    {
        m = gameObject.GetComponent<Mining>();
        c = gameObject.GetComponent<Construction>();
        d = gameObject.GetComponent<Deliveries>();
        StartCoroutine("Idle");

    }
    public IEnumerator Move(List<Vector3Int> path)
    {
        foreach (var p in path.ToList())
        {
            transform.localPosition = p;
            yield return new WaitForSeconds(speed);
        }
        switch (jData.job)
        {
            case jobs.free:
                StartCoroutine("Idle");
                break;
            case jobs.digging:
                names.Add($"DIG{debugIndex}");
                debugIndex++;
                m.StartCoroutine(m.DoDig());
                break;

            case jobs.building:
                StartCoroutine(c.Build());
                break;

            case jobs.carrying:
                StartCoroutine(d.Carry());
                break;

            case jobs.pickup:
                d.Take(true);
                List<Storage> s = GameObject.Find("Buildings").GetComponentsInChildren<Storage>().ToList();
                planA = new();
                planA = gameObject.GetComponent<PathFinder>().prep(Vector3Int.FloorToInt(transform.position), s.Where(g => g.build.capacity > g.build.localRes.ammount.Sum()).Select(g => g.gameObject).ToList(), maxCapacity, this).Result;
                if (planA.path.Count > 0)
                {
                    //names.Add(planA.interests[0].name);
                    jData.job = jobs.store;
                    StartCoroutine(Move(planA.path));
                }
                else
                {
                    Task _t = LookForNew();
                }
                break;

            case jobs.store:
                d.Store();
                //names.Add(planA.interests[0].name);
                break;

            default:
                Debug.LogError("Sometnig went wrong, don't know what to do!!!");
                break;
        }
        yield return null;
    }
    public IEnumerator Idle()
    {
        //print(name + "\n" + string.Join("\n", names));
        jData = new jobData();
        List<jobData> list = new List<jobData>();
        list.Add(new jobData(Vector3Int.FloorToInt(gameObject.GetComponentInParent<Humans>().grid.transform.GetChild(3).Find("Elevator").position))); // finds path to elevator
        List<Vector3Int> l = gameObject.GetComponent<JobFinder>().FindPath(Vector3Int.FloorToInt(transform.position), list).Result.path.ToList();
        if(l.Last() != transform.localPosition)
        {
            StartCoroutine(Move(l)); // moves to the elevator
        }
        else
        {
            yield return new WaitForSeconds(5); // Decrease to have better reactions
            //print("Waiting");
            Task t = LookForNew();
        }
        yield return null;
    }

    public async Task LookForNew()
    {
        planA = new();
        jData = new();
        StopC();
        List<jobData> j = GetComponentInParent<JobQueue>().getActiveJobs(); // Gets all active jobs
        
        if (j.Count > 0)
        {
            jobData jD = new();
            try
            {
                jD = await gameObject.GetComponent<JobFinder>().FindPath(Vector3Int.FloorToInt(transform.position), j); // Finds the one closest
                if (jD.ID != -1)
                {
                    jData = j.Where(g => g.ID == jD.ID).Single();
                    j.Where(g => g.ID == jD.ID).Single().human = this;
                    jData.path = jD.path;
                    if (jData.job == jobs.building) // if building call findResources
                    {
                        Task t = c.findResources();
                        return;
                    }
                    else // else move and do it
                    {
                        jData.human = this;
                        StartCoroutine(Move(jData.path));
                        return;
                    }
                }
                else
                {
                    Task t = d.Gather(); // if no found try to gather left overs
                    return;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"LookForNew(), {gameObject.name}  {e}");
            }
        }
        else
        {
            Task t = d.Gather(); // if no found try to gather leftovers
        }
    }
    public void StopC()
    {
        StopAllCoroutines();
        m.StopAllCoroutines();
        d.StopAllCoroutines();
        c.StopAllCoroutines();
    } 
}