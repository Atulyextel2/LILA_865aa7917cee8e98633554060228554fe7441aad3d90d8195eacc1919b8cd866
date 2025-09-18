using System;
using Networking;
using UnityEngine;

namespace Infra.SimpleCodec
{
    public sealed class BitPackedPositionCodec : IPositionCodec
    {
        private readonly DeltaQuantizer _quant;
        private Vector3 _lastReconstructed;
        private bool _hasInit;

        public BitPackedPositionCodec(DeltaQuantizer quant) { _quant = quant; }

        static void WriteInt16LE(short v, byte[] b, int o) { b[o] = (byte)(v & 0xFF); b[o + 1] = (byte)((v >> 8) & 0xFF); }
        static void WriteInt32LE(int v, byte[] b, int o)
        {
            b[o] = (byte)(v & 0xFF); b[o + 1] = (byte)((v >> 8) & 0xFF);
            b[o + 2] = (byte)((v >> 16) & 0xFF); b[o + 3] = (byte)((v >> 24) & 0xFF);
        }
        static short ReadInt16LE(byte[] b, int o) => (short)(b[o] | (b[o + 1] << 8));
        static int ReadInt32LE(byte[] b, int o) => b[o] | (b[o + 1] << 8) | (b[o + 2] << 16) | (b[o + 3] << 24);

        public byte[] EncodeInit(Vector3 absolute, uint tick)
        {
            int ax = Mathf.RoundToInt(absolute.x * _quant.Scale);
            int ay = Mathf.RoundToInt(absolute.y * _quant.Scale);
            int az = Mathf.RoundToInt(absolute.z * _quant.Scale);

            var buf = new byte[15];
            buf[0] = 0x00; // ABS32
            WriteInt32LE(ax, buf, 1);
            WriteInt32LE(ay, buf, 5);
            WriteInt32LE(az, buf, 9);
            ushort tk = (ushort)(tick & 0xFFFF);
            buf[13] = (byte)(tk & 0xFF);
            buf[14] = (byte)((tk >> 8) & 0xFF);
            return buf;
        }

        public byte[] EncodeDelta(in PositionDelta d, uint tick)
        {
            // If any delta would overflow int16, the caller should send ABS instead.
            var buf = new byte[9];
            buf[0] = 0x01; // DELTA
            WriteInt16LE((short)Mathf.Clamp(d.Dx, short.MinValue, short.MaxValue), buf, 1);
            WriteInt16LE((short)Mathf.Clamp(d.Dy, short.MinValue, short.MaxValue), buf, 3);
            WriteInt16LE((short)Mathf.Clamp(d.Dz, short.MinValue, short.MaxValue), buf, 5);
            ushort tk = (ushort)(tick & 0xFFFF);
            buf[7] = (byte)(tk & 0xFF);
            buf[8] = (byte)((tk >> 8) & 0xFF);
            return buf;
        }

        public bool TryDecode(byte[] payload, out PositionSnapshot snap)
        {
            snap = default;
            if (payload == null || payload.Length < 1) return false;

            var kind = payload[0];
            if (kind == 0x00) // ABS32
            {
                if (payload.Length != 15) return false;
                int ax = ReadInt32LE(payload, 1);
                int ay = ReadInt32LE(payload, 5);
                int az = ReadInt32LE(payload, 9);
                ushort tk = (ushort)(payload[13] | (payload[14] << 8));

                var pos = new Vector3(ax / _quant.Scale, ay / _quant.Scale, az / _quant.Scale);
                _lastReconstructed = pos;
                _hasInit = true;
                snap = new PositionSnapshot(tk, pos);
                return true;
            }
            if (kind == 0x01) // DELTA
            {
                if (payload.Length != 9 || !_hasInit) return false;
                short dx = ReadInt16LE(payload, 1);
                short dy = ReadInt16LE(payload, 3);
                short dz = ReadInt16LE(payload, 5);
                ushort tk = (ushort)(payload[7] | (payload[8] << 8));

                var pos = _quant.ApplyDelta(_lastReconstructed, new PositionDelta(dx, dy, dz));
                _lastReconstructed = pos;
                snap = new PositionSnapshot(tk, pos);
                return true;
            }
            return false;
        }
    }
}