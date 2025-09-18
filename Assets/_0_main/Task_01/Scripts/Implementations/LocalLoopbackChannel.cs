using Networking;
using UnityEngine;

namespace NetworkDemo
{
    public sealed class LocalLoopbackChannel : MonoBehaviour, ILoopbackChannel
    {
        public PositionReceiverBehaviour Receiver;
        public void Send(byte[] payload) => Receiver?.OnPacket(payload);
    }
}