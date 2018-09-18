using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputMaster : MonoBehaviour
{
    public List<Player> players;

    // 入力判定
    Vector3[] inputVec;
    // 移動量
    Vector3 moveVec;

    // 自身のRig
    Rigidbody playerRig;

    // 入力判定
    //void PlayerInput()
    //{
    //    /* ここから移動量判定 */
    //    //for (int i = 0; i < 2; i++)
    //    //{
    //    //    if (players[i].isAttack && players[i].playerRig.velocity == Vector3.zero)
    //    //    {
    //    //        players[i].isAttack = false;
    //    //    }

    //    //    inputVec[i].x = Input.GetAxisRaw("Horizontal_" + i);
    //    //    inputVec[i].z = Input.GetAxisRaw("Vertical_" + i);

    //    //    if (inputVec[i] != Vector3.zero)
    //    //    {
    //    //        players[i].PlayerMove(inputVec[i]);
    //    //    }

    //    //    if (Input.GetButtonDown("Fire2_" + i))
    //    //    {
    //    //        players[i].PlayerAttack();
    //    //    }
    //    //}
    //}

    // Use this for initialization
    void Start()
    {
        CountPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        //PlayerInput();
    }

    // プレイヤーのカウント
    void CountPlayers()
    {
        foreach (Player _player in FindObjectsOfType<Player>())
        {
            players.Add(_player);
        }

        inputVec = new Vector3[players.Count];
    }
}
