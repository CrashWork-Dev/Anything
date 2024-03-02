using Net.Component;

namespace Script.Init.Internal.Network
{
    public class Server : SingleCase<Server>
    {
        private readonly Service _service = new();
        public static bool StartTrigger;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            
        }

        private void FixedUpdate()
        {
            if (StartTrigger)
            {
                StartTrigger = false;
                _service.Start();
            }
        }
    }
}