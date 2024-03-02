using Net.Server;
using Net.Share;
using Script.Data;
using Script.Init.Internal.Object;
using UnityEngine;

namespace Script.Init.Internal.Network
{
    public class Service : TcpServer<Player, Scene>
    {
        protected override bool OnUnClientRequest(Player unClient, RPCModel model)
        {
            switch ((ProtoType)model.protocol)
            {
                case ProtoType.Login:
                    Login(unClient, model.AsString);
                    break;
                default:
                    break;
            }

            return false;
        }

        private void Login(Player client, string username)
        {
            var token = client.Token;
            client.PlayerName = username;
            Call(client, (ushort)ProtoType.Login, token, 0);
            Debug.Log("玩家 "+username+" 加入了服务器");
            LoginHandler(client);
        }

        protected override void OnRemoveClient(Player client)
        {
            Debug.Log("玩家: " + client.PlayerName + " 离开了服务器");
            base.OnRemoveClient(client);
        }
    }
}