using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VrgPlayerObject : MonoBehaviour
{
    [SerializeField] GameObject[] bodies, heads;
    GameObject body, head;
    string[] arrColor = {
       "#7CE5E5",   // Xanh nhạt
       "#FFFF7D",   // Vàng
       "#F16E87",   // Đỏ
       "#A0E27B",   // Xanh la
       "#DB87D2"    // Tím
    };
    int GetIndexColor(string color)
    {
        for (int i = 0; i < arrColor.Length; i++)
        {
            if (color == arrColor[i]) return i;
        }
        return 0;
    }
    // Start is called before the first frame update
    public void SetColor(string color, bool isLocalPlayer)
    {
        int index = GetIndexColor(color);

        //reset
        head?.gameObject.SetActive(false);
        body?.gameObject.SetActive(false);
        //add new
        head = heads[index];
        body = bodies[index];
        head.gameObject.SetActive(true);
        body.gameObject.SetActive(true);

        var player = head.GetComponentsInChildren<Renderer>();
        int LayerAvata = LayerMask.NameToLayer("Head");
        if (isLocalPlayer)
        {
            foreach (var d in player)
                d.gameObject.layer = LayerAvata;
        }
    }

}
