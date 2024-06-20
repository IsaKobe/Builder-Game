using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Research : MonoBehaviour
{
    public ResearchStruct[] researches; // The list of all researches
    public int selected_category = 1; // The index of the currently selected category
    private int seleted_category_preverious = 1; // The index of the previously selected category
    public int currently_researching = 0; // The index of the currently researched item
    public int research_progress = 0; // The progress of the research
    private Transform category1; // The transform of category 1
    private Transform category2; // The transform of category 2
    private Transform category3; // The transform of category 3
    private Transform category4; // The transform of category 4
    public ResearchStruct research;
    private ResearchStruct temp_research;
    private TMP_Text progress_text;
    private TMP_Text researching_text;
    public int number_of_researchers = 0;
    public bool researching = false;
    private float temp_progress;
    private float temp_needed;

    private void InitializeResearches(int capacity)
    {
        researches = new ResearchStruct[capacity];

        foreach (Transform category in transform.GetChild(0).GetChild(2).GetComponentsInChildren<Transform>())
        {
            foreach (Research_Button child in category.transform.GetComponentsInChildren<Research_Button>())
            {
                researches[child.id] = new ResearchStruct();
                research = researches[child.id];
                research.button = child.transform;
                research.research_progress = child.research_progress;
                research.research_needed = child.research_needed;
                research.unlocks = new List<Transform>();
                researches[child.id] = research; //TODO: Zkusit bez tohohle
            }
        }
    }
    
    private void Select_New_Research()
    {
        /*
        for(int i = 0; i < researches.Length; i++) // Start a random new research
        {
            if (!researches[i].completed && (researches[i].research_needed > research_progress) && researches[i].button.GetComponent<Research_Button>().Necesities_Completed())
            {
                researches[i].button.GetComponent<Research_Button>().Start_Research();
                currently_researching = i;
                break;
            }
        }
        */
    }

    private IEnumerator Researching()
    {
        while (true)
        {
            if (researching)
            {
                research = researches[currently_researching];
                research.research_progress += 50 * number_of_researchers;
                researches[currently_researching] = research;
                if (researches[currently_researching].research_needed <= researches[currently_researching].research_progress) // If the research is completed
                {
                    researches[currently_researching].button.GetComponent<Research_Button>().Complete_Research();
                }
                Update_Text();
                yield return new WaitForSeconds(0.1f);
            }
            else
            {
                Update_Text();
                break;
            }
        }
        
    }

    private void Awake()
    {
        InitializeResearches(4); // Initialzes list of researches !!!EDIT CAPACITY TO THE AMMOUNT OF RESEARCHES!!!
    }

    private void Start()
    {
        // Find and set categories to default
        selected_category = 1;
        category1 = transform.GetChild(0).GetChild(2).GetChild(0);
        category2 = transform.GetChild(0).GetChild(2).GetChild(1);
        category3 = transform.GetChild(0).GetChild(2).GetChild(2);
        category4 = transform.GetChild(0).GetChild(2).GetChild(3);

        progress_text = transform.GetChild(0).Find("Research Counter").GetChild(1).GetComponent<TMP_Text>(); // Get the text of the research counter
        researching_text = GameObject.Find("Researching").transform.GetChild(1).GetComponent<TMP_Text>(); // Get the text of the currently researching
        
        /*
        for(int i = 0; i < researches.Length; i++) // Start a random new research
        {
            if (!researches[i].completed && (researches[i].research_needed > research_progress) && researches[i].button.GetComponent<Research_Button>().Necesities_Completed())
            {
                researches[i].button.GetComponent<Research_Button>().Start_Research();
            }
        }
        */

        category2.gameObject.SetActive(false);
        category3.gameObject.SetActive(false);
        category4.gameObject.SetActive(false);
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void Start_Researching()
    {
        researching = true;
        StartCoroutine(Researching());
    }

    public void Update_Research_Progress_Text()
    {
        if (researching)
        {
            /*
            print(researches[currently_researching].research_progress /
                  researches[currently_researching].research_needed);
            print($"{(researches[currently_researching].research_progress / researches[currently_researching].research_needed) * 100} %");
            */
            temp_needed = researches[currently_researching].research_needed;
            temp_progress = researches[currently_researching].research_progress;
            
            progress_text.text =
                $"{Mathf.FloorToInt((temp_progress / temp_needed) * 100)} %";
        }
        else progress_text.text = "0%";
    }

    public void Update_Researching_Text()
    {
        if (researching)researching_text.text = researches[currently_researching].button.GetChild(0).GetComponent<TMP_Text>().text;
        else researching_text.text = "Nothing";
    }

    public void Update_Text()
    {
        // Updates UI resource text
            Update_Research_Progress_Text();
            Update_Researching_Text();
    }

    private void Change_Category()
    {
        switch (selected_category)
        {
            case 1:
                category1.gameObject.SetActive(true);
                break;
            case 2:
                category2.gameObject.SetActive(true);
                break;
            case 3:
                category3.gameObject.SetActive(true);
                break;
            case 4:
                category4.gameObject.SetActive(true);
                break;
        }
        switch (seleted_category_preverious)
        {
            case 1:
                category1.gameObject.SetActive(false);
                break;
            case 2:
                category2.gameObject.SetActive(false);
                break;
            case 3:
                category3.gameObject.SetActive(false);
                break;
            case 4:
                category4.gameObject.SetActive(false);
                break;
        }
        seleted_category_preverious = selected_category;
    }


    private void Update()
    {
        /*
        temp_research = researches[currently_researching];
        temp_research.research_progress += research_progress;
        //researches[currently_researching] = temp_research;
        */
        /*
        if (research_progress != 0)
        {
            research = researches[currently_researching];
            research.research_progress += research_progress;
            researches[currently_researching] = research;
            
            Update_Text();
        }

        research_progress = 0;
        */
        if(seleted_category_preverious!= selected_category)
        {
         Change_Category();
        }
    }
}

