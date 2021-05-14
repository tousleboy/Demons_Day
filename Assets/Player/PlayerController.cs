﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;
    public float runSpeed = 3.0f;
    float axisH = 0.0f;

    public float jump = 9.0f;
    public LayerMask groundLayer;
    bool goJump = false;
    public bool onGround = false;
    public int howManyJump = 2;
    int jumpCount = 0;
    bool jumpStop = false;

    Animator animator;
    public string stopAnime = "PlayerIdle";
    public string runAnime = "PlayerRun";
    public string jumpAnime = "PlayerJump";
    public string[] comboAnimes;
    //public string jabAnime = "PlayerJab";
    public string blockAnime = "PlayerBlock";
    public string duckingAnime = "PlayerDuck";
    public string bodyJabAnime = "PlayerBodyJab";
    public string damagedAnime = "PlayerDamaged";
    //public string straightAnime = "PlayerStraight";
    //public string kickAnime = "PlayerKick";
    string nowAnime;
    string oldAnime;
    string actionAnime;

    bool goAttack = false;
    public bool attacking = false;//アニメーション内で制御
    bool goBlock = false;
    public bool blocking = false;
    bool goDuck = false;
    public bool ducking = false;
    public bool combo = false;//アニメーション内で制御
    public bool comboFinisher = false;//アニメーション内で制御
    public int combocount = 1;
    public float comboInterval = 0.5f;
    //float pushTimes = 0.0f;
    //public bool high = false;
    //public bool low = false;
    GameObject attackZone;

    public static int life;
    public int damage;
    public int diffence = 1;
    public bool damaged = false;
    string collisionState;

    public static string gameState;

    // Start is called before the first frame update
    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        nowAnime = stopAnime;
        oldAnime = stopAnime;
        attackZone = transform.Find("AttackZone").gameObject;
        life = 3;
        gameState = "playing";
    }

    // Update is called once per frame
    void Update()
    {
        if(life <= 0)
        {
            gameState = "gameover";
        }
        if(gameState != "playing")
        {
            return;
        }

        axisH = Input.GetAxisRaw("Horizontal");
        //passedTimes += Time.deltaTime;

        /*if(damage > 0)
        {
            if(collisionState == "high" && blocking)
            {
                damage -= diffence;
            }

            life -= damage;

            if(damage > 0)
            {
                Damage();
            }

            damage = 0;
        }*/

        if(axisH > 0.0f)
        {
            transform.localScale = new Vector2(1, 1);
        }
        else if(axisH < 0.0f)
        {
            transform.localScale = new Vector2(-1, 1);
        }

        if(Input.GetButtonDown("Jump"))
        {
            Jump();
        }
        if(Input.GetButtonUp("Jump") && !onGround && rbody.velocity.y > 0)
        {
            jumpStop = true;
        }

        if(Input.GetMouseButtonDown(0) || Input.GetKeyDown("j"))
        {
            if(onGround)
            {
                AttackManager am = attackZone.GetComponent<AttackManager>();
                am.state = "high";

                if(IsInvoking("ComboReset"))
                {
                    CancelInvoke("ComboReset");
                }
                Invoke("ComboReset", comboInterval);

                if(ducking)
                {
                    am.state = "low";
                    Action(bodyJabAnime, "attack");
                }
                else if(!comboFinisher)
                {
                    Action(comboAnimes[combocount - 1], "attack");
                    combocount += 1;
                    //passedTimes = 0.0f;
                }
                else
                {
                    ComboReset();
                }
            }
        }
        /*if(combocount > comboAnimes.Length || passedTimes > comboInterval)
        {
            combocount = 1;
        }*/

        if(Input.GetMouseButton(1) || Input.GetKey("k"))
        {
            Action(blockAnime, "block");
            blocking = true;
        }
        else if(Input.GetMouseButtonUp(1) || Input.GetKeyUp("k"))
        {
            blocking = false;
        }

        if(Input.GetAxisRaw("Vertical") < 0)
        {
            if(!goAttack)
            {
                Action(duckingAnime, "ducking");
            }
            ducking = true;
            //passedTimes = 0.0f;
        }
        else
        {
            ducking = false;
        }
    }

    void FixedUpdate()
    {
        if(gameState != "playing")
        {
            return;
        }

        onGround = Physics2D.Linecast(transform.position, transform.position -(transform.up * 0.1f), groundLayer);

        if(damaged)
        {
            return;
        }

        if(attacking || blocking || ducking)
        {
            axisH = 0;
            goJump = false;
        }

        if(onGround || axisH != 0)//走り
        {
            rbody.velocity = new Vector2(axisH * runSpeed, rbody.velocity.y);
        }

        if((goJump && (jumpCount <= howManyJump)) || (onGround && goJump))//多段ジャンプ
        {
            Vector2 jumpPw = new Vector2(0, jump);
            rbody.AddForce(jumpPw, ForceMode2D.Impulse);
            goJump = false;
        }
        else if(onGround)
        {
            jumpCount = 0;
        }

        if(jumpStop)
        {
            //rbody.velocity = new Vector2(rbody.velocity.x, 0.0f);
            StartCoroutine("JumpStop");
            jumpStop = false;
        }

        if(!attacking)
        {
            if(onGround)
            {
                if(goAttack || goBlock || goDuck)
                {
                    nowAnime = actionAnime;
                    if(goAttack)
                    {
                        goAttack = false;
                    }
                    if(goBlock)
                    {
                        goBlock = false;
                    }
                    if(goDuck)
                    {
                        goDuck = false;
                    }
                }
                else if(axisH == 0)
                {
                    nowAnime = stopAnime;
                }
                else
                {
                    nowAnime = runAnime;
                }
            }
            else
            {
                goAttack = false;
                nowAnime = jumpAnime;
            }
        }
        else if(goAttack)
        {
            nowAnime = actionAnime;
            goAttack = false;
        }
        if(nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Attack")
        {
            Debug.Log("hit");
            AttackManager am = collision.gameObject.GetComponent<AttackManager>();
            collisionState = am.state;
            damage = am.val;

            if(collisionState == "high" && blocking)
            {
                damage -= diffence;
            }

            life -= damage;

            if(damage > 0)
            {
                Damage();
                GetComponent<Renderer>().material.color = Color.red;
                Invoke("ColorReset", 0.1f);
            }

            damage = 0;

            if(am.knockBack)
            {
                am.KnockBack(gameObject);
            }
        }
        if(collision.gameObject.tag == "Dead")
        {
            life = 0;
            Damage();
        }
        if(collision.gameObject.tag == "Item")
        {
            Debug.Log(collision.gameObject.GetComponent<ItemData>().val + "yen get");
            Destroy(collision.gameObject);
        }
    }

    void Jump()
    {
        goJump = true;
        jumpCount += 1;
    }

    void ComboReset()
    {
        combocount = 1;
    }

    void ColorReset()
    {
        GetComponent<Renderer>().material.color = Color.white;
    }

    IEnumerator JumpStop()
    {
        while(rbody.velocity.y >= 0)
        {
            rbody.velocity = new Vector2(rbody.velocity.x, rbody.velocity.y - 1.0f);
            yield return null;
        }
    }

    void Action(string action, string mode)
    {
        actionAnime = action;
        if(mode == "attack")
        {
            goAttack = true;
        }
        if(mode == "block")
        {
            goBlock = true;
        }
        if(mode == "ducking")
        {
            goDuck = true;
        }
    }

    void Damage()
    {
        damaged = true;
        oldAnime = damagedAnime;
        animator.Play(damagedAnime);
    }
}
