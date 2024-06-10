using UnityEngine;
using UnityEngine.Events;

#region AUTHOR & COPYRIGHT DETAILS
/// Original Author: Joshua Ferguson
/// Contact: Joshua Ferguson <Josh.Ferguson@smtafe.wa.edu.au>.
/// Contributing Authors: 
/// Last Updated: March, 2024
/// 
/// ###---**COPYRIGHT STATEMENT**---###
/// © Copyright 2024 South Metropolitan TAFE. All rights reserved.
/// This code is provided to student's of South Metropolitan TAFE for educational purposes only.
/// Unauthorized use, including but not limited to sharing, redistributing, copying, or commercialising
/// this code or any part of it, without the express written permission of the authors, is strictly prohibited.
/// </remarks>
#endregion

/// <summary>
/// Script responsible for executing gameplay events in response to trigger collision with the player.
/// </summary>
public class EventTriggerCollider : MonoBehaviour, ILoggable
{
    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Tooltip("If true, the trigger will be disabled after executing the events.")]
    [SerializeField] protected bool oneShot = true;
    [Tooltip("Defines the functions that will be executed upon a trigger collision event.")]
    [SerializeField] private UnityEvent triggerEvents;

    /// <summary>
    /// OnTriggerEnter is called when a foreign collider collides with this object's collider, if isTrigger is true.
    /// </summary>
    /// <param name="other">Stores a reference to the foreign collider that has entered the trigger collider.</param>
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") == true)
        {
            triggerEvents.Invoke();
            if (oneShot == true)
            {
                GetComponent<Collider>().enabled = false;
            }
        }
    }

    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.
    /// Will always have [EVENT TRIGGER] and the name of the associated game objected concatenated as a prefix.</param>
    /// <param name="level">A level of 0 prints a standard message.
    /// A level of 1 prints a warning message.
    /// A level of 2 prints an error message.</param>
    public void Log(string message, int level = 0)
    {
        if (debug == true)
        {
            switch (level)
            {
                default:
                case 0:
                    Debug.Log($"[EVENT TRIGGER] - {gameObject.name}: {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[EVENT TRIGGER] - {gameObject.name}: {message}");
                    break;
                case 2:
                    Debug.LogError($"[EVENT TRIGGER] - {gameObject.name}: {message}");
                    break;
            }
        }
    }
}
