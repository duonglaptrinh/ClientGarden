using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockJumpOnTree : MonoBehaviour
{
    // Start is called before the first frame update
    CharacterController character;
    private void Awake()
    {
        character = gameObject.GetComponent<CharacterController>();
    }
    void Start()
    {
       
    }
    // Update is called once per frame
    void Update()
    {  
    
    }
    public void OnTriggerStay(Collider other)
    {
        foreach (Collider m in other.GetComponentsInChildren<Collider>())
        {
            if ( m.gameObject.tag == "Tree")
                character.stepOffset = 0;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        character.stepOffset = 0.5f;
    }
    
}

