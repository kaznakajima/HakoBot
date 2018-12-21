using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LampController : SingletonMonobeBehaviour<LampController>
{
    // ランプの発光色
    public enum LAMP_LIGHT
    {
        RED = 0,
        BLUE
    }

    // ランプの発光色マテリアル
    [SerializeField]
    Material[] lampLight;

	// Use this for initialization
	void Start () {
        
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    /// <summary>
    /// ランプの発光色変更
    /// </summary>
    /// <param name="lampMesh">ランプのメッシュ</param>
    public void LampChange(MeshRenderer[] lampMesh, LAMP_LIGHT _LAMPLIGHT)
    {
        // ランプの色変更
        for(int i = 0;i < lampMesh.Length; ++i) {
            lampMesh[i].material = lampLight[(int)_LAMPLIGHT];
        }
    }
}
