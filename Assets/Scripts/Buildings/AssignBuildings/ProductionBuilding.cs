using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class ProductionBuilding : AssignBuilding
{
    [SerializeField] public List<Human> working = new();
    [SerializeField] public Resource inputResource = new();
    [SerializeField] public Resource productionCost = new();
    [SerializeField] public Resource production = new();
    [SerializeField] public int prodTime = 20;
    [SerializeField] public float currentTime = 0;
    [SerializeField] public float workInterval = 1f;
    [SerializeField] public int maxCapacity = 30;
    [SerializeField] readonly int modifier = 1;
    public bool supplied = false;
    public bool supply = true;
    public bool space = true;
    public bool stoped = false;
    public bool running = false;
    public override void FinishBuild()
    {
        base.FinishBuild();
        if (productionCost.ammount.Sum() == 0)
        {
            supplied = true;
            supply = false;
            return;
        }
        RequestRestock();
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
                if (currentTime == 0)
                {
                    ManageInputRes();
                }
                while (currentTime < prodTime) // timer
                {
                    if (working.Count == 0) yield break;
                    yield return new WaitForSeconds(workInterval);
                    currentTime += working.Count * modifier;
                    if (build.selected)
                    {
                        GameObject.Find("Production button").GetComponent<ProductionButton>().UpdateButtonState(currentTime, prodTime);
                    }
                }
                Product();
                if (build.localRes.ammount.Sum() + production.ammount.Sum() >= maxCapacity) // if next production whould cause overflow 
                {
                    if (build.localRes.ammount.Sum() >= maxCapacity)
                    {
                        transform.GetChild(1).GetChild(2).gameObject.SetActive(true);
                        space = false;
                        PauseProduction();
                    }
                }
                currentTime = 0;
                if(build.selected)
                GameObject.Find("Production button").GetComponent<ProductionButton>().UpdateButtonState(currentTime, prodTime);
            }
        }
        running = false;
    }

    private void ManageInputRes()
    {
        if (supply)
        {
            Resource r = GetDiff(new());
            RequestRestock();
            currentTime = 0.1f;
            if (r.ammount.Sum() == 0)
            {
                for (int i = 0; i < inputResource.ammount.Length; i++)
                {
                    inputResource.ammount[i] = inputResource.ammount[i] - productionCost.ammount[i];
                }
                UpdText();
            }
            else
            {
                transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                supplied = false;
                PauseProduction();
            }
        }
    }
    protected virtual void Product()
    {
        string s = "";
        for (int j = 0; j < production.ammount.Length; j++) // adds production yields to storage 
        {
            build.localRes.ammount[j] += production.ammount[j];
            if (build.localRes.ammount[j] > 0)
            {
                
                s += $"{build.localRes.names[j]}: {build.localRes.ammount[j]}";
            }
        }
        if (build.selected)
        {
            GameObject.Find("Generated Resource").GetComponent<TMP_Text>().text = s;
        }
        RequestPickup();
    }

    public bool StopProduction()
    {
        stoped = !stoped;
        transform.GetChild(1).GetChild(0).gameObject.SetActive(stoped);
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

    public void RequestRestock()
    {
        GameObject.Find("Humans").GetComponent<JobQueue>().AddJob(new(jobs.store, ToInt(transform.position), new(this)));
    }
    public void RequestPickup()
    {
        if(build.localRes.ammount.Sum() > 0)
        {
            GameObject.Find("Humans").GetComponent<JobQueue>().AddJob(new(jobs.pickup, ToInt(transform.position), new(this)));
        }
    }

    public override Resource GetDiff(Resource r)
    {
        Resource a = new();
        if (!build.constructed)
        {
            return base.GetDiff(r);
        }
        else
        {
            for (int i = 0; i < productionCost.ammount.Length; i++) // foreach different resource
            {
                a.ammount[i] = productionCost.ammount[i] - (inputResource.ammount[i] + r.ammount[i]); // cost - (delivered + inventory)
                a.ammount[i] = a.ammount[i] < 0 ? 0 : a.ammount[i];
            }
            return a;
        }       
    }
}