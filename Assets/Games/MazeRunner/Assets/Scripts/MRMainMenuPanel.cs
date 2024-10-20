using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRMainMenuPanel : MonoBehaviour
{
    [SerializeField] private Animator animator;

	private void OnEnable()
	{
		animator.SetTrigger("TriggerMoveMenu");
	}

	private void OnDisable()
	{
		animator.SetTrigger("TriggerMoveMenu");
	}
}
