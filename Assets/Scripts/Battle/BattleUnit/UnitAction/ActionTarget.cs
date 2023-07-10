public enum ActionTarget
{
    Single,
    /// <summary>
    /// Party is either the entire player party or the entire enemy party.
    /// </summary>
    Party,
    /// <summary>
    /// Select a single enemy, then check if multiple enemies in AoE. If used against the player, hits the entire party.
    /// </summary>
    AoE,
    /// <summary>
    /// Hits everyone in the battle, friend and foe alike.
    /// </summary>
    All,
    Random
}
