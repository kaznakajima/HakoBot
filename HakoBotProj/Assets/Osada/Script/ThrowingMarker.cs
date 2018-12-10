using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingMarker : MonoBehaviour
{
    [SerializeField]
    private GameObject mark;

    //マーカーを表示
    public void MarkDisplay(Vector3 pos)
    {
        Instantiate(mark, pos, Quaternion.Euler(90, 0, 0));
    }
}
