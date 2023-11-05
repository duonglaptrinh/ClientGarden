using System;
using UniRx;
using UnityEngine;

public class ObjectMove : ObjectPhysic, IObjectMovable
{
    public bool CanMoveAxisX { private get; set; }
    public bool CanMoveAxisY { private get; set; }
    public bool CanMoveAxisZ { private get; set; }
    
    public float Step { private get; set; }

    bool isObjectReady = false;
    float timeToInstantiateTrigger = 1;

    private void Start()
    {
        base.Start();
        AddRigidBody();
        ObjectPhysicReady();
    }

    private void OnDisable()
    {
        isObjectReady = false;
    }

    private void OnEnable()
    {
        ObjectPhysicReady();
    }

    void ObjectPhysicReady()
    {
        Observable.Timer(TimeSpan.FromSeconds(timeToInstantiateTrigger)).Subscribe(_ => { isObjectReady = true; });
    }

    public void Move(Vector3 direction)
    {
        if (!CanMoveAxisX)
            direction.x = 0;
        if (!CanMoveAxisY)
            direction.y = 0;
        if (!CanMoveAxisZ)
            direction.z = 0;

        var newPos = direction * Step;

        if (GetComponent<Rigidbody>())
        {
            GetComponent<Rigidbody>().MovePosition(transform.position + newPos);
        }
        else
        {
            transform.position += newPos;
        }
    }


    CompositeDisposable disposables = new CompositeDisposable();
    const string MOVE_OBJECT_COLLISION_ONE_SHOT_SOUND = "object collision one shot sound";

    private void OnCollisionEnter(Collision collision)
    {
        if (!isObjectReady)
            return;

        disposables.Clear();
        GetComponent<Rigidbody>().velocity = Vector3.zero;

        GameObject sensorAlertObject = GameObject.Find(MOVE_OBJECT_COLLISION_ONE_SHOT_SOUND);

        if (sensorAlertObject == null)
        {
            sensorAlertObject = new GameObject(MOVE_OBJECT_COLLISION_ONE_SHOT_SOUND);
            sensorAlertObject.transform.position = Camera.main.transform.position;
            sensorAlertObject.AddComponent<AudioSource>();
        }

        AudioSource audioSource = sensorAlertObject.GetComponent<AudioSource>();
        audioSource.Stop();
        audioSource.clip = Resources.Load<AudioClip>("metal_hit");
        audioSource.Play();

        Observable.EveryUpdate().Where(_ => audioSource != null && audioSource.isPlaying == false).
            Subscribe(_ => { Destroy(sensorAlertObject); disposables.Clear(); }).AddTo(disposables);
    }
}