using System;

namespace Weapons
{
    public interface IWeapon
    {
        string Id { get; }
        string DisplayName { get; }
        int MagazineSize { get; }
        int AmmoInMag { get; }
        int AmmoReserve { get; }
        float FireRate { get; }
        float ReloadTime { get; }
        WeaponState State { get; }

        bool CanFire();
        void StartFire();
        void StopFire();
        void Tick(float dt);
        bool TryReload();
    }

    public enum WeaponState { Idle, Firing, Reloading }

    public static class WeaponBus
    {
        public static Action<IWeapon> OnFired;
        public static Action<IWeapon> OnReloadStarted;
        public static Action<IWeapon> OnReloadFinished;
    }
}