using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Networking
{
    public sealed class SnapshotInterpolator : IInterpolator<PositionSnapshot>
    {
        private struct TimedSnap
        {
            public float t;              // arrival time (local)
            public uint tick;
            public Vector3 pos;
        }

        private readonly Queue<TimedSnap> _buffer = new();
        private readonly float _delay; // seconds, e.g. 0.12f

        public SnapshotInterpolator(float delay = 0.12f)
        {
            _delay = Mathf.Max(0.01f, delay);
        }

        // Clear buffer and seed with a pose so output is immediate after ABS
        public void ResetTo(Vector3 pos, uint tick)
        {
            _buffer.Clear();
            float now = Time.realtimeSinceStartup;
            // seed with two identical snaps spaced by delay so TrySample has [a,b]
            _buffer.Enqueue(new TimedSnap { t = now - _delay, tick = tick, pos = pos });
            _buffer.Enqueue(new TimedSnap { t = now, tick = tick, pos = pos });
        }

        public void Push(PositionSnapshot s)
        {
            _buffer.Enqueue(new TimedSnap
            {
                t = Time.realtimeSinceStartup,
                tick = s.Tick,
                pos = s.Position
            });
        }

        public bool TrySample(float dt, out PositionSnapshot sampled)
        {
            sampled = default;
            if (_buffer.Count == 0) return false;

            float targetTime = Time.realtimeSinceStartup - _delay;

            // Drop snaps that are too far behind the target
            while (_buffer.Count >= 2)
            {
                var first = _buffer.ElementAt(0);
                var second = _buffer.ElementAt(1);
                if (second.t <= targetTime)
                    _buffer.Dequeue();
                else
                    break;
            }

            // If we have at least two, interpolate between them at targetTime
            if (_buffer.Count >= 2)
            {
                var a = _buffer.ElementAt(0);
                var b = _buffer.ElementAt(1);
                float t = Mathf.InverseLerp(a.t, b.t, targetTime);
                var p = Vector3.LerpUnclamped(a.pos, b.pos, t);
                sampled = new PositionSnapshot(b.tick, p);
                return true;
            }

            // Only one available — output it as-is
            var only = _buffer.Peek();
            sampled = new PositionSnapshot(only.tick, only.pos);
            return true;
        }
    }
}