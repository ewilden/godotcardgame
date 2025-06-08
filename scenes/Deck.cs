using Godot;
using System.Collections.Generic;

/// <summary>
/// Represents a deck of cards in the game.
/// This script handles the visual display of stacked cards
/// and spawning new cards when clicked.
/// </summary>
public partial class Deck : Control
{
  // Visual offset between stacked cards
  [Export] public Vector2 StackOffset { get; set; } = new Vector2(2, 2);

  // Reference to the Card scene for spawning new cards
  private PackedScene _cardScene;

  // Available ranks and suits for card generation
  private static readonly string[] Ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
  private static readonly string[] Suits = { "Hearts", "Diamonds", "Clubs", "Spades" };

  // Random number generator
  private RandomNumberGenerator _rng;

  // Keep track of spawned card nodes
  private List<Card> _spawnedCards = new List<Card>();

  // The actual deck of cards
  private Stack<CardData> _cardDeck = new Stack<CardData>();

  // Node to hold the visual card backs
  private Control _visualStack;

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    GD.Print("Deck ready");

    // Initialize random number generator
    _rng = new RandomNumberGenerator();
    _rng.Randomize(); // Seed with current time

    // Load the Card scene
    _cardScene = GD.Load<PackedScene>("res://scenes/Card.tscn");

    // Set up the initial size of the deck
    CustomMinimumSize = new Vector2(100, 150); // Same size as cards
    Size = CustomMinimumSize;

    // Create container for visual stack
    _visualStack = new Control
    {
      LayoutMode = 1,
      AnchorsPreset = (int)LayoutPreset.FullRect,
      MouseFilter = MouseFilterEnum.Ignore
    };
    AddChild(_visualStack);

    // Initialize and shuffle the deck
    InitializeDeck();

    // Make the entire deck clickable
    MouseDefaultCursorShape = CursorShape.PointingHand;
  }

  /// <summary>
  /// Updates the visual representation of the deck based on number of cards remaining
  /// </summary>
  private void UpdateVisualStack()
  {
    // Remove all existing visual elements
    foreach (var child in _visualStack.GetChildren())
    {
      child.QueueFree();
    }

    if (_cardDeck.Count == 0)
    {
      // Create empty deck placeholder with dashed border
      var emptyDeck = new ColorRect
      {
        LayoutMode = 1,
        AnchorsPreset = (int)LayoutPreset.FullRect,
        Color = new Color(0, 0, 0, 0), // Transparent
        MouseFilter = MouseFilterEnum.Ignore
      };

      // Add a border line to create a dashed effect
      var border = new Line2D
      {
        DefaultColor = Colors.Gray,
        Width = 2,
        Closed = true
      };
      border.AddPoint(new Vector2(0, 0));
      border.AddPoint(new Vector2(Size.X, 0));
      border.AddPoint(new Vector2(Size.X, Size.Y));
      border.AddPoint(new Vector2(0, Size.Y));
      border.DefaultColor = Colors.Gray;
      border.Width = 2;
      border.Closed = true;

      // Set the line to be dashed
      var dashLength = 5;
      var gapLength = 5;
      var pattern = new float[] { dashLength, gapLength };
      border.SetMeta("_edit_lock_", true);
      border.Width = 2;
      border.DefaultColor = Colors.Gray;
      border.JointMode = Line2D.LineJointMode.Round;
      border.BeginCapMode = Line2D.LineCapMode.Round;
      border.EndCapMode = Line2D.LineCapMode.Round;

      emptyDeck.AddChild(border);
      _visualStack.AddChild(emptyDeck);
    }
    else
    {
      // Calculate number of visual cards to show (1 per 10 cards, rounding up)
      int visualCards = (_cardDeck.Count + 9) / 10; // Integer division rounding up

      // Create card backs from bottom to top
      for (int i = 0; i < visualCards; i++)
      {
        var cardBack = new ColorRect
        {
          LayoutMode = 1,
          CustomMinimumSize = Size,
          Size = Size,
          Color = new Color(1, 0, 0, 0.5f), // Same red as card back
          MouseFilter = MouseFilterEnum.Ignore,
          Position = StackOffset * i
        };

        _visualStack.AddChild(cardBack);
      }
    }
  }

  /// <summary>
  /// Initializes a fresh deck of 52 cards and shuffles them
  /// </summary>
  private void InitializeDeck()
  {
    // Clear the current deck
    _cardDeck.Clear();

    // Create a list of all possible cards
    var cards = new List<CardData>();
    foreach (var suit in Suits)
    {
      foreach (var rank in Ranks)
      {
        cards.Add(new CardData(rank, suit));
      }
    }

    // Shuffle the cards using Fisher-Yates shuffle
    for (int i = cards.Count - 1; i > 0; i--)
    {
      int j = _rng.RandiRange(0, i);
      (cards[i], cards[j]) = (cards[j], cards[i]); // Swap
    }

    // Push all cards onto the deck
    foreach (var card in cards)
    {
      _cardDeck.Push(card);
    }

    // Update the visual representation
    UpdateVisualStack();

    GD.Print($"Deck initialized with {_cardDeck.Count} cards");
  }

  /// <summary>
  /// Handles GUI input events, specifically clicking to spawn cards
  /// </summary>
  public override void _GuiInput(InputEvent @event)
  {
    if (@event.IsActionPressed("click"))
    {
      AcceptEvent();
      if (_cardDeck.Count > 0)
      {
        SpawnCard();
      }
      else
      {
        GD.Print("No more cards in deck!");
      }
    }
  }

  /// <summary>
  /// Spawns a new card instance slightly offset from the deck's position
  /// </summary>
  private void SpawnCard()
  {
    // Pop the next card from the deck
    var cardData = _cardDeck.Pop();

    // Instance a new card
    var card = _cardScene.Instantiate<Card>();

    // Set the card's rank and suit from the popped data
    card.Rank = cardData.Rank;
    card.Suit = cardData.Suit;

    // Position it slightly to the right of the deck
    card.Position = GlobalPosition + new Vector2(120, 0);

    // Add it to the same parent as the deck
    GetParent().AddChild(card);

    // Keep track of the spawned card
    _spawnedCards.Add(card);

    // Update the visual representation
    UpdateVisualStack();

    GD.Print($"Drew {cardData}, {_cardDeck.Count} cards remaining");
  }

  /// <summary>
  /// Clears all spawned cards and reinitializes the deck
  /// </summary>
  private void OnClearButtonPressed()
  {
    // Remove all spawned card nodes
    foreach (var card in _spawnedCards)
    {
      if (IsInstanceValid(card))
      {
        card.QueueFree();
      }
    }
    _spawnedCards.Clear();

    // Reinitialize and shuffle the deck
    InitializeDeck();
    GD.Print("Cleared cards and reshuffled deck");
  }
}