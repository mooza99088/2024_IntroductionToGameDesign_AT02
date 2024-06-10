using UnityEngine;

#region AUTHOR & COPYRIGHT DETAILS
/// Original Author: Joshua Ferguson
/// Contact: Joshua Ferguson <Josh.Ferguson@smtafe.wa.edu.au>.
/// Contributing Authors: 
/// Last Updated: March, 2024
/// 
/// ###---**COPYRIGHT STATEMENT**---###
/// � Copyright 2024 South Metropolitan TAFE. All rights reserved.
/// This code is provided to student's of South Metropolitan TAFE for educational purposes only.
/// Unauthorized use, including but not limited to sharing, redistributing, copying, or commercialising
/// this code or any part of it, without the express written permission of the authors, is strictly prohibited.
/// </remarks>
#endregion

/// <summary>
/// Script responsible for door interaction functionality.
/// </summary>
public class DoorInteraction : Interactable
{
    [Tooltip("Set to true to have the door open when the game starts. If true, must be enabled for all doors in lock group.")]
    [SerializeField] private bool openOnStart = false;
    [Tooltip("Include other doors that should lock/unlock when this door is locked/unlocked.")]
    [SerializeField] private DoorInteraction[] lockGroup;
    [Tooltip("The audio clip that plays on interaction while door is locked.")]
    [SerializeField] protected AudioClip lockedInteractionClip;
    [Tooltip("Set to true to lock the door when the game starts.")]
    [SerializeField] private bool lockedOnStart = false;

    private bool locked = false;
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
                Log($"{transform.parent.name} requires an Animator component!", 1);
            }
            if (transform.parent.TryGetComponent(out aSrc) == false)
            {
                Log($"{transform.parent.name} requires an Audio Source component!", 1);
            }
        }
        else
        {
            Log($"{gameObject.name} requires a parent object.", 2);
        }
    }

    /// <summary>
    /// Start is called before the first frame update
    /// </summary>
    void Start()
    {
        if (anim != null)
        {
            if (anim.GetBool("open") == true && openOnStart == false)
            {
                anim.SetBool("open", false);
            }
            else
            {
                anim.SetBool("open", openOnStart);
            }
        }
        if(lockedOnStart == true)
        {
            ToggleLockState(true);
        }
    }

    /// <summary>
    /// Executed when door is interacted with by the player.
    /// </summary>
    /// <param name="interactionInfo">The interaction data from the interaction system.</param>
    /// <param name="engagedAction">Outs a reference back to the interaction if it requires disengagement.</param>
    /// <returns>Returns true if the interaction was successfully completed.</returns>
    public override bool OnInteract(out Interactable engagedAction)
    {
        engagedAction = null;
        if (Active == true)
        {
            ToggleDoorState();
            if (disableOnSuccessfulInteraction == true)
            {
                Active = false;
                foreach (DoorInteraction door in lockGroup)
                {
                    if (door != this)
                    {
                        door.Active = false;
                    }
                }
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Toggles the current door state between open and closed.
    /// </summary>
    public void ToggleDoorState()
    {
        if (locked == false)
        {
            if (anim != null)
            {
                if (anim.GetBool("open") == false)
                {
                    anim.SetBool("open", true);
                }
                else if (anim.GetBool("open") == true)
                {
                    anim.SetBool("open", false);
                }
            }
            PlaySound(interactionClip, aSrc);
        }
        else
        {
            PlaySound(lockedInteractionClip, aSrc);
        }
    }

    /// <summary>
    /// Toggles the current lock state of the door and it's associated lock group.
    /// Door will also close if already open and lock state changed to true.
    /// </summary>
    /// <param name="lockState">If true, the door will be locked. If false, the door will be unlocked.</param>
    public void ToggleLockState(bool lockState = true)
    {
        locked = lockState;
        foreach (DoorInteraction door in lockGroup)
        {
            if (door != this && door.locked != lockState)
            {
                door.ToggleLockState(lockState);
            }
        }
        Log("Lockstate set to " + locked);
        if (locked == true)
        {
            if (anim != null)
            {
                if (anim.GetBool("open") == true)
                {
                    anim.SetBool("open", false);
                    PlaySound(interactionClip, aSrc);
                }
            }
            else
            {
                PlaySound(interactionClip, aSrc);
            }
        }
    }
}
