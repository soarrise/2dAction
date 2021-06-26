using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class player : MonoBehaviour
 {
    #region//インスペクターで設定する
    [Header("移動速度")] public float speed;
    [Header("ジャンプ速度")] public float jumpSpeed;
    [Header("ジャンプする高さ")] public float jumpHeight;
    [Header("ジャンプする長さ")] public float jumpLimitTime;
    [Header("踏みつけ判定の高さの割合")] public float stepOnRate;
    [Header("重力")] public float gravity;
    [Header("接地判定")] public GroundChaeck ground;
    [Header("天井判定")] public GroundChaeck head;
    [Header("ダッシュの速さ表現")] public AnimationCurve dashCurve;
    [Header("ジャンプの速さ表現")] public AnimationCurve jumpCurve;
    [Header("ジャンプするときに鳴らすSE")] public AudioClip jumpSE;
    [Header("しぬするときに鳴らすSE")] public AudioClip playerdieSE;
    #endregion

    #region//プライベート変数 
    private Animator anim = null;
    private Rigidbody2D rb = null;
    private CapsuleCollider2D capcol = null;
    //private SpriteRenderer sr = null;
    private bool isGround = false;
    private bool isJump = false;
    private bool isRun = false;
    private bool isDown = false;
    private bool isHead = false;
    private bool isOtherJump = false;
    private bool isContinue = false;
    private bool nonDownAnim = false;
    private bool isClearMotion = false;
    private float jumpPos = 0.0f;
    private float otherJumpHeight = 0.0f;
    private float dashTime = 0.0f;
    private float jumpTime = 0.0f;
    private float beforeKey = 0.0f;
    private float continueTime = 0.0f;
    private float blinkTime = 0.0f;
    private string enemyTag = "Enemy";
    private string deadAreaTag = "DeadArea";
    private string hitAreaTag = "HitArea";
    #endregion

    void Start()
{
    //コンポーネントのインスタンスを捕まえる
    anim = GetComponent<Animator>();
    rb = GetComponent<Rigidbody2D>();
    capcol = GetComponent<CapsuleCollider2D>();
    //sr = GetComponent<SpriteRenderer>();
}
    private void Update()
    {
        if (isContinue)
        {
            //明滅　ついている時に戻る
            if (blinkTime > 0.2f)
            {
                //sr.enabled = true;
                blinkTime = 0.0f;
            }
            //明滅　消えているとき
            else if (blinkTime > 0.1f)
            {
                //sr.enabled = false;
            }
            //明滅　ついているとき
            else
            {
                //sr.enabled = true;
            }
            //1秒たったら明滅終わり
            if (continueTime > 1.0f)
            {
                isContinue = false;
                blinkTime = 0.0f;
                continueTime = 0.0f;
                //sr.enabled = true;
            }
            else
            {
                blinkTime += Time.deltaTime;
                continueTime += Time.deltaTime;
            }
        }
    }
    void FixedUpdate()
{
        if (!isDown && !GameManager.instance.isGameOver && !GameManager.instance.isStageClear)
        {
            //接地判定を得る
            isGround = ground.IsGround();
            isHead = head.IsGround();

            //各種座標軸の速度を求める
            float xSpeed = GetXSpeed();
            float ySpeed = GetYSpeed();

            //アニメーションを適用
            SetAnimation();

            //移動速度を設定
            rb.velocity = new Vector2(xSpeed, ySpeed);
        }
        else
        {
            if(!isClearMotion && GameManager.instance.isStageClear)
            {
                anim.Play("player_clear");
                isClearMotion = true;
            }
            rb.velocity = new Vector2(0, -gravity);
        }
}

/// <summary> 
/// Y成分で必要な計算をし、速度を返す。 
/// </summary> 
/// <returns>Y軸の速さ</returns> 
private float GetYSpeed()
{
    float verticalKey = Input.GetAxis("Vertical");
    float ySpeed = -gravity;

    if (isOtherJump)
    {
        //現在の高さが飛べる高さより下か
         bool canHeight = jumpPos + otherJumpHeight > transform.position.y;
         //ジャンプ時間が長くなりすぎてないか
         bool canTime = jumpLimitTime > jumpTime;

         if (canHeight && canTime && !isHead)
         {
             ySpeed = jumpSpeed;
             jumpTime += Time.deltaTime;
         }
         else
         {
             isOtherJump = false;
             jumpTime = 0.0f;
         }
    }    
    else if (isGround)
    {
        if (verticalKey > 0)
        {
                if (!isJump)
                {
                    GameManager.instance.PlaySE(jumpSE);
                }
            ySpeed = jumpSpeed;
            jumpPos = transform.position.y; //ジャンプした位置を記録する
            isJump = true;
            jumpTime = 0.0f;
        }
        else
        {
            isJump = false;
        }
    }
    else if (isJump)
    {
        //上方向キーを押しているか
        bool pushUpKey = verticalKey > 0;
        //現在の高さが飛べる高さより下か
        bool canHeight = jumpPos + jumpHeight > transform.position.y;
        //ジャンプ時間が長くなりすぎてないか
        bool canTime = jumpLimitTime > jumpTime;

        if (pushUpKey && canHeight && canTime && !isHead)
        {
            ySpeed = jumpSpeed;
            jumpTime += Time.deltaTime;
        }
        else
        {
            isJump = false;
            jumpTime = 0.0f;
        }
    }

    if (isJump)
    {
        ySpeed *= jumpCurve.Evaluate(jumpTime);
    }

    return ySpeed;
}

/// <summary> 
/// X成分で必要な計算をし、速度を返す。 
/// </summary> 
/// <returns>X軸の速さ</returns> 
private float GetXSpeed()
{
    float horizontalKey = Input.GetAxis("Horizontal");
    float xSpeed = 0.0f;

    if (horizontalKey > 0)
    {
        transform.localScale = new Vector3(1, 1, 1);
        isRun = true;
        dashTime += Time.deltaTime;
        xSpeed = speed;
    }
    else if (horizontalKey < 0)
    {
        transform.localScale = new Vector3(-1, 1, 1);
        isRun = true;
        dashTime += Time.deltaTime;
        xSpeed = -speed;
    }
    else
    {
        isRun = false;
        xSpeed = 0.0f;
        dashTime = 0.0f;
    }

    //前回の入力からダッシュの反転を判断して速度を変える
    if (horizontalKey > 0 && beforeKey < 0)
    {
        dashTime = 0.0f;
    }
    else if (horizontalKey < 0 && beforeKey > 0)
    {
        dashTime = 0.0f;
    }

    beforeKey = horizontalKey;
    xSpeed *= dashCurve.Evaluate(dashTime);
    beforeKey = horizontalKey;
    return xSpeed;
}

/// <summary> 
/// アニメーションを設定する 
/// </summary> 
private void SetAnimation()
{
    anim.SetBool("jump", isJump || isOtherJump);
    anim.SetBool("ground", isGround);
    anim.SetBool("run", isRun);
}
    /// <summary> 
    　　/// コンティニュー待機状態か 
    /// </summary> 
    /// <returns></returns> 
    public bool IsContinueWaiting()
    {
        if (GameManager.instance.isGameOver)  // New!
        {
            return false;
        }
        else
        {
            return IsDownAnimEnd() || nonDownAnim;  // New!
        }
    }

    //ダウンアニメーションが完了しているかどうか 
    private bool IsDownAnimEnd()
    {
        if (isDown && anim != null)
        {
            AnimatorStateInfo currentState = anim.GetCurrentAnimatorStateInfo(0);
            if (currentState.IsName("player_down"))
            {
                if (currentState.normalizedTime >= 1)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary> 
    /// コンティニューする 
    /// </summary> 
    public void ContinuePlayer()
    {
        isDown = false;
        anim.Play("player");
        isJump = false;
        isOtherJump = false;
        isRun = false;
        isContinue = true;
        nonDownAnim = false;
    }

    private void ReceiveDamage(bool downAnim)
    {
        if (isDown || GameManager.instance.isStageClear)
        {
            return;
        }
        else
        {
            if (downAnim)
            {
                anim.Play("player_down");
                GameManager.instance.PlaySE(playerdieSE);
            }
            else
            {
                nonDownAnim = true;
            }
            isDown = true;
            GameManager.instance.SubHeartNum();
        }
    }

    #region//接触判定
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.collider.tag == enemyTag)
        {
            foreach(ContactPoint2D p in collision.contacts)
            {
                //踏みつけ判定になる高さ
                float stepOnHeight = (capcol.size.y * (stepOnRate / 100f));

                //踏みつけ判定のワールド座標
                float judgePos = transform.position.y - (capcol.size.y / 2f) + stepOnHeight;

                if (p.point.y < judgePos)//衝突した位置が足元だったら
                {
                    
                    //もう一度跳ねる
                    objectcolison o = collision.gameObject.GetComponent<objectcolison>();

                    if(o != null)
                    {
                        otherJumpHeight = o.boundHeight;
                        o.playerStepOn = true;
                        jumpPos = transform.position.y;
                        isOtherJump = true;
                        isJump= false;
                        jumpTime = 0.0f;
                    }
                    else
                    {
                        Debug.Log("ObjectCollisionがついてないよ！");
                    }
                }
                else
                {
                    //ダウンする
                    ReceiveDamage(true);
                    break;
                }
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == deadAreaTag)
        {
            ReceiveDamage(false);
        }
        else if (collision.tag == hitAreaTag)
        {
            ReceiveDamage(true);
        }
    }
    #endregion
}