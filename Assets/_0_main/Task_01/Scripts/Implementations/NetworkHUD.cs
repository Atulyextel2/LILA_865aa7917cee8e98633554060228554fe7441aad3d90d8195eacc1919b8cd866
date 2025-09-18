using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NetworkDemo
{
    public sealed class NetworkHUD : MonoBehaviour
    {
        public TextMeshProUGUI TxtSent;
        public TextMeshProUGUI TxtRecv;

        public void Bind(PositionSenderBehaviour sender, PositionReceiverBehaviour receiver)
        {
            if (sender) sender.OnSent += (pos, bytes) =>
                { if (TxtSent) TxtSent.text = $"Sent: {pos:F3} | {bytes} bytes"; };

            if (receiver) receiver.OnReceived += pos =>
                { if (TxtRecv) TxtRecv.text = $"Recv: {pos:F3}"; };
        }
    }
}