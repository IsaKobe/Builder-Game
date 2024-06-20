using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.IO;
using Newtonsoft.Json;

public class ResearchBackend : MonoBehaviour
{
    //Variables
    public ResearchStructs[] researches;
    private string saveFileLocation;
    private string defaultReseatchLocation;

    private string saveJson;
    private string defaultJson;

    //Temp
    private ResearchStructs research;
    [SerializeField]
    private List<ResearchStructSaved> savedResearches;

    //Methods
        private void GenrateResearchesFromDeafult()
        {
            Directory.CreateDirectory(Application.persistentDataPath + "/Save");
            File.Create(saveFileLocation);
            defaultJson = File.ReadAllText(defaultReseatchLocation);
            File.WriteAllText(saveFileLocation, defaultJson);
            
        }
        public void LoadResearches()                                                           
            {
                if (!File.Exists(saveFileLocation))
                {
                    return;
                }
                saveJson = File.ReadAllText(saveFileLocation);
                if(savedResearches == null) savedResearches = new List<ResearchStructSaved>();
                savedResearches.Clear();
                savedResearches = JsonConvert.DeserializeObject<List<ResearchStructSaved>>(saveJson);
                //savedResearches = JsonUtility.FromJson<List<ResearchStructSaved>>(saveJson);
            }
        private void InitializeResearches(int capacity)
            {
                researches = new ResearchStructs[capacity];
                LoadResearches();
                foreach (ResearchStructSaved saved in savedResearches)
                {
                    research = new ResearchStructs();
                    research.id = saved.id;
                    research.button = saved.button;
                    research.researchName = saved.researchName;
                    research.MainBackend = saved.MainBackend;
                    research.text = saved.text;
                    research.researchNeeded = saved.researchNeeded;
                    research.researchCost = saved.researchCost;
                    research.beingResearched = saved.beingResearched;
                    research.researchProgress = saved.researchProgress;
                    research.completed = saved.completed;
                    research.unlocks = saved.unlocks;
                    researches[research.id] = research;
                }
            }

            public void SaveResearches()
            {
                /*if (!File.Exists(saveFileLocation))
                {
                    Directory.CreateDirectory(Application.persistentDataPath + "/Save");
                    File.Create(saveFileLocation);
                    File.WriteAllText(saveFileLocation, "{}");
                }
                foreach (ResearchStructs research in researches)
                {
                    savedResearches.Add(research.SaveBuilder(research));
                }
                saveJson = Newtonsoft.Json.JsonConvert.SerializeObject(savedResearches, Formatting.Indented);
                //saveJson = JsonUtility.ToJson(savedResearches);
                if (!string.IsNullOrEmpty(saveJson) && !saveJson.Equals("{}"))
                {
                    File.WriteAllText(saveFileLocation, saveJson);
                    Debug.Log("Saved research data to file."); // Debug log to indicate successful write
                }
                else
                {
                    Debug.Log("JSON data is empty despite having research data.");
                }*/
            }

            private void FUJ() //The disgrace of humanity
        {
            researches = new ResearchStructs[2];
            research = new ResearchStructs();
            research.id = 0;
            research.researchName = "FUJ";
            research.researchNeeded = 5000;
            research.researchCost = new Resource();
            research.beingResearched = false;
            research.researchProgress = 0;
            research.completed = false;
            research.unlocks = new List<Transform>();
            researches[0] = research;
            research = new ResearchStructs();
            research.id = 1;
            research.researchName = "FUJ2";
            research.researchNeeded = 10000;
            research.researchCost = new Resource();
            research.beingResearched = false;
            research.researchProgress = 0;
            research.completed = false;
            research.unlocks = new List<Transform>();
            researches[1] = research;
        }
    //Start & Awake
        private void Awake()
        {
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.OSXPlayer)
            {
                saveFileLocation = Application.persistentDataPath + "/Save/Researches.json";
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsPlayer)
            {
                saveFileLocation = Application.persistentDataPath + "/Save/Researches.json";
            }
            else
            {
                saveFileLocation = Application.persistentDataPath + "/Save/Researches.json";
            }
        }
        private void Start()
        {
            //LoadResearches(); // Load existing research data from the JSON file

            // Add default research data if necessary
            /*if (researches == null || researches.Length == 0)
            {
                InitializeResearches(2); // Initialize researches array
            }*/
            //FUJ();

            SaveResearches(); // Save the updated research data to the JSON file
        }
}

[Serializable]
public class ResearchStructs
{
    //Variables
        // General
        public int id;
        public Transform button;
        public string researchName;
        public Transform MainBackend;
        public TMP_Text text;
        private ResearchStructSaved save;
        private string saveJson;
        //Pre-research
        public int researchNeeded;
        public Resource researchCost;
        //In progress
        public bool beingResearched;
        public int researchProgress;
        //Post-research
        public bool completed;
        public List<Transform> unlocks = new List<Transform>();
        

    //Methods
        public ResearchStructSaved SaveBuilder(ResearchStructs saved)
        {
            save = new ResearchStructSaved();
            save.id = saved.id;
            save.button = saved.button;
            save.researchName = saved.researchName;
            save.MainBackend = saved.MainBackend;
            save.text = saved.text;
            save.researchNeeded = saved.researchNeeded;
            save.researchCost = saved.researchCost;
            save.beingResearched = saved.beingResearched;
            save.researchProgress = saved.researchProgress;
            save.completed = saved.completed;
            save.unlocks = saved.unlocks;
            return save;
        }
        public void SaveLoader(ResearchStructSaved saved)
        {
            id = saved.id;
            button = saved.button;
            researchName = saved.researchName;
            MainBackend = saved.MainBackend;
            text = saved.text;
            researchNeeded = saved.researchNeeded;
            researchCost = saved.researchCost;
            beingResearched = saved.beingResearched;
            researchProgress = saved.researchProgress;
            completed = saved.completed;
            unlocks = saved.unlocks;
        }
        public void CompleteResearch()
        {
            if (completed) return;
            completed = true;
            foreach (Transform unlock in unlocks)
            {
                unlock.gameObject.SetActive(true);
            }
        }
        
        public void StartResearch()
        {
            if (beingResearched) return;
            if (completed) return;
            beingResearched = true;
            if(researchNeeded <= researchProgress)
            {
                CompleteResearch();
            }
            
        }
    //Start & Awake
        private void Awake()
        {
            MainBackend = GameObject.Find("Research Tree(Stays Active)").transform;
        }

        private void Start()
        {
            text = button.GetComponentInChildren<TMP_Text>();
            text.text = researchName;
        }
}

[Serializable]
public class ResearchStructSaved
{
    //Variables
        // General
            public int id;
            public Transform button;
            public string researchName;
            public Transform MainBackend;
            public TMP_Text text;
        //Pre-research
            public int researchNeeded;
            public Resource researchCost;
        //In progress
            public bool beingResearched;
            public int researchProgress;
        //Post-research
            public bool completed;
            public List<Transform> unlocks = new List<Transform>();

}