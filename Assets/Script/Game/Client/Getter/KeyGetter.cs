using Net.Component;
using UnityEngine;

namespace Script.Game.Client.Getter
{
    public class KeyGetter : SingleCase<KeyGetter>
    {
        private Controller.Controller _controller;

        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _controller = new Controller.Controller();
        }

        private void OnEnable()
        {
            _controller.Enable();
        }

        private void OnDisable()
        {
            _controller.Disable();
        }

        private void FixedUpdate()
        {
            PlayerDir = _controller.Gaming.Player.ReadValue<Vector2>();
        }
        
        public static Vector2 PlayerDir { get; private set; }
    }
}
