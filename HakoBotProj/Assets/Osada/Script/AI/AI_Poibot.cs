using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UniRx;

public class AI_Poibot : MonoBehaviour
{
    //移動処理用Script
    [SerializeField]
    private Poibot_Move move;
    //投てき処理用Script
    [SerializeField]
    private Poibot_Throwing throwing;

    //移動が終了しているかどうか
    [SerializeField]
    BoolReactiveProperty moveEnd = new BoolReactiveProperty(false);

    private void Start()
    {
        moveEnd.Where(c => !c)
            .Subscribe(c =>
            {
                //移動処理の準備
                move.MoveSetUp(() => moveEnd.Value = true);
            }).AddTo(this);

        moveEnd.Where(c => c)
            .Subscribe(c =>
            {
                //投てき処理の準備
                throwing.ThrowingsetUp(() => moveEnd.Value = false);
            }).AddTo(this);
    }
}
