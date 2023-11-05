using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private Transform _transform;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(_prefab, _transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
