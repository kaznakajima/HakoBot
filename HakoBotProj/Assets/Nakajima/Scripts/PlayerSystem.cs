using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GamepadInput;
using UniRx;
using UniRx.Triggers;

public class PlayerSystem : MonoBehaviour
{
    // 4人のプレイヤー用の入力状態の保存変数
    private GamepadState[] state = new GamepadState[5];

	// Use this for initialization
	void Start () {
        // Update
        this.UpdateAsObservable().Subscribe(c =>
        {
            // 各プレイヤーのコントローラーの入力状態を確認する
            for (int i = 0; i < 5; i++)
            {
                state[i] = GamePad.GetState((GamePad.Index)i);
            }
        }).AddTo(this);
	}
	
    /// <summary>
    /// 左スティックの入力状態の確認
    /// </summary>
    /// <param name="playerNo">プレイヤー番号</param>
    /// <returns></returns>
    public Vector2 LeftStickAxis(int playerNo)
    {
        if(state[playerNo] != null)
        {
            return state[playerNo].LeftStickAxis;
        }
        else
        {
            return new Vector3(0, 0);
        }
    }

    /// <summary>
    /// Aボタンの入力状態の確認
    /// </summary>
    /// <param name="playerNo">プレイヤー番号</param>
    /// <returns></returns>
    public bool Button_A(int playerNo)
    {
        if(state[playerNo] != null)
        {
            return state[playerNo].B;
        }
        else
        {
            return false;
        }
    }

	// Update is called once per frame
	void Update () {
		
	}
}

public class Editor_PlayerSystem : Editor
{
    public override void OnInspectorGUI()
    {

    }
}
