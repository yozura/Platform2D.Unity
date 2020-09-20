using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    Rigidbody2D myrigid;
    Animator animator;
    CircleCollider2D circleCollider;

    public int nextMove;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        myrigid = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        Think();
    }

    private void FixedUpdate()
    {
        AIMove();
    }

    private void AIMove()
    {
        // Move
        myrigid.velocity = new Vector2(nextMove, myrigid.velocity.y);

        // Platform Check
        Vector2 frontVector = new Vector2(myrigid.position.x + nextMove * 0.2f, myrigid.position.y);
        Debug.DrawRay(frontVector, Vector2.down, new Color(0f, 1f, 0f));
        RaycastHit2D rayHit2D = Physics2D.Raycast(frontVector, Vector2.down, 1f, LayerMask.GetMask("Platform"));

        if (rayHit2D.collider == null)
            Turn();
    }

    private void Think()
    {
        // Next Active
        float nextThinkTime = Random.Range(2f, 5f);
        nextMove = Random.Range(-1, 2);

        // Animation
        animator.SetInteger("WalkSpeed", nextMove);

        // Filp
        if (nextMove != 0)
            spriteRenderer.flipX = nextMove == 1;

        // Recursive
        Invoke("Think", nextThinkTime);
    }

    private void Turn()
    {
        nextMove *= -1;
        spriteRenderer.flipX = nextMove == 1;
        CancelInvoke();
        Invoke("Think", 2f);
    }
    
    public void OnDamaged()
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1,1,1,0.4f);

        // Sprtie Flip Y
        spriteRenderer.flipY = true;

        // Animator Disable
        animator.enabled = false;

        // Collider Disable
        circleCollider.enabled = false;

        // Die Effect Jump
        myrigid.AddForce(Vector2.up * 5f, ForceMode2D.Impulse);

        // Destory
        Invoke("DeActivate", 5f);
    }

    private void DeActivate()
    {
        gameObject.SetActive(false);
    }
}
