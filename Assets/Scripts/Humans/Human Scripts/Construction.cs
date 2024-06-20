using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Construction : MasterClass
{
    Human h;
    private void Awake()
    {
        h = gameObject.GetComponent<Human>();
    }

    public IEnumerator Build()
    {
        for (int i = 0; i < h.inventory.ammount.Length; i++) // foreach type of material 
        {
            h.jData.objects.building.build.localRes.ammount[i] += h.inventory.ammount[i]; //add to building
            h.inventory.ammount[i] = 0; // remove from inventory
            yield return new WaitForSecondsRealtime(0.05f);
        }
        h.jData.objects.building.UpdText();
        if (h.jData.objects.building.build.localRes.ammount.Sum() != h.jData.objects.building.build.cost.ammount.Sum()) // if all resources are stored in the build
        {
            FindResources();
        }
        else
        {
            yield return new WaitForSeconds(1);
            
            h.jData.objects.building.FinishBuild();
            
            h.GetComponentInParent<JobQueue>().RemoveJob(h.jData.ID);
            StartCoroutine(h.LookForNew()); // finds new
        }
    }
    public IEnumerator Demolish()
    {
        yield return new WaitForSeconds(1);
        h.jData.objects.building.Deconstruct(transform.localPosition);
        h.GetComponentInParent<JobQueue>().RemoveJob(h.jData.ID);
        StartCoroutine(h.LookForNew()); 
    }
    public void FindResources()
    {
        Resource diff = h.jData.objects.building.GetDiff(h.inventory);
        if (h.jData.objects.building.GetComponent<ProductionBuilding>() && diff.ammount.Sum() == 0)
        {
            diff = h.jData.objects.building.GetComponent<ProductionBuilding>().productionCost;
        }

        // looks what needs to be picked up
        if (diff.ammount.Sum() > 0)
        {
            List<Storage> store = GameObject.Find("Buildings").GetComponentsInChildren<Storage>().ToList();
            List<Chunk> chunks = GameObject.Find("Chunks").GetComponentsInChildren<Chunk>().ToList();
            List<GameObject> filtered = new(); 
            for (int i = 0; i < h.inventory.ammount.Length; i++) // creates a list of all posible sources of needed resources
            {
                if (diff.ammount[i] > 0)
                {
                    if (store.Count > 0)
                    {
                        filtered = filtered.Union(store.Where(q => q.build.localRes.ammount[i] > 0 && q.canStore[i]/*&& q.build.capacity > q.build.localRes.ammount.Sum()*/).Select(q => q.gameObject)).ToList();
                    }
                    if (chunks.Count > 0)
                    {
                        filtered = filtered.Union(chunks.Where(q => q.localRes.ammount[i] > 0).Select(q => q.gameObject)).ToList();
                    }
                }
            }
            if (filtered.Count > 0) // finds the one closest(if there is any...)
            {
                h.planA = gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), filtered.Select(q => q.gameObject).ToList(), h).Result;
                h.jData.job = jobs.carrying;
                h.jData.path = h.planA.path;
                h.jData.human = h;
                StartCoroutine(h.Move(h.jData.path, h.d.Carry())); // moves to the closest stockpile and takes what it needs
                return;
            }
            if (h.jData.objects.building.build.constructed) // no resources for supplying
            {
                JobQueue jQ = transform.parent.parent.GetComponent<JobQueue>();
                int i = transform.parent.parent.GetComponent<JobQueue>().JobIndex(h.jData.ID);
                jQ._jobs[i].human = null;
                jobData jd = jQ._jobs[i];
                jQ._jobs[i] = jQ._jobs.Last();
                jQ._jobs[^1] = jd;
                StartCoroutine(h.Idle());
                return;
            }
        }
        if (h.inventory.ammount.Sum() > 0 || diff.ammount.Sum() == 0) // when inventory is full or the building doesn't need anything
        {
            List<jobData> jd = new();
            h.jData.job = jobs.building;
            jd.Add(h.jData);
            jd[0] = gameObject.GetComponent<PathFinder>().FindJob(ToInt(transform.position), jd).Result;
            h.jData.path = jd[0].path;
            StartCoroutine(h.Move(h.jData.path, h.c.Build())); // finds path to the build and moves there to build and/or deposit inventory
        }
        else if (h.inventory.ammount.Sum() == 0 && diff.ammount.Sum() > 0)
        {
            transform.GetComponentInParent<JobQueue>()._jobs[transform.GetComponentInParent<JobQueue>().JobIndex(h.jData.ID)].human = null;

            StartCoroutine(h.Idle()); // empty inventory, no usable stockpiles and build doesn't have enought resources
        }
        return;
    }
}
