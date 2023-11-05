using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DomeSetting : CommonDialog
{
    [SerializeField] Button buttonClose;

    // Start is called before the first frame update
    void Start()
    {
        buttonClose.onClick.AddListener(OnCloseClick);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
