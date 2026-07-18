namespace Uno.Data.Models;

public sealed class StoredPlayerProfile
{
    public int PlayerId { get; init; }

    public string DisplayName { get; init; } = string.Empty;

    public DateTimeOffset CreatedAtUtc { get; init; }
}
