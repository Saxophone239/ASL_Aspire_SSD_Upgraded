using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class NNVideoQuestionController : MonoBehaviour
{
    private VideoPlayer vP;
    private NNQuestionManager qM;
    public Vector3 defaultScale = new Vector3(5, 2.8125f, 1);
    public Vector3 startingScale = new Vector3(0, 0, 0);
    public Vector3 defaultRotation = new Vector3(0, 0, 0);
    public Vector3 startingRotation = new Vector3(0, 0, -360);

    // Animation stuff
    private float slerpPercRot = 0.0f;
    private float slerpPercScale = 0.0f;
    private bool isStartingAnimationActive = false;

    // Start is called before the first frame update
    void Start()
    {
        vP = GetComponent<VideoPlayer>();
        qM = GameObject.Find("Question Manager").GetComponent<NNQuestionManager>();
        PlayVideo(qM.CorrectWord);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartingAnimationActive)
        {
            // Save progress on animation/slerping
            slerpPercRot = Mathf.MoveTowardsAngle(slerpPercRot, 1f, Time.deltaTime * 1.5f);
            slerpPercScale = Mathf.MoveTowards(slerpPercScale, 1f, Time.deltaTime * 1.5f);

            // Begin slerping
            transform.eulerAngles = Vector3.Slerp(startingRotation, defaultRotation, slerpPercRot);
            transform.localScale = Vector3.Slerp(startingScale, defaultScale, slerpPercScale);

            // Check if slerping finished
            if (slerpPercRot >= 1f && slerpPercScale >= 1f)
            {
                isStartingAnimationActive = false;
            }
        }
    }

    public void PlayVideo(string vocabWord)
    {
        vP.url = VideoManager.VocabWordToPathDict[vocabWord];
    }

    private void OnEnable()
    {
        slerpPercRot = 0.0f;
        slerpPercScale = 0.0f;
        transform.localScale = startingScale;
        transform.eulerAngles = startingRotation;
        isStartingAnimationActive = true;
    }
}
