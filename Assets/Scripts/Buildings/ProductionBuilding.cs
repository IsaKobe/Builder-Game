using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class ProductionBuilding : AssignBuilding
{
    [SerializeField] public List<Human> working = new();
    [SerializeField] public Resource inputResource = new();
    [SerializeField] public Resource productionCost = new();
    [SerializeField] Resource production = new();
    [SerializeField] public int prodTime = 20;
    [SerializeField] public int currentTime = 0;
    [SerializeField] public float workInterval = 1f;
    [SerializeField] int maxCapacity = 30;
    [SerializeField] readonly int modifier = 1;
    public bool supplied = false;
    public bool space = true;
    public bool stoped = false;
    public bool running = false;
    public override void FinishBuild()
    {
        base.FinishBuild();
    //    RequestRestock();
        // StartCoroutine(Produce());
    }
    public void Work(Human h)
    {
        working.Add(h);
        if (working.Count == 1) // nobody is working right now
        {
            StartCoroutine(Produce()); // start production
        }

    }
    IEnumerator Produce()
    {
        running = true;
        if (!stoped)
        {
            while (working.Count > 0)
            {
                /*if (currentTime == 0)
                {
                    Resource r = GetDiff(new());
                    //RequestRestock();
                    if (r.ammount.Sum() == 0)
                    {
                        for (int i = 0; i < inputResource.ammount.Length; i++)
                        {
                            inputResource.ammount[i] = inputResource.ammount[i] - productionCost.ammount[i];
                        }
                    }
                    else
                    {
                        transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
                        supplied = false;
                        PauseProduction();
                    }
                }*/

                while (currentTime < prodTime && working.Count > 0) // timer
                {
                    yield return new WaitForSeconds(workInterval);
                    currentTime += working.Count * modifier;
                    if (build.selected)
                    {
                        GameObject.Find("Production button").GetComponent<ProductionButton>().UpdateButtonState(currentTime, prodTime);
                    }
                }

                for (int j = 0; j < production.ammount.Length; j++) // adds production yields to storage 
                {
                    build.localRes.ammount[j] += production.ammount[j];
                }

                if (build.localRes.ammount.Sum() + production.ammount.Sum() >= maxCapacity) // if next production whould cause overflow 
                {
                    RequestPickup();
                    /*if (build.localRes.ammount.Sum() >= maxCapacity)
                    {
                        transform.GetChild(2).GetChild(2).gameObject.SetActive(true);
                        space = false;
                        PauseProduction();
                    }*/
                }
                currentTime = 0;
            }
        }
        running = false;
    }
    public bool StopProduction()
    {
        stoped = !stoped;
        transform.GetChild(2).GetChild(0).gameObject.SetActive(stoped);
        PauseProduction();
        return stoped;
    }
    public void PauseProduction()
    {
        if (stoped || !space || !supplied)
        {
            StopAllCoroutines();
            running = false;
        }
        else if (!running)
        {
            StartCoroutine(Produce());
        }
    }

    /*public void RequestRestock()
    {
        GameObject.Find("Humans").GetComponent<JobQueue>().AddJob(new(jobs.store, ToInt(transform.position), new(this)));
    }*/
    public void RequestPickup()
    {
        GameObject.Find("Humans").GetComponent<JobQueue>().AddJob(new(jobs.pickup, ToInt(transform.position), new(this)));
    }

    public override Resource GetDiff(Resource r)
    {
        if (!build.constructed)
        {
            return base.GetDiff(r);
        }
        else
        {
            for (int i = 0; i < productionCost.ammount.Length; i++) // foreach different resource
            {
                r.ammount[i] = productionCost.ammount[i] - (inputResource.ammount[i] + r.ammount[i]); // cost - (delivered + inventory)
                r.ammount[i] = r.ammount[i] < 0 ? 0 : r.ammount[i];
            }
            return r;
        }       
    }
}