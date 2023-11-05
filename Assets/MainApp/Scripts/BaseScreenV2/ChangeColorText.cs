using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Events;


public class ChangeColorText : Selectable
{
    // Start is called before the first frame update
    public Text m_Text;
    //public List<Text> Text; 
    public Color color;
    void Start()
    {
        //m_Text  = GetComponentInChildren<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach ( Text m_Text in GetComponentsInChildren<Text>())
        {
            if (IsHighlighted())
            {
                m_Text.color = Color.white;
            }
            else
            {
                if (ColorUtility.TryParseHtmlString("#0EA29A", out color))
                    m_Text.color = color;
            }
        }

    }
   
}
