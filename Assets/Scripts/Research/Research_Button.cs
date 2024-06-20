using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Research_Button : MonoBehaviour
{
    public int research_progress = 0; // The progress of the research on this button
    public bool research_completed = false; // Is the research on this button completed?
    public research_struct research; // The research that this button is linked to
    public int id = 0; // The id of this research
    private TMP_Text text; // The text of the research progress
    
    
    // Start is called before the first frame update
    void Start()
    {
        // Add new research to the list "researches"
        id = transform.parent.parent.GetComponent<Research>().researches.Count;
        transform.parent.parent.GetComponent<Research>().researches.Add(new research_struct());
        
        // Set known variables in the research struct
        research = transform.parent.parent.GetComponent<Research>().researches[id];
        research.button = transform;
        research.research_progress = research_progress;
        research.research_needed = Variables.Object(transform).Get<int>("needed_research_progress");
        
        transform.parent.parent.GetComponent<Research>().researches[id] = research;
        
    }
    
    public bool Necesities_Completed()
    {
        return true; // TODO: Add necesities
    }
    
    public void Start_Research()
    {
        research = transform.parent.parent.GetComponent<Research>().researches[id];
        research_progress = transform.parent.parent.GetComponent<Research>().research_progress;
        if (research_progress >= research.research_needed) // If the research is already completed
        {
            research.completed = true;
        }
        else
        {
            research.is_being_researched = true;
            text.text = (research_progress.ToString() + " / " + research.research_needed.ToString());
            transform.parent.parent.GetComponent<Research>().currently_researching = id;
        }

        transform.parent.parent.GetComponent<Research>().researches[id] = research;

    }

    public void Complete_Research()
    {
        research = transform.parent.parent.GetComponent<Research>().researches[id];
        research.completed = true;
        
        
        transform.parent.parent.GetComponent<Research>().researches[id] = research;
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
