namespace WildsOfCloverhollow.Minigames
{
    /// <summary>
    /// Reward tiers based on timing accuracy in claw machine.
    /// Better timing = better tier = better prizes.
    /// </summary>
    public enum PrizeTier
    {
        Low,        // 60-100% from center
        Medium,     // 30-60% from center
        Good,       // 10-30% from center
        Best        // 0-10% from center (bullseye!)
    }
}
