using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class CameraScroller : MonoBehaviour
{
    [SerializeField] private Scrollbar scrollbar;
	[SerializeField] private Vector2 cameraYRange;
	[SerializeField] private float scrollAmount = 0.0005f;

	private float wholeCameraRange;

	private void Start()
	{
		wholeCameraRange = cameraYRange.y - cameraYRange.x;
	}

	private void Update()
	{
		// Change scrollbar value according to mouse scroll
		if (Mouse.current != null)
		{
			Vector2 vec = Mouse.current.scroll.ReadValue();
			// if (vec != Vector2.zero) Debug.Log(vec);
			if (vec.y != 0)
				scrollbar.value -= scrollAmount * vec.y;
		}
		scrollbar.value = Mathf.Clamp(scrollbar.value, 0.0f, 1.0f);

		// If scrollbar is changed, change camera position accordingly
		if (transform.position.y + wholeCameraRange / 2 != scrollbar.value)
		{
			float newY = cameraYRange.y - scrollbar.value * wholeCameraRange;

			transform.position = new Vector3
			(
				transform.position.x,
				newY,
				transform.position.z	
			);
		}
	}
}
