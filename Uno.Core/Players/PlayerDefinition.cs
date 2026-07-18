namespace Uno.Core.Players;

public sealed class PlayerDefinition
{
    public PlayerDefinition(string name, PlayerType type, int? profileId = null)
    {
        Name = string.IsNullOrWhiteSpace(name) ? "Player" : name.Trim();
        Type = type;
        ProfileId = profileId;
    }

    public string Name { get; }

    public PlayerType Type { get; }

    public int? ProfileId { get; }
}
