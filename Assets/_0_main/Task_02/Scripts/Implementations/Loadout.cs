namespace Weapons
{
    public sealed class WeaponLoadout
    {
        public IWeapon Primary1 { get; private set; }
        public IWeapon Primary2 { get; private set; }
        public IWeapon Secondary { get; private set; }
        public IWeapon Active { get; private set; }

        public void EquipPrimary1(IWeapon w) { Primary1 = w; if (Active == null) Active = w; }
        public void EquipPrimary2(IWeapon w) { Primary2 = w; }
        public void EquipSecondary(IWeapon w) { Secondary = w; }

        public bool SwitchTo(string slot)
        {
            IWeapon target = slot == "P1" ? Primary1 :
                             slot == "P2" ? Primary2 :
                             slot == "S" ? Secondary : null;
            if (target == null) return false;
            Active?.StopFire();
            Active = target;
            return true;
        }
    }

    public sealed class PlayerCombat
    {
        public WeaponLoadout Loadout { get; } = new();

        public void Tick(float dt) => Loadout.Active?.Tick(dt);
        public void FireDown() => Loadout.Active?.StartFire();
        public void FireUp() => Loadout.Active?.StopFire();
        public void Reload() => Loadout.Active?.TryReload();
    }
}