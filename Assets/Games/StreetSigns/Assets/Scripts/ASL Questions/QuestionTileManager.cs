using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class QuestionTileManager : MonoBehaviour
{
    private SSQuestionManager qM;
    // private VideoPlayer vp;

    // Start is called before the first frame update
    void Start()
    {
        qM = GameObject.FindObjectOfType<SSQuestionManager>();
        // vp = GetComponent<VideoPlayer>();
        // vp.url = qM.GetWordURL();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
