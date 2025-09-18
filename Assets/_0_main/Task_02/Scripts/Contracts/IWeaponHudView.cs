namespace Weapons.Hud
{
    /// Implement this in tests or a thin Unity adapter later.
    public interface IWeaponHudView
    {
        void Render(WeaponHudState state);
    }
}