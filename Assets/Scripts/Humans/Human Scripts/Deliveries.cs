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
        yield return Take(false);

        if (h.jData.objects.building.build.constructed)
        {
            h.planA = gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), new() { h.jData.objects.building.gameObject }, h).Result;
            h.jData.job = jobs.store;
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
            h.c.FindResources(); // finds closest useful stockpile
        }
    }

    public async Task Gather()
    {
        try
        {
            if (h.inventory.ammount.Sum() == h.maxCapacity)
            {
                await FindStorage();
                return;
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
                if (h.inventory.ammount.Sum() > 0)
                {
                    await FindStorage();
                    return;
                }
            }
            StartCoroutine(h.Idle());
        }
        catch (Exception e)
        {
            Debug.LogError("Can't gather" + e);
            StartCoroutine(h.Idle());
        }
    }

    public async Task FindStorage()
    {
        List<Storage> storages = GameObject.Find("Buildings").GetComponentsInChildren<Storage>().Where(q => q.build.localRes.ammount.Sum() < q.build.capacity).ToList();
        if (storages.Count > 0)
        {
            List<Storage> _stores = new();
            foreach(Storage _s in storages)
            {
                for(int i = 0; i < _s.canStore.Count; i++)
                {
                    if (h.inventory.ammount[i] > 0 && _s.canStore[i])
                    {
                        _stores.Add(_s);
                        break;
                    }
                }
            }
            h.planA = new();
            h.planA = await gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), _stores.Select(q => q.gameObject).ToList(), h); // finds the closest
            h.jData.job = jobs.store;
            h.StopC();
            StartCoroutine(h.Move(h.planA.path, Store()));
            return;
        }
        else
        {
            Debug.LogError("STORAGE FULL");
            h.StopC();
            StartCoroutine(h.Idle());
            return;
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
            if (h.jData.objects.building.GetComponent<ProductionBuilding>() && diff.ammount.Sum() == 0)
            {
                diff = h.jData.objects.building.GetComponent<ProductionBuilding>().productionCost;
            }
        }
        for (int i = 0; i < h.inventory.ammount.Length; i++) // foreach different resource
        {
            int j = 0;
            int x = 0;
            
            while (x < 20) // fail-save
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
            if (storage.GetComponent<ProductionBuilding>())
            {
                if(storage.GetComponent<ProductionBuilding>().maxCapacity > storage.GetComponent<ProductionBuilding>().build.localRes.ammount.Sum())
                {
                    storage.GetComponent<ProductionBuilding>().space = true;
                    storage.transform.GetChild(1).GetChild(2).gameObject.SetActive(false);
                }
            }
        }
        if (storage)
        {
            if (storage.GetComponent<Chunk>())
            {
                storage.GetComponent<Chunk>().human = null;
                yield break;
            }
            else if (storage.GetComponent<ProductionBuilding>())
            {
                Task t = FindStorage();
            }
            storage.GetComponent<Building>().UpdText();
        }
        
    }
    public IEnumerator Store()
    {
        GameObject storage = h.planA.interest;
        bool fin = false;
        List<bool> canStore = new();
        if (storage.GetComponent<Storage>())
        canStore = storage.GetComponent<Storage>().canStore;
        for (int i = 0; i < h.inventory.ammount.Length; i++) // foreach different resource
        {
            if(canStore.Count > 0 && !canStore[i]) // if the resource is not supposed to be stored here
            {
                continue;
            }
            int j = 0;
            while (h.inventory.ammount[i] > 0) // if there is this resource in the inventory
            {
                if (storage.GetComponent<Building>().build.localRes.ammount.Sum() < storage.GetComponent<Building>().build.capacity) // if there is space in the storage
                {
                    h.inventory.ammount[i]--; // remove from inventory
                    if (storage.GetComponent<ProductionBuilding>())
                    {
                        storage.GetComponent<ProductionBuilding>().inputResource.ammount[i]++;
                        if (storage.GetComponent<ProductionBuilding>().GetDiff(new()).ammount.Sum() == 0 && fin == false)
                        {
                            fin = true;
                            storage.GetComponent<ProductionBuilding>().supplied = true;
                            storage.GetComponent<ProductionBuilding>().PauseProduction();
                            storage.transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
                            transform.parent.parent.GetComponent<JobQueue>().RemoveJob(h.jData.ID);
                        }
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
        if(storage.GetComponent<Building>())
            storage.GetComponent<Building>().UpdText();//updates text on the info window

        /*if (h.jData.job == jobs.fullTime)
        {
            h.planA = gameObject.GetComponent<PathFinder>().FindPath(ToInt(transform.position), new() { h.jData.objects.building.gameObject }, h).Result;
            StartCoroutine(h.Move(h.planA.path, h.Decide()));
        }
        else
        {*/
        if(h.inventory.ammount.Sum() > 0)
        {
            Task t = FindStorage();
            yield break;
        }
            h.jData = new();
            StartCoroutine(h.LookForNew());
        //}
    }
}
