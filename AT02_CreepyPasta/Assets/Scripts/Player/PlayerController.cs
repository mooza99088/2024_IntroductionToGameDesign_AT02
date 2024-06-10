using UnityEngine;

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
/// Script responsible for responding to player movement inputs, allowing them to navigate the game world.
/// </summary>
[RequireComponent(typeof(CharacterController), typeof(AudioSource))]
public class PlayerController : MonoBehaviour
{
    [Tooltip("Toggle on to print console messages from this component.")]
    [SerializeField] private bool debug;
    [Header("Movement Properties")]
    [Tooltip("Defines the default speed at which the player will move.")]
    [Range(1f, 10f)][SerializeField] private float defaultSpeed;
    [Header("Physics Properties")]
    [Tooltip("Defines how strong gravity is.")]
    [Range(1f, 100f)][SerializeField] private float gravity;
    [Header("Jumping Properties")]
    [Tooltip("Disables the ability for the player to jump when toggled to true.")]
    [SerializeField] private bool disableJump = false;
    [Tooltip("Defines the force at which the player will jump.")]
    [Range(0f, 30f)][SerializeField] private float jumpForce;
    [Tooltip("The audio clips for jumping. (Will be randomly selected from.)")]
    [SerializeField] private AudioClip[] jumpClips;

    private float velocity = 0f;
    private float currentMovementSpeed = 0f;
    private Vector3 motionFrameStep;
    private CharacterController controller;
    private AudioSource aSrc;

    /// <summary>
    /// Enables/disables the ability to move the player character by device input.
    /// </summary>
    public bool MovementEnabled { get; set; } = true;
    /// <summary>
    /// A static reference for immediate access to the active instance of the player controller.
    /// This implements the singleton pattern to ensure that only one active instance of the player controller can be active in the scene.
    /// </summary>
    public static PlayerController Instance { get; private set; }

    /// <summary>
    /// Called when the script component is enabled.
    /// </summary>
    private void OnEnable()
    {
        NoteInteraction.NoteInteractionEvent += DisableMovementOnNoteInteraction;
        GameManager.GameWinEvent += DisableMovementOnGameEnd;
        GameManager.GameLoseEvent += DisableMovementOnGameEnd;
    }

    /// <summary>
    /// Called when the script component is disabled.
    /// </summary>
    private void OnDisable()
    {
        NoteInteraction.NoteInteractionEvent -= DisableMovementOnNoteInteraction;
        GameManager.GameWinEvent -= DisableMovementOnGameEnd;
        GameManager.GameLoseEvent -= DisableMovementOnGameEnd;
    }

    /// <summary>
    /// Called when the script component is destroyed.
    /// </summary>
    private void OnDestroy()
    {
        NoteInteraction.NoteInteractionEvent -= DisableMovementOnNoteInteraction;
        GameManager.GameWinEvent -= DisableMovementOnGameEnd;
        GameManager.GameLoseEvent -= DisableMovementOnGameEnd;
    }

    /// <summary>
    /// Awake is called before Start.
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            if (TryGetComponent(out controller) == false)
            {
                Log($"{gameObject.name} requires a Character Controller component!", 1);
            }
            if (TryGetComponent(out aSrc) == false)
            {
                Log($"{gameObject.name} requires an Audio Source component!", 1);
            }
        }
        else if(Instance != this)
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Start is called before the first frame update.
    /// </summary>
    void Start()
    {
        currentMovementSpeed = defaultSpeed;
    }

    /// <summary>
    /// FixedUpdate may be called more than once per frame.
    /// </summary>
    private void FixedUpdate()
    {
        if (controller != null)
        {
            if (controller.isGrounded == true)
            {
                velocity = -gravity * Time.deltaTime;
            }
            else
            {
                velocity -= gravity * Time.deltaTime;
            }
        }
    }

    /// <summary>
    /// Update is called once per frame.
    /// </summary>
    void Update()
    {
        if (MovementEnabled == true)
        {
            Jump();
            ApplyMovementTick();
        }
        else
        {
            ApplyMovementTick(true);
        }
    }

    /// <summary>
    /// Causes the character to jump when jumping input has been detected.
    /// </summary>
    private void Jump()
    {
        if (disableJump == false)
        {
            if (controller != null && controller.isGrounded == true && Input.GetButtonDown("Jump") == true)
            {
                velocity = jumpForce;
                if(aSrc != null && jumpClips.Length > 0)
                {
                    aSrc.PlayOneShot(jumpClips[Random.Range(0, jumpClips.Length)]);
                }
            }
        }
    }

    /// <summary>
    /// Calculates the movement step for the current frame, and applies movement to the character controller.
    /// </summary>
    /// <param name="ignoreInputs">When true, the calculated motion step for the frame will ignore the detected movement inputs.
    /// When false,  the calculated motion step will account for detected movement inputs.</param>
    private void ApplyMovementTick(bool ignoreInputs = false)
    {
        if (controller != null)
        {
            motionFrameStep = Vector3.zero;
            if (ignoreInputs == false)
            {
                float verticalInput = Input.GetAxisRaw("Vertical");
                float horizontalInput = Input.GetAxisRaw("Horizontal");
                motionFrameStep += transform.forward * verticalInput;
                motionFrameStep += transform.right * horizontalInput;
                motionFrameStep = currentMovementSpeed * motionFrameStep.normalized;
            }
            motionFrameStep.y += velocity;
            controller.Move(motionFrameStep * Time.deltaTime);
        }
    }

    /// <summary>
    /// Toggles the player's ability to move the player controller.
    /// </summary>
    /// <param name="toggle">A value of true will enable movement if not already enabled.
    /// A value of false will disable movement if not already disabled.</param>
    /// <returns>Returns true if movement was successfully toggled to the provided state.
    /// Returns false if the current movement state is already the toggled to the provided state.</returns>
    public bool ToggleMovement(bool toggle)
    {
        if (toggle == true && MovementEnabled == true)
        {
            MovementEnabled = false;
            return true;
        }
        else if (toggle == false && MovementEnabled == false)
        {
            MovementEnabled = true;
            return true;
        }
        return false;
    }

    private void DisableMovementOnNoteInteraction(NoteInteraction note, bool toggle)
    {
        MovementEnabled = !toggle;
    }

    private void DisableMovementOnGameEnd(float time)
    {
        MovementEnabled = false;
    }

    /// <summary>
    /// Logs a formatted debugging messaged to the console, of the warning level specified.
    /// </summary>
    /// <param name="message">The message to be printed in the console.
    /// Will always have [PLAYER CONTROLLER] and the name of the associated game objected concatenated as a prefix.</param>
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
                    Debug.Log($"[PLAYER CONTROLLER] - {gameObject.name}: {message}");
                    break;
                case 1:
                    Debug.LogWarning($"[PLAYER CONTROLLER] - {gameObject.name}: {message}");
                    break;
                case 2:
                    Debug.LogError($"[PLAYER CONTROLLER] - {gameObject.name}: {message}");
                    break;
            }
        }
    }
}