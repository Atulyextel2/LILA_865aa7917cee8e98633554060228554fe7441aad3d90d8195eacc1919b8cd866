using System.Collections.Generic;
using Weapons;

namespace Weapons.Hud
{
    public sealed class WeaponHudPresenter
    {
        readonly PlayerCombat _combat;
        readonly IWeaponHudView _view;

        public WeaponHudPresenter(PlayerCombat combat, IWeaponHudView view)
        {
            _combat = combat;
            _view = view;

            // Initial paint
            _view.Render(BuildState());

            // Auto-refresh on domain events
            WeaponBus.OnFired += _ => _view.Render(BuildState());
            WeaponBus.OnReloadStarted += _ => _view.Render(BuildState());
            WeaponBus.OnReloadFinished += _ => _view.Render(BuildState());
        }

        public void Refresh() => _view.Render(BuildState());

        WeaponHudState BuildState()
        {
            var slots = new List<WeaponSlotHud>(3);

            void Add(WeaponSlot slot, IWeapon w, IWeapon active)
            {
                if (w == null)
                {
                    slots.Add(new WeaponSlotHud(slot, "(empty)", 0, 0, false));
                    return;
                }
                int current = w.AmmoInMag;
                int total = w.AmmoInMag + w.AmmoReserve; // “Total” = mag + reserve
                bool eq = ReferenceEquals(active, w);
                slots.Add(new WeaponSlotHud(slot, w.DisplayName, current, total, eq));
            }

            var active = _combat.Loadout.Active;
            Add(WeaponSlot.P1, _combat.Loadout.Primary1, active);
            Add(WeaponSlot.P2, _combat.Loadout.Primary2, active);
            Add(WeaponSlot.S, _combat.Loadout.Secondary, active);

            var activeName = active?.DisplayName ?? "(none)";
            return new WeaponHudState(activeName, slots);
        }
    }
}