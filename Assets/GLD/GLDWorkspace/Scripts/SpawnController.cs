using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Spawn object at mouse position
/// </summary>
public class SpawnController : MonoBehaviour
{
    [SerializeField] private GameObject _object;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Instantiate(_object, hit.point, Quaternion.identity);
                //_object.transform.position = hit.point;
                //Debug.Log(hit.transform.position);
                Debug.Log(hit.point);
                //Debug.DrawRay(ray.origin, ray.direction * 10, Color.red, 5);
            }
        }
    }
}
