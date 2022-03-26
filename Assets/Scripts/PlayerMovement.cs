using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;


public class PlayerMovement : NetworkBehaviour
{
    [Header("Player Movement")]
    [SerializeField] private float airSpeed;

    [SerializeField] private float jumpForce;
    [SerializeField] private Camera playerCam;
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
    public float targetAngle = 99999f;

    [SyncVar] [SerializeField] public int health = 100;

    //Hashed Animation Variables
    private static readonly int inAirAnime = Animator.StringToHash("In Air");
    private static readonly int jumpingAnime = Animator.StringToHash("Jumping");
    private static readonly int speedAnime = Animator.StringToHash("Speed");
    private static readonly int turningAngleAnime = Animator.StringToHash("Turning Angle");

    private GameObject mainCamera;

    [SerializeField] GameObject particleSmall;
    [SerializeField] GameObject particleMedium;
    [SerializeField] GameObject particleLarge;


    // Start is called before the first frame update
    void Start()
    {

        if (!hasAuthority || !isLocalPlayer) return;

        rb = GetComponent<Rigidbody>();
        anime = GetComponent<Animator>();

        mainCamera = GameObject.FindWithTag("MainCamera");
        mainCamera.SetActive(false);

        cam.SetActive(true);

        health = 100;

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


        if (health <= 0)
        {
            //this.transform.position = new Vector3(-36.34f, -2.75f, -1.62f);
            //this.gameObject.SetActive(false);
            NetworkServer.Destroy(this.gameObject);
            mainCamera.SetActive(true);
            GameObject.FindGameObjectWithTag("PlayerManager").GetComponent<PlayerManager>().DeadTrue();
            CmdDisconnect();
        }

    }

    private void FixedUpdate()
    {

        if (!hasAuthority || !isLocalPlayer) return;
        applyMovement();
    }

    private float angleDifference()
    {
        Vector3 playerDir = transform.forward;
        Vector3 cameraDir = playerCam.transform.forward;
        cameraDir.y = 0f;

        Quaternion cameraToWorldDiff = Quaternion.FromToRotation(Vector3.forward, Vector3.Normalize(cameraDir));
        Vector3 moveTargetDir = cameraToWorldDiff * movementInput;
        Vector3 sign = Vector3.Cross(moveTargetDir, playerDir);
        float angleDiff = Vector3.Angle(playerDir, moveTargetDir) * (sign.y < 0 ? 1f : -1f);

        return angleDiff;
    }

    private void updateDirInput()
    {
        if(Input.GetKey(KeyCode.K))
        {
            decreaseHealth(100);
        }

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
        
        float angleChange = angleDifference();
        anime.SetFloat(turningAngleAnime, angleChange);

        targetAngle = angleChange != 0f ? angleChange + transform.eulerAngles.y : targetAngle;

        if (playingMovingAnime() || !isGrounded())
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

        anime.applyRootMotion = isGrounded();
        if (!isGrounded() && (moveLR != 0f || moveFB != 0f))
        {
            rb.velocity = new Vector3(transform.forward.x * airSpeed, rb.velocity.y, transform.forward.z * airSpeed);
        }

    }

    public bool isGrounded()
    {
        return Physics.CheckSphere(transform.position, groundDistance, groundMask);
    }

    public void decreaseHealth(int damage)
    {

        health -= damage;

        /*if (health <= 0)
        {

            connectionToClient.Disconnect();
        }*/

    }

    [Command]
    void CmdDisconnect()
    {
        connectionToClient.Disconnect();
    }

    [ClientRpc]
    public void RpcParticle(Vector3 pos, float s)
    {
        GameObject particle = null;
        switch (s)
        {
            case < 2f:
                particle = Instantiate(particleSmall, pos, Quaternion.identity);
                break;
            case >= 2f and < 4f:
                particle = Instantiate(particleMedium, pos, Quaternion.identity);
                break;
            case >= 4f:
                particle = Instantiate(particleLarge, pos, Quaternion.identity);
                break;
        }
        Destroy(particle, 1f);
    }


}