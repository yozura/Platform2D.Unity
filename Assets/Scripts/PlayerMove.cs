using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public GameManager GM;

    public AudioClip jump;
    public AudioClip attack;
    public AudioClip damaged;
    public AudioClip item;
    public AudioClip die;
    public AudioClip finish;

    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D myrigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    CircleCollider2D circleCollider;
    AudioSource audioSource;

    private void Awake()
    {
        myrigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        circleCollider = GetComponent<CircleCollider2D>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Jump();

        // Stop Speed
        if (Input.GetButtonUp("Horizontal"))
            myrigid.velocity = new Vector2(myrigid.velocity.normalized.x * 0.5f, myrigid.velocity.y);

        // Direction Sprtie
        if (Input.GetButton("Horizontal"))
            spriteRenderer.flipX = Input.GetAxisRaw("Horizontal") == -1;
    
        // Animation
        if (Mathf.Abs(myrigid.velocity.x) < 0.3f)
            animator.SetBool("IsWalk", false);
        else
            animator.SetBool("IsWalk", true);
            
    }

    private void FixedUpdate()
    {
        Move();

        // Landing Platform
        if(myrigid.velocity.y < 0f)
        {
            Debug.DrawRay(myrigid.position, new Vector2(0f, -0.5f), new Color(0f,1f,0f));
            RaycastHit2D rayHit2D = Physics2D.Raycast(myrigid.position, Vector2.down, 1f, LayerMask.GetMask("Platform"));

            if(rayHit2D.collider != null)
            {
                if (rayHit2D.distance < 0.5f)
                    animator.SetBool("IsJump", false);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            if (myrigid.velocity.y < 0 && transform.position.y > collision.transform.position.y)
                Attack(collision.transform);
            else
                OnDamaged(collision.transform.position);
                
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Item"))
        {
            // Point
            bool isBronze = collision.gameObject.name.Contains("Bronze");
            bool isSilver = collision.gameObject.name.Contains("Silver");
            bool isGold = collision.gameObject.name.Contains("Gold");

            if (isBronze)
                GM.stagePoint += 50;
            else if (isSilver)
                GM.stagePoint += 100;
            else if (isGold)
                GM.stagePoint += 300;

            // DeActive Item
            collision.gameObject.SetActive(false);

            // Sound
            PlaySound("ITEM");
        }
        else if (collision.gameObject.CompareTag("Finish"))
        {
            // Next Stage
            GM.NextStage();
            PlaySound("FINISH");
        }
    }

    private void Attack(Transform _enemy)
    {
        // Point
        GM.stagePoint += 100;

        // Reaction
        myrigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse);

        // Enemy Die
        EnemyMove enemyMove = _enemy.GetComponent<EnemyMove>();
        enemyMove.OnDamaged();

        // Sound
        PlaySound("ATTACK");
    }

    private void OnDamaged(Vector2 _targetPos)
    {
        // HP Down
        GM.HealthDown();

        // Change Layer
        gameObject.layer = 11;

        // View Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Reaction
        // 타겟의 x값이 플레이어의 x값보다 크면 음수가 나오고
        // 그 반대면 양수가 나온다.
        int direction = transform.position.x - _targetPos.x > 0 ? 1 : -1;
        myrigid.AddForce(new Vector2(direction, 1) * 7, ForceMode2D.Impulse);

        // Animation
        animator.SetTrigger("Damaged");

        // Sound
        PlaySound("DAMAGED");

        Invoke("OffDamaged", 3f);
    }
    
    private void OffDamaged()
    {
        gameObject.layer = 10;
        spriteRenderer.color = new Color(1, 1, 1, 1);
    }

    private void Jump()
    {
        // Jump
        if (Input.GetButtonDown("Jump") && !animator.GetBool("IsJump"))
        {
            myrigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("IsJump", true);

            // Sound
            PlaySound("JUMP");
        }
    }

    private void Move()
    {
        // Move Speed
        float h = Input.GetAxisRaw("Horizontal");
        myrigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);

        // Max Speed
        if (myrigid.velocity.x > maxSpeed)
            myrigid.velocity = new Vector2(maxSpeed, myrigid.velocity.y);
        else if (myrigid.velocity.x < -maxSpeed)
            myrigid.velocity = new Vector2(-maxSpeed, myrigid.velocity.y);
    }

    public void OnDie()
    {
        // Sprite Alpha
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);

        // Sprtie Flip Y
        spriteRenderer.flipY = true;

        // Animator Disable
        animator.enabled = false;

        // Collider Disable
        circleCollider.enabled = false;

        // Sound
        PlaySound("DIE");
    }

    public void VelocityZero()
    {
        myrigid.velocity = Vector2.zero;
    }

    public void PlaySound(string _clipName)
    {
        switch(_clipName)
        {
            case "JUMP":
                audioSource.clip = jump;
                break;
            case "ATTACK":
                audioSource.clip = attack; 
                break;
            case "DAMAGED":
                audioSource.clip = damaged;
                break;
            case "ITEM":
                audioSource.clip = item; 
                break;
            case "DIE":
                audioSource.clip = die;
                break;
            case "FINISH":
                audioSource.clip = finish;
                break;
        }

        audioSource.Play();
    }
}
