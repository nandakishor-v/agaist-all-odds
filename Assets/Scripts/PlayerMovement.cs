using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    #region Variables
    private Vector2 moveInput;
    private Rigidbody2D playerRigidBody;
    private Animator playerAnimator;
    private CapsuleCollider2D bodyCollider;
    private BoxCollider2D feetCollider;
    private bool isAlive = true;
    private float playerGravity;
    private float speedOffset = 100f;
    private float nextAttackTime = 0f;
    private Animator animator;
   
    private string fallingState = "Falling";
    private string jumpingState = "Jumping";
   
    
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask enemyLayers;
    [SerializeField] private Vector2 deathKick = new Vector2(0f, 0f);
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRange = 0.5f;
    [SerializeField] private float attackRate = 2f;
    [SerializeField] private int attackDamage = 1;
    [SerializeField] private float runSpeed = 5f;
    [SerializeField] private float walkSpeed = 2.1f;
    [SerializeField] private float jumpHorizontalSpeedOffset = 0;
    [SerializeField] private float jumpSpeed = 5f;
    [SerializeField] private float erasingTime;
    [SerializeField] public bool enableRun = false;
    [SerializeField] public bool gotEraser = false;
    [SerializeField] public bool isJumping = false;
    [SerializeField] public bool isGrounded = true;
    [SerializeField] public bool isAttacking = false;
    

    //[SerializeField] private Transform gun;
    #endregion

    #region Unity Methods

    private void Start()
    {
        playerRigidBody = GetComponent<Rigidbody2D>();
        playerAnimator = GetComponent<Animator>();
        bodyCollider = GetComponent<CapsuleCollider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        playerGravity = playerRigidBody.gravityScale;
    }


    private void FixedUpdate()
    { 
       if (!isAlive) { return; }

       Movement();
       //Die();
       HandleFallingAndLanding();
    }

    private void Update()
    {
        CheckingEraser();
    }

    private void LateUpdate()
    {
        isJumping = animator.GetBool("isJumping");
        animator.SetBool("isGrounded", isGrounded);
    }
    
    private void CheckingEraser()
    {
        animator.SetBool("gotEraser", gotEraser); //TODO temp erasor toggle
        if(gotEraser)
        {
            jumpingState = "Eraser Jumping";
            fallingState = "Eraser Falling";
        }
        else
        {
            jumpingState = "Jumping";
            fallingState = "Falling";
            
        }
    }
    
    private void Movement()
    {

        if (enableRun)
            Run();
        else
            Walk();
        FlipPlayer();
    }

    private void OnMove(InputValue value)
    {
        if (!isAlive) { return; }
        moveInput = value.Get<Vector2>();
    }
    
    private void OnJump(InputValue value)
    {
        if (!isAlive) { return; }
        if(!isGrounded) { return; }
        if (value.isPressed)
        {
            animator.SetBool("isJumping", true);
            animator.CrossFade(jumpingState, 0f);
            playerRigidBody.velocity += new Vector2(0f, jumpSpeed);
        }
    }

    private void HandleFallingAndLanding()
    {  
        if(playerRigidBody.velocity.y < 0)
            animator.SetBool("isJumping", false);
        
        if (!isGrounded && !isJumping && !isAttacking)
        {
            animator.SetBool("isFalling", true);
            animator.CrossFade(fallingState, 0.47f);
        }

        if (feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground")))
        {
            isGrounded = true;
            speedOffset = 0;
            animator.SetBool("isFalling", false);
        }
        else
        {
            isGrounded = false;
            speedOffset = jumpHorizontalSpeedOffset;
        }
    }
    
    
    private void OnAttack(InputValue value)
    {
        
        if (!isAlive || !gotEraser) { return; }
        if (value.isPressed)
        {
            Attack();
        }
    }

    private void Attack()
    {
        if (Time.time >= nextAttackTime)
        {
            animator.SetTrigger("isAttacking");
            var hitEnemies = Physics2D.OverlapCircleAll(
                attackPoint.position, attackRange, enemyLayers);
            foreach (var enemy in hitEnemies)
            {
                StartCoroutine(ErasingEnemy(enemy));
            }
            nextAttackTime = Time.time + 1f / attackRate;
        }
    }

    private IEnumerator ErasingEnemy(Component enemy)
    {
        yield return new WaitForSecondsRealtime(erasingTime);
        enemy.GetComponent<EnemyManager>().TakeDamage(attackDamage);
    }

    private void Run()
    {
        if(animator.GetBool("isWalking"))
            animator.SetBool("isWalking", false);
        if (isAttacking && isGrounded)
        {
            speedOffset = -runSpeed;
        }
        var playerVelocity = new Vector2(moveInput.x * (runSpeed + speedOffset), playerRigidBody.velocity.y);
        playerRigidBody.velocity = playerVelocity;
        var playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        playerAnimator.SetBool("isRunning", playerHasHorizontalSpeed);
    }
    
    private void Walk()
    {
        var playerVelocity = new Vector2(moveInput.x * (walkSpeed + speedOffset), playerRigidBody.velocity.y);
        playerRigidBody.velocity = playerVelocity;
        var playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        playerAnimator.SetBool("isWalking", playerHasHorizontalSpeed);
    }

    private void FlipPlayer()
    {
        var playerHasHorizontalSpeed = Mathf.Abs(playerRigidBody.velocity.x) > Mathf.Epsilon;
        if (playerHasHorizontalSpeed)
        {
            transform.localScale = new Vector2(Mathf.Sign(playerRigidBody.velocity.x), 1f);
        }
    }
    
    
    private void Die()
    {
        FindObjectOfType<GameSession>().ProcessPlayerDeath();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Eraser"))
        {
            gotEraser =  true;
            enableRun = true;
            animator.CrossFade("Eraser Idling", 0f);
            Destroy(other.gameObject);
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Trap"))
        {
            Debug.Log("die");
            Die();
        }
    }
    public void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
    #endregion
}
