using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;

public enum selectionMode
{
    nothing,
    cancel,
    dig,
    build,
    destroy
}
public class GridTiles : MonoBehaviour
{
    [Header("Tilemaps")]
    public Tile mouseMark;
    public Tile cancelMark;
    public Tile mark;
    public Tile digMark;
    public GameObject roadPrefab;
    public GameObject buildingPrefab;
    public Resources resources;
    public selectionMode selMode = selectionMode.nothing;
    Tilemap marks;
    Tilemap ops;
    Tilemap ground;
    Vector3Int activePos;
    public List<Vector3Int> markedTiles;
    public List<GameObject> toBeDigged = new();
    GameObject build;
    public bool drag = false;

    void Awake()
    {
        marks = transform.GetChild(0).GetComponent<Tilemap>();
        ops = transform.GetChild(1).GetComponent<Tilemap>();
        ground = transform.GetChild(2).GetComponent<Tilemap>();
        markedTiles = new();
    }
    // Start is called before the first frame update
    public void Enter(Vector3Int vec)
    {
        activePos = vec;
        switch (selMode)
        {
            case selectionMode.nothing:
                marks.SetTile(marks.WorldToCell(activePos), mouseMark);
                break;
            case selectionMode.cancel:
                marks.SetTile(marks.WorldToCell(activePos), cancelMark);
                break;
            case selectionMode.dig:
                if (drag)
                {
                    marks.SetTile(markedTiles[0], null);
                    markedTiles = CalcTiles().ToList();
                }
                else
                {
                    marks.SetTile(marks.WorldToCell(activePos), digMark);
                }
                break;
            case selectionMode.build:
                build.transform.position = new Vector3(activePos.x, 1, activePos.z + (build.GetComponent<Building>().build.sizeZ / 2));
                build.GetComponent<MeshRenderer>().material.color = canPlace() ? Color.blue : Color.red;
                break;
            case selectionMode.destroy:
                break;
            default:
                Debug.LogError("assing selection mode!!!");
                break;
        }
    }
    public List<Vector3Int> CalcTiles()
    {
        clearMarks();
        List<Vector3Int> _vectors = new();
        int _x = activePos.x - markedTiles[0].x;
        int _y = activePos.z - markedTiles[0].y;
        _vectors.Add(markedTiles[0]);
        for (int i = 0; i != (_y < 0 ? _y - 1 : _y + 1); i = _y < 0 ? i - 1 : i + 1)
        {
            for (int j = 0; j != (_x < 0 ? _x - 1 : _x + 1); j = _x < 0 ? j - 1 : j + 1)
            {
                try
                {
                    RaycastHit[] hit = Physics.RaycastAll(new Vector3(markedTiles[0].x + j + 0.5f, 1, markedTiles[0].y + i + 0.5f), Vector3.down, 1f).Where(g => g.transform.GetComponent<Rock>()).ToArray(); // returns all hit
                    if (hit.Length > 0) // if there are any
                    {
                        Vector3Int tPos = new(markedTiles[0].x + j, markedTiles[0].y + i, 0);
                        _vectors.Add(tPos);
                        marks.SetTile(tPos, digMark);
                    }
                }
                catch
                {
                    Debug.LogError("Cannot Calculate");
                }
            }
        }
        _vectors.RemoveAt(0);
        return _vectors;
    }
    public void clearMarks() // deletes all tiles
    {
        if (markedTiles != null)
        {
            foreach (Vector3Int vector in markedTiles)
            {
                marks.SetTile(vector, null);
            }
        }
    }
    public void Exit(Vector3Int vec) // triggers on exit
    {
        switch (selMode)
        {
            case selectionMode.nothing:
            case selectionMode.cancel:
                marks.SetTile(marks.WorldToCell(vec), null);
                break;
            case selectionMode.dig:
                if (!drag) marks.SetTile(marks.WorldToCell(vec), null);
                break;

        }
    }
    public void Down(Vector3Int vec) // triggers on mouse down
    {
        switch (selMode)
        {
            case selectionMode.nothing:
                // inspect? \\
                break;
            case selectionMode.cancel:
                /*List<GameObject> hit = Physics.RaycastAll(new Vector3(markedTiles[0].x + 0.5f, 1, markedTiles[0].y + 0.5f), Vector3.down, 1f).Select(q => q.collider.gameObject).ToList(); // returns all hit
                hit.RemoveAll(q => q.GetComponent<Road>() || q.GetComponent<Rock>());
                */

                break;
            case selectionMode.dig:
                markedTiles = new List<Vector3Int>();
                drag = true;
                markedTiles.Add(new Vector3Int(activePos.x, activePos.z, 0));
                marks.SetTile(markedTiles[0], digMark);
                break;
        }
    }
    public void Up() // triggers on mouse up
    {
        switch (selMode)
        {
            case selectionMode.nothing:
                break;
            case selectionMode.cancel:
                /*List<GameObject> hit = Physics.RaycastAll(new Vector3(markedTiles[0].x + 0.5f, 1, markedTiles[0].y + 0.5f), Vector3.down, 1f).Select(q => q.collider.gameObject).ToList(); // returns all hit
                hit.RemoveAll(q => q.GetComponent<Road>() || q.GetComponent<Rock>());
                */
                JobQueue queue = GameObject.Find("Humans").GetComponent<JobQueue>();
                queue.CancelJob(queue.JobIndex(activePos));
                ops.SetTile(ops.WorldToCell(activePos), null);
                break;
            case selectionMode.dig:
                PrepDig();
                break;
            case selectionMode.build:
                if (canPlace())
                {
                    prepBuild();
                    blueprint();
                }
                break;
        }
    }
    public void BreakAction()
    {
        switch (selMode)
        {
            case selectionMode.nothing:
                return;
            case selectionMode.dig:
                drag = false;
                clearMarks();
                markedTiles = new();
                break;
            case selectionMode.build:
                Destroy(build);
                break;

        }
        selMode = selectionMode.nothing;
    }
    public void ChangeSelMode(selectionMode mode)
    {
        if (mode != selectionMode.build)
        {
            Destroy(build);
        }
        switch (mode)
        {
            case selectionMode.nothing:
                break;
            case selectionMode.cancel:
            case selectionMode.dig:
                if (selMode == mode)
                    selMode = selectionMode.nothing;
                else
                    selMode = mode;
                break;
            case selectionMode.build:
                if (buildingPrefab.name != (build != null ? build.name : ""))
                {
                    blueprint();
                    selMode = selectionMode.build;
                }
                else
                {
                    Destroy(build);
                    selMode = selectionMode.nothing;
                }
                break;
        }
    }
    public void RemoveTiles(Rock _r)
    {
        toBeDigged.Remove(_r.gameObject); // removes from list
        ops.SetTile(Vector3Int.FloorToInt(new Vector3(_r.transform.position.x, _r.transform.position.z, 0)), null);
        GameObject _road = Instantiate(roadPrefab, _r.transform.position, Quaternion.identity, GameObject.Find("Roads").transform); // creates a road on the place of tiles
        _road.name = "Road";
        Exit(Vector3Int.FloorToInt(_r.transform.position));
        Destroy(_r.gameObject); // destroyes object
    }
    public void blueprint()
    {
        Vector3 buildPos = new(activePos.x + (buildingPrefab.GetComponent<Building>().build.sizeX / 2), -10, activePos.z + (buildingPrefab.GetComponent<Building>().build.sizeZ / 2)); // get the position
        build = Instantiate(buildingPrefab, buildPos, Quaternion.identity, GameObject.Find("Buildings").transform); // creates the building prefab
        build.name = build.name.Replace("(Clone)", ""); // removes (Clone) from its name
        build.GetComponent<MeshRenderer>().material.color = canPlace() ? Color.blue : Color.red;
    }
    public bool canPlace()
    {
        Building b = build.GetComponent<Building>();
        if (resources.canAfford(b.build.cost))
        {
            int i = 0;
            int x = 0;
            int z = 0;
            for (x = 0; x < b.build.sizeX; x++) // for each tile of size X
            {
                for (z = 0; z < b.build.sizeZ; z++) // for each tile of size Y
                {
                    try
                    {
                        List<Transform> t = Physics.RaycastAll(new Vector3(activePos.x + 0.5f - x, 2, activePos.z + 0.5f + z), Vector3.down, 3f).Select(g => g.transform).ToList();

                        if (t.Where(g => g.GetComponent<Building>()).ToList().Count > 0)
                        {
                            //print("not here");
                        }
                        else if (t.Where(g => g.GetComponent<Road>()).ToList().Count > 0)
                        {
                            i++;
                        }
                        else if (t.Where(g => g.GetComponent<Rock>()).ToList().Count > 0)
                        {
                            //print("rock solid");
                            //PrepDig();
                        }
                    }
                    catch
                    {
                        Debug.LogError("missing tiles!!!");
                    }
                }
            }
            if (i == (x * z))
            {
                return true;
            }
            else return false;
        }
        return false;
    }
    public void PrepDig()
    {
        drag = false;
        clearMarks();

        Humans humans = transform.parent.parent.GetChild(2).GetComponent<Humans>();
        jobData data;
        foreach (var toBe in toBeDigged) // removes all things that are in toBeDigged from markedTiles
        {
            markedTiles.Remove(Vector3Int.FloorToInt(new Vector3(toBe.transform.position.x, toBe.transform.position.z)));
        }
        foreach (var tile in markedTiles) 
        {
            Rock _rock = Physics.RaycastAll(new Vector3(tile.x + 0.5f, 1, tile.y + 0.5f), Vector3.down, 2f) // gets the rock where the tile is
                .Where(g => g.transform.GetComponent<Rock>())
                .Select(g => g.transform.GetComponent<Rock>()).FirstOrDefault();
            if (_rock)
            {
                toBeDigged.Add(_rock.gameObject); // add rock
                ops.SetTile(tile, mark); // creates mark 
                data = new jobData(jobs.digging, Vector3Int.FloorToInt(_rock.transform.position), new JobObjects(_rock));
                humans.GetComponent<JobQueue>().AddJob(data);
            }
        }
        markedTiles.Clear();
        List<Human> workers = humans.GetComponentsInChildren<Human>().Where(h => h.jData.job == jobs.free).ToList();
        foreach (var worker in workers) // triggers on every free human
        {
            worker.StopC();
            //print("stoped all on: " + worker.name);
            Task t = worker.LookForNew(); // look for new jobs;
        }
    }
    public void prepBuild()
    {
        Humans humans = transform.parent.parent.GetChild(2).GetComponent<Humans>();
        build.name = buildingPrefab.name;
        build.GetComponent<SortingGroup>().sortingOrder = 0;
        build.GetComponent<MeshRenderer>().material.color = new Color(1, 1, 0);
        humans.GetComponent<JobQueue>().AddJob(new jobData(jobs.building, Vector3Int.FloorToInt(build.transform.position), new JobObjects(build.GetComponent<Building>()))); // creates a new job with the data above
        resources.UpdateResource(buildingPrefab.GetComponent<Building>().build.cost, false);

    }
}
