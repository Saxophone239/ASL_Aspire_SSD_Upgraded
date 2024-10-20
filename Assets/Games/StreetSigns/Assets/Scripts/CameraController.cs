using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Managers")]
    [SerializeField] private GameMechanics gameMechanics;

    [Header("Parameters")]
    public Transform PlayerTransform;
    private Vector3 offset;
    public Vector3 GameplayCameraPos;
    public Quaternion GameplayCameraRot;
    public Vector3 MenuCameraPos;
    public Quaternion MenuCameraRot;

    private bool isSwitchCamera;
    private bool isConstantYOffset = false;

    float slerpPercPos = 0.0f;
    float slerpPercRot = 0.0f;
    float gameplaySlerpPercPos = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (gameMechanics.IsMainMenu)
        {
            transform.position = MenuCameraPos;
            transform.rotation = MenuCameraRot;
            offset = MenuCameraPos - PlayerTransform.position;
        }
        else
        {
            transform.position = GameplayCameraPos;
            transform.rotation = GameplayCameraRot;
            offset = GameplayCameraPos - PlayerTransform.position;
        }
    }

    void FixedUpdate()
    {
        gameplaySlerpPercPos = Mathf.MoveTowards(gameplaySlerpPercPos, 1f, Time.fixedDeltaTime * 0.05f);
        int yFactor = isConstantYOffset ? 0 : 1;
        Vector3 newPosition = new Vector3(
            PlayerTransform.position.x + offset.x,
            PlayerTransform.position.y * yFactor + offset.y,
            PlayerTransform.position.z + offset.z
        );
        transform.position = Vector3.Slerp(transform.position, newPosition, gameplaySlerpPercPos);

        if (isSwitchCamera)
        {
            // Debug.Log($"slerping progress = {slerpPercPos}");
            slerpPercPos = Mathf.MoveTowards(slerpPercPos, 1f, Time.fixedDeltaTime * 1.0f);
            slerpPercRot = Mathf.MoveTowards(slerpPercRot, 1f, Time.fixedDeltaTime * 1.0f);

            transform.position = Vector3.Slerp(transform.position, PlayerTransform.position + GameplayCameraPos, slerpPercPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, GameplayCameraRot, slerpPercRot);          
            
            if (slerpPercPos >= 1f && slerpPercRot >= 1f)
            {
                offset = GameplayCameraPos;
                isConstantYOffset = true;
                // Debug.Log("done slerping");
                isSwitchCamera = false;
            }
        }
    }

    public void SwitchToGameplayCamera()
    {
        isSwitchCamera = true;
    }
}
