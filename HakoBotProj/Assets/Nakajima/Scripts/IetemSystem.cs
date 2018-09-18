using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;
using UniRx.Triggers;


public class IetemSystem : MonoBehaviour
{
    [SerializeField]
    private GameObject ietemPre;
    [SerializeField]
    private GameObject[] ietem = new GameObject[4];

    [SerializeField]
    private float max_H;
    [SerializeField]
    private float max_V;

	// Use this for initialization
	void Start () {
        var x = Random.Range(-max_H, max_H);
        var y = Random.Range(-max_V, max_V);
        var obj = Instantiate(ietemPre, new Vector3(x, 1.5f, y), transform.rotation);
        ietem[0] = obj;

        Observable.Timer(System.TimeSpan.FromSeconds(3), System.TimeSpan.FromSeconds(3))
            .Where(c => ietem.Any(t => t == null))
            .Subscribe(c =>
            {
                for (int i = 0; i < ietem.Length; i++)
                {
                    if (ietem[i] == null)
                    {
                        var x2 = Random.Range(-max_H, max_H);
                        var y2 = Random.Range(-max_V, max_V);
                        var obj2 = Instantiate(ietemPre, new Vector3(x2, 1.5f, y2), transform.rotation);
                        ietem[i] = obj2;
                        break;
                    }
                }
            }).AddTo(this);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
