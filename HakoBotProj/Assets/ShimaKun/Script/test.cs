using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public float size = -1.0f;
    

    // Use this for initialization
    void Start () {
        Text text = GetComponent<Text>();
        //1回目はWidthの幅が合わさる
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
        //2回目でHeigjhtの高さが合わさる
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
    }

    // Update is called once per frame
    void Update () {
        //移動するやつ
        this.gameObject.transform.Translate(size, 0, 0);
    }
}
