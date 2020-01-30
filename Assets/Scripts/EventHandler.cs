using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventHandler : MonoBehaviour
{
    /*
    *   Call UI updates from here
    */

    public bool showLogs = false;
    public Text BallSizeText;

    public void UpdatePickup(int pickups)
    {
        if (showLogs) Debug.Log("Objects picked up: " + pickups);

        // Update UI element here
    }

    public void UpdateVolume(float volume)
    {
        if (showLogs) Debug.Log("Ball volume: " + volume);

        // Update UI element here
    }

    public void UpdateMass(float mass)
    {
        if (showLogs) Debug.Log("Ball mass: " + mass);

        // Update UI element here
    }

    public void UpdateRadius(float radius)
    {
        if (showLogs) Debug.Log("Ball radius: " + radius);

        // Fix the scale slightly...
        radius *= 2;

        BallSizeText.text = radius.ToString() + "m";
    }
}
