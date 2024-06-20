using System;
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
    List<GameObject> objects;
    List<Vector2Int> pos;
    List<int> entryPoints;

    int check;
    bool fin;

    public async Task<Plan> FindPath(Vector3Int _start, List<GameObject> objects, /*int maxInventory,*/ Human h)
    {
        await Prep(_start, objects);
        plan.interest = objects[entryPoints[index]];

        if (objects[entryPoints[index]].GetComponent<Chunk>()) // it's a chunk
        {
            objects[entryPoints[index]].GetComponent<Chunk>().AssingH(h);
        }
        else // it's a building
        {
            if (plan.path.Count > 0)
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
            Building b = _jobs[entryPoints[index]].objects.building;
            if (b != null)
            {
                plan.path.Add(LastStep(plan.path.Count > 0 ? plan.path[^1]: new(_start.x, 0, _start.z), b.gameObject, 1));
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
                Transform tran = objects[i].transform.GetChild(0);
                for (int j = 0; j < tran.childCount; j++)
                {
                    pos.Add(GetVec(ToInt(tran.GetChild(j).position)));
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
            if (GetVec(_start) == GetVec(ToInt(objects[i].transform.localPosition))) // when on the same tile as the building
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
        check = 0;
        visited = new();
        paths = new();
        toBeRemoved = new();
        paths.Add(new());
        paths[0].Add(new Vector3Int(_start.x, 0, _start.z));
        int i = 0;
        Building b = Physics.RaycastAll(new Vector3(_start.x + 0.5f, _start.y + 2, _start.z + 0.5f), Vector3.down, 3f).Where(q => q.collider.GetComponent<Building>()).Select(q => q.collider.GetComponent<Building>()).FirstOrDefault();
        if (b != null)
        {
            Vector3Int vec = LastStep(_start, b.gameObject, -1);
            if(Check(vec, 0))
            {
                paths[0].Add(vec);
            }
        }
        if (!fin && Check(paths[0][^1], 0)) // Am I standing on an entry point or next to the job
        {
            //Am I startring on a building
            while (i < 30 && paths.Count > 0 && !fin) // when finished, when no paths, when out of range
            {
                toBeRemoved = new();
                int c = paths.Count;
                for (int j = 0; j < c; j++) // foreach active path
                {
                    if (!fin)
                    {
                        await CheckMove(j); // check if there is anywhere to continue from the last tile of seleted path
                    }
                    else
                    {
                        return;
                    }
                }
                if (paths.Count > 0) // removes no longer active paths
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
    public Task CheckMove(int j)
    {
        try
        {
            Vector3Int vec = paths[j][^1];
            check = 0;
            for (int i = 0; i < 4; i++) // checks in every direction
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
                if (CanEnter(checkVec)) // if there is a road on the new position
                {
                    if (!visited.Contains(GetVec(checkVec))) // checks if already visited
                    {
                        if (Check(checkVec, j)) // if nothing found
                        {
                            if (check == 0) // if first then add
                            {
                                paths[j].Add(checkVec);
                                check++;
                            }
                            else // else create a new brench
                            {
                                paths.Add(paths[j].ToList());
                                paths[^1][^1] = checkVec;
                                check++;
                            }
                            visited.Add(GetVec(checkVec));
                        }
                        else
                        {
                            return Task.CompletedTask;
                        }
                    }
                }
            }
            if (check == 0)
            {
                toBeRemoved.Add(j); // if none found from this position delete the whole path
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("haha it's null you dumbass: " + e); // log error
        }

        return Task.CompletedTask;
    }

    bool CanEnter(Vector3Int vec)
    {
        List<GameObject> g = Physics.RaycastAll(new Vector3(vec.x + 0.5f, 2, vec.z + 0.5f), Vector3.down, 2.5f).Select(c => c.collider.gameObject).ToList();
        if(g.Where(q => q.GetComponent<Building>()).Count() > 0)
        {
            // don't do anything TODO: ROCKS?
        }
        else if(g.Where(q => q.GetComponent<Road>()).Count() > 0)
        {
            return true;
        }
        return false;
    }

    bool Check(Vector3Int checkVec, int j)
    {
        int id = pos.IndexOf(GetVec(checkVec));
        if (id > -1)//&& index != id) // if there is an entry point or a job on the checkVec
        {
            if (check != 0) // removes the last element of the path if there already is one
            {
                paths[j].RemoveAt(paths[j].Count-1);
            }
            paths[j].Add(new(checkVec.x, 0, checkVec.z)); // add the new pos to the path
            paths[j].RemoveAt(0);
            plan.path.AddRange(paths[j].ToList());
            index = id;
            fin = true;
            return false;
        }
        else
        {
            return true;
        }
    }
    Vector2Int GetVec(Vector3Int v) // converts vector3Int to Vector2Int
    {
        Vector2Int vec;
        vec = Vector2Int.FloorToInt(new Vector2(v.x, v.z));
        return vec;
    }
    Vector3Int LastStep(Vector3Int _vec, GameObject build, int mod)
    {
        Vector3Int vec = new(_vec.x, 0, _vec.z);
        switch (build.transform.eulerAngles.y) // switch(build rotation)
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