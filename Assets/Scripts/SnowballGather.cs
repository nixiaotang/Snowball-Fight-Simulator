using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SnowballGather : NetworkBehaviour
{
    
    [Header("Ball Parameters")] 
    public float maxSize;
    public float ballSize;
    public int maxNum;
    public int ballNum;
    [SerializeField] private float projectileSpeed;
    [SerializeField] private float growthRate;
    [SerializeField] private Transform rightHandPos;
    [SerializeField] private LayerMask groundMask;

    [Header("Accessed Objects")]
    public GameObject ball;
    public GameObject projectilePref;
    public GameObject[] shootBalls;
    public int curBall;
    public Camera cam;

    private Rigidbody rigidBody;
    private bool startGather;
    private float aimRotation;
    private Animator anime;

    private static readonly int throwAnime = Animator.StringToHash("Throw");
    private static readonly int shootAnime = Animator.StringToHash("Shoot");
    private PlayerMovement movementScript;

    private string animState = "IDLE"; //"IDLE", "AIM", "THROW"
    

    void Start()
    {
        if (!hasAuthority || !isLocalPlayer) return;

        aimRotation = 0f;
        startGather = false;
        curBall = 0;
        anime = GetComponent<Animator>();
        movementScript = GetComponent<PlayerMovement>();

    }

    
    void Update()
    {
        if (!hasAuthority || !isLocalPlayer) return;
        
        gatherBalls();
        shootingBalls();

    }

    void LateUpdate()
    {
        if (!hasAuthority || !isLocalPlayer) return;

        if (isAim)
        {
            movementScript.targetAngle = aimRotation;
        }
        if (!isShoot && !isAim) { anime.SetInteger(throwAnime, 0); }
    }

    void gatherBalls()
    {

        if (isGather)
        {


            //Debug.Log("kermit");
            ballSize += growthRate;
            ball.SetActive(true);
            ball.transform.localScale = new Vector3(ballSize, ballSize, ballSize);
            ball.transform.localPosition = new Vector3(0, 2.1f, 1.5f);
            startGather = true;


        }
        if (endGather)
        {
            shootBalls[Array.IndexOf(shootBalls, Array.Find(shootBalls, element => element.transform.localScale.x == -1))].transform.localScale = new Vector3(ballSize, ballSize, ballSize);

            ballSize = 0.1f;
            ballNum++;

            ball.SetActive(false);

            startGather = false;

            if (curBall < GetComponent<SnowballGather>().maxNum)
            {
                curBall++;
            }
            else
            {
                curBall = 0;
            }

            /*shootBalls[Array.IndexOf(shootBalls, Array.Find(shootBalls, element => element.transform.localScale.x == -1 ))].transform.localScale = new Vector3(ballSize, ballSize, ballSize);
            
            ballSize = 0.1f;
            ballNum++;
            ball.SetActive(false);
            startGather = false;

            if (curBall < GetComponent<SnowballGather>().maxNum)
            {
                curBall++;
            }
            else
            {
                curBall = 0;
            }*/


        }


    }

    void shootingBalls()
    {
        //anime.ResetTrigger("Shoot");

        if(animState == "IDLE")
        {
            anime.SetBool(shootAnime, false);
            if (isAim)
            {
                animState = "AIM";
            }

        } else if(animState == "AIM")
        {
            anime.SetInteger(throwAnime, 1);
            shootBalls[0].SetActive(true);

            var ray = cam.ScreenPointToRay(Input.mousePosition);


            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundMask))
            {

                // shootBalls[0].transform.localPosition = new Vector3(0, 2.1f, 1.5f);
                shootBalls[0].transform.position = rightHandPos.position + rightHandPos.forward * (shootBalls[0].transform.localScale.x * 0.65f);
                aimRotation = Mathf.Atan2((hit.point.x - shootBalls[0].transform.position.x), (hit.point.z - shootBalls[0].transform.position.z)) * Mathf.Rad2Deg;
                transform.eulerAngles = new Vector3(transform.eulerAngles.x, aimRotation, transform.eulerAngles.z);
            }

            if (isShoot)
            {
                animState = "SHOOT";

                //////////////////////////////////////////////
                
                shootBalls[0].SetActive(false);

                ballNum--;
                var ray2 = cam.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray2, out RaycastHit hit2, Mathf.Infinity, groundMask))
                {

                    CmdServerFire(hit2.point, aimRotation, shootBalls[0].transform.localScale);

                    anime.SetBool(shootAnime, true);


                    for (int i = 0; i < maxNum; i++)
                    {
                        if (i < maxNum - 1)
                        {
                            shootBalls[i].transform.localScale = shootBalls[i + 1].transform.localScale;
                        }
                        else
                        {
                            shootBalls[i].transform.localScale = new Vector3(-1, -1, -1);
                        }
                    }


                }

                ////////////////////////////////////////////
            }

        } else if(animState == "SHOOT")
        {

            if (anime.GetCurrentAnimatorStateInfo(0).IsName("Throw Release") || anime.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
            {
                animState = "IDLE";
            }
        }

    }

    [Command]
    void CmdServerFire(Vector3 hitPoint, float aimRot, Vector3 ballLocalScale)
    {

        //GameObject projectile = Instantiate(projectilePref, rightHandPos.position + rightHandPos.forward * (shootBalls[0].transform.localScale.x * 0.65f), Quaternion.identity);

        GameObject projectile = Instantiate(projectilePref, shootBalls[0].transform.position, Quaternion.identity);
        projectile.transform.localScale = new Vector3(ballLocalScale.x, ballLocalScale.x, ballLocalScale.x);
        
        projectile.SetActive(true);

        SnowBallz projScript = projectile.GetComponent<SnowBallz>();


        projScript.shooterID = this.gameObject.ToString();
        projScript.size = ballLocalScale.x;
        projScript.shooterScript = this.gameObject.GetComponent<PlayerMovement>();

        rigidBody = projectile.GetComponent<Rigidbody>();

        float shootRotation = Mathf.Atan2(hitPoint.y - transform.position.y ,
                    Mathf.Sqrt((hitPoint.x - transform.position.x) * (hitPoint.x - transform.position.x) + (hitPoint.z - transform.position.z) * (hitPoint.z - transform.position.z)));

        Vector3 force = new Vector3(Mathf.Sin(aimRot * Mathf.Deg2Rad) * projectileSpeed, Mathf.Sin(shootRotation) * projectileSpeed, Mathf.Cos(aimRot * Mathf.Deg2Rad) * projectileSpeed);
        projScript.force = force;


        NetworkServer.Spawn(projectile);
    }

    public bool isShoot => Input.GetMouseButtonUp(1) && shootBalls[0].transform.localScale.x != -1;
    public bool isAim => Input.GetMouseButton(1) && shootBalls[0].transform.localScale.x != -1;
    public bool endGather => !isGather && startGather;
    public bool isGather => movementScript.isGrounded() && Input.GetKey("e") && Input.GetAxis("Horizontal") == 0 && Input.GetAxis("Vertical") == 0 && ballSize <= maxSize && ballNum < maxNum;
}
