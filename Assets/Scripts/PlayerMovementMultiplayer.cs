using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMovementMultiplayer : NetworkBehaviour
{
    [Header("Camera")]
    //[SerializeField] private GameObject camera;

    [Header("Player Movement")]
    [SerializeField]
    private float speed;

    [SerializeField] private float jumpForce;
    //[SerializeField] private Camera playerCam;
    [SerializeField] private GameObject cam;

    [Header("Ground Detection")]
    [SerializeField]
    private float groundDistance;

    [SerializeField] private LayerMask groundMask;

    private Vector3 movementInput;
    private Rigidbody rb;
    private Animator anime;
    private bool jumpPressed = false;
    private float moveFB, moveLR;
    private float targetAngle = 99999f;

    //Hashed Animation Variables
    private static readonly int inAirAnime = Animator.StringToHash("In Air");
    private static readonly int jumpingAnime = Animator.StringToHash("Jumping");
    private static readonly int speedAnime = Animator.StringToHash("Speed");
    private static readonly int turningAngleAnime = Animator.StringToHash("Turning Angle");

    [Header("Snowball Mechanics")]
    [SerializeField] private GameObject snowball;
    [SyncVar] public int health = 100;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();

        if (!isLocalPlayer) cam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

        if (!hasAuthority || !isLocalPlayer) return;

        // Update Movement Inputs
        updateDirInput();
        movementInput = new Vector3(moveLR, 0f, moveFB);
        //Debug.Log(playingMovingAnime());
        //Jumping and Check inAir
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded())
        {
            jumpPressed = true;
            anime.SetTrigger(jumpingAnime);
        }
        anime.SetBool(inAirAnime, !isGrounded());



        if (Input.GetKeyDown(KeyCode.E)) {
            CmdFire();
        }

    }

    [Command]
    private void CmdFire()
    {
        GameObject ball = Instantiate(snowball, new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z), transform.rotation);
        ball.GetComponent<SnowBallzMultiplayer>().setSnowballSettings(this.GetComponent<Collider>(), transform.forward);

        NetworkServer.Spawn(ball);
    }

    public void decreaseHealth()
    {
        Debug.Log("Health decreased");
        health -= 10;
    }


    private void FixedUpdate()
    {
        if (!hasAuthority || !isLocalPlayer) return;

        applyMovement();
    }
    private float angleDifference()
    {
        Vector3 playerDir = transform.forward;
        //Vector3 cameraDir = playerCam.transform.forward;
        //cameraDir.y = 0f;

        Vector3 cameraDir = new Vector3(0f, 0f, 0.7f);

        Quaternion cameraToWorldDiff = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(cameraDir));
        Vector3 moveTargetDir = cameraToWorldDiff * movementInput;
        Vector3 sign = Vector3.Cross(moveTargetDir, playerDir);
        float angleDiff = Vector3.Angle(playerDir, moveTargetDir) * (sign.y < 0 ? 1f : -1f);

        return angleDiff;
    }

    private void updateDirInput()
    {
        //Movement Key Pressed
        if (Input.GetKey(KeyCode.W) && Input.GetKey(KeyCode.S))
            moveFB = 0f;

        else if (Input.GetKey(KeyCode.W))
            moveFB = 1f;

        else if (Input.GetKey(KeyCode.S))
            moveFB = -1f;

        else
            moveFB = 0f;

        if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D))
            moveLR = 0f;

        else if (Input.GetKey(KeyCode.D))
            moveLR = 1f;

        else if (Input.GetKey(KeyCode.A))
            moveLR = -1f;

        else
            moveLR = 0f;

        if (moveLR != 0f || moveFB != 0f)
        {
            anime.SetInteger(speedAnime, Input.GetKey(KeyCode.LeftShift) ? 2 : 1);
        }
        else
        {
            anime.SetInteger(speedAnime, 0);
        }

    }

    private bool playingMovingAnime()
    {
        if (anime.GetCurrentAnimatorStateInfo(0).IsName("Running.Running"))
            return true;

        else if (anime.GetCurrentAnimatorStateInfo(0).IsName("Idle / Walk.Walking"))
            return true;

        else if (anime.GetCurrentAnimatorStateInfo(0).IsName("Idle / Walking.Idle"))
            return true;

        return false;
    }

    private void applyMovement()
    {
        // Vector3 moveVector = transform.TransformDirection(movementInput) * speed;
        // rb.velocity = new Vector3(moveVector.x, rb.velocity.y, moveVector.z);
        // anime.SetFloat(velXAnime, moveVector.x / speed);
        // anime.SetFloat(velYAnime, moveVector.z / speed);

        float angleChange = angleDifference();
        anime.SetFloat(turningAngleAnime, angleChange);

        targetAngle = angleChange != 0f ? angleChange + transform.eulerAngles.y : targetAngle;

        if (playingMovingAnime())
        {
            float rot = Mathf.LerpAngle(0, targetAngle - transform.eulerAngles.y, Time.deltaTime * 10f);
            transform.Rotate(0, rot, 0, Space.World);
        }

        else if (!playingMovingAnime() && anime.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.85f)
        {
            //Debug.Log(targetAngle);
            transform.eulerAngles = new Vector3(0f, targetAngle, 0f);
        }




        if (jumpPressed)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            jumpPressed = false;
        }
    }

    private bool isGrounded()
    {
        return Physics.CheckSphere(transform.position, groundDistance, groundMask);
    }

}
