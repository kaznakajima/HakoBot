using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCircle : SingletonMonobeBehaviour<HPCircle>
{
    // プレイヤーごとのHPゲージ
    [SerializeField]
    Image[] image;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    public void HPDown(float current, int max)
    {
        //float maxfill = 0.75f;

        //image.GetComponent<Image>().fillAmount += current;

        
    }

    /// <summary>
    /// オーバーヒートするかのチェック
    /// </summary>
    /// <param name="playerObj">プレイヤーオブジェクト</param>
    /// <param name="playerNum">プレイヤー番号</param>
    /// <param name="_chargeLevel">チャージ段階</param>
    public IEnumerator CheckOverHeat(GameObject playerObj, int playerNum, int _chargeLevel)
    {
        // キャラクターのインターフェイスを取得
        var character = playerObj.GetComponent(typeof(Character)) as Character;
        //if (character.myEnergy >= 100)
        //    return;

        // Hpゲージの値を格納
        character.myEnergy = 1 * _chargeLevel;
        float time = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime / 1.0f;
            image[playerNum - 1].fillAmount = Mathf.Lerp(image[playerNum - 1].fillAmount, image[playerNum - 1].fillAmount + 0.1f * _chargeLevel, time);
            yield return null;
        }
        //image[playerNum - 1].fillAmount += 0.1f * _chargeLevel;
    }
}