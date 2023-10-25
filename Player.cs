using System.Collections;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private float speed = 7;

    // 默认转向右侧，速度为正
    [SerializeField]
    private bool faceingRight = true;

    [Header("Collision info")]
    [SerializeField]
    private float groundCheckDistance = 1;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private LayerMask wallLayer;

    [SerializeField]
    private float jumpSpeed = 10;

    [Header("Dash info")]
    [SerializeField]
    private double dashDuration = 0.3;

    [SerializeField]
    private float dashSpeed = 14;
    private double dashTime = 0;

    private Rigidbody2D rb;
    private Animator anim;

    [Header("Attack info")]
    private bool isAttacking = false;
    private int comboCounter = 0;
    private float comboTime = .7f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        checkAttack();
        checkMovement();
        checkJump();
    }

    public void attackOver()
    {
        isAttacking = false;
        comboCounter++;
        if (comboCounter > 2 || comboTime <= 0)
            comboCounter = 0;
    }

    void checkAttack()
    {
        comboTime -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q))
        {
            isAttacking = true;
            comboTime = .3f;
        }


        anim.SetBool("isAttacking", isAttacking);
        anim.SetInteger("comboCounter", comboCounter);
    }

    private void checkMovement()
    {
        // hack
        if (isAttacking)
        {
            anim.SetBool("isMoving", false);
            rb.velocity = new Vector2(0, rb.velocity.y);
            return;
        }

        float xInput = Input.GetAxisRaw("Horizontal");

        dashTime -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.LeftControl) && xInput != 0)
        {
            dashTime = dashDuration;
        }

        // 是否处在冲刺时间
        if (dashTime > 0)
        {
            rb.velocity = new Vector2(xInput * dashSpeed, 0);
        }
        else
        {
            rb.velocity = new Vector2(xInput * speed, rb.velocity.y);
        }

        // 控制翻转
        if (rb.velocity.x > 0 && !faceingRight || rb.velocity.x < 0 && faceingRight)
            flip();

        // 控制是否在移动
        bool isMoving = rb.velocity.x != 0;
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("isDashing", dashTime > 0);
    }

    private void checkJump()
    {
        // 判断是否任务是否在地面上
        // 其实就是判断从中心引一条线段出去，是否能和地面相交
        bool isGrounded = Physics2D.Raycast(
            transform.position,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
        }
        anim.SetBool("isGrounded", isGrounded);
        anim.SetFloat("yVelocity", rb.velocity.y);
    }

    private void flip()
    {
        // 这里是翻转y轴
        faceingRight = !faceingRight;
        transform.Rotate(0, 180, 0);
    }

    void OnDrawGizmos()
    {
        Gizmos.DrawLine(
            transform.position,
            new Vector3(transform.position.x, transform.position.y - groundCheckDistance)
        );
    }
}
