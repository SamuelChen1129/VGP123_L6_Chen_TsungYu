using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(SpriteRenderer), typeof(Animator))]
public class PlayerController : MonoBehaviour
{
    //Player gameplay variables
    private Coroutine jumpForceChange;
    private Coroutine speedChange;

    public void PowerupValueChange(Pickup.PickupType type)
    {
        if (type == Pickup.PickupType.PowerupSpeed)
            StartPowerupCoroutine(ref speedChange, ref speed, type);

        if (type == Pickup.PickupType.PowerupJump)
            StartPowerupCoroutine(ref jumpForceChange, ref jumpForce, type);

    }

    public void StartPowerupCoroutine(ref Coroutine InCoroutine, ref float inVar, Pickup.PickupType type)
    {
        if (jumpForceChange != null)
        {
            StopCoroutine(InCoroutine);
            InCoroutine = null;
            inVar /= 2;

        }

        InCoroutine = StartCoroutine(PowerupChange(type));
    }

    IEnumerator PowerupChange(Pickup.PickupType type)
    {
        //this code runs before the wait
        if (type == Pickup.PickupType.PowerupSpeed)
            speed *= 2;

        if (type == Pickup.PickupType.PowerupJump)
            jumpForce *= 2;


        Debug.Log($"Jump force value is {jumpForce}, Speed Value is {speed}");

        yield return new WaitForSeconds(5.0f);

        //this code runs after the wait
        if (type == Pickup.PickupType.PowerupSpeed)
        {
            speed /= 2;
            speedChange = null;
        }
        if (type == Pickup.PickupType.PowerupJump)
        {
            jumpForce /= 2;
            jumpForceChange = null;
        }


        Debug.Log($"Jump force value is {jumpForce}");


    }

    //Private Lives Variable
    private int _lives = 5;

    //public variable for getting and setting lives

    public int lives
    {
        get
        {
            return _lives;
        }
        set
        {
            //all lives lost (zero counts as a life due to the check)
            if (value < 0)
            {
                //game over function called here
                //return to prevent the rest of the function to be called
                return;
            }

            //lost a life
            if (value < _lives)
            {
                //Respawn function called he re

            }

            if (value > maxLives)
            {
                value = maxLives;
            }

            _lives = value;

            Debug.Log($"Lives value on {gameObject.name} has changed to {lives}");
        }
    }

    //max lives possible
    [SerializeField] private int maxLives = 10;

    //Movement Variables
    [SerializeField, Range(1, 20)] private float speed = 5;
    [SerializeField, Range(1, 20)] private float jumpForce = 10;
    [SerializeField, Range(0.01f, 1)] private float groundCheckRadius = 0.02f;
    [SerializeField] private LayerMask isGroundLayer;


    //GroundCHeck Stuff 
    private Transform groundCheck;
    private bool isGrounded = false;

    //Component References 

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    // Start is called before the first frame update
    void Start()
    {
        //Component References Filled 
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();

        // Debug.Log(rb.name);

        //Checking values to ensure non garbage data
        if (speed <= 0)
        {
            speed = 5;
            Debug.Log("Speed was set Incorrectly");
        }

        if (jumpForce <= 0)
        {
            jumpForce = 10;
            Debug.Log("jumpForce was set Incorrectly");
        }

        //Creating groundcheck object
        if (!groundCheck)
        {
            GameObject obj = new GameObject();
            obj.transform.SetParent(transform);
            obj.transform.localPosition = Vector3.zero;
            obj.name = "GroundCheck";
            groundCheck = obj.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //AnimatoprClipInfo[] curPlayingClips = anim.GetCurrentAnimatorClipInfo(0);

        //Create a small overlap collider to check if we are touching the ground
        IsGrounded();

        //Animation check for our physics
        //if (curPlayingClips.Length > 0)
        //{
        //    if (curPlayingClips[0].clip.name == "Attack" && isGrounded)
        //        rb.velocity = new Vector2.(0, rb,velocity.y);
        //    else
        //    {
        //        rb.velocity = new Vector2(hInput * speed, rb.velocity.y);
        //    }

        //}



        //grab horizontal axis - Check Project Settings > Input Manage4r to see the inputs defined 
        float hInput = Input.GetAxis("Horizontal");


        rb.velocity = new Vector2(hInput * speed, rb.velocity.y);

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }

        if (Input.GetButtonDown("Fire1") && isGrounded)
        {
            anim.SetTrigger("isAttacking");
        }

        if (Input.GetButtonDown("Fire1") && !isGrounded)
        {
            anim.SetTrigger("isJumpAttacking");
        }



        if (hInput != 0) sr.flipX = (hInput < 0);
        //if (hInput > 0 && sr.flipX || hInput < 0 && !sr.flipX) sr.flipX = !sr.flipX; {

        anim.SetFloat("hInput", Mathf.Abs(hInput));
        anim.SetBool("isGrounded", isGrounded);

    }
    void IsGrounded()
    {
        if (!isGrounded)
        {
            if (rb.velocity.y <= 0)
            {
                isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, isGroundLayer);
            }
        }
        else
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, isGroundLayer);
    }
    //void OnTriggerEvent2D(Collider2D other)
    //{
    //    if (other.CompareTag("OneUp"))
    //    {
    //        Destroy(other.gameObject);
    //        // Perform actions when the player collects the object
    //        // Add score, play a sound, or activate a power-up
    //        //Collect();
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Squish"))
        {
            collision.gameObject.GetComponentInParent<Enemy>().TakeDamage(9999);
            rb.velocity = Vector2.zero;
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }
}