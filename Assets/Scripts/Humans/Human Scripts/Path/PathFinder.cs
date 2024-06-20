using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PathFinder : MasterClass
{
    List<List<Vector3Int>> paths;
    List<Vector2Int> visited;
    List<int> toBeRemoved;
    int index;
    Plan plan;
    List<Vector2Int> pos;
    List<int> entryPoints;

    int check;
    bool fin;
    

    public async Task<Plan> FindPath(Vector3Int _start, List<GameObject> objects, /*int maxInventory,*/ Human h)
    {
        await Prep(_start, objects);
        plan.interest = objects[entryPoints[index]];

        if (objects[entryPoints[index]].GetComponent<Chunk>())
        {
            objects[entryPoints[index]].GetComponent<Chunk>().AssingH(h);
        }
        else
        {
            if(plan.path.Count > 0)
            {
                plan.path.Add(LastStep(plan.path.Last(), plan.interest, 1));
            }
        }
        return plan;
    }

    public async Task<jobData> FindJob(Vector3Int _start, List<jobData> _jobs)
    {
        List<GameObject> objects = new();
        if(_jobs.Where(q => (q.job == jobs.building || q.job == jobs.demolishing || q.job == jobs.pickup || q.job == jobs.store)).ToList().Count > 0)
        {
            objects.AddRange(_jobs.Select(q => q.objects.building.gameObject).ToList());
        }
        if(_jobs.Where(q => q.job == jobs.digging).ToList().Count > 0)
        {
            objects.AddRange(_jobs.Select(q => q.objects.r.gameObject).ToList());
        }

        await Prep(_start, objects);
        if(index > -1)
        {
            int id = _jobs[entryPoints[index]].ID;
            if (_jobs[entryPoints[index]].job == jobs.digging) 
            {
       //         plan.path.RemoveAt(plan.path.Count-1);
            }
            return new jobData(_path: plan.path, _ID: id);
        }
        return new jobData(_path: null, _ID: -1);
    }
    public async Task Prep(Vector3Int _start, List<GameObject> objects)
    { 
        index = -1;
        entryPoints = new();
        pos = new();
        
        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i].GetComponent<Building>() != null)
            {
                for (int j = 0; j < objects[i].transform.childCount; j++)
                {
                    pos.Add(GetVec(ToInt(objects[i].transform.GetChild(j).position)));
                    entryPoints.Add(i);
                }
            }
            else
            {
                if(objects[i].GetComponent<Rock>() != null)
                {
                    Vector2Int v = GetVec(ToInt(objects[i].transform.position));
                    pos.Add(new Vector2Int(v.x + 1, v.y));
                    pos.Add(new Vector2Int(v.x - 1, v.y));
                    pos.Add(new Vector2Int(v.x, v.y + 1));
                    pos.Add(new Vector2Int(v.x, v.y - 1));
                    for (int j = 0; j < 4; j++)
                    {
                        entryPoints.Add(i);
                    }
                }
                else
                {
                    pos.Add(GetVec(ToInt(objects[i].transform.position)));
                    entryPoints.Add(i);
                }
                
            }
            if (GetVec(_start) == GetVec(ToInt(objects[i].transform.localPosition)))
            {
                plan.path = new();
                plan.interest = objects[i];
                index = i;
                return;
            }
        }
        plan = new();
        fin = false;
        await FindPath(_start);
        print($"{_start}\n{plan.path}");
    }

    public async Task FindPath(Vector3Int _start)
    {
        visited = new();
        paths = new();
        toBeRemoved = new();
        paths.Add(new());
        paths[0].Add(new Vector3Int(_start.x, 0, _start.z));
        int i = 0;
        if (Check(_start, 0))
        {
            Building b = Physics.RaycastAll(new Vector3(_start.x + 0.5f, _start.y+1, _start.z + 0.5f), Vector3.down, 2f).Where(q => q.collider.GetComponent<Building>()).Select(q => q.collider.GetComponent<Building>()).FirstOrDefault();
            if (b != null) 
            {
                paths[0].Add(LastStep(_start, b.gameObject, -1));
            }

            while (i < 30 && paths.Count > 0 && !fin)
            {
                toBeRemoved = new();
                int c = paths.Count;
                for (int j = 0; j < c; j++)
                {
                    if (!fin)
                    {
                        await CheckMove(j);
                    }
                    else
                    {
                        return;
                    }

                }
                if (paths.Count > 0)
                {
                    for (int x = toBeRemoved.Count - 1; x >= 0; x--)
                    {
                        int y = toBeRemoved[x];
                        paths.RemoveAt(y);
                    }
                }
                i++;
            }
        }
    }
    public async Task CheckMove(int j)
    {
        try
        {
            Vector3Int vec = paths[j][^1];
            check = 0;
            List<GameObject> g = Physics.OverlapSphere(new Vector3(vec.x + 0.5f, 0, vec.z + 0.5f), 0.5f).Select(c => c.gameObject).ToList();
            g.RemoveAll(s => s.name != "Road");
            List<Vector3Int> road = g.Select(c => ToInt(c.transform.position)).ToList();
            for (int i = 0; i < 4; i++)
            {
                Vector3Int checkVec = new();
                switch (i)
                {
                    case 0:
                        checkVec = new(vec.x + 1, 0, vec.z);
                        break;
                    case 1:
                        checkVec = new(vec.x - 1, 0, vec.z);
                        break;
                    case 2:
                        checkVec = new(vec.x, 0, vec.z + 1);
                        break;
                    case 3:
                        checkVec = new(vec.x, 0, vec.z - 1);
                        break;
                }
                if (road.Contains(checkVec))
                {
                    bool Visited = false;
                    Vector2Int checkV = GetVec(checkVec);
                    foreach (var l in visited)
                    {
                        if (l == checkV)
                        {
                            Visited = true;
                            break;
                        }
                    }
                    if (Visited == false)
                    {
                        if (Check(checkVec, j))
                        {
                            if (check == 0)
                            {
                                paths[j].Add(checkVec);
                                check++;
                            }
                            else
                            {
                                paths.Add(paths[j].ToList());
                                paths[^1][^1] = checkVec;
                                check++;
                            }
                            visited.Add(checkV);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
            }
            if (check == 0)
            {
                toBeRemoved.Add(j);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("haha it's null you dumb ass: " + e);
        }

    }

    bool Check(Vector3Int checkVec, int j)
    {
        int id = pos.IndexOf(GetVec(checkVec));
        if (id > -1 && index != id)
        {
            if (check != 0)
            {
                paths[j].RemoveAt(paths[j].Count - 1);
            }
            paths[j].Add(checkVec);
            paths[j].RemoveAt(0);
            plan.path.AddRange(paths[j].ToList());
            index = id;
            //if (lookForFirst == true)
            //{
            fin = true;
            /*}
            else
            {
                capacity--;
                if (capacity == 0)
                {
                    pos = GameObject.Find("Buildings").GetComponentsInChildren<Storage>().Select(g => ToInt(g.transform.position)).ToList();
                }
                await FindPath(paths[j][paths[j].Count - 1]);
            }*/
            return false;

        }
        else
        {
            return true;
        }
    }
    Vector2Int GetVec(Vector3Int v)
    {
        Vector2Int vec;
        vec = Vector2Int.FloorToInt(new Vector2(v.x, v.z));
        return vec;
    }
    Vector3Int LastStep(Vector3Int vec, GameObject build, int mod)
    {
        switch (build.transform.eulerAngles.y)
        {
            case 0:
                vec.z += (1 * mod);
                break;
            case 90:
                vec.x += (1 * mod);
                break;
            case 180:
                vec.z -= (1 * mod);
                break;
            case 270:
                vec.x -= (1 * mod);
                break;
        }
        return new(vec.x, 0, vec.z);
    }
}