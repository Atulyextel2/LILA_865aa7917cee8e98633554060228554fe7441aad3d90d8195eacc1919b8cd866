using UnityEngine;

namespace Weapons
{
    public abstract class WeaponBase : IWeapon
    {
        public string Id { get; protected set; }
        public string DisplayName { get; protected set; }
        public int MagazineSize { get; protected set; }
        public int AmmoInMag { get; protected set; }
        public int AmmoReserve { get; protected set; }
        public float FireRate { get; protected set; }
        public float ReloadTime { get; protected set; }
        public WeaponState State { get; protected set; } = WeaponState.Idle;

        float _cooldown;
        float _reloadTimer;
        bool _isFiringRequested;

        public virtual bool CanFire() =>
            State != WeaponState.Reloading && AmmoInMag > 0 && _cooldown <= 0f;

        public void StartFire() { _isFiringRequested = true; }
        public void StopFire() { _isFiringRequested = false; }

        public virtual void Tick(float dt)
        {
            if (_cooldown > 0f) _cooldown -= dt;

            if (State == WeaponState.Reloading)
            {
                _reloadTimer -= dt;
                if (_reloadTimer <= 0f)
                {
                    var need = MagazineSize - AmmoInMag;
                    var load = Mathf.Min(need, AmmoReserve);
                    AmmoInMag += load;
                    AmmoReserve -= load;
                    State = WeaponState.Idle;
                    WeaponBus.OnReloadFinished?.Invoke(this);
                }
                return;
            }

            if (_isFiringRequested && CanFire())
            {
                State = WeaponState.Firing;
                FireOne();
                _cooldown = 1f / Mathf.Max(0.0001f, FireRate);
            }
            else
            {
                if (State == WeaponState.Firing) State = WeaponState.Idle;
            }
        }

        protected virtual void FireOne()
        {
            AmmoInMag--;
            WeaponBus.OnFired?.Invoke(this);
        }

        public virtual bool TryReload()
        {
            if (State == WeaponState.Reloading) return false;
            if (AmmoReserve <= 0 || AmmoInMag == MagazineSize) return false;
            State = WeaponState.Reloading;
            _reloadTimer = ReloadTime;
            WeaponBus.OnReloadStarted?.Invoke(this);
            return true;
        }
    }

    public sealed class Rifle : WeaponBase
    {
        public Rifle(int reserve)
        {
            Id = "rifle_m4"; DisplayName = "M4";
            MagazineSize = 30; AmmoInMag = 30; AmmoReserve = reserve;
            FireRate = 10f; ReloadTime = 2.1f;
        }
    }

    public sealed class AK47 : WeaponBase
    {
        public AK47(int reserve)
        {
            Id = "ak_47"; DisplayName = "AK-47";
            MagazineSize = 35; AmmoInMag = 35; AmmoReserve = reserve;
            FireRate = 20f; ReloadTime = 1.1f;
        }
    }

    public sealed class Pistol : WeaponBase
    {
        public Pistol(int reserve)
        {
            Id = "pistol_9mm"; DisplayName = "9mm";
            MagazineSize = 15; AmmoInMag = 15; AmmoReserve = reserve;
            FireRate = 6f; ReloadTime = 1.5f;
        }
    }
}