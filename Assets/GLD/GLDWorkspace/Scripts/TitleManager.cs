using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Takasho.GLD.VGS
{
    public class TitleManager : MonoBehaviour
    {

        public void OnRoomButtonClicked()
        {
            SceneManager.LoadScene("LobbyScene");
        }
    }
}
