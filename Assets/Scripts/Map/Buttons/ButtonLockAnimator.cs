using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ButtonLockAnimator : MonoBehaviour
{
    [SerializeField] private Animator animator;

	public void OnUnlockAnimationFinished()
	{
		Debug.Log("unlock animation finished");
	}

	public void OnLockAnimationFinished()
	{
		Debug.Log("unlock animation finished");
	}

	// private void OnEnable()
	// {
	// 	animator.SetBool("isLocked", true);
	// }

	// private void OnDisable()
	// {
	// 	animator.SetBool("isLocked", false);
	// }
}
