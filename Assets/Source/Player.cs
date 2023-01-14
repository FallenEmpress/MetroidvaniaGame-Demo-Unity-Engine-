using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Character
{
    [Header("Input")]
    public KeyCode meleeAttackKey = KeyCode.Mouse0;
    public KeyCode rangedAttackKey = KeyCode.Mouse1;
    public KeyCode jumpKey = KeyCode.Space;
    public string xMoveAxis = "Horizontal";

    [Header("Combat")]
    public Transform meleeAttackOrigin = null;
    public Transform rangedAttackOrigin = null;
    public GameObject projectile = null;
    public float meleeAttackRadius = 0.6f;
    public float meleeDamage = 2f;
    public float meleeAttackDelay = 1.1f;
    public float rangedAttackDelay = 0.3f;
    public float freezeDelay = 0.4f;
    public LayerMask enemyLayer = 8;

    
    private float moveIntentionX = 0;
    private bool attemptJump = false;
    private bool attemptMeleeAttack = false;
    private bool attemptRangedAttack = false;
    private float timeUntilMeleeReadied = 0;
    private float timeUntilRangedReadied = 0;
    private bool isMeleeAttacking = false;
    private bool isRangedAttacking = false;
    private bool isFrozen = false;
    




    // Update is called once per frame
    void Update()
    {
        GetInput();

        HandleJump();
        HandleMeleeAttack();
        HandleRangedAttack();
        HandleAnimations();
       
    }

    void FixedUpdate()
    {
        HandleRun();
    }

    private void OnDrawGizmosSelected()
    {
        Debug.DrawRay(transform.position, -Vector2.up * groundedLeeway, Color.red);
        if (meleeAttackOrigin != null)
        {
            Gizmos.DrawWireSphere(meleeAttackOrigin.position, meleeAttackRadius);
        }
    }

    public void GotHurt()
    {
        Animator.SetTrigger("Hurt");
        Rb2D.velocity = new Vector2(Rb2D.velocity.x, 5);
        //Rb2D.velocity = new Vector2(-5, Rb2D.velocity.y);

    }

    private void GetInput()
    {
        moveIntentionX = Input.GetAxis(xMoveAxis);
        attemptMeleeAttack = Input.GetKeyDown(meleeAttackKey);
        attemptRangedAttack = Input.GetKeyDown(rangedAttackKey);
        attemptJump = Input.GetKeyDown(jumpKey);
    }

    private void HandleRun()
    {
        if (moveIntentionX < 0 && transform.rotation.y == 0 && !isMeleeAttacking)
        {
            transform.rotation = Quaternion.Euler(0, 180f, 0);
        }
        else if (moveIntentionX > 0 && transform.rotation.y != 0 && !isMeleeAttacking)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        if (!isFrozen || !CheckGrounded())
        {
            Rb2D.velocity = new Vector2(moveIntentionX * speed, Rb2D.velocity.y);
        }
        else if(CheckGrounded())
        {
            Rb2D.velocity = new Vector2(0, Rb2D.velocity.y);
        }
    }

    private void HandleJump()
    {
        if (attemptJump && CheckGrounded())
        {
            Rb2D.velocity = new Vector2(Rb2D.velocity.x, jumpForce);
        }
    }

    private void Anika()
    {
        if (!CheckGrounded() )
        {
            Animator.SetTrigger("Edge-Grab");
        }
    }

    private void HandleMeleeAttack()
    {
        if (attemptMeleeAttack && timeUntilMeleeReadied <= 0 && timeUntilRangedReadied <= 0)
        {
            Debug.Log("Player: Attempting to Melee Attack!");
            Collider2D[] overlappedColliders = Physics2D.OverlapCircleAll(meleeAttackOrigin.position, meleeAttackRadius, enemyLayer);
            for (int i = 0; i < overlappedColliders.Length; i++)
            {
                IDamageable enemyAttributes = overlappedColliders[i].GetComponent<IDamageable>();
                if (enemyAttributes != null)
                {
                    enemyAttributes.ApplyDamage(meleeDamage);
                }
            }

            timeUntilMeleeReadied = meleeAttackDelay;
        }
        else
        {
            timeUntilMeleeReadied -= Time.deltaTime;
        }

    }

    private void HandleRangedAttack()
    {
        if (attemptRangedAttack && timeUntilRangedReadied <= 0 && timeUntilMeleeReadied <= 0)
        {
            Debug.Log("Player: Attempting Ranged Attack!");
            Instantiate(projectile, rangedAttackOrigin.position, rangedAttackOrigin.rotation);

            timeUntilRangedReadied = rangedAttackDelay;
        }
        else
        {
            timeUntilRangedReadied -= Time.deltaTime;
        }
    }

    private void HandleAnimations()
    {
        Animator.SetBool("Grounded", CheckGrounded());

        if (attemptMeleeAttack)
        {
            if (!isMeleeAttacking)
            {
                StartCoroutine(MeleeAttackAnimDelay());
                StartCoroutine(ActionFreezeDelay());
            }
        }

        if (attemptRangedAttack)
        {
            if (!isRangedAttacking)
            {
                StartCoroutine(RAngedAttackAnimDelay());
            }
            
        }

        if (attemptJump && CheckGrounded() || Rb2D.velocity.y > 1f)
        {
            if (!isMeleeAttacking)
            {
                Animator.SetTrigger("Jump");
            }
        }

        if ( Rb2D.velocity.y < 0f)
        {
            if (!isMeleeAttacking)
            {
                Animator.SetTrigger("Fall");
            }
        }

        if (Mathf.Abs(moveIntentionX) > 0.1f && CheckGrounded())
        {
            Animator.SetInteger("AnimState", 2);
        }
        else
        {
            Animator.SetInteger("AnimState", 0);
        }
    }

    

    private IEnumerator MeleeAttackAnimDelay()
    {
        Animator.SetTrigger("Attack");
        isMeleeAttacking = true;
        isRangedAttacking = true;
        yield return new WaitForSeconds(meleeAttackDelay);
        isMeleeAttacking = false;
        isRangedAttacking = false;
    }

    private IEnumerator RAngedAttackAnimDelay()
    {
        Animator.SetTrigger("Shoot");
        isMeleeAttacking = true;
        isRangedAttacking = true;
        yield return new WaitForSeconds(rangedAttackDelay);
        isMeleeAttacking = false;
        isRangedAttacking = false;
    }

    private IEnumerator ActionFreezeDelay()
    {
        isFrozen = true;
        yield return new WaitForSeconds(freezeDelay);
        isFrozen = false;
    }

    public void DiePlayer()
    {
        
        FindObjectOfType<LevelManager>().Restart();

    }

    




}
