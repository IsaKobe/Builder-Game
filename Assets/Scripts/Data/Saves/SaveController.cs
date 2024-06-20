using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Linq;

public class SaveController : MonoBehaviour
{
    public string activeFolder;
    void OnApplicationQuit()
    {
        SaveGame();
    }
    public void SaveGame()
    {
        Debug.Log("Application ending after " + Time.time + " seconds");
        if (activeFolder == "")
            activeFolder = "new folder";
        if (Directory.GetDirectories($"{Application.persistentDataPath}/saves").FirstOrDefault(q => LoadMenu.GetSaveName(q) == activeFolder) == null)
            Directory.CreateDirectory($"{Application.persistentDataPath}/saves/{activeFolder}");

        SaveJobs();
        SaveGrid();
        SaveHumans();
    }
    void SaveJobs()
    {
        /*List<JobData> jD = null;//GameObject.Find("Humans").GetComponent<JobQueue>()._jobs;
        List<JobSave> _jobs = new();
        foreach (JobData _jD in jD)
        {
            _jobs.Add(new(_jD));
        }
        JsonTextWriter jsonTextWriter = new(new StreamWriter($"{Application.persistentDataPath}/saves/{activeFolder}/Jobs.json"));
        PrepSerializer().Serialize(jsonTextWriter, _jobs);
        jsonTextWriter.Close();*/
    }
    void SaveGrid()
    {
        GridSave gridSave = new();
        gridSave.width = MyGrid.width;
        gridSave.height = MyGrid.height;
        gridSave.gridItems = new ClickableObjectSave[gridSave.width, gridSave.height];
        if (MyGrid.grid != null)
        {
            for (int x = 0; x < MyGrid.height; x++)
            {
                for (int z = 0; z < MyGrid.width; z++)
                {
                    gridSave.gridItems[x, z] = MyGrid.grid[x, z].Save();
                }
            }
            SaveBuildings(gridSave);
            JsonTextWriter jsonTextWriter = new(new StreamWriter($"{Application.persistentDataPath}/saves/{activeFolder}/Grid.json"));
            PrepSerializer().Serialize(jsonTextWriter, gridSave);
            jsonTextWriter.Close();
        }
        /*gridSave.toBeDug = GameObject.Find("Grid").GetComponent<GridTiles>().toBeDigged.Select(q => q.id).ToList();
        SaveChunks(gridSave);
        
        if(MyGrid.grid != null)
        {
            for (int x = 0; x < MyGrid.height; x++)
            {
                for (int z = 0; z < MyGrid.width; z++)
                {
                    if (MyGrid.grid[x, z] != null)
                    {
                        //gridSave.gridItems.Add(new(x, z, MyGrid.grid[x, z]));
                    }
                }
            }
            JsonTextWriter jsonTextWriter = new(new StreamWriter($"{Application.persistentDataPath}/saves/{activeFolder}/Grid.json"));
            PrepSerializer().Serialize(jsonTextWriter, gridSave);
            jsonTextWriter.Close();
        }*/
    }
    void SaveBuildings(GridSave gridSave)
    {
        gridSave.buildings = new();
        foreach (Building building in MyGrid.buildings)
        {
            BSave clickable = building.Save(null) as BSave;
            if (clickable != null)
                gridSave.buildings.Add(clickable);
        }
    }
    void SaveChunks(GridSave gridSave)
    {
        gridSave.chunks = new();
        foreach(Chunk chunk in MyGrid.chunks)
        {
            gridSave.chunks.Add(new(chunk));
        }
    }
    void SaveHumans()
    {
        List<HumanSave> humanSave = new();
        foreach (Human h in GameObject.Find("Humans").GetComponent<Humans>().humen)
            humanSave.Add(new(h));
        JsonTextWriter jsonTextWriter = new(new StreamWriter($"{Application.persistentDataPath}/saves/{activeFolder}/Humans.json"));
        PrepSerializer().Serialize(jsonTextWriter, humanSave);
        jsonTextWriter.Close();
    }

    JsonSerializer PrepSerializer()
    {
        JsonSerializer jsonSerializer = new();
        jsonSerializer.TypeNameHandling = TypeNameHandling.Auto;
        jsonSerializer.Formatting = Formatting.Indented;
        return jsonSerializer;
    }
}

