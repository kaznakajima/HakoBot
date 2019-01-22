using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class test : MonoBehaviour
{
    public Text text;

    public GameObject CanvasPos;

    // Use this for initialization
    void Start () {
        Text text = GetComponent<Text>();
        //1回目はWidthの幅が合わさる
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
        //2回目でHeigjhtの高さが合わさる
        text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.preferredHeight);
        StartCoroutine("TextMove");
        
    }

    // Update is called once per frame
    void Update () {
    }

    void Test(string str)
    {
        text.text = str;    
    }

    //必要な処理　実行前スタート位置に戻す　サイズに応じてスタート位置を変化させること　キャンバスのサイズを取得 
    public IEnumerator TextMove()
    {
        //textサイズ取得 width height
        float width = text.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        float height = text.gameObject.GetComponent<RectTransform>().sizeDelta.y;
        Debug.Log(width);

        //Canvasサイズ取得
        float canvaswidth = CanvasPos.gameObject.GetComponent<RectTransform>().sizeDelta.x;
        float canvasheight = CanvasPos.gameObject.GetComponent<RectTransform>().sizeDelta.y;

        //目的地取得
        Vector3 targetPos = new Vector3(CanvasPos.transform.position.x - text.rectTransform.sizeDelta.x, transform.position.y, transform.position.z);

        //初期のスタート場所
        text.transform.position = new Vector2(canvaswidth / 2 + width, 0);

        //移動するやつ
        while (true)
        {
            
            //ベクトル取って
            Vector3 ver = targetPos - transform.position;
            //動かして
            transform.position += ver * Time.deltaTime;
            //距離はかって
            float dis = Vector3.Distance(transform.position, targetPos);
            //していないに行ったら終了
            if (dis < 0.5f)
            {
                
                transform.position = targetPos;
                //適当な場所に戻す
                text.transform.position = new Vector2(canvaswidth/2 + width, 0);
                //break;    
            }
            yield return null;
        }
    }
}
