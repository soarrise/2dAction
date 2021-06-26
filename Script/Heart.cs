using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Heart : MonoBehaviour
{
    private Text heartText = null;
    private int oldheart = 0;

    // Start is called before the first frame update
    void Start()
    {
        heartText = GetComponent<Text>();
        if (GameManager.instance != null)
        {
            heartText.text = "×" + GameManager.instance.heartNum;
        }
        else
        {
            Debug.Log("ゲームマネージャー置き忘れ");
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (oldheart != GameManager.instance.heartNum)
        {
            heartText.text = "×" + GameManager.instance.heartNum;
            oldheart = GameManager.instance.heartNum;
        }
    }
}
