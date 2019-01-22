using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class ThrowEnemy : MonoBehaviour
{
    // 自身のAnimator
    Animator myAnim;

    // 投げるアイテム
    [HideInInspector]
    public Item item;

    float step = 0;
    //向くスピード(秒速)
    float speed = 0.01f;

    // Use this for initialization
    void Start () {
        // 自身のAnimatorを取得
        myAnim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        if (item == null)
            return;

        //向き始めと終わりの点を計算して、stepの値より、今向くべき方向を算出
        Vector3 throwDir = (item.throwPos - transform.position).normalized;
        throwDir.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(throwDir);
    }

    // 荷物を投げる
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
