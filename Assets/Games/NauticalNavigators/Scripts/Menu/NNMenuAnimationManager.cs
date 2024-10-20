using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NNMenuAnimationManager : MonoBehaviour
{
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnEnable()
    {
        //animator.SetTrigger("PlayAnimation");
    }

    public void DisableMenuScreen()
    {
        StartCoroutine(DisableMenuScreenCoroutine());
    }

    private IEnumerator DisableMenuScreenCoroutine()
    {
        animator.SetTrigger("PlayAnimation");

        // Wait for the transition to end
        yield return new WaitUntil(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f );
        
        // Do some action
        //Debug.Log("transition ended");
        
        // Wait for the animation to end
        yield return new WaitWhile(() => animator.GetCurrentAnimatorStateInfo(0).normalizedTime <= 1.0f);
        
        // Do some action
        //Debug.Log("animation ended");
        gameObject.SetActive(false);
    }
}
