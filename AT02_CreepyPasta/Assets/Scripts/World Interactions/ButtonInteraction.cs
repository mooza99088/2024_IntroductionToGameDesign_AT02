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
/// Script responsible for button interaction functionality.
/// </summary>
public class ButtonInteraction : Interactable
{
    [Tooltip("Defines the functions that will be executed upon an interaction event.")]
    [SerializeField] private UnityEvent interactionEvents;

    private Animator anim;

    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    protected override void Awake()
    {
        if (transform.parent != null)
        {
            if (transform.parent.TryGetComponent(out anim) == false)
            {
                Log($"{transform.parent.name} expected to have an Animator component!", 1);
            }
            if (TryGetComponent(out aSrc) == false)
            {
                Log($"{gameObject.name} requires an Audio Source component!", 1);
            }
        }
        else
        {
            Log($"{gameObject.name} expects a parent object.", 1);
        }
    }

    /// <summary>
    /// Executed when the button is interacted with by the player.
    /// </summary>
    /// <param name="interactionInfo">The interaction data from the interaction system.</param>
    /// <param name="engagedAction">Outs a reference back to the interaction if it requires disengagement.</param>
    /// <returns>Returns true if the interaction was successfully completed.</returns>
    public override bool OnInteract(out Interactable engagedAction)
    {
        if (base.OnInteract(out engagedAction) == true)
        {
            interactionEvents.Invoke();
            if (disableOnSuccessfulInteraction == true)
            {
                Active = false;
            }
            if (anim != null)
            {
                anim.SetTrigger("pressed");
            }
            return true;
        }
        return false;
    }
}
