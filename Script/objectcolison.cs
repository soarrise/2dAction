using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class objectcolison : MonoBehaviour
{
    [Header("踏んだときのプレイヤーが跳ねる高さ")] public float boundHeight;

    /// <summary>
    /// このオブジェクトをプレイヤーが踏んだかどうか
    /// </summary>
    [HideInInspector]public bool playerStepOn;


}
