using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
/// Script responsible for providing win and loss functions, as well as detecting quit functionality.
/// </summary>
public class GameManager : MonoBehaviour, ILoggable
{
    public delegate void GameEndStateDelegate(float duration);
    public delegate void AuthorCardDelegate(string cardText);

    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Tooltip("The amount of time the player will idle for before game restarts after game loss.")]
    [SerializeField] private float gameOverStateDuration = 3f;
    [Tooltip("The amount of time the player will idle for after game completion, before the camera cuts to black.")]
    [SerializeField] private float timeToAuthorCard = 3f;
    [Tooltip("The amount of time the author card text will appear on screen before the game restarts.")]
    [SerializeField] private float authorCardDuration = 5f;
    [Tooltip("The text that will appear on tlhe author card when the game is finished.")]
    [SerializeField][TextArea] private string authorCardText;

    private int gameState = 0;

    /// <summary>
    /// Static event called when the player completes the game.
    /// </summary>
    public static event GameEndStateDelegate GameWinEvent = delegate { };
    /// <summary>
    /// Static event called when the player fails the game.
    /// </summary>
    public static event GameEndStateDelegate GameLoseEvent = delegate { };
    /// <summary>
    /// Static event called at the end of the game when the player completes the game.
    /// </summary>
    public static event AuthorCardDelegate AuthorCardEvent = delegate { };

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape) == true)
        {
            Application.Quit();
        }
    }

    /// <summary>
    /// Coroutine responsible for executing the end game functionality when the player successfully completes the game.
    /// </summary>
    /// <returns>Returns new WaitForSeconds initially while waiting to invoke the author card, and after waiting to reset the game.</returns>
    private IEnumerator StartGameCompleteSequence()
    {
        if (GameWinEvent != null)
        {
            GameWinEvent.Invoke(timeToAuthorCard);
        }
        yield return new WaitForSeconds(timeToAuthorCard);
        if (AuthorCardEvent != null)
        {
            AuthorCardEvent.Invoke(authorCardText);
        }

        yield return new WaitForSeconds(authorCardDuration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameWinEvent = null;
        GameLoseEvent = null;
        AuthorCardEvent = null;
    }

    /// <summary>
    /// Coroutine responsible for executing the end game functionality when the player triggers the game's fail/lose state.
    /// </summary>
    /// <returns>Returns a new WaitForSeconds while waiting to reset the game.</returns>
    private IEnumerator StartGameOverSequence()
    {
        if (GameLoseEvent != null)
        {
            GameLoseEvent.Invoke(gameOverStateDuration);
        }
        yield return new WaitForSeconds(gameOverStateDuration);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameWinEvent = null;
        GameLoseEvent = null;
        AuthorCardEvent = null;
    }

    /// <summary>
    /// Executes the game win functionality, for when the player successfully completes the game.
    /// </summary>
    public void TriggerWinState()
    {
        if (gameState == 0)
        {
            gameState = 1;
            StartCoroutine(StartGameCompleteSequence());
        }
    }

    /// <summary>
    /// Executes the game loss functionality, for when the player does not successfully complete the game.
    /// </summary>
    public void TriggerLoseState()
    {
        if (gameState == 0)
        {
            gameState = -1;
            StartCoroutine(StartGameOverSequence());
        }
    }

    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.
    /// Will always have [GAME MANAGER] and the name of the associated game objected concatenated as a prefix.</param>
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
                    Debug.Log($"[GAME MANAGER] - {gameObject.name}: {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[GAME MANAGER] - {gameObject.name}: {message}");
                    break;
                case 2:
                    Debug.LogError($"[GAME MANAGER] - {gameObject.name}: {message}");
                    break;
            }
        }
    }
}

/// <summary>
/// Interface that provides the framework for tracking logged messages.
/// </summary>
public interface ILoggable
{
    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.</param>
    /// <param name="level">A level of 0 prints a standard message. A level of 1 prints a warning message. A level of 2 prints an error message.</param>
    public void Log(string message, int level = 0);
}