using System.Collections;
using System.Collections.Generic;
using System.Xml.Schema;
using UnityEngine;

public class Enemy_Zako1 : MonoBehaviour
{
    #region//インスペクターで設定する
    [Header("加算スコア")] public int myScore;
    [Header("移動速度")] public float speed;
    [Header("重力")] public float gravity;
    [Header("画面外でも行動するか")] public bool nonVisibleAct;
    [Header("接触判定")] public EnemyCollision checkCollison;
    [Header("敵死ぬときに鳴らすSE")] public AudioClip dieSE;
    #endregion

    #region//プライベート変数
    private Rigidbody2D rb = null;
    private SpriteRenderer sr = null;
    private Animator anim = null;
    private objectcolison oc = null;
    private BoxCollider2D col = null;
    private bool rightTleftF = false;
    private bool isDead = false;
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        oc = GetComponent<objectcolison>();
        col = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!oc.playerStepOn)
        {
            if (sr.isVisible || nonVisibleAct)
            {
                if (checkCollison.isOn)
                {
                    Debug.Log("壁");
                    rightTleftF = !rightTleftF;
                }
                int xVector = -1;
                if (rightTleftF)
                {
                    xVector = 1;
                    transform.localScale = new Vector3(-1, 1, 1);
                }
                else
                {
                    transform.localScale = new Vector3(1, 1, 1);
                }
                rb.velocity = new Vector2(xVector * speed, -gravity);
            }
            else
            {
                rb.Sleep();
            }
        }
        else
        {
            if (!isDead)
            {
                anim.Play("enemy_zako_down");
                GameManager.instance.PlaySE(dieSE);
                rb.velocity = new Vector2(0, -gravity);
                isDead = true;
                col.enabled = false;
                if(GameManager.instance != null)
                {
                    GameManager.instance.score += myScore;  
                }
                Destroy(gameObject, 3f);
            }
            else
            {
                transform.Rotate(new Vector3(0, 0, 5));
            }
        }
    }
}
