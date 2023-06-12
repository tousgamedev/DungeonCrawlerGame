public static class Layers
{
    public const int Interactable = 3;
    public const int Player = 7;
    public const int InteractableMask = 1 << 3;
    public const int ClimbableMask = 1 << 6;
    public const int PlayerMask = 1 << 7;
    public const int IgnoreInteractableMask = ~(1 << 3);
    public const int IgnorePlayerMask = ~(1 << 7);
    public const int IgnorePlayerAndInteractableMask = ~(PlayerMask | InteractableMask);
}