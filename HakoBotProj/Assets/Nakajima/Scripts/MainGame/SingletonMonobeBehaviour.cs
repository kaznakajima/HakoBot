using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// シングルトンクラス
/// ---------------------------------------------------------------
/// 変数の参照の仕方
/// クラス名.Instance.変数名
/// ---------------------------------------------------------------
/// メソッドの使い方
/// クラス名.Instance.メソッド名(必要な場合は引数));
/// ---------------------------------------------------------------
/// </summary>
/// <typeparam name="T">継承させるクラス名</typeparam>
public abstract class SingletonMonobeBehaviour<T> : MonoBehaviour where T : MonoBehaviour
{
    // 継承先のクラスのインスタンス
    private static T instance;
    public static T Instance
    {
        get
        {
            // インスタンスが存在しないならインスタンス生成
            if (instance == null)
            {
                Type t = typeof(T);

                instance = (T)FindObjectOfType(t);
            }
            return instance;
        }
    }

    virtual protected void Awake()
    {
        CheckInstance();
    }

    // 他のゲームオブジェクトにアタッチされているか調べる
    // アタッチされている場合は破棄する
    protected bool CheckInstance()
    {
        if(instance == null)
        {
            instance = this as T;
            return true;
        }else if(Instance == this)
        {
            return true;
        }
        Destroy(gameObject);
        return false;
    }
}
