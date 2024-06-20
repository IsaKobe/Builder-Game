using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading.Tasks;

public class JobQueue : MonoBehaviour
{
    public List<jobData> _jobs;
    
    private void Awake()
    {
        _jobs = new List<jobData>();
        
    }

    // adding data
    public void AddJob(jobData job)
    {
        _jobs.Add(job);
    }

    // removing data
    public void RemoveJob(int _i)
    {
        _jobs.Remove(_jobs.Where(g => g.ID == _i).Single());
    }
    public List<jobData> getActiveJobs()
    {
        return _jobs.Where(g => g.human == null).ToList();
    }
    public int unigueID()
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
