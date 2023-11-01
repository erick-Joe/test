using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainPathCheckManager : MonoBehaviour
{
    public event Action<bool> OnCheckpointTriggered;
    public int CheckpointIndex;

    // Define a method to get the checkpoint index
    public int GetCheckpointIndex(GameObject checkpointObject)
    {
        var checkpointScript = checkpointObject.GetComponent<MainPathCheckManager>();
        if (checkpointScript != null)
        {
            return checkpointScript.CheckpointIndex;
        }
        return -1;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Solder"))
        {
            // Notify listeners when a collision with the agent occurs.
            OnCheckpointTriggered?.Invoke(true);
        }
    }
}