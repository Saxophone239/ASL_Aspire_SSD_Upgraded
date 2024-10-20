using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MRAlwaysFaceCamera : MonoBehaviour
{
	private Camera cameraController;

    // Start is called before the first frame update
    private void Start()
    {
        cameraController = FindObjectOfType<Camera>();
    }

    private void LateUpdate()
    {
        transform.LookAt(cameraController.transform);
    }
}
