using System.Collections.Generic;

namespace Weapons.Hud
{
    public sealed class WeaponSlotHud
    {
        public WeaponSlot Slot { get; }
        public string Name { get; }
        public int CurrentAmmo { get; }   // in-mag
        public int TotalAmmo { get; }     // in-mag + reserve
        public bool IsEquipped { get; }

        public WeaponSlotHud(WeaponSlot slot, string name, int current, int total, bool isEquipped)
        { Slot = slot; Name = name; CurrentAmmo = current; TotalAmmo = total; IsEquipped = isEquipped; }
    }

    public sealed class WeaponHudState
    {
        public string ActiveDisplayName { get; }
        public IReadOnlyList<WeaponSlotHud> Slots { get; }

        public WeaponHudState(string activeDisplayName, IReadOnlyList<WeaponSlotHud> slots)
        { ActiveDisplayName = activeDisplayName; Slots = slots; }
    }
}