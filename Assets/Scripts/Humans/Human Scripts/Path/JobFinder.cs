using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class JobFinder : MonoBehaviour
{
    List<List<Vector3Int>> paths;
    List<Vector3Int> visited;
    List<Vector3Int> path;
    List<int> toBeRemoved;

    Vector3Int start;
    List<jobData> finish;

    bool search;
    int id;
    public async Task<jobData> FindPath(Vector3Int _start, List<jobData> _jobs)
    {
        this.start = _start;
        visited = new List<Vector3Int>();
        finish = _jobs.ToList();
        paths = new List<List<Vector3Int>>();
        paths.Add(new List<Vector3Int>());
        paths[0].Add(start);
        int i = 0;
        id = -1;
        search = true;

        foreach (var f in finish)
        {
            if (Vector3.Distance(_start, f.jobPos) == 1)
            {
                search = false;
                path = new List<Vector3Int>();
                path.Add(_start);
                id = i;
                //print($"Found it!{_start}");
            }
            i++;
        }
        i = 0;
        while (search && i < 30 && paths.Count >= 1)
        {
            int j = 0;
            toBeRemoved = new List<int>();
            foreach (List<Vector3Int> vecs in paths.ToList())
            {
                if (vecs != null && search)
                {
                    await CheckMove(j);
                }
                j++;
            }
            for (int x = toBeRemoved.Count - 1; x >= 0; x--)
            {
                int y = toBeRemoved[x];
                paths.RemoveAt(y);
            }
            i++;
        }
        if (path != null && path.Count > 0)
        {
            //print($"path: {path[path.Count - 1]}");
            //print($"finish:{finish[0]}");
            //print(id);
            if (id >= 0)
            {
                if (_jobs[id].job != jobs.free)
                {
                    id = _jobs[id].ID;
                }
            }
        }
        return new jobData(_path: path, _ID: id);
    }

    public async Task CheckMove(int j)
    {

        Vector3Int vec = paths[j][paths[j].Count - 1];
        int x = 0;
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
                        //print($"I Remember!{checkVec}, {x}");
                        Visited = true;
                        break;
                    }
                }
                if (Visited == false)
                {
                    foreach (var f in finish)
                    {
                        if (f.job == jobs.building)
                        {
                            if (Mathf.Abs(checkVec.x - f.jobPos.x) <= (f.objects.building.build.sizeX / 2) && Mathf.Abs(checkVec.z - f.jobPos.z) <= (f.objects.building.build.sizeZ / 2)) // TODO: continue
                            {
                                search = false;
                                //print($"Found it!{checkVec}");
                                if (x == 0)
                                {
                                    paths[j].Add(checkVec);
                                    path = paths[j].ToList();
                                }
                                else
                                {
                                    paths.Add(paths[j].ToList());
                                    paths[paths.Count - 1][paths[paths.Count - 1].Count - 1] = checkVec;
                                    path = paths[paths.Count - 1].ToList();
                                }
                                id = finish.IndexOf(f);
                                return;
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(checkVec, f.jobPos) <= 1)
                            {
                                search = false;
                                //print($"Found it!{checkVec}");
                                if (x == 0)
                                {
                                    paths[j].Add(checkVec);
                                    path = paths[j].ToList();
                                }
                                else
                                {
                                    paths.Add(paths[j].ToList());
                                    paths[paths.Count - 1][paths[paths.Count - 1].Count - 1] = checkVec;
                                    path = paths[paths.Count - 1].ToList();
                                }
                                id = finish.IndexOf(f);
                                return;
                            }
                        }
                    }

                    if (x == 0)
                    {
                        paths[j].Add(checkVec);
                        x++;
                    }
                    else
                    {
                        paths.Add(paths[j].ToList());
                        paths[paths.Count - 1][paths[paths.Count - 1].Count - 1] = checkVec;
                        x++;
                    }
                    visited.Add(checkVec);
                }

            }
            // print($"x: {x}");
        }
        if (x == 0)
        {
            toBeRemoved.Add(j);
        }
    }
}