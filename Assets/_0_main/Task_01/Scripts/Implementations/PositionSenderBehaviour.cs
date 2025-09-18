using UnityEngine;
using Networking;
using Infra.SimpleCodec; // << use our bit-packed codec

namespace NetworkDemo
{
    public sealed class PositionSenderBehaviour : MonoBehaviour
    {
        public Transform LocalPlayer;
        [SerializeField] private LocalLoopbackChannel Channel;
        public float MaxSendHz = 20f;

        public event System.Action<Vector3, int> OnSent;

        Vector3 _lastSent;
        float _timer;
        uint _tick;
        DeltaQuantizer _quant;
        BitPackedPositionCodec _codec;
        bool _sentInit;

        void Awake()
        {
            _quant = new DeltaQuantizer(100f, 0.01f);
            _codec = new BitPackedPositionCodec(_quant);
            if (!LocalPlayer) LocalPlayer = transform;
            if (!Channel) Channel = FindObjectOfType<LocalLoopbackChannel>();
            _lastSent = LocalPlayer.position;
        }

        void Start()
        {
            SendABS(LocalPlayer.position);
        }

        void Update()
        {
            if (!LocalPlayer || !Channel || !_sentInit) return;

            _timer += Time.deltaTime;
            if (_timer < 1f / Mathf.Max(1f, MaxSendHz)) return;

            var now = LocalPlayer.position;

            if (_quant.ShouldSend(_lastSent, now))
            {
                var d = _quant.QuantizeDelta(_lastSent, now);

                // Teleport/overflow guard: if any |delta| exceeds short.MaxValue, send ABS instead
                if (Mathf.Abs(d.Dx) > short.MaxValue || Mathf.Abs(d.Dy) > short.MaxValue || Mathf.Abs(d.Dz) > short.MaxValue)
                {
                    SendABS(now);
                }
                else
                {
                    var bytes = _codec.EncodeDelta(d, ++_tick);
                    Debug.Log($"[SEND-DELTA] pos={now:F3} bytes={bytes.Length}");
                    OnSent?.Invoke(now, bytes.Length);
                    Channel.Send(bytes);
                    _lastSent = now;
                }
            }
            _timer = 0f;
        }

        void SendABS(Vector3 pos)
        {
            var bytes = _codec.EncodeInit(pos, ++_tick);
            Channel?.Send(bytes);
            Debug.Log($"[SEND-ABS] pos={pos:F3} bytes={bytes.Length}");
            OnSent?.Invoke(pos, bytes.Length);
            _lastSent = pos;
            _sentInit = true;
        }
    }
}