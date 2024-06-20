using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class JobQueue : MasterClass
{
    public List<jobData> _jobs;

    private void Awake()
    {
        _jobs = new List<jobData>();
    }

    // adding data
    public void AddJob(jobData job) // creates a job order with unigue ID
    {
        job.ID = UnigueID();
        _jobs.Add(job);
        Human h = gameObject.GetComponentsInChildren<Human>().Where(q => q.jData.job == jobs.free).FirstOrDefault();
        if (h)
        {
            StartCoroutine(h.LookForNew());
        }
    }

    // removing data
    public void CancelJob(int _i) 
    {
        if(_i > 0)
        {
            jobData j = _jobs.Where(q => q.ID == _i).Single();
            if (j.human != null)
            {
                j.human.StopC();
                StartCoroutine(j.human.Idle());
            }
            if(j.objects.building)
            {
                j.objects.building.Deconstruct(j.human.transform.position);
            }
            _jobs.Remove(j);
        }
        else
        {
            Debug.LogError("no such jobs!!!");
        }
    }
    public void RemoveJob(int _i)
    {
        _jobs.Remove(_jobs.Where(q => q.ID == _i).Single());
    }
    public List<jobData> GetActiveJobs() // return jobs with no humans assinged
    {
        List<jobData> _js = _jobs.Where(g => g.human == null).ToList();
        return _js;
    }
    public int JobIndex(int id)
    {
        return _jobs.IndexOf(_jobs.Single(q => q.ID == id));
    }
    public int JobIndex(Vector3Int vec)
    {
        int i = -1;
        i = _jobs.Where(q => q.jobPos == vec).Select(q => q.ID).SingleOrDefault();
        return i == -1? -1 : i;
    }
    public int JobIndex(Building building)
    {
        int i = -1;
        i = _jobs.Where(q => q.objects.building == building).Select(q => q.ID).SingleOrDefault();
        return i == -1 ? -1 : i;
    }
    public int UnigueID() // creates a random int
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
