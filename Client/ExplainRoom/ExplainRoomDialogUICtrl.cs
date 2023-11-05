using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.NetworkSystem;

public class ExplainRoomDialogUICtrl : MonoBehaviour
{
    public Button okBtn;
    public Text dialogText;

    public DialogType type;

    const string JOIN_ROOM_TEXT = "解説部屋に移動します　OK？";
    const string LEAVE_ROOM_TEXT = "解説部屋を抜けます";

    [SerializeField]
    public enum DialogType
    {
        JoinDialog,
        LeaveDialog,
    }

    private void Start()
    {
        okBtn.onClick.AddListener(OnConfirmOk);
        SetDialogType(type);
    }

    void SetDialogType(DialogType dialogType)
    {
        switch (dialogType)
        {
            case DialogType.JoinDialog:
                dialogText.text = JOIN_ROOM_TEXT;
                break;
            case DialogType.LeaveDialog:
                dialogText.text = LEAVE_ROOM_TEXT;
                break;
        }
    }

    public void Show(DialogType dialogType)
    {
        gameObject.SetActive(true);
        type = dialogType;
        okBtn.interactable = true;
        SetDialogType(type);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Refresh()
    {
        okBtn.interactable = true;
    }

    public void OnConfirmOk()
    {
        okBtn.interactable = false;

        switch (type)
        {
            case DialogType.JoinDialog:
                NetworkClient.allClients[0].Send(MyMessageType.ER_CONFIRM_JOIN, new EmptyMessage());
                break;
            case DialogType.LeaveDialog:
                NetworkClient.allClients[0].Send(MyMessageType.ER_CONFIRM_LEAVE, new EmptyMessage());
                break;
        }
    }
}
