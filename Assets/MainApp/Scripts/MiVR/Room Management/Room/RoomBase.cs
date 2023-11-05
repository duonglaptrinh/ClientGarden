using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using Cysharp.Threading.Tasks;
using LucidSightTools;
using UnityEngine;

namespace jp.co.mirabo.Application.RoomManagement
{
    public abstract class RoomBase<T>
    {
        protected float defaultTimeoutInSec = 15f;
        public string currentRoomName = null;
        public string currentRoomId = null;
        protected string endpoint;
        public string EndPoint { get => endpoint; set => endpoint = value; }
        protected ColyseusClient client;
        protected ColyseusRoom<T> _room;

        public RoomBase(string endpoint)
        {
            this.endpoint = endpoint;
            RoomManager.OnApplicationQuitEvent += OnApplicationQuitEvent;
        }


        protected abstract void OnRegisterRoomHandlers();

        public void JoinRoomById(string roomId, Action onSuccess = null, Action<string> onError = null, Dictionary<string, object> options = null)
        {
            JoinRoomByIdAsync(roomId, onSuccess, onError, options).Timeout(TimeSpan.FromSeconds(defaultTimeoutInSec)).Forget();
        }

        public async UniTask JoinRoomByNameAsync(string roomName, Action onSuccess = null, Action<string> onError = null, Dictionary<string, object> options = null)
        {
            await Leave();

            DebugExtension.Log($"Connecting to { roomName } Room");

            client = new ColyseusClient(endpoint);
            try
            {
                // _room = await client.JoinOrCreate<T>(ValidateRequestRoomName(roomName), options);
                //_room = await client.Join<T>(roomName, options);
                //var task = client.JoinOrCreate<T>(roomName, options);

                var task = client.Join<T>(roomName, options);

                if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(defaultTimeoutInSec))) == task)
                {
                    _room = task.Result;
                }
                else
                {
                    // throw new TimeoutException();
                    onError?.Invoke("Timeout");
                    return;
                }
                task.Dispose();

                currentRoomName = roomName;

                OnRegisterRoomHandlers();
                onSuccess?.Invoke();
            }
            catch (AggregateException err)
            {
                // foreach (var errInner in err.InnerExceptions)
                // {
                // 	DebugExtension.LogError(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                // }
                if (err.InnerExceptions.Count > 0) onError?.Invoke(err.InnerExceptions[0].Message);
                else onError?.Invoke(err.ToString());
                // DebugExtension.LogError(err.ToString());
            }
            catch (TimeoutException ex)
            {
                onError?.Invoke(ex.Message);
            }
            catch (Exception ex)
            {
                onError?.Invoke(ex.Message);
            }
        }

        public async UniTask JoinRoomByIdAsync(string roomId, Action onSuccess = null, Action<string> onError = null, Dictionary<string, object> options = null)
        {
            DebugExtension.Log($"Connecting to room Id: { roomId }");

            await Leave();

            client = new ColyseusClient(endpoint);

            try
            {
                // _room = await client.JoinById<T>(roomId, options);
                var task = client.JoinById<T>(roomId, options);
                if (await Task.WhenAny(task, Task.Delay(TimeSpan.FromSeconds(defaultTimeoutInSec))) == task)
                {
                    _room = task.Result;
                }
                else
                {
                    // throw new TimeoutException();
                    onError?.Invoke("Timeout");
                    return;
                }
                task.Dispose();
                currentRoomId = roomId;

                OnRegisterRoomHandlers();

                onSuccess?.Invoke();
            }
            catch (AggregateException err)
            {
                // foreach (var errInner in err.InnerExceptions)
                // {
                // 	DebugExtension.LogError(errInner); //this will call ToString() on the inner execption and get you message, stacktrace and you could perhaps drill down further into the inner exception of it if necessary 
                // }
                if (err.InnerExceptions.Count > 0) onError?.Invoke(err.InnerExceptions[0].Message);
                else onError?.Invoke(err.ToString());
                // DebugExtension.LogError(err.ToString());
            }
            catch (TimeoutException ex)
            {
                // throw new TimeoutException();
                onError?.Invoke(ex.Message);
            }
            catch (Exception ex)
            {
                // DebugExtension.LogError($"Error Connect: {ex}");
                onError?.Invoke(ex.Message);
            }
        }

        public async UniTask JoinRoomAsync(RoomData room, Action onSuccess = null, Action<string> onError = null, Dictionary<string, object> options = null)
        {
            LSLog.Log($"Connecting to {room.name}, room Id: { room.roomId }");

            await Leave();

            client = new ColyseusClient(endpoint);

            try
            {
                _room = await client.JoinById<T>(room.roomId, options);

                currentRoomId = room.roomId;

                OnRegisterRoomHandlers();

                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                // DebugExtension.LogError($"Error Connect: {ex}");
                onError?.Invoke(ex.Message);
            }
        }

        public async void Send(string eventName, object message, Action onSuccess = null, Action<string> onError = null)
        {
            //LSLog.Log($"Sending: {eventName}\nData:\n{message}");

            try
            {
                await _room.Send(eventName, message);
                onSuccess?.Invoke();
            }
            catch (Exception ex)
            {
                DebugExtension.LogError($"Send Message Error: {ex.Message}");
                onError?.Invoke(ex.Message);
            }
        }

        public async Task SendAsync(string eventName, object message)
        {
            await _room.Send(eventName, message);
        }

        public async Task Reconnect()
        {
            DebugExtension.LogError(_room.Id);
            await client.Reconnect(_room.Id, _room.SessionId);
            LSLog.Log($"Reconnected");
        }

        protected virtual string ValidateRequestRoomName(string input)
        {
            var p = input.Split('\\', '/');
            string requestRoom = p[0];
            for (int i = 1; i < p.Length; ++i) requestRoom += $"-{p[i]}";
            return requestRoom;
        }

        void OnApplicationQuitEvent()
        {
            RoomManager.OnApplicationQuitEvent -= OnApplicationQuitEvent;

            Leave().Forget();
        }

        public async UniTask Leave()
        {
            if (client == null || _room == null)
                return;

            try
            {
                LSLog.Log($"{this.GetType().Name}: <b>Closed</b>");
                await _room.Leave(true);
            }
            catch (Exception ex)
            {
                DebugExtension.LogError($"Error: {ex.Message}");
            }
        }
    }
}