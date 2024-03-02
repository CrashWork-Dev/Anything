using UnityEngine;

namespace Script.Init.Internal.Network
{
    public class ServerDebug : MonoBehaviour
    {
        public void StartServer()
        {
            Server.StartTrigger = true;
            print("Server Started \n Don't launch again!");
        }
    }
}
