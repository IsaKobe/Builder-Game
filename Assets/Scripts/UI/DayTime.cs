using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class DayTime : MasterClass
{
    int dayT = 60 * 10;
    int increment = 60;
    [SerializeField] Humans humans;
    private void Start()
    {
        StartCoroutine(CountTime());
    }
    IEnumerator CountTime()
    {
        //yield return new WaitForSeconds(1);
        while (true)
        {
            dayT += increment;
            gameObject.GetComponent<TMP_Text>().text = $"Time: {dayT/60}:{dayT%60}";
            if(dayT == 5 * 60)
            {
                print("day, go to work!!!");
                humans.DayChange(true);
                
            }
            else if(dayT == 20 * 60)
            {
                print("night, go to sleep");
                humans.DayChange(false);
                
            }
            else if(dayT == 24 * 60)
            {
                print("new DAY!!!");
                dayT = 0;
                // místo pro save 
            }
            yield return new WaitForSeconds(1);
        }
    }
}
