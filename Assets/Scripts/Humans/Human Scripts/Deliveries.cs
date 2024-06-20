using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Deliveries : MonoBehaviour
{
    Human h;
    private void Awake()
    {
        h = gameObject.GetComponent<Human>(); // creates a reference to the gameObject
    }
    public IEnumerator Carry()
    {
        Take(false); // takes resources from storage
        yield return new WaitForSeconds(1f);
        if (h.inventory.ammount.Sum() == h.maxCapacity || h.jData.objects.building.getDiff(h.inventory).ammount.Sum() == 0) // if inventory full or no additional resources to be picked up
        {
            List<jobData> jd = new();
            h.jData.job = jobs.building;
            jd.Add(h.jData);
            Task<jobData> _t = gameObject.GetComponent<JobFinder>().FindPath(Vector3Int.FloorToInt(transform.position), jd); // finds path to the build
            h.jData.path = _t.Result.path;
            StartCoroutine(h.Move(h.jData.path)); // Moves to the build
        }
        else
        {
            Task t = h.c.findResources(); // finds closest useful stockpile
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
                    h.planA = await gameObject.GetComponent<PathFinder>().prep(Vector3Int.FloorToInt(transform.position), stor.Select(q => q.gameObject).ToList(), h.maxCapacity, h); // finds the closest
                    h.jData.job = jobs.store;
                    h.StopC();
                    h.StartCoroutine(h.Move(h.planA.path));
                    return;
                }
                else
                {
                    Debug.LogWarning("STORAGE FULL");
                    h.StopC();
                    h.StartCoroutine(h.Idle());
                    return;
                }
            }
            else 
            {
                List<GameObject> chunks = GameObject.Find("Chunks").GetComponentsInChildren<Chunk>().Where(g => g.human == null).Select(g => g.gameObject).ToList(); // finds all chunks with no humans attached
                if (chunks.Count > 0)
                {
                    h.planA = new();
                    h.planA = await gameObject.GetComponent<PathFinder>().prep(Vector3Int.FloorToInt(transform.position), chunks, h.maxCapacity, h); // finds the closest
                    if (h.planA.path.Count > 0)
                    {
                        h.jData.job = jobs.pickup;
                        h.StopC();
                        StartCoroutine(h.Move(h.planA.path)); // assings pickup as a job mode and move to it
                        return;
                    }

                }
            }
            StartCoroutine(h.Idle()); // start idle

        }
        catch (Exception e)
        {
            Debug.LogError("Can't gather" + e);
        }
    }

    public void Take(bool ignore)
    {
        GameObject storage = h.planA.interests[0];
        Resource diff = new Resource();
        if(storage == null)
        {
            Task t = Gather();
            return;
        }
        if (!ignore)
        {
            diff = h.jData.objects.building.getDiff(h.inventory); // gets diff
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
                        if (storage.GetComponent<Storage>()) // the stockpile is "Storage"
                        {
                            if (0 < storage.GetComponent<Storage>().build.localRes.ammount[i])
                            {
                                storage.GetComponent<Storage>().build.localRes.ammount[i]--; // removes from storage
                                h.inventory.ammount[i]++; // adds to inventory
                                j++;
                                continue;
                            }
                        }
                        else if (storage.GetComponent<Chunk>()) // the stockpile is "Chunk"
                        {
                            if (0 < storage.GetComponent<Chunk>().localRes.ammount[i])
                            {
                                storage.GetComponent<Chunk>().removeRes(i); // call method to remove > destroy
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
                storage = null;
            }
        }
    }
    public void Store()
    {
        GameObject storage = h.planA.interests[0];
        for (int i = 0; i < h.inventory.ammount.Length; i++) // foreach different resource
        {
            int j = 0;
            while (h.inventory.ammount[i] > 0) // if there is this resource in the inventory
            {
                if (storage.GetComponent<Storage>().build.localRes.ammount.Sum() < storage.GetComponent<Storage>().build.capacity) // if there is space in the storage
                {
                    storage.GetComponent<Storage>().build.localRes.ammount[i]++; // add to storage
                    h.inventory.ammount[i]--; // remove from inventory
                    j++;
                }
                else
                {
                    break;
                }
            }
        }
        h.jData = new();
        Task t = h.LookForNew();
    }
}
