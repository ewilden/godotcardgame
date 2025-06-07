using Godot;
using System;
using System.Collections.Generic;

/// <summary>
/// Represents a deck of cards in the game.
/// This script handles the visual display of stacked cards
/// and spawning new cards when clicked.
/// </summary>
public partial class Deck : Control
{
  // Number of visual card backs to show in the stack
  [Export] public int StackSize { get; set; } = 5;

  // Visual offset between stacked cards
  [Export] public Vector2 StackOffset { get; set; } = new Vector2(2, 2);

  // Reference to the Card scene for spawning new cards
  private PackedScene _cardScene;

  // Available ranks and suits for card generation
  private static readonly string[] Ranks = { "A", "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K" };
  private static readonly string[] Suits = { "Hearts", "Diamonds", "Clubs", "Spades" };

  // Random number generator
  private RandomNumberGenerator _rng;

  // Keep track of spawned cards
  private List<Card> _spawnedCards = new List<Card>();

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

    // Create the visual stack of card backs
    CreateVisualStack();
  }

  /// <summary>
  /// Creates the visual representation of stacked cards
  /// </summary>
  private void CreateVisualStack()
  {
    // Create card backs from bottom to top
    for (int i = 0; i < StackSize; i++)
    {
      var cardBack = new ColorRect
      {
        LayoutMode = 1, // Use anchors/containers layout mode
        CustomMinimumSize = Size,
        Size = Size,
        Color = new Color(1, 0, 0, 0.5f), // Same red as card back
        MouseFilter = MouseFilterEnum.Ignore,
        Position = StackOffset * i,
      };

      // Add the card back to the scene
      AddChild(cardBack);
    }

    // Make the entire stack clickable
    MouseDefaultCursorShape = CursorShape.PointingHand;
  }

  /// <summary>
  /// Handles GUI input events, specifically clicking to spawn cards
  /// </summary>
  public override void _GuiInput(InputEvent @event)
  {
    if (@event.IsActionPressed("click"))
    {
      AcceptEvent();
      SpawnCard();
    }
  }

  /// <summary>
  /// Spawns a new card instance slightly offset from the deck's position
  /// </summary>
  private void SpawnCard()
  {
    // Instance a new card
    var card = _cardScene.Instantiate<Card>();

    // Set random rank and suit
    card.Rank = GetRandomRank();
    card.Suit = GetRandomSuit();

    // Position it slightly to the right of the deck
    card.Position = GlobalPosition + new Vector2(120, 0);

    // Add it to the same parent as the deck
    GetParent().AddChild(card);

    // Keep track of the spawned card
    _spawnedCards.Add(card);
  }

  /// <summary>
  /// Gets a random rank from the available ranks
  /// </summary>
  private string GetRandomRank()
  {
    int index = _rng.RandiRange(0, Ranks.Length - 1);
    return Ranks[index];
  }

  /// <summary>
  /// Gets a random suit from the available suits
  /// </summary>
  private string GetRandomSuit()
  {
    int index = _rng.RandiRange(0, Suits.Length - 1);
    return Suits[index];
  }

  /// <summary>
  /// Clears all cards that were spawned from this deck
  /// </summary>
  private void OnClearButtonPressed()
  {
    foreach (var card in _spawnedCards)
    {
      if (IsInstanceValid(card)) // Check if the card still exists
      {
        card.QueueFree();
      }
    }
    _spawnedCards.Clear();
  }
}