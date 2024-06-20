using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public enum selectionMode{
    nothing,
    cancel,
    dig,
    build,
    destroy
}
public class GridTiles : MonoBehaviour
{
    [Header("Tilemaps")]
    [SerializeField] public Tile mouseMark;
    [SerializeField] public Tile mark;
    [SerializeField] public Tile digMark;
    [SerializeField] public GameObject roadPrefab;
    [SerializeField] public GameObject buildingPrefab;
    [SerializeField] public Resources resources;
    public selectionMode selMode = selectionMode.nothing;
    Tilemap marks;
    Tilemap ops;
    Tilemap ground;
    Vector3Int activePos;
    List<Vector3Int> markedTiles;
    public List<GameObject> toBeDigged = new();
    GameObject build;
    bool drag = false;
    
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
        List<Vector3Int> _vectors = new List<Vector3Int>();
        int _x = activePos.x - markedTiles[0].x;
        int _y = activePos.z - markedTiles[0].y;
        _vectors.Add(markedTiles[0]);
        for (int i = 0; i != (_y < 0 ? _y - 1 : _y + 1); i = _y < 0 ? i - 1 : i + 1)
        {
            for (int j = 0; j != (_x < 0 ? _x - 1 : _x + 1); j = _x < 0 ? j - 1 : j + 1)
            {
                try
                {
                    if (Physics.RaycastAll(new Vector3(markedTiles[0].x + j + 0.5f, 1, markedTiles[0].y + i + 0.5f), Vector3.down, 1f).Where(g => g.transform.GetComponent<Rock>()).Select(g => g.transform.GetComponent<Rock>()).First())
                    {
                        Vector3Int tPos = new Vector3Int(markedTiles[0].x + j, markedTiles[0].y + i, 0);
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
    public void clearMarks()
    {
        if(markedTiles != null)
        {
            foreach (Vector3Int vector in markedTiles)
            {
                marks.SetTile(vector, null);
            }
        }
    }
    public void Exit(Vector3Int vec)
    {
        switch (selMode)
        {
            case selectionMode.nothing:
                marks.SetTile(marks.WorldToCell(vec), null);
                break;
            case selectionMode.dig:
                if (!drag) marks.SetTile(marks.WorldToCell(vec), null); 
                break;
        }
    }
    public void Down(Vector3Int vec)
    {
        switch (selMode)
        {
            case selectionMode.nothing:
                // inspect? \\
                break;
            case selectionMode.dig:                
                    markedTiles = new List<Vector3Int>();
                    drag = true;
                    markedTiles.Add(new Vector3Int(activePos.x, activePos.z, 0));
                    marks.SetTile(markedTiles[0], digMark);
                break;
        }
    }
    public void Up()
    {
        switch (selMode)
        {
            case selectionMode.nothing:
                break;
            case selectionMode.dig:
                PrepDig();
                break;
            case selectionMode.build:
                if (canPlace())
                {
                    build.transform.parent = ground.transform;
                    build.layer = 5;
                    build.GetComponent<SortingGroup>().sortingOrder = 0;
                    build.GetComponent<MeshRenderer>().material.color = Color.white;
                    resources.UpdateResource(buildingPrefab.GetComponent<Building>().build.cost, false);
                    blueprint();
                }
                break;
        }
    }
    public void ChangeSelMode(selectionMode mode)
    {
        if(mode != selectionMode.build)
        {
            Destroy(build);
        }
        switch (mode)
        {
            case selectionMode.nothing:
                break;
            case selectionMode.dig:
                if (selMode == mode)
                    selMode = selectionMode.nothing;
                else
                    selMode = mode;
                break;
            case selectionMode.build:
                if(buildingPrefab.name != (build != null? build.name: ""))
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
        toBeDigged.Remove(_r.gameObject);
        ops.SetTile(Vector3Int.FloorToInt(new Vector3(_r.transform.position.x, _r.transform.position.z, 0)), null);
        GameObject _road = Instantiate(roadPrefab, _r.transform.position, Quaternion.identity, transform.GetChild(2).GetChild(1));
        _road.name = "Road";
        Exit(Vector3Int.FloorToInt(_r.transform.position));
        Destroy(_r.gameObject);
    }
    public void blueprint()
    {
        Vector3 buildPos = new Vector3(activePos.x + (buildingPrefab.GetComponent<Building>().build.sizeX / 2), -10, activePos.z + (buildingPrefab.GetComponent<Building>().build.sizeZ / 2));
        build = Instantiate(buildingPrefab, buildPos, Quaternion.identity);
        build.name = build.name.Replace("(Clone)", "");
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
            for(x = 0; x < b.build.sizeX; x++)
            {
                for (z = 0; z < b.build.sizeZ; z++)
                {
                    try
                    {
                        List<Transform> t = Physics.RaycastAll(new Vector3(activePos.x + 0.5f - x, 2, activePos.z + 0.5f + z), Vector3.down, 3f).Select(g => g.transform).ToList();
                        
                        if (t.Where(g => g.GetComponent<Building>()).ToList().Count > 0)
                        {
                            print("not here");
                        }
                        else if (t.Where(g => g.GetComponent<Road>()).ToList().Count > 0)
                        {
                            i++;
                        }
                        else if (t.Where(g => g.GetComponent<Rock>()).ToList().Count > 0)
                        {
                            print("rock solid");
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
        List<Human> workers = humans.GetComponentsInChildren<Human>().Where(h => h.jData.job == jobs.free).ToList();
        jobData data;
        foreach (var toBe in toBeDigged)
        {
            markedTiles.Remove(Vector3Int.FloorToInt(new Vector3(toBe.transform.position.x, toBe.transform.position.z)));
        }
        foreach (var tile in markedTiles)
        {
            Rock _rock = Physics.RaycastAll(new Vector3(tile.x + 0.5f, 1, tile.y + 0.5f), Vector3.down, 3f)
                .Where(g => g.transform.GetComponent<Rock>())
                .Select(g => g.transform.GetComponent<Rock>()).First();
            toBeDigged.Add(_rock.gameObject);
            ops.SetTile(tile, mark);
            data = new jobData(jobs.digging, Vector3Int.FloorToInt(_rock.transform.position), new JobObjects(_rock));
            data.ID = humans.GetComponent<JobQueue>().unigueID();
            humans.GetComponent<JobQueue>().AddJob(data);
        }
        foreach (var worker in workers)
        {
            Task t = worker.LookForNew();
        }
    }
}
