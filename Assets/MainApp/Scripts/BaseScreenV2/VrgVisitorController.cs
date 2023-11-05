using Colyseus.Schema;
using jp.co.mirabo.Application.RoomManagement;
using Player_Management;
using SyncRoom.Schemas;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using static jp.co.mirabo.Application.RoomManagement.RoomConfig;

public class VrgVisitorController : MonoBehaviour
{
    [SerializeField] private Transform head;
    [SerializeField] private Transform body;
    [SerializeField] private Text txtPlayerName;
    [SerializeField] VrgPlayerObject playerObject;

    public bool isLocalPlayer { get; private set; }
    UserVisitorData player;
    bool isStartDone = false;
    private Transform cameraForFollow;

    #region Sync Data
    private float _timeSendSyncData;
    private const float STEP_TIME_SEND_SYNC_DATA = 0.3f;

    private Entity _entity;
    private bool _pause;
    private Vector3 startPosition = new Vector3(-10000, -10000, -10000);
    private Vector3 _newPosition;
    private Quaternion _newRotation;
    private readonly BoolReactiveProperty _onPopupUI = new BoolReactiveProperty();
    private bool _openNTab;
    private bool _isAnimating, _step;
    private Transform _anotherObject;
    private Vector3 _target, _targetRotation, _targetRotationCam;
    private GameObject _fakeObject, _fakeObjectCam;
    #endregion

    private void Awake()
    {
        _newPosition = startPosition;
    }
    // Start is called before the first frame update
    void Start()
    {
        isStartDone = true;
    }

    public void SetData(Visitor player, bool isLocalPlayer)
    {
        this.isLocalPlayer = isLocalPlayer;
        this.player = UserVisitorData.Convert(player);
        txtPlayerName.text = player.name;
        if (isLocalPlayer)
        {
            txtPlayerName.gameObject.SetActive(false);
            RuntimeData.CurrentUser = this.player;
            cameraForFollow = PlayerManagerSwitch.myTransform;// Camera.main.transform;
            CreateEntity($"{player.sessionId}");
        }
        else
            txtPlayerName.gameObject.SetActive(true);
        playerObject.SetColor(this.player.Data.themeColor, isLocalPlayer);
    }

    private void CreateEntity(string playerID)
    {
        _entity = new Entity
        {
            id = playerID,
            type = SyncEntity.SYNC_OBJECT.ToString()
        };
        _entity.AddAttributeValue(EntityAttribute.POSITION.ToString(), SyncDataType.JSON_OBJECT.ToString(),
            JsonUtility.ToJson(transform.position));
        _entity.AddAttributeValue(EntityAttribute.ROTATION.ToString(), SyncDataType.JSON_OBJECT.ToString(),
            JsonUtility.ToJson(body.transform.rotation));
        //_entity.AddAttributeValue(EntityAttribute.AVATAR.ToString(), SyncDataType.NUMBER.ToString(),
        //    ConnectServer.avatarID);
        //DebugExtension.Log("Create \n" + _entity.attributes[EntityAttribute.AVATAR.ToString()].dataValue);
        RoomManager.Instance.GameRoom.Send(SyncMessage.CREATE_ENTITY.ToString(), _entity.OptimizeDataToSend());
    }

    private void UpdateEntity()
    {
        if (_entity == null) return;
        _entity.UpdateAttribute(EntityAttribute.POSITION.ToString(), JsonUtility.ToJson(transform.position));
        _entity.UpdateAttribute(EntityAttribute.ROTATION.ToString(), JsonUtility.ToJson(body.transform.rotation));
        //_entity.UpdateAttribute(EntityAttribute.AVATAR.ToString(), ConnectServer.avatarID.ToString());
        RoomManager.Instance.GameRoom.Send(SyncMessage.UPDATE_ENTITY.ToString(), _entity.OptimizeDataToSend());
    }
    public void SetEntity(Entity entity)
    {
        if (isLocalPlayer || _entity != null)
        {
            return;
        }

        _entity = entity;
        if (entity == null)
            return;
        _entity.attributes[EntityAttribute.POSITION.ToString()].OnChange += (List<DataChange> changes) =>
        {
            var strPos = entity.attributes[EntityAttribute.POSITION.ToString()].dataValue;
            Vector3 pos = JsonUtility.FromJson<Vector3>(strPos);
            if (_newPosition == startPosition)
            {
                transform.position = pos;
            }
            _newPosition = pos;
        };

        _entity.attributes[EntityAttribute.ROTATION.ToString()].OnChange += (List<DataChange> changes) =>
        {
            var strRot = entity.attributes[EntityAttribute.ROTATION.ToString()].dataValue;
            _newRotation = JsonUtility.FromJson<Quaternion>(strRot);
        };
        //_entity.attributes[EntityAttribute.AVATAR.ToString()].OnChange += (List<DataChange> changes) => {
        //    var strAva = entity.attributes[EntityAttribute.AVATAR.ToString()].dataValue;
        //    _avatarID = Convert.ToInt32(strAva); //JsonUtility.FromJson<int>(str_ava);
        //    DebugExtension.Log(_avatarID);
        //};
    }
    private void Update()
    {
        if (!isStartDone)
            return;
        if (isLocalPlayer)
        {
            if (cameraForFollow != null)
            {
                // Setup camera here
                var cameraPosition = cameraForFollow.position;
                transform.position = cameraPosition;
                Vector3 v = body.eulerAngles;
                v.y = cameraForFollow.eulerAngles.y;
                body.eulerAngles = v;
            }
        }
        SyncPlayerThroughNetwork();
    }

    private void SyncPlayerThroughNetwork()
    {
        if (isLocalPlayer)
        {
            _timeSendSyncData += Time.deltaTime;
            if (_timeSendSyncData >= STEP_TIME_SEND_SYNC_DATA)
            {
                _timeSendSyncData -= STEP_TIME_SEND_SYNC_DATA;
                UpdateEntity();
                // DebugExtension.Log("Sync Data ........");
            }
        }
        else
        {
            ProcessViewSync();
        }
    }

    private void ProcessViewSync()
    {
        var trans = transform;
        transform.position = Vector3.Lerp(trans.localPosition, _newPosition, Time.deltaTime * 2);
        body.transform.rotation = Quaternion.Lerp(body.transform.rotation, _newRotation, Time.deltaTime * 5);
        //_front.sprite = _back.sprite = PlayerManager.GetPlayerAvatar(_avatarID);
    }
    public void DestroyPlayer()
    {
        Destroy(gameObject);
    }
}
