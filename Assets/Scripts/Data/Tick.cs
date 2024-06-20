using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tick : MonoBehaviour
{
    public float prePauseSpeed;
    public event Action tickAction;
    public void AwakeTicks()
    {
        Time.timeScale = prePauseSpeed;
        StartCoroutine(DoTick());
    }
    public void ChangeGameSpeed(int _speed)
    {
        StopAllCoroutines();
        if (_speed > 0)
        {
            Time.timeScale = _speed;
            StartCoroutine(DoTick());
        }
        else if(Time.timeScale > 0)
            prePauseSpeed = Time.timeScale;
    }
    public void Unpause()
    {
        StopAllCoroutines();
        Time.timeScale = prePauseSpeed;
        StartCoroutine(DoTick());
    }
    public IEnumerator DoTick()
    {
        while (true)
        {
            yield return new WaitForSeconds(1);
            tickAction?.Invoke();
            //print("tick" + speed);
        }
    }
}
