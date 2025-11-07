namespace ToP.Domain.Classes
{
    // Value Object representing an immutable fixture
    public record Match(Player Player1, Player Player2)
    {
        // Optional: Custom logic for display or data transfer
        public override string ToString() => $"{Player1.Name} vs {Player2.Name}";
    }
}