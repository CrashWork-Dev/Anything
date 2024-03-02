using Net.Client;
using Net.UnityComponent;
using Script.Game.Client.Object;
using UnityEngine;

namespace Script.Game.Internal.Object
{
    public class PlayerSpawnPoint : MonoBehaviour
    {
        [SerializeField] private NetworkObject player;

        private void Start()
        {
            var initPlayer = Instantiate(player);
            initPlayer.Identity = ClientBase.Instance.UID;

            initPlayer.GetComponent<Player>().isLocalPlayer = true;
        }
    }
}