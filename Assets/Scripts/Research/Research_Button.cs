using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Research_Button : MonoBehaviour
{
    public int research_progress = 0; // The progress of the research on this button
    public bool research_completed = false; // Is the research on this button completed?
    public int research_needed; // The research points needed to unlock this research
    public ResearchStruct research; // The research that this button is linked to
    public int id = 0; // The id of this research
    private TMP_Text text; // The text of the research progress
    private Transform research_transform; // The transform of the research
    private Research Research_Script; // The script of the research
    public Resource research_cost = new(); // The cost of the research

    private void Awake()
    {
        research_transform = transform.parent.parent.parent.parent;
        Research_Script = research_transform.GetComponent<Research>();
    }
        
    public bool Necesities_Completed()
    {
        return true; // TODO: Add necesities
    }

    public void Start_Research()
    {
        /*
        if ((Resources_Script.DiffRes(research_cost, Resources_Script.resources, new())).ammount.Sum() != 0)
            return false;
        Resources_Script.ManageRes(,research_cost,-1);
        */
        research = Research_Script.researches[id];
        if (!research.completed)
        {
            if (!Research_Script.researching)
            {
                Research_Script.Start_Researching();
            }

            research_progress = Research_Script.research_progress;
            if (research_progress >= research.research_needed) // If the research is already completed
            {
                Complete_Research();
            }
            else
            {
                Research_Script.Update_Text();
            }

            Research_Script.researches[id] = research;
            Research_Script.currently_researching = id;
        }
        //return true;
    }

    private void Unlock_Researches(ResearchStruct research)
    {
        foreach (Transform button in research.unlocks)
        {
            button.GetComponent<Button>().interactable = true;
        }
    }
    
    public void Complete_Research()
    {
        research = Research_Script.researches[id];
        research.completed = true;
        Unlock_Researches(research);
        Research_Script.researching = false;
        
        Research_Script.researches[id] = research;
    }
    
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
