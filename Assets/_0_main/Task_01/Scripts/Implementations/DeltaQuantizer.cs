using UnityEngine;

namespace Networking
{
    public sealed class DeltaQuantizer
    {
        public readonly float Scale;   // 100f => 1 unit = 100 "cm"
        public readonly float Epsilon; // send threshold in world units

        public DeltaQuantizer(float scale = 100f, float epsilon = 0.01f)
        { Scale = scale; Epsilon = epsilon; }

        public bool ShouldSend(Vector3 from, Vector3 to)
        {
            var d = to - from;
            return Mathf.Abs(d.x) > Epsilon || Mathf.Abs(d.y) > Epsilon || Mathf.Abs(d.z) > Epsilon;
        }

        // --- Delta path
        public PositionDelta QuantizeDelta(Vector3 from, Vector3 to)
        {
            var d = to - from;
            int qx = Mathf.RoundToInt(d.x * Scale);
            int qy = Mathf.RoundToInt(d.y * Scale);
            int qz = Mathf.RoundToInt(d.z * Scale);
            return new PositionDelta(qx, qy, qz);
        }
        public Vector3 ApplyDelta(Vector3 last, PositionDelta q)
        {
            return new Vector3(
                last.x + q.Dx / Scale,
                last.y + q.Dy / Scale,
                last.z + q.Dz / Scale
            );
        }

        // --- Absolute path (for the very first packet)
        public (short ax, short ay, short az) QuantizeAbsolute(Vector3 pos)
        {
            // Clamp to Int16 range after scaling
            int ix = Mathf.RoundToInt(pos.x * Scale);
            int iy = Mathf.RoundToInt(pos.y * Scale);
            int iz = Mathf.RoundToInt(pos.z * Scale);
            short cx = (short)Mathf.Clamp(ix, short.MinValue, short.MaxValue);
            short cy = (short)Mathf.Clamp(iy, short.MinValue, short.MaxValue);
            short cz = (short)Mathf.Clamp(iz, short.MinValue, short.MaxValue);
            return (cx, cy, cz);
        }
        public Vector3 DequantizeAbsolute(short ax, short ay, short az)
        {
            return new Vector3(ax / Scale, ay / Scale, az / Scale);
        }
    }
}