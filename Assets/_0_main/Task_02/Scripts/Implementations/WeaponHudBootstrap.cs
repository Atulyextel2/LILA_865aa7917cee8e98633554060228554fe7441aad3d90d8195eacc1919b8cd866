using UnityEngine;
using Weapons;
using Weapons.Hud;

public sealed class WeaponHudBootstrap : MonoBehaviour
{
    public UnityWeaponHudView View;

    private PlayerCombat _combat;
    private WeaponHudPresenter _presenter;

    void Awake()
    {
        _combat = new PlayerCombat();
        _combat.Loadout.EquipPrimary1(new Rifle(reserve: 120));
        _combat.Loadout.EquipPrimary2(new AK47(reserve: 200));
        _combat.Loadout.EquipSecondary(new Pistol(reserve: 60));

        _presenter = new WeaponHudPresenter(_combat, View);
    }

    void Update()
    {
        _combat.Tick(Time.deltaTime);

        // Fire/Reload
        if (Input.GetKeyDown(KeyCode.F)) _combat.FireDown();
        if (Input.GetKeyUp(KeyCode.G)) _combat.FireUp();
        if (Input.GetKeyDown(KeyCode.R)) _combat.Reload();

        // Switch slots
        if (Input.GetKeyDown(KeyCode.Alpha1)) { _combat.Loadout.SwitchTo("P1"); _presenter.Refresh(); }
        if (Input.GetKeyDown(KeyCode.Alpha2)) { _combat.Loadout.SwitchTo("P2"); _presenter.Refresh(); }
        if (Input.GetKeyDown(KeyCode.Alpha3)) { _combat.Loadout.SwitchTo("S"); _presenter.Refresh(); }
    }
}