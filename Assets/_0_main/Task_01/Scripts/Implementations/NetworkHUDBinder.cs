using UnityEngine;

namespace NetworkDemo
{
    public sealed class NetworkHUDBinder : MonoBehaviour
    {
        public NetworkHUD Hud;
        public PositionSenderBehaviour Sender;
        public PositionReceiverBehaviour Receiver;

        void Awake()
        {
            if (!Hud || !Sender || !Receiver)
            {
                Debug.LogError("[NetworkHUDBinder] Assign Hud, Sender, Receiver in Inspector.");
                return;
            }
            Hud.Bind(Sender, Receiver);
        }
    }
}