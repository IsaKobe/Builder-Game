using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PathFinder : MonoBehaviour
{
    List<List<Vector3Int>> paths;
    List<Vector3Int> visited;
    List<int> toBeRemoved;
    int index;
    Plan plan;
    List<Vector3Int> pos;

    int capacity;
    int check;
    bool fin;
    public async Task<Plan> prep(Vector3Int _start, List<GameObject> objects, int maxInventory, Human h)
    {
        pos = objects.Select(g => Vector3Int.FloorToInt(g.transform.position)).ToList();
        plan = new();
        fin = false;
        capacity = maxInventory > objects.Count ? objects.Count : maxInventory;
        await FindPath(_start);
        plan.interests.Add(objects[index]);
        if (objects[index].GetComponent<Chunk>() && plan.path.Count > 0)
        {
            objects[index].GetComponent<Chunk>().AssingH(h);
        }
        return plan;
    }
    public async Task FindPath(Vector3Int _start)
    {
        visited = new();
        paths = new();
        toBeRemoved = new();
        paths.Add(new());
        paths[0].Add(_start);
        int i = 0;
        if (await Check(_start, 0))
        {
            while (i < 30 && paths.Count > 0 && !fin)
            {
                int j;
                toBeRemoved = new();

                for (j = 0; j < paths.Count; j++)
                {
                    if (!fin)
                    {
                        await CheckMove(j);
                    }
                    else
                    {
                        break;
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
            Vector3Int vec = paths[j][paths[j].Count - 1];
            check = 0;
            List<GameObject> g = Physics.OverlapSphere(new Vector3(vec.x + 0.5f, vec.y, vec.z + 0.5f), 0.5f).Select(c => c.gameObject).ToList();
            g.RemoveAll(s => s.name != "Road");
            List<Vector3Int> road = g.Select(c => Vector3Int.FloorToInt(c.transform.position)).ToList();
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
                    foreach (var l in visited)
                    {
                        if (l == checkVec)
                        {
                            Visited = true;
                            break;
                        }
                    }
                    if (Visited == false)
                    {
                        if (await Check(checkVec, j))
                        {
                            if (check == 0)
                            {
                                paths[j].Add(checkVec);
                                check++;
                            }
                            else
                            {
                                paths.Add(paths[j].ToList());
                                paths[paths.Count - 1][paths[paths.Count - 1].Count - 1] = checkVec;
                                check++;
                            }
                            visited.Add(checkVec);
                        }
                        else
                        {
                            return;
                        }
                    }
                }
                else if (pos.Contains(checkVec))
                {
                    if (check != 0)
                    {
                        paths[j].RemoveAt(paths[j].Count - 1);
                    }
                    paths[j].Add(checkVec);
                    paths[j].RemoveAt(0);
                    plan.path.AddRange(paths[j].ToList());
                    fin = true;
                    index = pos.IndexOf(checkVec);
                    return;
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

    async Task<bool> Check(Vector3Int checkVec, int j)
    {
        int id = pos.IndexOf(checkVec);
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
                    pos = GameObject.Find("Buildings").GetComponentsInChildren<Storage>().Select(g => Vector3Int.FloorToInt(g.transform.position)).ToList();
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
}