using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

/// <summary>
/// アイテムを投げ込んでくるEnemy(バトルには参加しない)
/// </summary>
public class ThrowEnemy : MonoBehaviour
{
    // 自身のAnimator
    private Animator myAnim;

    // 投げるアイテム
    [HideInInspector]
    public Item item;

    // Use this for initialization
    void Start () {
        // 自身のAnimatorを取得
        myAnim = GetComponent<Animator>();
	}
	
	// 更新処理
	void Update () {
        if (item == null)
            return;

        //向き始めと終わりの点を計算して、stepの値より、今向くべき方向を算出
        Vector3 throwDir = (item.throwPos - transform.position).normalized;
        throwDir.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(throwDir);
    }

    /// <summary>
    /// 荷物を投下していく
    /// </summary>
    public void Throw()
    {
        myAnim.SetInteger("PlayAnimNum", 1);

        // 待機状態にする
        Observable.Timer(TimeSpan.FromSeconds(1.0f)).Subscribe(time =>
        {
            myAnim.SetInteger("PlayAnimNum", 8);
        }).AddTo(this);
    }
}
