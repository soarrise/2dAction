using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scoreitem : MonoBehaviour
{
    [Header("加算スコア")] public int myScore;
    [Header("プレイヤーの判定")] public PlayerTriggerCheck playerCheck;
    [Header("とったときに鳴らすSE")] public AudioClip itemSE;

    // Update is called once per frame
    void Update()
    {
        if (playerCheck.isOn)
        {
            if(GameManager.instance != null)
            {
                GameManager.instance.score += myScore;
                GameManager.instance.PlaySE(itemSE);
                Destroy(this.gameObject);
            }
        }
    }
}
