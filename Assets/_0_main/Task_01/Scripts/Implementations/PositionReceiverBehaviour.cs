using UnityEngine;
using Networking;
using Infra.SimpleCodec;

namespace NetworkDemo
{
    public enum OffsetSpace { World, RelativeToReference }

    public sealed class PositionReceiverBehaviour : MonoBehaviour
    {
        public Transform RemotePlayer;
        public bool EnableOffset = false;
        public Vector3 Offset = new Vector3(2f, 0f, 0f);
        public bool LogDecodedValues = false;

        public event System.Action<Vector3> OnReceived;

        DeltaQuantizer _quantizer;
        BitPackedPositionCodec _codec;
        SnapshotInterpolator _interp;
        bool _hasInit;

        void Awake()
        {
            _quantizer = new DeltaQuantizer(100f, 0.01f);
            _codec = new BitPackedPositionCodec(_quantizer);
            _interp = new SnapshotInterpolator(0.12f);
            if (!RemotePlayer) RemotePlayer = transform;
        }

        public void OnPacket(byte[] payload)
        {
            if (_codec.TryDecode(payload, out var snap))
            {
                var adjusted = ApplyOffset(snap.Position);

                if (!_hasInit)
                {
                    // Seed the interpolator so sampling is immediate and stable
                    _interp.ResetTo(adjusted, snap.Tick);
                    RemotePlayer.position = adjusted;
                    _hasInit = true;
                }
                else
                {
                    _interp.Push(new PositionSnapshot(snap.Tick, adjusted));
                }

                if (LogDecodedValues)
                    Debug.Log($"[RECV raw={snap.Position:F3}] adjusted={adjusted:F3}");

                OnReceived?.Invoke(adjusted);
            }
        }

        void Update()
        {
            if (!_hasInit || !RemotePlayer) return;

            if (_interp.TrySample(Time.deltaTime, out var s))
                RemotePlayer.position = s.Position;
        }

        Vector3 ApplyOffset(Vector3 p) => EnableOffset ? p + Offset : p;
    }
}