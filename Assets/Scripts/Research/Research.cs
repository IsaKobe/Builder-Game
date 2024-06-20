using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = UnityEngine.Object;
using TMPro;
using UnityEngine.UI;

public class Research : MonoBehaviour
{
    public int selected_category = 1; // The index of the currently selected category
    private int seleted_category_preverious = 1; // The index of the previously selected category
    public int currently_researching = 0; // The index of the currently researched item
    public List<research_struct> researches = new(); // The list of all researches
    public int research_progress = 0; // The progress of the research
    private Transform category1; // The transform of category 1
    private Transform category2; // The transform of category 2
    private Transform category3; // The transform of category 3
    private Transform category4; // The transform of category 4
    private Transform _Resources; // The transform of Resources
    public Resource resources;
    private research_struct temp_research;
    private TMP_Text text;
    

    private void Start()
    {
        // Find and set categories to default
        selected_category = 1;
        category1 = transform.Find("Category 1");
        category2 = transform.Find("Category 2");
        category3 = transform.Find("Category 3");
        category4 = transform.Find("Category 4");
        category2.gameObject.SetActive(false);
        category3.gameObject.SetActive(false);
        category4.gameObject.SetActive(false);
        
        
        text = transform.Find("Research Counter").GetChild(1 ).GetComponent<TMP_Text>(); // Get the text of the research counter
        
        for(int i = 0; i < researches.Count; i++) // Start a random new research
        {
            if (!researches[i].completed && (researches[i].research_needed > research_progress) && researches[i].button.GetComponent<Research_Button>().Necesities_Completed())
            {
                researches[i].button.GetComponent<Research_Button>().Start_Research();
                     
            }
        }
    }
    
    public void UpdateText()
    {
        // Updates UI resource text
            text.text = (researches[currently_researching].research_progress.ToString() + " / " + researches[currently_researching].research_needed.ToString());

    }

    private void Update()
    {
        temp_research = researches[currently_researching];
        temp_research.research_progress += research_progress;
        researches[currently_researching] = temp_research;
        
        if (research_progress != 0)
        {
            print(research_progress);
            UpdateText();
        }

        research_progress = 0;
        
         if(seleted_category_preverious!= selected_category)
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

         if (researches[currently_researching].research_needed <= research_progress) // If the research is completed
         {
             category1.GetChild(0).GetComponent<Research_Button>().Complete_Research();
             for(int i = 0; i < researches.Count; i++) // Start a random new research
             {
                 if (!researches[i].completed && (researches[i].research_needed > research_progress) && researches[i].button.GetComponent<Research_Button>().Necesities_Completed())
                 {
                     researches[i].button.GetComponent<Research_Button>().Start_Research();
                     
                 }
             }
         }
    }
}

