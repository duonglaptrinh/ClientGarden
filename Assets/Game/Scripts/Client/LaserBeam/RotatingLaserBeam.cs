using Game.Client;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingLaserBeam : MonoBehaviour
{
    public static RotatingLaserBeam instance;

    public Transform laser;
    public GameObject beam;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        TeleportTargetDetector.OnTeleport += RotateBeam;
        VRObjectSelectHelper.OnTouchToScreen += RotateBeam;
        beam.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void RotateBeam(Vector3 touch_position)
    {
        if (touch_position.x != 10000)
        {
            laser.transform.LookAt(touch_position);
            beam.SetActive(true);
        }
        else
        {
            beam.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        TeleportTargetDetector.OnTeleport -= RotateBeam;
        VrObjectEditSelectHelperV2.OnSelectVrObject -= RotateBeam;
        VRObjectSelectHelper.OnTouchToScreen -= RotateBeam;
    }
}
