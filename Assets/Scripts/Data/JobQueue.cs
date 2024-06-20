using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class JobQueue : MonoBehaviour
{
    public List<jobData> _jobs;

    private void Awake()
    {
        _jobs = new List<jobData>();
    }

    // adding data
    public void AddJob(jobData job) // creates a job order with unigue ID
    {
        job.ID = unigueID();
        _jobs.Add(job);
    }

    // removing data
    public void CancelJob(int _i) 
    {
        jobData j = _jobs.Where(q => q.ID == _i).Single();
        if (j.human != null) 
        {
            j.human.StopAllCoroutines();
            j.human.StartCoroutine("Idle");
        }

        _jobs.Remove(j);
    }
    public void RemoveJob(int _i)
    {
        _jobs.Remove(_jobs.Where(q => q.ID == _i).Single());
    }
    public List<jobData> getActiveJobs() // return jobs with no humans assinged
    {
        //List<jobData> j = _jobs.Where(g => g.human == null).ToList();
        //if(j.)
        List<jobData> _js = _jobs.Where(g => g.human == null).ToList();
        //_js = _js. remove unposible builds
        return _js;
    }
    public int JobIndex(int id)
    {
        return _jobs.IndexOf(_jobs.Single(q => q.ID == id));
    }
    public int JobIndex(Vector3Int vec)
    {
        return _jobs.SingleOrDefault(q => q.jobPos == vec).ID;
    }
    public int unigueID() // creates a random int
    {
        int i;
        do
        {
            i = Random.Range(0, 2000000);
        }
        while (_jobs.Where(g => g.ID == i).ToList().Count > 0);
        return i;
    }
}
