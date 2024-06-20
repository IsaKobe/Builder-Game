using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;

public enum jobs
{
    free,
    digging,
    building,
    demolishing,
}
public class Human : MonoBehaviour
{
    //[SerializeField] int health = 100;
    [SerializeField] public float speed = 0.5f;
    [SerializeField] public jobData jData = new jobData();
    void Awake()
    {
        StartCoroutine("Idle");
    }
    public IEnumerator Move(List<Vector3Int> path)
    {
        path.RemoveAt(0);
        foreach(var p in path)
        {
            transform.localPosition = p;
            yield return new WaitForSeconds(speed);
        }
        switch (jData.job)
        {
            case jobs.digging:
                StartCoroutine(Dig());
                break;
        }
        yield return null;
    }
    public IEnumerator Dig()
    {
        Humans h = transform.parent.GetComponent<Humans>();
        Rock r = jData.objects.r;
        yield return new WaitForSeconds(r.data.hardness);
        foreach(var _r in r.data.resources)
        {
            h.resources.UpdateResource(_r, true);
        }
        h.grid.RemoveTiles(r);
        h.GetComponent<JobQueue>().RemoveJob(jData.ID);
        jData = new jobData();
        Task t = LookForNew();
    }
    public IEnumerator Idle()
    {
        List<jobData> list = new List<jobData>();
        list.Add(new jobData(Vector3Int.FloorToInt(gameObject.GetComponentInParent<Humans>().grid.transform.GetChild(3).position)));
        List<Vector3Int> l = gameObject.GetComponent<JobFinder>().FindPath(Vector3Int.FloorToInt(transform.position), list, null).Result.path.ToList();
        StartCoroutine(Move(l));
        while(jData.job == jobs.free)
        {
            yield return new WaitForSeconds(5); // Decrease to have better reactions
            print("Waiting");
            Task t = LookForNew();
        }
        yield return null;
    }
    public async Task LookForNew() {
        List<jobData> j = transform.parent.GetComponent<JobQueue>().getActiveJobs();
        if (j.Count > 0)
        {
            StopAllCoroutines();
            jobData jD;
            try
            {
                jD = await gameObject.GetComponent<JobFinder>().FindPath(Vector3Int.FloorToInt(transform.position), j, null);
                if (jD.ID == -1)
                {
                    StartCoroutine("Idle");
                    print("none found?");
                }
                else
                {
                    jData = j.Where(g => g.ID == jD.ID).Single();
                    j.Where(g => g.ID == jD.ID).Single().human = this;
                    jData.path = jD.path;
                    jData.human = this;
                    StartCoroutine(Move(jData.path));
                }
            }
            catch (Exception e)
            {

                Debug.LogError($"FUCK, {gameObject.name}  {e}");
            }
        }
        else StartCoroutine("Idle"); 
    }
}