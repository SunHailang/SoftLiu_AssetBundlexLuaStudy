using SoftLiu.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BootLoader : MonoBehaviour
{
    private void Start()
    {
        Dictionary<string, object> dic = new Dictionary<string, object>();
        dic.Add("state", 0);
        string json = MiniJSON.Serialize(dic);

        Dictionary<string, object> dicOut = MiniJSON.Deserialize(json) as Dictionary<string, object>;
        Debug.Log(dicOut["state"]);
    }
}
