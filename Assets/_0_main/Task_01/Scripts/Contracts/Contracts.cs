namespace Networking
{
    public interface IPositionCodec
    {
        byte[] EncodeDelta(in PositionDelta delta, uint tick);
        bool TryDecode(byte[] payload, out PositionSnapshot snapshot);
    }

    public interface IInterpolator<T>
    {
        void Push(T item);
        bool TrySample(float dt, out T sampled);
    }

    public readonly struct PositionSnapshot
    {
        public readonly uint Tick;
        public readonly UnityEngine.Vector3 Position;
        public PositionSnapshot(uint tick, UnityEngine.Vector3 pos)
        { Tick = tick; Position = pos; }
    }

    public readonly struct PositionDelta
    {
        public readonly int Dx, Dy, Dz;
        public PositionDelta(int dx, int dy, int dz) { Dx = dx; Dy = dy; Dz = dz; }
    }

    public interface ILoopbackChannel { void Send(byte[] payload); }
}