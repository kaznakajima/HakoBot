﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererCollision : MonoBehaviour
{
    // 自身のLineRenderer
    LineRenderer myLine;

	// Use this for initialization
	void Start () {
        myLine = GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        CheckLineHit();
	}

    public void CheckLineHit()
    {

        // すべての線にRayを飛ばす
        for(int num = 0;num < myLine.positionCount; num++)
        {
            if (num == myLine.positionCount - 1)
                break;

            // 2点間の距離を取得
            float distance = Vector3.Distance(myLine.GetPosition(num), myLine.GetPosition(num + 1));

            // 2点間の方向を取得
            Vector3 direction = myLine.GetPosition(num + 1) - myLine.GetPosition(num);

            // 次の座標にRayを飛ばす
            Ray lineRay = new Ray(myLine.GetPosition(num), myLine.GetPosition(num + 1));
            RaycastHit lineHit;

            // デバッグ表示
            Debug.DrawRay(myLine.GetPosition(num), direction, Color.blue);

            // 衝突判定
            if (Physics.Raycast(lineRay, out lineHit, distance)) {
                if (lineHit.collider.gameObject.tag == "Character") {
                    // キャラクターのインターフェイスのインスタンス
                    var character = lineHit.collider.gameObject.GetComponent(typeof(Character)) as Character;

                    // コントローラーのバイブレーション
                    VibrationController.Instance.PlayVibration(character.myNumber - 1, true);
                    character.Stan();
                }
            }

        }
    }
}