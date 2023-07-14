using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    private Collision coll;
    [HideInInspector]
    public Rigidbody2D rb;
    //private AnimationScript anim;

    [Space]
    [Header("Stats")]
    public int jumpsLeft = 2;
    public float speed = 20;
    public float jumpForce = 20;
    public float slideSpeed = 5;
    public float controlSpeed = 50;
    public float dashSpeed = 50;
    public float dragTimer = 0.25f;
    public float dragAmount = 15;
    [SerializeField] private AnimationCurve curve;

    [Space]
    [Header("Booleans")]
    public bool canMove;
    public bool wallGrab;
    public bool wallJumped;
    public bool wallSlide;
    public bool isDashing;

    [Space]

    private bool groundTouch;
    private bool hasDashed;

    public int side = 1;

    [Space]
    [Header("Polish")]
    public ParticleSystem dashParticle;
    public ParticleSystem jumpParticle;
    public ParticleSystem wallJumpParticle;
    public ParticleSystem slideParticle;
    [SerializeField] private TrailRenderer tr;

    // Start is called before the first frame update
    void Start()
    {
        coll = GetComponent<Collision>();
        rb = GetComponent<Rigidbody2D>();
        //anim = GetComponentInChildren<AnimationScript>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        float xRaw = Input.GetAxisRaw("Horizontal");
        float yRaw = Input.GetAxisRaw("Vertical");
        Vector2 dir = new Vector2(x, y);
        Walk(dir);
        //anim.SetHorizontalMovement(x, y, rb.velocity.y);


        if (coll.onGround && !isDashing)
        {
            wallJumped = false;
            GetComponent<BetterJumping>().enabled = true;
        }


        if (Input.GetButtonDown("Jump"))
        {
            //anim.SetTrigger("jump");
            //rb.drag = 0;
            Debug.Log("ground: " + coll.onGround);
            if (jumpsLeft > 0)
                Jump(Vector2.up, false);
        }

        if (Input.GetButtonDown("Fire1") && !hasDashed)
        {
            if (xRaw != 0 || yRaw != 0)
                Dash(xRaw, yRaw);
        }

        if (coll.onGround && !groundTouch)
        {
            GroundTouch();
            groundTouch = true;
        }

        if (!coll.onGround && groundTouch)
        {
            controlSpeed = 10;
            groundTouch = false;
        }


        if (x > 0 && coll.onGround)
        {
            side = 1;
            //anim.Flip(side);
        }
        if (x < 0 && coll.onGround)
        {
            side = -1;
            //anim.Flip(side);
        }


    }

    private void FixedUpdate()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, 75);
    }

    void GroundTouch()
    {
        hasDashed = false;
        isDashing = false;
        controlSpeed = 1000;
        jumpsLeft = 2;

        //side = anim.sr.flipX ? -1 : 1;
    }

    private void Dash(float x, float y)
    {
        controlSpeed = 10;
        hasDashed = true;

        rb.velocity = Vector2.zero;
        Vector2 dir = new Vector2(x, y).normalized;

        rb.velocity += dir * dashSpeed;
        Debug.Log(dir * dashSpeed);
        Debug.Log(rb.velocity);
        StartCoroutine(DashWait());
    }

    IEnumerator DashWait()
    {
        StartCoroutine(GroundDash());
        if(rb.velocity.y > 0)
        {
            DOVirtual.Float(0, dragAmount, dragTimer, RigidbodyDrag).OnComplete(() => {
                rb.drag = 0;
            });
        }
        

        rb.gravityScale = 0;
        GetComponent<BetterJumping>().enabled = false;
        wallJumped = true;
        isDashing = true;
        tr.emitting = true;
        yield return new WaitForSeconds(.2f);
        tr.emitting = false;
        rb.gravityScale = 20;
        GetComponent<BetterJumping>().enabled = true;
        wallJumped = false;
        isDashing = false;
    }

    IEnumerator GroundDash()
    {
        yield return new WaitForSeconds(.15f);
        if (coll.onGround)
            hasDashed = false;
    }


    private void Walk(Vector2 dir)
    {
        if (rb.drag != 0)
        {
            rb.drag = 0;
        }

        rb.velocity = Vector2.Lerp(rb.velocity, (new Vector2(dir.x * speed, rb.velocity.y)), controlSpeed * Time.deltaTime);
        
       
    }

    private void Jump(Vector2 dir, bool wall)
    {
        
        jumpsLeft -= 1;
        rb.velocity = new Vector2(rb.velocity.x, 0);
        rb.velocity += dir * jumpForce;

    }

    void RigidbodyDrag(float x)
    {
        rb.drag = x;
    }

}
