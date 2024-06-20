using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Deliveries : MasterClass
{
    Human h;
    private void Awake()
    {
        h = gameObject.GetComponent<Human>(); // creates a reference to the gameObject
    }
    public IEnumerator Carry()
    {
        StartCoroutine(Take(false)); // takes resources from storage
        yield return new WaitForSeconds(1f);
        if (h.jData.objects.building.build.constructed)
        {
            h.planA = gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), new() { h.jData.objects.building.gameObject }, h).Result;
            StartCoroutine(h.Move(h.planA.path, Store()));
        }
        else if (h.inventory.ammount.Sum() == h.maxCapacity || h.jData.objects.building.GetDiff(h.inventory).ammount.Sum() == 0) // if inventory full or no additional resources to be picked up
        {
            List<jobData> jd = new();
            h.jData.job = jobs.building;
            jd.Add(h.jData);
            Task<jobData> _t = gameObject.GetComponent<PathFinder>().FindJob(ToInt(transform.position), jd); // finds path to the build
            h.jData.path = _t.Result.path;
            StartCoroutine(h.Move(h.jData.path, h.c.Build())); // Moves to the build
        }
        else
        {
            Task t = h.c.FindResources(); // finds closest useful stockpile
        }
    }

    public async Task Gather()
    {
        try
        {
            if (h.inventory.ammount.Sum() == h.maxCapacity)
            {
                List<Storage> stor = GameObject.Find("Buildings").GetComponentsInChildren<Storage>().Where(q => q.build.localRes.ammount.Sum() < q.build.capacity).ToList();
                if(stor.Count > 0)
                {
                    h.planA = new();
                    h.planA = await gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), stor.Select(q => q.gameObject).ToList(), h); // finds the closest
                    h.jData.job = jobs.store;
                    h.StopC();
                    StartCoroutine(h.Move(h.planA.path, Store()));
                    return;
                }
                else
                {
                    Debug.LogWarning("STORAGE FULL");
                    h.StopC();
                    StartCoroutine(h.Idle());
                    return;
                }
            }
            else 
            {
                List<GameObject> chunks = GameObject.Find("Chunks").GetComponentsInChildren<Chunk>().Where(g => g.human == null).Select(g => g.gameObject).ToList(); // finds all chunks with no humans attached
                if (chunks.Count > 0)
                {
                    h.planA = new();
                    h.planA = await gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), chunks, h); // finds the closest
                    if (h.planA.interest != null)
                    {
                        h.jData.job = jobs.pickup;
                        h.StopC();
                        StartCoroutine(h.Move(h.planA.path, h.Decide())); // assings pickup as a job mode and move to it
                        return;
                    }

                }
            }
            StartCoroutine(h.Idle()); // start idle

        }
        catch (Exception e)
        {
            Debug.LogError("Can't gather" + e);
            StartCoroutine(h.Idle());
        }
    }

    public IEnumerator Take(bool ignore)
    {
        GameObject storage = h.planA.interest;
        Resource diff = new();
        if(storage == null)
        {
            Task t = Gather();
            yield break;
        }
        if (!ignore) // ignore diff when not supplying constructions
        {
            diff = h.jData.objects.building.GetDiff(h.inventory); // gets diff
        }
        for (int i = 0; i < h.inventory.ammount.Length; i++) // foreach different resource
        {
            int j = 0;
            int x = 0;
            while (x < 20) // until broken
            {
                x++;
                if (diff.ammount[i] > j || ignore) // if I have less resources than is ordered
                {
                    if (h.inventory.ammount.Sum() < h.maxCapacity) // if there is space in inventory
                    {
                        if (storage.GetComponent<Building>()) // the stockpile is "Storage"
                        {
                            if (0 < storage.GetComponent<Building>().build.localRes.ammount[i])
                            {
                                storage.GetComponent<Building>().build.localRes.ammount[i]--; // removes from storage
                                h.inventory.ammount[i]++; // adds to inventory
                                j++;
                                continue;
                            }
                        }
                        else if (storage.GetComponent<Chunk>()) // the stockpile is "Chunk"
                        {
                            if (0 < storage.GetComponent<Chunk>().localRes.ammount[i])
                            {
                                storage.GetComponent<Chunk>().RemoveRes(i); // call method to remove > destroy
                                h.inventory.ammount[i]++; // adds to inventory
                                j++;
                                continue;
                            }
                        }
                    }
                }
                break;
            }
        }
        if (storage)
        {
            if (storage.GetComponent<Chunk>())
            {
                storage.GetComponent<Chunk>().human = null;
            }
            else if (storage.GetComponent<ProductionBuilding>())
            {
                List<Storage> stores = GameObject.Find("Buildings").GetComponentsInChildren<Storage>().ToList();
                stores.RemoveAll(q => q.build.capacity <= q.build.localRes.ammount.Sum());
                h.planA = h.GetComponent<PathFinder>().FindPath(ToInt(h.transform.position), stores.Select(q => q.gameObject).ToList(), h).Result;
                StartCoroutine(h.Move(h.planA.path, Store()));
            }
        }
    }
    public IEnumerator Store()
    {
        GameObject storage = h.planA.interest;
        for (int i = 0; i < h.inventory.ammount.Length; i++) // foreach different resource
        {
            int j = 0;
            while (h.inventory.ammount[i] > 0) // if there is this resource in the inventory
            {
                if (storage.GetComponent<Building>().build.localRes.ammount.Sum() < storage.GetComponent<Building>().build.capacity) // if there is space in the storage
                {
                    h.inventory.ammount[i]--; // remove from inventory
                    if (storage.GetComponent<ProductionBuilding>())
                    {
                        storage.GetComponent<ProductionBuilding>().inputResource.ammount[i]++; // add resources needed for production
                        if (storage.GetComponent<ProductionBuilding>().GetDiff(new()).ammount.Sum() == 0) storage.GetComponent<ProductionBuilding>().PauseProduction();
                    }
                    else 
                    {
                        if (storage.GetComponent<Storage>()) 
                            transform.parent.parent.GetComponent<Humans>().resources.UpdateResourceAmmount(i, 1); //adds to global counter
                        storage.GetComponent<Building>().build.localRes.ammount[i]++; // add to storage
                    }
                    j++;
                }
                else
                {
                    break;
                }
                yield return new WaitForSeconds(0.2f);
            }
            
        }
        
        if (h.jData.job == jobs.fullTime)
        {
            h.planA = gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), new() { h.jData.objects.building.gameObject }, h).Result;
            StartCoroutine(h.Move(h.planA.path, h.Decide()));
        }
        else
        {
            h.jData = new();
            StartCoroutine(h.LookForNew());
        }
    }
}
