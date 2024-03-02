using Script.Game.Client.Getter;
using UnityEngine;

namespace Script.Game.Client.Object
{
    public class Player : MonoBehaviour
    {
        private Rigidbody2D rb;
        public bool isLocalPlayer;
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            rb.freezeRotation = true;
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;
            rb.velocity = KeyGetter.PlayerDir;
        }
    }
}