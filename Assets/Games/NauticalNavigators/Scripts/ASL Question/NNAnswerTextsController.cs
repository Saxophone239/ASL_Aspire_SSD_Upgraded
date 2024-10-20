using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NNAnswerTextsController : MonoBehaviour
{
    private NNQuestionManager qM;
    public TextMeshPro[] texts;
    public Vector3 defaultPosition = new Vector3(4, 0, 0);
    public Vector3 startingPosition = new Vector3(-10, 0, 0);

    // Animation stuff
    private float slerpPercPos = 0.0f;
    private bool isStartingAnimationActive = false;

    // Start is called before the first frame update
    void Start()
    {
        qM = GameObject.Find("Question Manager").GetComponent<NNQuestionManager>();
        SetTexts(qM.ThreeWords);
    }

    // Update is called once per frame
    void Update()
    {
        if (isStartingAnimationActive)
        {
            // Save progress of animation/slerping
            slerpPercPos = Mathf.MoveTowards(slerpPercPos, 1f, Time.deltaTime * 2.0f);

            // Begin slerping
            transform.position = Vector3.Slerp(startingPosition, defaultPosition, slerpPercPos);

            // Check if slerping finished
            if (slerpPercPos >= 1f)
            {
                isStartingAnimationActive = false;
            }
        }
    }

    public void SetTexts(string[] words)
    {
        for (int i = 0; i < texts.Length; i++)
        {
            texts[i].text = words[i];
        }
    }

    private void OnEnable()
    {
        this.transform.position = startingPosition;
        slerpPercPos = 0.0f;
        isStartingAnimationActive = true;
    }
}
