using System;
using UniRx;
using UnityEngine;

public class ObjectSensor : ObjectPhysic
{
    const string SENSOR_TAG = "Sensor";
    const string SENSOR_ALERT_OBJECT_NAME = "sensor alert one shot sound";

    public float lv1Scale;
    public float lv2Scale;
    public float lv3Scale;

    public AudioClip level1WarningSound;
    public AudioClip level2WarningSound;
    public AudioClip level3WarningSound;

    [HideInInspector]
    public GameObject lastCollisionObject;
    [HideInInspector]
    public bool isDisable = false;

    private GameObject sensorContainer;
    private GameObject cloneObject;
    private int numberOfSensor = 3;

    private float scaleFactorX;
    private float scaleFactorY;
    private float scaleFactorZ;

    private ISubject<int> OnSensorTriggerEnter = new Subject<int>();
    private ISubject<GameObject> OnSensorTriggerExit = new Subject<GameObject>();

    public void Instantiate(float lv1Scale, AudioClip lv1AC, float lv2Scale, AudioClip lv2AC, float lv3Scale, AudioClip lv3AC)
    {
        this.lv1Scale = lv1Scale;
        this.lv2Scale = lv2Scale;
        this.lv3Scale = lv3Scale;

        this.level1WarningSound = lv1AC;
        this.level2WarningSound = lv2AC;
        this.level3WarningSound = lv3AC;
    }

    void Start()
    {
        base.Start();
        AddRigidBody();

        cloneObject = this.gameObject;

        for (int i = 1; i <= numberOfSensor; i++)
        {
            CreateSensor(i);
        }

        OnSensorTriggerEnter.Subscribe(sensorLevel => { DoSensorTriggerEnter(sensorLevel); });
        OnSensorTriggerExit.Subscribe(sensorLevel => { DoSensorTriggerExit(sensorLevel); });
    }

    void CaculateScaleFactor(int level)
    {
        Bounds objectBounds = GetComponent<Renderer>().bounds;
        float averageSensorDistance = 0;

        switch (level)
        {
            case 1:
                averageSensorDistance = objectBounds.size.y * lv1Scale - objectBounds.size.y;
                break;
            case 2:
                averageSensorDistance = objectBounds.size.y * lv2Scale - objectBounds.size.y;
                break;
            case 3:
                averageSensorDistance = objectBounds.size.y * lv3Scale - objectBounds.size.y;
                break;
        }

        scaleFactorX = (objectBounds.size.x + averageSensorDistance) / objectBounds.size.x;
        scaleFactorY = (objectBounds.size.y + averageSensorDistance) / objectBounds.size.y;
        scaleFactorZ = (objectBounds.size.z + averageSensorDistance) / objectBounds.size.z;
    }

    void CreateSensor(int level)
    {
        sensorContainer = new GameObject(SENSOR_TAG + level);
        sensorContainer.transform.parent = transform;
        sensorContainer.transform.position = GetComponent<Renderer>().bounds.center;

        CaculateScaleFactor(level);

        GameObject sensor = Instantiate(cloneObject);
        sensor.name = "Trigger";
        sensor.gameObject.tag = SENSOR_TAG;
        sensor.transform.parent = sensorContainer.transform;
        sensor.transform.position = this.transform.position;
        sensor.transform.localScale = Vector3.one;

        if (sensor.GetComponent<Collider>() as MeshCollider)
        {
            //Only convex MeshCollider support trigger
            sensor.GetComponent<MeshCollider>().convex = true;
        }
        sensor.GetComponent<Collider>().isTrigger = true;

        if (!sensor.GetComponent<ObjectSensorTrigger>())
            sensor.AddComponent<ObjectSensorTrigger>();

        sensor.GetComponent<ObjectSensorTrigger>().Instantiate(this, level, OnSensorTriggerEnter, OnSensorTriggerExit);

        foreach (Transform child in sensor.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (Component c in sensor.GetComponents(typeof(Component)))
        {
            if (!(c as Collider) && !(c as Transform) && !(c as ObjectSensorTrigger))
            {
                try
                {
                    Destroy(c);
                }
                catch (Exception) { }
            }
        }  

        sensorContainer.transform.localScale = new Vector3(scaleFactorX, scaleFactorY, scaleFactorZ);
        cloneObject = sensor;
    }

    void DoSensorTriggerEnter(int level)
    {
        GameObject sensorAlertObject = GameObject.Find(SENSOR_ALERT_OBJECT_NAME);

        if (sensorAlertObject == null)
        {
            sensorAlertObject = new GameObject(SENSOR_ALERT_OBJECT_NAME);
            sensorAlertObject.transform.position = Camera.main.transform.position;
            sensorAlertObject.AddComponent<AudioSource>();
            sensorAlertObject.AddComponent<SensorAlertTrigger>();
        }

        AudioSource audioSource = sensorAlertObject.GetComponent<AudioSource>();
        audioSource.Stop();

        switch (level)
        {
            case 1:
                audioSource.clip = level1WarningSound;
                break;
            case 2:
                audioSource.clip = level2WarningSound;
                break;
            case 3:
                audioSource.clip = level3WarningSound;
                break;
        }

        audioSource.Play();  
    }

    void DoSensorTriggerExit(GameObject obj)
    {
        if (lastCollisionObject.GetInstanceID() != obj.GetInstanceID())
            return;

        GameObject sensorAlertObject = GameObject.Find(SENSOR_ALERT_OBJECT_NAME);
        if (sensorAlertObject)
        {
            Destroy(sensorAlertObject);
        }
    }
}
