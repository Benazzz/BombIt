namespace BombIt.Client.Settings;

// Command Pattern
public class MatchSettings
{
    public const int MinRounds = 1;
    public const int MaxRounds = 10;
    public const int DefaultRounds = 3;

    public const int MinRoundTime = 60;
    public const int MaxRoundTime = 300;
    public const int DefaultRoundTime = 220;
    public const int RoundTimeStep = 10;

    public int Rounds { get; private set; } = DefaultRounds;
    public int RoundTimeSeconds { get; private set; } = DefaultRoundTime;

    public bool IsDefault => Rounds == DefaultRounds && RoundTimeSeconds == DefaultRoundTime;

    public void SetRounds(int rounds)
    {
        Rounds = Math.Clamp(rounds, MinRounds, MaxRounds);
    }

    public void SetRoundTime(int seconds)
    {
        RoundTimeSeconds = Math.Clamp(seconds, MinRoundTime, MaxRoundTime);
    }
}