using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public enum SelectionMode
{
    nothing,
    cancel,
    dig,
    build,
    destroy
}
public class GridTiles : MasterClass
{
    [Header("Tilemaps")]
    public GameObject roadPrefab;
    public GameObject buildingPrefab;
    public Resources resources;
    public SelectionMode selMode = SelectionMode.nothing;
    Vector3Int activePos;
    public List<Rock> markedTiles;
    public List<Rock> toBeDigged = new();
    public GameObject build;
    public bool drag = false;
    public GameObject chunk;
    public Building activeBuild;
    public Building selectedObject;
    public Color highlight;
    public Color prevHighlight;
    Vector3 startPos;

    // Start is called before the first frame update
    public void Enter(Vector3Int vec, GameObject g)
    {
        activePos = vec;
        Color c;
        switch (selMode)
        {
            case SelectionMode.nothing:
                c = highlight;
                if (g.GetComponent<Building>() != null)
                {
                    selectedObject = g.GetComponent<Building>();

                }
                break;

            case SelectionMode.cancel:
                if (g != null && (selectedObject = g.GetComponent<Building>()) != null)
                {
                    selectedObject.changeColor(Color.red);
                    return;
                }
                c = new Color(1, 0, 0, 0.5f);
                break;

            case SelectionMode.dig:
                c = Color.yellow;
                if (drag)
                {
                    markedTiles = CalcTiles().ToList();
                    return;
                }
                if (toBeDigged.Contains(g.GetComponent<Rock>()))
                {
                    prevHighlight = g.GetComponent<MeshRenderer>().material.GetColor("_EmissionColor");
                    g.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", prevHighlight + c / 2);
                    return;
                }
                break;

            case SelectionMode.build:
                build.transform.position = new Vector3(activePos.x, 1, activePos.z + (build.GetComponent<Building>().build.sizeZ / 2));
                build.GetComponent<MeshRenderer>().material.color = canPlace() ? Color.blue : Color.red;
                return;

            case SelectionMode.destroy:
                return;

            default:
                Debug.LogError("assing selection mode!!!");
                return;
        }
        g.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
        g.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", c/2);

    }
    public List<Rock> CalcTiles()
    {
        ClearMarks();
        List<Rock> rocks = new();
        float x = (Mathf.FloorToInt(startPos.x) - activePos.x) / 2f;
        float z = (Mathf.FloorToInt(startPos.z) - activePos.z) / 2f;
        rocks.AddRange(Physics.OverlapBox(new Vector3(startPos.x - x, startPos.y, startPos.z - z), new(Mathf.Abs(x), 0.5f, Mathf.Abs(z))).Where(q => q.GetComponent<Rock>() != null).Select(q => q.GetComponent<Rock>()).ToList());
        List<Rock> filtered = rocks.ToList();
        foreach (Rock g in rocks)
        {
            if (toBeDigged.Contains(g))
            {
                filtered.Remove(g);
            }
            g.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
            g.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", (Color.yellow + Color.red) / 2);
        }
        return filtered;
    }
    public void ClearMarks() // deletes all tiles
    {
        if (markedTiles != null)
        {
            foreach (Rock r in markedTiles)
            {
                r.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
            }
        }
    }

    public void Exit(GameObject g) // triggers on exit
    {
        switch (selMode)
        {
            case SelectionMode.nothing:
                break;

            case SelectionMode.cancel:
                break;

            case SelectionMode.dig:
                if (drag) return;
                break;
        }
        if (selectedObject)
        {
            selectedObject.changeColor(new Color());
        }
        selectedObject = null;
        if (!toBeDigged.Contains(g.GetComponent<Rock>()))
        {
            g.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
        }
        else
        {
            g.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", (Color.yellow + Color.red) / 2);
        }
    }

