
using System;

// ReSharper disable once CheckNamespace
namespace jp.co.mirabo.Application.RoomManagement
{
    [Serializable]
    public class ResultGame
    {
        public int userId;
        public int courseId;
        public float calories;
        public float mileages;
        public float time;
        public float[] speed;
        public float[] power;
        public float[] cadence;
        public float[] heartRate;
        public float[] lacticAcid;
        public float powerMax;
        public float speedMax;
        public float pwr;
        public int bestTime;
        public int point;
        public string rank;
        public bool isUpRank;
    }
}
