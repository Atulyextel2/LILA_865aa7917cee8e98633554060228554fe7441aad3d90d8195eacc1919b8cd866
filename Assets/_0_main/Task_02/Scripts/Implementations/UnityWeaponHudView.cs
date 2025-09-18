using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Weapons.Hud;

public sealed class UnityWeaponHudView : MonoBehaviour, IWeaponHudView
{
    [Header("Top line")]
    public TextMeshProUGUI TxtActive;

    [Header("Slots")]
    public TextMeshProUGUI TxtP1;
    public TextMeshProUGUI TxtP2;
    public TextMeshProUGUI TxtS;

    public void Render(WeaponHudState state)
    {
        if (TxtActive) TxtActive.text = $"Active: {state.ActiveDisplayName}";

        foreach (var s in state.Slots)
        {
            var line = $"{(s.IsEquipped ? "*" : " ")} {s.Slot}: {s.Name} | Current {s.CurrentAmmo} | Total {s.TotalAmmo}";
            switch (s.Slot)
            {
                case WeaponSlot.P1: if (TxtP1) TxtP1.text = line; break;
                case WeaponSlot.P2: if (TxtP2) TxtP2.text = line; break;
                case WeaponSlot.S: if (TxtS) TxtS.text = line; break;
            }
        }
    }
}