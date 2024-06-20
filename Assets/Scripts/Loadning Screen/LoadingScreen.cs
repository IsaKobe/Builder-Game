using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingScreen : MonoBehaviour
{
    string folderName;
    int buildWeigth = 5;
    public GridSave gridSave;
    public List<JobSave> jobSaves;
    public List<HumanSave> humanSaves;
    public SceneReferences sceneReferences;

    int progressGlobal = 1;
    public event Action humanActivation;
    public void LoadMainMenu()
    {
        StartCoroutine(Load(2)); // the loading Process
    }
    IEnumerator Load(int id)
    {
        var scene = SceneManager.LoadSceneAsync(id, LoadSceneMode.Additive);
        // starts the loading
        do
        {
            yield return new WaitForSeconds(0.2f);
        }
        while (!scene.isDone);
        Debug.Log("start:" + Time.realtimeSinceStartup);
        if(id == 2) // if main menu is being activated
        {
            // sets Action Listeners
            MainMenu mainMenu = GameObject.Find("Main Menu").GetComponent<MainMenu>();
            mainMenu.newGame += NewGame;
            LoadMenu saveLoader = mainMenu.transform.GetChild(3).GetComponent<LoadMenu>();
            saveLoader.loadGame += LoadSave;
        }
        else // if loading level
        {
            sceneReferences = GameObject.Find("Scene").GetComponent<SceneReferences>();
            MyGrid.PrepGrid(sceneReferences.eventSystem.GetChild(0).transform);
            AfterLoad(true);
        }
        Debug.Log("end:" + Time.realtimeSinceStartup);
        transform.GetChild(0).gameObject.SetActive(false);
    }
    private void NewGame(string _folderName)
    {
        folderName = _folderName;
        AsyncOperation sceneUnloading = SceneManager.UnloadSceneAsync("Main Menu");
        sceneUnloading.completed += CreateW;
    }
    void CreateW(AsyncOperation obj)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        StartCoroutine(Load(3));
    }

    //////////////////////////////////////////////////////////////////
    //-----------------------------Loading--------------------------//
    //////////////////////////////////////////////////////////////////
    
    public void LoadSave(GridSave _gridSave, List<JobSave> _jobSaves, List<HumanSave> _humanSaves, string _folderName)
    {
        gridSave = _gridSave;
        jobSaves = _jobSaves;
        humanSaves = _humanSaves;
        folderName = _folderName;

        if (GameObject.Find("Main Menu"))
        {
            var unloading = SceneManager.UnloadSceneAsync("Main Menu");
            unloading.completed += LoadLevel;
        }
        else 
        {
            LoadLevel(null);
        }
    }
    void LoadLevel(AsyncOperation obj)
    {
        transform.GetChild(0).gameObject.SetActive(true);
        transform.GetChild(0).GetChild(2).GetComponent<TMP_Text>().text = folderName;
        transform.GetChild(0).GetChild(2).gameObject.SetActive(true);
        var loading = SceneManager.LoadSceneAsync("Level Template", LoadSceneMode.Additive);
        loading.completed += LoadWorld;
    }
    void LoadWorld(AsyncOperation obj)
    {
        sceneReferences = GameObject.Find("Scene").GetComponent<SceneReferences>();
        Slider slider = transform.GetChild(0).GetChild(1).GetComponent<Slider>();
        // create Progress val for loading slider
        int maxProgress = gridSave.width * gridSave.height*2;
        maxProgress += gridSave.buildings.Count * buildWeigth; //scale number
        /*maxProgress += gridSave.chunks.Count;
        maxProgress += jobSaves.Count;
        maxProgress += humanSaves.Count;*/
        slider.maxValue = maxProgress;
        Progress<int> progress = new Progress<int>(value =>
        {
            slider.value = value;
        });
        FillGrid(progress);
        /*FillJobs(progress);
        FillHumans(progress);
        //...*/
        StartCoroutine(LoadWaiting());
    }
    IEnumerator LoadWaiting()
    {
        yield return new WaitForSeconds(2);
        MyGrid.gridTiles = sceneReferences.eventSystem.GetChild(0).GetComponent<GridTiles>();
        AfterLoad(false);
    }
    void FillJobs(IProgress<int> progress)
    {
        JobQueue jobQueue = sceneReferences.humans.GetComponent<JobQueue>();
        GridTiles gridTiles = sceneReferences.GetComponentInChildren<GridTiles>();
        foreach (JobSave jobS in jobSaves)
        {
            JobData jobData = new(jobS);
            
            jobQueue.AddJob(jobData.job, jobData.interest);
            progress.Report(progressGlobal++);
        }
    }
    //////////////////////////////////////////////////////////////////
    //-----------------------------Grid-----------------------------//
    //////////////////////////////////////////////////////////////////
    void FillGrid(IProgress<int> progress)
    {
        CreateGrid(progress);
        /*CreateChunks(progress);
        CreateBuilding(progress);*/
    }

    void CreateGrid(IProgress<int> progress)
    {
        // creates new grid
        MyGrid.ClearGrid();
        MyGrid.grid = new ClickableObject[gridSave.width, gridSave.height];
        
        // gets gridTiles reference for rocks
        GridTiles gridTiles = sceneReferences.eventSystem.GetChild(0).GetComponent<GridTiles>();
        Camera.main.GetComponent<PhysicsRaycaster>().eventMask = gridTiles.defaultMask;
        // process of creating items
        
        CreateTiles(progress);
        CreateBuilding(progress);
        /* foreach (GridItemSave _gItem in gridSave.gridItems)
         {
             // assign item to grid, set pos values 
             int x = (int)_gItem.gridPos.x;
             int z = (int)_gItem.gridPos.z;
             GridItem item = _gItem.gridItem;
             gridItems[x, z] = item;

             // instantiate the object if neccesary(not a building)
             GameObject instance = null;
             if (item is GridOre)
             {
                 instance = Instantiate(ores[oreNames.IndexOf((item as GridOre).oreName)],
                     new Vector3(x, 1, z),
                     Quaternion.identity,
                     sceneReferences.rocks);
                 Rock r = instance.GetComponent<Rock>();
                 r.data = item as GridOre;
                 if (gridSave.toBeDug.Contains(r.id))
                 {
                     MeshRenderer meshRenderer = r.GetComponent<MeshRenderer>();
                     meshRenderer.material.EnableKeyword("_EMISSION");
                     meshRenderer.material.SetColor("_EmissionColor", (Color.yellow + Color.red) / 2);
                     gridTiles.toBeDigged.Add(r);
                 }
             }
             else if (item is Road)
             {
                 instance = Instantiate(roadPref,
                     new Vector3(x, 0, z),
                     Quaternion.identity,
                     sceneReferences.roads);
             }
             if (instance != null)
             {
                 instance.name = instance.name.Replace("(Clone)", "");
             }
             progress.Report(progressGlobal++);
         }
         MyGrid.grid = gridItems;*/
    }
    /// <summary>
    /// Instantiates and fills ClickableObject tiles (Rock, Water, Road).
    /// </summary>
    /// <param name="progress"></param>
    void CreateTiles(IProgress<int> progress)
    {
        List<GameObject> ores = Resources.LoadAll("Tile/Ores").Cast<GameObject>().ToList();
        GameObject roadPref = Resources.Load("Tile/Road") as GameObject;
        GameObject waterPref = Resources.Load("Tile/Hidden Objects/Water") as GameObject;//Assets/Resources/.prefab
        for (int x = 0; x < gridSave.height; x++)
        {
            for (int z = 0; z < gridSave.width; z++)
            {
                // Creates the tile object
                ClickableObjectSave objectSave = gridSave.gridItems[x, z];
                switch (objectSave)
                {
                    case RockSave:
                        Rock rock = Instantiate(ores.First(q => q.name == (objectSave as RockSave).oreName), new(x, 1, z), Quaternion.identity, sceneReferences.rocks).GetComponent<Rock>();
                        rock.Load(objectSave);
                        MyGrid.grid[x, z] = rock;
                        break;
                    case BSave:
                        MyGrid.grid[x, z] = null;
                        break;
                    case WaterSave:
                        Water water = Instantiate(waterPref, new(x, 0, z), Quaternion.identity, sceneReferences.water).GetComponent<Water>();
                        water.Load(objectSave);
                        MyGrid.grid[x, z] = water;
                        break;
                    default:
                        Road road = Instantiate(roadPref, new(x, 0, z), Quaternion.identity, sceneReferences.roads).GetComponent<Road>();
                        MyGrid.grid[x, z] = road;
                        break;
                }
                progress.Report(progressGlobal += 2);
            }
        }
    }
    /// <summary>
    /// Instantiates and places all the buildings.
    /// </summary>
    /// <param name="progress"></param>
    void CreateBuilding(IProgress<int> progress)
    {
        List<Building> prefabs = (Resources.Load("Buildings/Building Holder") as BuildingHolder).prefabs.Select(q=> q.GetComponent<Building>()).ToList();
        
        foreach (BSave save in gridSave.buildings) // for each saved building
        {
            Building _pref = prefabs.FirstOrDefault(q => q.name == save.prefabName); // find the prefab
            // create the prefab
            Building b = Instantiate(_pref,
                new(save.gridPos.x, 1, save.gridPos.z),
                Quaternion.Euler(0, save.rotationY, 0),
                sceneReferences.buildings);

            // fill the prefab with saved Data
            b.Load(save);
            MyGrid.PlaceBuild(b);
            progress.Report(progressGlobal += buildWeigth);
        }
    }
    void CreateChunks(IProgress<int> progress)
    {
        /* MyGrid.chunks = new();
         GameObject chunkPref = Resources.Load("Chunk") as GameObject;
         List<Human> humans = GameObject.FindWithTag("Humans").transform.GetComponentsInChildren<Human>().ToList();
         foreach(ChunkSave chunkSave in gridSave.chunks)
         {
             Vector3 vec = chunkSave.gridPos.ToVec();
             MyGrid.chunks.Add(Instantiate(chunkPref, new(vec.x, 1, vec.z), chunkPref.transform.rotation, sceneReferences.chunks).GetComponent<Chunk>());
             MyGrid.chunks[^1].localRes = new(chunkSave.localRes, humans);
             MyGrid.chunks[^1].id = chunkSave.id;
         }   */
    }
    
    //////////////////////////////////////////////////////////////////
    //-----------------------------Humans---------------------------//
    //////////////////////////////////////////////////////////////////
    void FillHumans(IProgress<int> progress)
    {
        GameObject humanPrefab = Resources.Load("Humans/Capsule") as GameObject;
        Humans humans = sceneReferences.humans.GetComponent<Humans>();
        JobQueue jobQueue = humans.GetComponent<JobQueue>();
        humans.humen = new();
        foreach (HumanSave h in humanSaves)
        {
            int parent = h.workplaceId > -1 ? 1 : 0;
            Human human = GameObject.Instantiate(humanPrefab,
                new Vector3(h.gridPos.x, 1, h.gridPos.z), 
                Quaternion.identity,
                sceneReferences.humans.GetChild(parent)).GetComponent<Human>();
            human.GetComponent<MeshRenderer>().material.color = h.color.ConvertColor();
            human.id = h.id;
            human.name = h.name;
            human.inventory = h.inventory;
            human.specialization = h.specs;
            human.sleep = h.sleep;
            // job assigment TODO
            /*human.jData = jobQueue._jobs.SingleOrDefault(q => q.ID == h.jobSave.id);
            if(human.jData == null)
            {
                human.jData = new(h.jobSave);
            }
            human.jData.human = human;*/
            // house assigment
            if (h.houseID != -1)
            {
                human.home = MyGrid.buildings.Where(q => q.id == h.houseID).
                    SingleOrDefault().GetComponent<House>();
                human.home.assigned.Add(human);
            }
            // workplace assigment
            if (h.workplaceId != -1)
            {
                human.workplace = MyGrid.buildings.Where(q => q.id == h.workplaceId).
                    SingleOrDefault().GetComponent<ProductionBuilding>();
                human.workplace.assigned.Add(human);
            }
            humans.AddHuman(human, ref humanActivation);
            progress.Report(progressGlobal++);
        }
    }
    void AfterLoad(bool newGame)
    {
        transform.parent.GetChild(1).GetComponent<AudioListener>().enabled = false;

        sceneReferences.humans.GetComponent<Humans>().GetHumans();
        sceneReferences.eventSystem.transform.GetChild(1).gameObject.SetActive(true);
        sceneReferences.eventSystem.transform.GetChild(1).GetChild(1).GetChild(0).GetChild(2).GetComponent<PreBuildInfo>().SetUp();
        sceneReferences.GetComponent<SaveController>().activeFolder = folderName;

        Camera.main.GetComponent<PhysicsRaycaster>().eventMask = GameObject.FindWithTag("Grid").GetComponent<GridTiles>().defaultMask;
        Camera.main.GetComponent<PhysicsRaycaster>().enabled = true;
        Camera.main.GetComponent<Physics2DRaycaster>().enabled = true;
        Camera.main.GetComponent<AudioListener>().enabled = true;
        MyRes.ActivateResources(newGame);
        sceneReferences.GetComponent<Tick>().AwakeTicks();
        SceneManager.UnloadSceneAsync("LoadingScreen");
    }
}
