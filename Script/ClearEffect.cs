using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearEffect : MonoBehaviour
{
    // Start is called before the first frame update
    [Header("拡大縮小のアニメーションカーブ")] public AnimationCurve curve;
    [Header("ステージコントローラー")] public StageCtrl ctrl;
    private bool comp = false;
    private float timer;

    void Start()
    {
        transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        if (!comp)
        {
            if (timer < 1.0f)
            {
                transform.localScale = Vector3.one * curve.Evaluate(timer);
                timer += Time.deltaTime;
            }
            else
            {
                transform.localScale = Vector3.one;
                ctrl.ChangeScene(GameManager.instance.stageNum + 1);
                comp = true;
            }
        }
    }
}
