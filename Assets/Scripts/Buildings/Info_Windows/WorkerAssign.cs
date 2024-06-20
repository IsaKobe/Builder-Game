using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System.Threading.Tasks;

public class WorkerAssign : MasterClass
{
    [SerializeField] GameObject addPrefab;
    [SerializeField] GameObject removePrefab;
    GameObject humans;
    public AssignBuilding _building;

    public void FillStart(AssignBuilding _build)
    {
        _building = _build;
        humans = GameObject.Find("Humans");
        Transform ContentWorkers = transform.GetChild(0).GetChild(0).GetChild(0).transform;
        Transform ContentAssign = transform.GetChild(1).GetChild(0).GetChild(0).transform;

        Fill(ContentWorkers, _build.assigned, removePrefab);
        if (_build.GetComponent<ProductionBuilding>())
        {
            Fill(ContentAssign, humans.transform.GetChild(0).GetComponentsInChildren<Human>().ToList(), addPrefab);
        }
        else
        {
            List<Human> humen = humans.transform.GetChild(0).GetComponentsInChildren<Human>().ToList();
            humen.AddRange(humans.transform.GetChild(1).GetComponentsInChildren<Human>().ToList());
            humen.RemoveAll(q => _build.assigned.Contains(q));
            Fill(ContentAssign, humen, addPrefab);
        }
        
        gameObject.SetActive(true);
    }

    public void Fill(Transform listView, List<Human> humans, GameObject pref)
    {
        List<string> rendered = new();
        for (int i = 0; i < listView.childCount; i++)
        {
            rendered.Add(listView.GetChild(i).GetChild(0).GetComponent<TMP_Text>().text);
        }
        List<string> oldItems = rendered.ToList();
        foreach (Human h in humans)
        {
            oldItems.Remove(h.name);
        }
        foreach (Human h in humans)
        {
            int i = rendered.IndexOf(h.name);
            Transform g;
            if (i == -1)
            {
                if (oldItems.Count > 0)
                {
                    g = listView.GetChild(rendered.IndexOf(oldItems[0]));
                    oldItems.RemoveAt(0);
                }
                else
                {
                    g = Instantiate(pref, listView.transform).transform;
                }
                g.GetChild(0).GetComponent<TMP_Text>().text = h.name;
            }
            else
            {
                g = listView.GetChild(i);
                
            }
            g.GetChild(1).GetComponent<TMP_Text>().text = h.specialization.ToString();
            g.GetChild(2).GetComponent<TMP_Text>().text = h.jData.job.ToString();
            g.GetComponent<InfoButtons>().id = h.id;
        }
        foreach(string name in oldItems)
        {
            Destroy(listView.GetChild(rendered.IndexOf(name)).gameObject);
        }
    } // filling the table with human elements
    public async Task ManageHuman(int id, bool add) //adding or removing humans
    {
        if (add && _building.limit <=  _building.assigned.Count)
        {
            print("no space");
            return;
        }
        ProductionBuilding prodBuilding = _building.GetComponent<ProductionBuilding>();
        if (prodBuilding)
        {
            if (add)
            {
                
                Human human = humans.transform.GetChild(0).GetComponentsInChildren<Human>().Single(q => q.id == id); // get human reference
                human.StopC();
                prodBuilding.assigned.Add(human);
                human.transform.SetParent(humans.transform.GetChild(1).transform);

                Plan plan = await human.GetComponent<PathFinder>().FindPath(
                    ToInt(human.transform.position),
                    new List<GameObject>() { _building.gameObject },
                    human);
                human.jData.job = jobs.fullTime;
                human.jData.path = plan.path;
                human.jData.objects = new(_building);
                human.jData.jobPos = ToInt(plan.interest.transform.position);
                human.StartCoroutine(human.Move(plan.path, human.Decide()));
            }
            else
            {
                
                Human human = humans.transform.GetChild(1).GetComponentsInChildren<Human>().Single(q => q.id == id);
                human.StopC();
                prodBuilding.assigned.Remove(human);
                human.transform.SetParent(humans.transform.GetChild(0).transform);
                human.jData.job = jobs.free;
                human.StartCoroutine(human.Idle());
            }
        }
        else if (_building.GetComponent<House>())
        {
            Human human = humans.transform.GetChild(0).GetComponentsInChildren<Human>().Single(q => q.id == id);
            if (add)
            {
                human.home = _building.GetComponent<House>();
                _building.assigned.Add(human);
            }
            else
            {
                human.home = null;
                _building.assigned.Remove(human);
            }
            _building.UpdText();
        }
        
        FillStart(_building);
    }
}