    public void Down(GameObject g) // triggers on mouse down
    {
        switch (selMode)
        {
            case SelectionMode.nothing:
                // inspect? \\
                break;

            case SelectionMode.cancel:
                //List<GameObject> hit = Physics.RaycastAll(new Vector3(markedTiles[0].x + 0.5f, 1, markedTiles[0].y + 0.5f), Vector3.down, 1f).Select(q => q.collider.gameObject).ToList(); // returns all hit
                //hit.RemoveAll(q => q.GetComponent<Road>());
                
                break;

            case SelectionMode.dig:
                Rock r = g.GetComponent<Rock>();
                if (r != null)
                {
                    markedTiles = new();
                    drag = true;
                    markedTiles.Add(r);
                    startPos = g.transform.position;
                    g.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", (Color.yellow + Color.red) / 2);
                }               
                break;
        }
    }
    public void Up() // triggers on mouse up
    {
        switch (selMode)
        {
            case SelectionMode.nothing:
                if (selectedObject && activeBuild != selectedObject)
                {
                    if (activeBuild)
                    {
                        activeBuild.build.selected = false;
                    }
                    activeBuild = selectedObject;
                    GameObject infoWindow = GameObject.Find("Building Window toggle");//.transform.GetChild(0).gameObject;
                    
                    infoWindow.transform.GetChild(0).GetComponent<InfoWindow>().SetUpTextFields(selectedObject, selectedObject.name);
                    infoWindow.transform.GetChild(0).gameObject.SetActive(true);
                }
                break;

            case SelectionMode.cancel:
                /*List<GameObject> hit = Physics.RaycastAll(new Vector3(markedTiles[0].x + 0.5f, 1, markedTiles[0].y + 0.5f), Vector3.down, 1f).Select(q => q.collider.gameObject).ToList(); // returns all hit
                hit.RemoveAll(q => q.GetComponent<Road>() || q.GetComponent<Rock>());
                */
                JobQueue queue = GameObject.Find("Humans").GetComponent<JobQueue>();
                if (selectedObject)
                {
                    if (selectedObject.build.constructed)
                    {
                        if(queue._jobs.Where(q => q.objects.building == selectedObject).Count() == 0)
                        {
                            queue.AddJob(new jobData(_job: jobs.demolishing, _jobPos: ToInt(selectedObject.transform.position), _objects: new JobObjects(selectedObject)));
                            selectedObject.changeColor(Color.magenta);
                            return;
                        }
                        else
                        {
                            queue.RemoveJob(queue.JobIndex(selectedObject));
                        }
                    }
                    queue.CancelJob(queue.JobIndex(selectedObject));
                    selectedObject = null;
                    
                }
                else
                {
                    if(toBeDigged.Select(q => ToInt(q.transform.position)).Contains(activePos))
                    {
                        queue.CancelJob(queue.JobIndex(activePos));
                       // ops.SetTile(ops.WorldToCell(activePos), null);
                    }
                }
                break;
            case SelectionMode.dig:
                PrepDig();
                break;
            case SelectionMode.build:
                if(canPlace())
                {
                    FindPathBuild();
                    Blueprint();
                }
                break;
        }
    }
    public void BreakAction(Vector3 vec)
    {
        switch (selMode)
        {
            case SelectionMode.nothing:
                return;
            case SelectionMode.dig:
                drag = false;
                ClearMarks();
                markedTiles = new();
                break;
            case SelectionMode.build:
                Destroy(build);
                break;
        }
        selMode = SelectionMode.nothing;
        vec = ToHalf(vec);
        GameObject g = Physics.RaycastAll(new(vec.x, vec.y + 1, vec.z), Vector3.down, 1f).Select(q => q.collider.gameObject).OrderByDescending(q => q.transform.position.y).FirstOrDefault();
        if (g != null)
        {
            Enter(ToInt(vec), g);
        }
    }
    public void ChangeSelMode(SelectionMode mode)
    {
        if (mode != SelectionMode.build)
        {
            Destroy(build);
        }
        switch (mode)
        {
            case SelectionMode.nothing:
                break;
            case SelectionMode.cancel:
            case SelectionMode.dig:
                if (selMode == mode)
                    selMode = SelectionMode.nothing;
                else
                    selMode = mode;
                break;
            case SelectionMode.build:
                if (buildingPrefab.name != (build != null ? build.name : ""))
                {
                    Blueprint();
                    selMode = SelectionMode.build;
                }
                else
                {
                    Destroy(build);
                    selMode = SelectionMode.nothing;
                }
                break;
        }
    }
    public void RemoveTiles(Rock _r)
    {
        Vector3 vec = _r.transform.position;
        toBeDigged.Remove(_r); // removes from list
        //ops.SetTile(ToInt(new Vector3(vec.x, vec.z, 0)), null);
        GameObject _road = Instantiate(roadPrefab, new(vec.x, _r.transform.localPosition.y, vec.z), Quaternion.identity, GameObject.Find("Roads").transform); // creates a road on the place of tiles
        _road.name = "Road";
        Exit(_r.gameObject);
        Destroy(_r.gameObject); // destroyes object
    }
    public void Blueprint()
    {
        Vector3 buildPos = new(activePos.x + (buildingPrefab.GetComponent<Building>().build.sizeX / 2), -10, activePos.z + (buildingPrefab.GetComponent<Building>().build.sizeZ / 2)); // get the position
        build = Instantiate(buildingPrefab, buildPos, Quaternion.identity, GameObject.Find("Buildings").transform); // creates the building prefab
        build.name = build.name.Replace("(Clone)", ""); // removes (Clone) from its name
        build.GetComponent<MeshRenderer>().material.color = canPlace() ? Color.blue : Color.red;
    }
    public bool canPlace()
    {
        Building b = build.GetComponent<Building>();
        if (resources.CanAfford(b.build.cost))
        {
            int x = 0;
            int z = 0;
            for (x = 0; x < b.build.sizeX; x++) // for each tile of size X
            {
                for (z = 0; z < b.build.sizeZ; z++) // for each tile of size Y
                {
                    try
                    {
                        List<Transform> t = Physics.RaycastAll(new Vector3(activePos.x + 0.5f - x, 2, activePos.z + 0.5f + z), Vector3.down, 3f).Select(g => g.transform).ToList();

                        if (t.Where(g => g.gameObject.layer == 6).ToList().Count > 0)
                        {
                            return false;
                        }
                        else if (t.Where(g => g.GetComponent<Road>()).ToList().Count > 0)
                        {
                            // nothing
                        }
                        else if (t.Where(g => g.GetComponent<Rock>()).ToList().Count > 0)
                        {
                            return false;
                        }
                    }
                    catch
                    {
                        Debug.LogError("missing tiles!!!");
                    }
                }
            }
            return true;
        }
        return false;
    }
    public void PrepDig()
    {
        drag = false;

        Humans humans = transform.parent.parent.GetChild(2).GetComponent<Humans>();
        jobData data;
        foreach (var toBe in toBeDigged) // removes all things that are alredy ordered from markedTiles
        {
            markedTiles.RemoveAll(q => q == toBe);
        }
        foreach (Rock tile in markedTiles) 
        {
                toBeDigged.Add(tile); // add rock
                data = new jobData(jobs.digging, ToInt(tile.transform.position), new JobObjects(tile));
                humans.GetComponent<JobQueue>().AddJob(data);
        }
        markedTiles.Clear();
        List<Human> workers = humans.transform.GetChild(0).GetComponentsInChildren<Human>().Where(h => h.jData.job == jobs.free).ToList();
        foreach (var worker in workers) // triggers on every free human
        {
            if (!worker.nightTime)
            {
                worker.StopC();
                StartCoroutine(worker.LookForNew()); // look for new jobs;
            }
        }
    }
    public void FindPathBuild()
    {
        Humans humans = transform.parent.parent.GetChild(2).GetComponent<Humans>();
        build.name = buildingPrefab.name.Replace("_blueprint", "");
        build.layer = 6;
        build.GetComponent<SortingGroup>().sortingLayerName = "Buildings";
        build.GetComponent<Building>().changeColor(new Color());        
        humans.GetComponent<JobQueue>().AddJob(new jobData(jobs.building, ToInt(build.transform.position), new JobObjects(build.GetComponent<Building>()))); // creates a new job with the data above
        resources.UpdateResource(buildingPrefab.GetComponent<Building>().build.cost, false);
    }
}
