using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class Selection : MonoBehaviour//, IPointerMoveHandler, IPointerDownHandler, IPointerUpHandler
{/*
    [Header("Tiles")]
    [SerializeField] public BuildCost cost; // cost of active building
    [SerializeField] public Tile selTile; // active building 
    [SerializeField] public Tile errorTile; // tile to display when an error occurs
    [SerializeField] Tile Mousemark; // mouse indicator
    [SerializeField] Tile mark; // dig mark
    Tilemap ground; // ground TileMap
    Tilemap marks;  // mark TileMap

    bool drag = false; // mouse down?
    // mouse positions
    Vector2 lastPos;
    Vector2 currentPos;
    // tile positions
    Vector3Int lastTile;
    Vector3Int currentTile;
    List<Vector3Int> markedTiles; // List of positions of all marked tiles

    void Awake()
    {
        ground = transform.GetChild(0).GetComponent<Tilemap>();
        marks = transform.GetChild(1).GetComponent<Tilemap>();
        markedTiles = new List<Vector3Int>();
        lastPos = new Vector2();
        lastTile = new Vector3Int();
    }
    void Update()
    {
        // Listens for right mouse press. cleans building
        if (Input.GetMouseButtonDown(1))
        {
            selTile = null;
            
        }
    }

    // Listens for mouse press. turns on drag and changes mark Tile
    public void OnPointerDown(PointerEventData eventData)
    {
        if (selTile != null)
        {
            drag = true;
            markedTiles.Add(marks.WorldToCell(currentPos));
            marks.SetTile(marks.WorldToCell(currentPos), mark);
        }
    }

    // Listens for left mouse release. turns off drag and builds
    public void OnPointerUp(PointerEventData eventData)
    {
        drag = false;
        clearMarks(false);
        if (markedTiles.Count > 0)
        {
            marks.SetTile(marks.WorldToCell(currentPos), Mousemark);
            build();
            markedTiles.Clear();
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        DiffTile();
    }
    public void Move()
    {
        currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        marks.SetTile(marks.WorldToCell(currentPos), Mousemark);
    }

    public void DiffTile()
    {
        if(lastPos != currentPos) // checks if the mouse moved
        {
            currentTile = ground.WorldToCell(currentPos);
            if(currentTile != lastTile) // checks if the tile changed
            {

                if(drag == true) // asings tiles to a list
                {
                    if(selTile != null)
                    {
                        markedTiles = CalcTiles().ToList();
                    }
                }
                else // marks one tile
                {
                    marks.SetTile(marks.WorldToCell(lastPos), null);
                    marks.SetTile(marks.WorldToCell(currentPos), Mousemark);
                }
                lastTile = currentTile;
            }
            lastPos = currentPos;
        }
    }
    public List<Vector3Int> CalcTiles()
    {
        List<Vector3Int> _vectors = new List<Vector3Int>();
        //_vectors.Add(markedTiles[0]);
        int _x = currentTile.x - markedTiles[0].x;
        int _y = currentTile.y - markedTiles[0].y;
        clearMarks(false);
        
        for (int i = 0; i != (_y < 0 ? _y - 1 : _y + 1); i = _y < 0 ? i - 1 : i + 1) {
            for (int j = 0; j != (_x < 0 ? _x - 1 : _x + 1); j = _x < 0 ? j - 1 : j + 1)
            {
                Vector3Int tPos = new Vector3Int(markedTiles[0].x + j, markedTiles[0].y + i);
                _vectors.Add(tPos);
                marks.SetTile(tPos, mark);
            } 
        }
        return _vectors;
    }
    // Clears Tiles that are marked
    public void clearMarks(bool mouse)
    {
        if (markedTiles.Count > 0)
        {
            foreach (Vector3Int vector in markedTiles)
            {
                marks.SetTile(vector, null);
            }
        }
        if (mouse)
        {
            marks.SetTile(lastTile, null);
            markedTiles.Clear();
            drag = false;
        }
    }
    
    public void build()
    {
        Resources res = GameObject.Find("Resources").GetComponent<Resources>();
        bool error = false;
        List<Vector3Int> errors = new List<Vector3Int>();
        int[] missingRes = new int[cost.names.Length];

        foreach (Vector3Int vec in markedTiles)
        {
            int costItem = 0;
            int build = 0;
            foreach(string n in cost.names)
            {
                if(error == true)
                {
                    missingRes[costItem] += cost.costs[costItem];
                    if (marks.GetTile(marks.WorldToCell(vec)) == null)
                    {
                        marks.SetTile(marks.WorldToCell(vec), errorTile);
                        errors.Add(vec);
                    }
                    
                }
                else if (res.GetResourse(n) >= cost.costs[costItem])
                {
                    build++;
                }
                else
                {
                    error = true;
                    missingRes[costItem] += cost.costs[costItem];
                    if(marks.GetTile(marks.WorldToCell(vec)) == null)
                    {
                        marks.SetTile(marks.WorldToCell(vec), errorTile);
                        errors.Add(vec);
                    }
                    
                }
                costItem++;
            }
            if(build == costItem)
            {
                res.UpdateResource(cost, false);
                ground.SetTile(ground.WorldToCell(vec), selTile);
            }
        }
        if (error)
        {
            int i = 0;
            foreach (int r in missingRes) {
                print($"not enought {cost.names[i]}: {res.GetResourse(cost.names[i])}, but needed: {r}");
                i++;
            }
            StartCoroutine(fade(errors));
        }
    }

    // Coroutine that makes the error tiles fade
    IEnumerator fade(List<Vector3Int> errs)
    {
        yield return new WaitForSeconds(0.5f);
        for (float f = 0; f < 1; f += 0.02f) {
            Color col = new Color(errorTile.color.r, errorTile.color.g, errorTile.color.b, 1 - f);
            foreach (Vector3Int err in errs)
            {
                marks.SetColor(err, col);
            }
            yield return new WaitForSeconds(0.5f);
        }
        foreach (Vector3Int err in errs)
        {
            marks.SetTile(err, null);
        }
    }*/
}