/// <summary>
/// Represents the data for a playing card without the visual representation.
/// This is used to efficiently store cards in the deck.
/// </summary>
public readonly struct CardData
{
  public string Rank { get; }
  public string Suit { get; }

  public CardData(string rank, string suit)
  {
    Rank = rank;
    Suit = suit;
  }

  public override string ToString()
  {
    return $"{Rank} of {Suit}";
  }
}