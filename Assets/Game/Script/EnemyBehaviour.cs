using System.Runtime.CompilerServices;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour
{
    private Animator animator;
    public Transform HomePosition;
    public GameObject player;
    private Transform target;
    [SerializeField] 
    float speed;
    [SerializeField]
    float maxFollowRange;
    [SerializeField]
    float minFollowRange;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        animator = GetComponent<Animator>();
        target = FindFirstObjectByType<PlayerController>().transform;
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 scale = transform.localScale;

        if (player.transform.position.x > transform.position.x)
        {
            scale.x = Mathf.Abs(scale.x) * -1;
        }
        else
        {
            scale.x = Mathf.Abs(scale.x);
        }
        transform.localScale = scale;

        if(Vector3.Distance(transform.position, target.position) <= maxFollowRange && Vector3.Distance(target.position, transform.position) >= minFollowRange)
        {
            FollowPlayer();
        }
        else
        {
            ReturnHome();
        }

    }

    public void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, target.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
            

        }
    }

    public void ReturnHome()
    {
        if (Vector3.Distance(transform.position, HomePosition.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, HomePosition.position, speed * Time.deltaTime);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }
}
