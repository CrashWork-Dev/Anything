using System;
using Cysharp.Threading.Tasks;
using Net.Client;
using Net.Component;
using Script.Data;
using Script.Data.Client;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Script.Game.Client
{
    public class Client : SingleCase<Client>
    {
        [SerializeField] private Button btn;
        private bool _gameSceneTrigger;
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            btn.onClick.AddListener(() => RequestFromServer());
        }

        private void FixedUpdate()
        {
            LoadGameScene();
        }

        private void LoadGameScene()
        {
            if (!_gameSceneTrigger) return;
            _gameSceneTrigger = false;
            SceneManager.LoadSceneAsync("Game");
        }

        private async UniTaskVoid RequestFromServer()
        {
            gameObject.AddComponent<ClientManager>();
            print("Connecting");
            await UniTask.Delay(TimeSpan.FromSeconds(1), ignoreTimeScale: false);//太快会导致客户端连接失去目标
            var task = await ClientBase.Instance.Request((ushort)ProtoType.Login,PlayerData.PlayerName);
            if (!task.IsCompleted)
            {
                print("Timeout");
            }
            var code = task.model.AsInt;
            switch (code)
            {
                case 0:
                    print("Connected");
                    _gameSceneTrigger = true;
                    break;
                default:
                    print("fail");
                    break;
            }
        }
    }
}