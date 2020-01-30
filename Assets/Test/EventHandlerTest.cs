using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventHandlerTest : MonoBehaviour
{
    /*
    *   Call UI updates from here
    */

    public bool showLogs = false;

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

        // Update UI element here
    }
}
