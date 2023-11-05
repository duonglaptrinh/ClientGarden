using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserBeamOtherPlayer : MonoBehaviour
{
    [SerializeField]
    public Transform LaserBeamTransform;
    [SerializeField] public MeshRenderer Beam;
    [SerializeField] public MeshRenderer Dot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeBeamColor(Color color) 
    {
        //Beam.material.color = color;
        //Dot.material.color = color;
        Beam.material.color = new Color(0f, 1f, 0f, 1f);
        Dot.material.color = new Color(0f, 1f, 0f, 1f);
    }
}
