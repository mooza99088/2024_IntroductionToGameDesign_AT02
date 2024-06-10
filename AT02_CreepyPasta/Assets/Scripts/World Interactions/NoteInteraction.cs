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
/// Script responsible for defining note interaction functionality.
/// </summary>
public class NoteInteraction : Interactable
{
    public delegate void NoteInteractionDelegate(NoteInteraction note, bool toggle);

    [Tooltip("The text content that will be displayed when the player interacts with the note.")]
    [SerializeField][TextArea] private string readingContent;
    [Tooltip("The audio clip that plays on disengaging with the interaction.")]
    [SerializeField] protected AudioClip disengageClip;
    [Tooltip("If true, the interaction events will only execute the first time the note is interacted with.")]
    [SerializeField] private bool interactionEventsFireOnce = true;
    [Tooltip("If true, the interaction events are executed when the note is disengaged with. If false, the interaction events are executed when the note is initially interacted with.")]
    [SerializeField] private bool interactionEventsFireOnDisengage = true;
    [Tooltip("Defines the functions that will be executed upon an interaction event.")]
    [SerializeField] private UnityEvent interactionEvents;

    private bool readingActive = false;
    private bool eventsFired = false;

    /// <summary>
    /// Returns the text content that will be displayed when the player interacts with the note.
    /// </summary>
    public string ReadingContent { get { return readingContent; } }

    /// <summary>
    /// Event invoked on execution of interaction on disengage interaction methods.
    /// </summary>
    public static event NoteInteractionDelegate NoteInteractionEvent = delegate { };

    /// <summary>
    /// Invokes events associated with player interaction and disengagement.
    /// </summary>
    /// <returns>Returns true if the events were successfully invoked.</returns>
    private bool InvokeInteractionEvents()
    {
        if (interactionEventsFireOnce == true)
        {
            if (eventsFired == false)
            {
                interactionEvents.Invoke();
                eventsFired = true;
                return true;
            }
            return false;
        }
        else
        {
            interactionEvents.Invoke();
            return true;
        }
    }

    /// <summary>
    /// Executed when the note is interacted with by the player.
    /// </summary>
    /// <param name="interactionInfo">The interaction data from the interaction system.</param>
    /// <param name="engagedAction">Outs a reference back to the interaction if it requires disengagement.</param>
    /// <returns>Returns true if the interaction was successfully completed.</returns>
    public override bool OnInteract(out Interactable engagedAction)
    {
        if(base.OnInteract(out engagedAction) == true)
        {
            if(NoteInteractionEvent != null)
            {
                NoteInteractionEvent.Invoke(this, true);
                readingActive = true;
                engagedAction = this;
                if (interactionEventsFireOnDisengage == false)
                {
                    InvokeInteractionEvents();
                }
            }
        }
        return readingActive;
    }

    /// <summary>
    /// Executed when the player activates the interaction input while a note is actively engaged.
    /// </summary>
    /// <returns>Returns true if the note was active and successfully disengaged.</returns>
    public override bool OnDisengageInteraction()
    {
        if (readingActive == true)
        {
            NoteInteractionEvent.Invoke(null, false);
            readingActive = false;
            PlaySound(disengageClip, aSrc);
            if(interactionEventsFireOnDisengage == true)
            {
                InvokeInteractionEvents();
            }
            return true;
        }
        return false;
    }
}
