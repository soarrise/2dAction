using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StageCtrl : MonoBehaviour
{
    [Header("プレイヤーゲームオブジェクト")] public GameObject playerObj;
    [Header("コンティニュー位置")] public GameObject[] continuePoint;
    [Header("ゲームオーバー")] public GameObject gameOverObj;
    [Header("ステージクリア")] public GameObject stageClearObj;
    [Header("ステージクリア判定")] public PlayerTriggerCheck stageClearTrigger;
    [Header("フェード")] public FadeImage fade;
    [Header("ゲームオーバー時の音")] public AudioClip gameoverSE;
    [Header("リトライ時の音")] public AudioClip retrySE;
    [Header("ステージクリアの音")] public AudioClip clearSE;


    private player p;
    private int nextStageNum;
    private bool startFade = false;
    private bool doGameOver = false;
    private bool retryGame = false;
    private bool doSceneChange = false;
    private bool doClear = false;

    // Start is called before the first frame update
    void Start()
    {
        if (playerObj != null && continuePoint != null && continuePoint.Length > 0)
        {
            gameOverObj.SetActive(false);
            stageClearObj.SetActive(false);
            playerObj.transform.position = continuePoint[0].transform.position;
            p = playerObj.GetComponent<player>();
            if (p == null)
            {
                Debug.Log("プレイヤーじゃない物がアタッチされているよ！");
            }
        }
        else
        {
            Debug.Log("設定が足りてないよ！");
        }
    }

    // Update is called once per frame 
    void Update()
    {
        if(GameManager.instance.isGameOver && !doGameOver)
        {
            gameOverObj.SetActive(true);
            doGameOver = true;
            GameManager.instance.PlaySE(gameoverSE);
        }
        else if (p != null && p.IsContinueWaiting() && !doGameOver)
        {
            if (continuePoint.Length > GameManager.instance.continueNum)
            {
                playerObj.transform.position = continuePoint[GameManager.instance.continueNum].transform.position;
                p.ContinuePlayer();
            }
            else
            {
                Debug.Log("コンティニューポイントの設定が足りてないよ！");
            }
        }else if(stageClearTrigger != null && stageClearTrigger.isOn && !doGameOver&&!doClear)
        {
            StageClear();
            doClear = true;
        }
        //ステージを切り替える
        if (fade != null && startFade && !doSceneChange)
        {
            if (fade.IsFadeOutComplete())
            {
                //ゲームリトライ
                if (retryGame)
                {
                    GameManager.instance.RetryGame();
                    GameManager.instance.PlaySE(retrySE);
                }
                //次のステージ
                else
                {
                    GameManager.instance.stageNum = nextStageNum;
                }
                GameManager.instance.isStageClear = false;
                SceneManager.LoadScene("stage" + nextStageNum);
                doSceneChange = true;
            }
        }
    }
    /// <summary>
    /// 最初から始める New!
    /// </summary>
    public void Retry()
    {
        ChangeScene(1); //最初のステージに戻るので１
        retryGame = true;
    }

    /// <summary>
    /// ステージを切り替えます。 New!
    /// </summary>
    /// <param name="num">ステージ番号</param>
    public void ChangeScene(int num)
    {
        if (fade != null)
        {
            nextStageNum = num;
            fade.StartFadeOut();
            startFade = true;
        }
    }

    /// <summary>
    /// ステージをクリアした
    /// </summary>
    public void StageClear()
    {
        GameManager.instance.isStageClear = true;
        stageClearObj.SetActive(true);
        GameManager.instance.PlaySE(clearSE);
    }
}
