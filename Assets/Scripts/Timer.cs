using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Timer : MonoBehaviour
{
    private float elapsedTime = 0.0f;
    private bool isExcute = false;

    public float Current
    {
        get => elapsedTime;
        set => elapsedTime = value;
    }

    public void ResetTimer ()
    {
        elapsedTime = 0.0f;
        StopTimer();
    }
    public void StartTimer () => isExcute = true;
    public void StopTimer () => isExcute = false;
    
    public string TimeToString ()
    {
        return $"{(int)elapsedTime / 60}:" + ((int)elapsedTime % 60 < 10 ? "0" : "") + (int)elapsedTime % 60;
    }
    public static string TimeToString (float time)
    {
        return $"{(int)time / 60}:" + ((int)time % 60 < 10 ? "0" : "") + (int)time % 60;
    }

    private void Update ()
    {
        if (isExcute)
        {
            elapsedTime += Time.deltaTime;
        }
    }
}
