using Script.Game.Client.Getter;
using UnityEngine;

namespace Script.Game.Client.Object
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D rb;
        public bool isLocalPlayer;
        private void Start()
        {
            rb = GetComponent<Rigidbody2D>();
        }

        private void Update()
        {
            if (!isLocalPlayer)
                return;
            rb.velocity = KeyGetter.PlayerDir;
        }
    }
}