using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// アイテムの生成クラス
/// </summary>
public class IetemSystem : MonoBehaviour
{
    // 生成するアイテム
    [SerializeField]
    private GameObject ietemPre;
    // ゲーム中に存在できる最大アイテム数
    [SerializeField]
    private GameObject[] ietem = new GameObject[4];

    // ステージサイズ
    [SerializeField]
    private float max_H;
    [SerializeField]
    private float max_V;

    // 投げるロボット
    [SerializeField]
    ThrowEnemy th_Enemy;

    // 初回処理
    void Start () {
        var obj = Instantiate(ietemPre, transform.position, transform.rotation);
        ietem[0] = obj;

        // 3秒ごとに新たに生成
        Observable.Timer(System.TimeSpan.FromSeconds(3), System.TimeSpan.FromSeconds(3))
            .Where(c => ietem.Any(t => t == null))
            .Subscribe(c =>
            {
                // 中身がない場合は生成
                for (int i = 0; i < ietem.Length; i++)
                {
                    if (ietem[i] == null)
                    {
                        var obj2 = Instantiate(ietemPre, transform.position, transform.rotation);
                        ietem[i] = obj2;
                        th_Enemy.item = obj2.GetComponent<Item>();
                        th_Enemy.Throw();
                        break;
                    }
                }
            }).AddTo(this);
	}
}
