using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemManagement : MonoBehaviour
{
    //アイテムリスト
    private List<GameObject> ItemList = new List<GameObject>();

    //リストに追加
    public void add_to_list(GameObject obj)
    {
        ItemList.Add(obj);
    }
    //リストから削除
    public void delete_from_list(GameObject obj)
    {
        ItemList.Remove(obj);
    }
    //リストの要素数を取得
    public int get_list_factor_count()
    {
        return ItemList.Count();
    }
}
