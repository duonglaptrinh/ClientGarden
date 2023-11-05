using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Takasho.GLD.VGS
{
    public class RoomSelectManager : MonoBehaviour
    {
        public void OnCubeHouseButtonClicked()
        {
            SceneManager.LoadScene("CubeHouse1");
        }
    }
}
