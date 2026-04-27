using UnityEngine;

public class TestPlayer : MonoBehaviour
{
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        int dirX = (int)Input.GetAxisRaw("Horizontal");
        
        if (dirX == 0)
        {
            animator.SetInteger("dirX", dirX);
        }
        else if (dirX == -1)
        {
            animator.SetInteger("dirX", dirX);
        }
        else if (dirX == 1)
        {
            animator.SetInteger("dirX", dirX);
        }

    }
}