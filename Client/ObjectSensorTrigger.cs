using UnityEngine;
using UniRx;
using System;

public class ObjectSensorTrigger : MonoBehaviour
{
    ObjectSensor sensor;
    int sensorLevel;
    ISubject<int> OnSensorTriggerEnter;
    ISubject<GameObject> OnSensorTriggerExit;

    bool isTriggerReady = false;
    float timeToInstantiateTrigger = 1;

    private void Start()
    {
        ObjectPhysicReady();
    }

    private void OnDisable()
    {
        isTriggerReady = false;
    }

    private void OnEnable()
    {
        ObjectPhysicReady();
    }

    void ObjectPhysicReady()
    {
        Observable.Timer(TimeSpan.FromSeconds(timeToInstantiateTrigger)).Subscribe(_ => { isTriggerReady = true; });
    }

    public void Instantiate(ObjectSensor sensor, int sensorLevel, ISubject<int> onSensorTriggerEnter, ISubject<GameObject> onSensorTriggerExit)
    {
        this.sensor = sensor;
        this.sensorLevel = sensorLevel;
        this.OnSensorTriggerEnter = onSensorTriggerEnter;
        this.OnSensorTriggerExit = onSensorTriggerExit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isTriggerReady || other.isTrigger)
            return;

        if (sensor.isDisable)
        {
            sensor.isDisable = false;
            return;
        }

        if (other.gameObject.GetComponent<ObjectSensor>())
        {
            ObjectSensor otherSensor = other.gameObject.GetComponent<ObjectSensor>();
            otherSensor.isDisable = true;
        }

        sensor.lastCollisionObject = other.gameObject;

        if(OnSensorTriggerEnter != null)
            OnSensorTriggerEnter.OnNext(sensorLevel);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!isTriggerReady || other.isTrigger)
            return;

        if (OnSensorTriggerExit != null)
            OnSensorTriggerExit.OnNext(other.gameObject);
    }
}
