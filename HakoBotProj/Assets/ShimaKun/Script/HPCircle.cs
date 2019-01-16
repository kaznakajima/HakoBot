using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPCircle : SingletonMonobeBehaviour<HPCircle>
{

    // Hpゲージのメモリパラメータ
    [SerializeField]
    HPCircle_Param[] _param;

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
    public IEnumerator CheckOverHeat(GameObject playerObj, int playerNum)
    {
        // キャラクターのインターフェイスを取得
        var character = playerObj.GetComponent(typeof(Character)) as Character;
        //if (character.myEnergy >= 100)
        //    return;

        // Hpゲージの値を格納
        character.myEnergy = 2;
        float time = 0.0f;
        float energy = 0.0f;
        while (time < 1.0f)
        {
            time += Time.deltaTime;
            energy = Mathf.Lerp(energy, character.myEnergy, time);
            _param[playerNum - 1].energyImage[(int)energy].SetActive(true);
            yield return null;
        }
        //image[playerNum - 1].fillAmount += 0.1f * _chargeLevel;
    }

    /// <summary>
    /// エナジーゲージの初期化
    /// </summary>
    /// <param name="playerObj">プレイヤーオブジェクト</param>
    /// <param name="playerNum">プレイヤー番号</param>
    /// <returns></returns>
    public IEnumerator EnergyReset(GameObject playerObj, int playerNum)
    {
        var character = playerObj.GetComponent(typeof(Character)) as Character;

        float time = 0.0f;
        float energy = 10.0f;
        while (time < 2.0f)
        {
            time += Time.deltaTime;
            energy = Mathf.Lerp(energy, character.myEnergy, time / 1.5f);
            _param[playerNum - 1].energyImage[(int)energy].SetActive(false);
            yield return null;
        }
    }

}