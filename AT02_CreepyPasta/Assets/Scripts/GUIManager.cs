using UnityEngine;
using UnityEngine.UI;

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
/// Script responsible for updating and managing elements of the user interface in response to gameplay events.
/// </summary>
public class GUIManager : MonoBehaviour, ILoggable
{
    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Header("Note Overlay Properties")]
    [Tooltip("A reference to the root note overlay object.")]
    [SerializeField] private GameObject noteOverlay;
    [Tooltip("A reference to the note text object.")]
    [SerializeField] private Text noteText;
    [Header("Author Card Properties")]
    [Tooltip("A reference to the author card overlay object.")]
    [SerializeField] private GameObject authorCardOverlay;
    [Tooltip("A reference to the author card text object.")]
    [SerializeField] private Text authorCardText;

    /// <summary>
    /// Called when the script component is enabled.
    /// </summary>
    private void OnEnable()
    {
        NoteInteraction.NoteInteractionEvent += ToggleNoteOverlay;
        GameManager.AuthorCardEvent += TriggerAuthorCard;
    }

    /// <summary>
    /// Called when the script component is disabled.
    /// </summary>
    private void OnDisable()
    {
        NoteInteraction.NoteInteractionEvent -= ToggleNoteOverlay;
        GameManager.AuthorCardEvent -= TriggerAuthorCard;
    }

    /// <summary>
    /// Called when the script component is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        NoteInteraction.NoteInteractionEvent -= ToggleNoteOverlay;
        GameManager.AuthorCardEvent -= TriggerAuthorCard;
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    private void Start()
    {
        if(noteOverlay != null && noteOverlay.activeSelf == true)
        {
            ToggleNoteOverlay(null, false);
        }
        if(authorCardOverlay != null && authorCardOverlay.activeSelf == true)
        {
            authorCardOverlay.SetActive(false);
        }
    }

    /// <summary>
    /// Toggles the note overlay active state and updates the text to match the provided note interaction.
    /// </summary>
    /// <param name="noteInteraction">The note to update the note text to.</param>
    private void ToggleNoteOverlay(NoteInteraction noteInteraction, bool toggle)
    {
        if (noteOverlay != null)
        {
            Log($"Overlay toggled to {!noteOverlay.activeSelf}.");
            noteOverlay.SetActive(!noteOverlay.activeSelf);
            if (toggle == true)
            {
                if (noteInteraction != null && noteText != null)
                {
                    noteText.text = noteInteraction.ReadingContent;
                }
            }
            else
            {
                if (noteText != null && noteText != null)
                {
                    noteText.text = "";
                }
            }
        }
    }

    /// <summary>
    /// Turns the author card on, and sets the text value to the passed author text parameter.
    /// </summary>
    /// <param name="authorText">The string to be displayed on the author card.</param>
    private void TriggerAuthorCard(string authorText)
    {
        if (authorCardOverlay != null && authorCardOverlay.activeSelf == false)
        {
            if (authorCardText != null)
            {
                authorCardText.text = authorText;
                authorCardOverlay.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.
    /// Will always have [GUI MANAGER] and the name of the associated game objected concatenated as a prefix.</param>
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
                    Debug.Log($"[GUI MANAGER] - {gameObject.name}: {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[GUI MANAGER] - {gameObject.name}: {message}");
                    break;
                case 2:
                    Debug.LogError($"[GUI MANAGER] - {gameObject.name}: {message}");
                    break;
            }
        }
    }
}