public static class Layers
{
    public const int InteractableLayer = 3;
    public const int ClimbableLayerMask = 1 << 6;
    public const int IgnorePlayerLayerMask = ~(1 << 7);
}
