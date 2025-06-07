using Godot;
using System;

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

  // Called when the node enters the scene tree for the first time.
  public override void _Ready()
  {
    GD.Print("Deck ready");

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
        Color = new Color(1, 0, 0, 0.5f) // Same red as card back
      };

      // Position each card back with an offset
      cardBack.Position = StackOffset * i;
      cardBack.MouseFilter = MouseFilterEnum.Pass;

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
    if (@event is InputEventMouseButton mouseButton)
    {
      if (mouseButton.ButtonIndex == MouseButton.Left && mouseButton.Pressed)
      {
        SpawnCard();
      }
    }
  }

  /// <summary>
  /// Spawns a new card instance slightly offset from the deck's position
  /// </summary>
  private void SpawnCard()
  {
    // Instance a new card
    var card = _cardScene.Instantiate<Card>();

    // Position it slightly to the right of the deck
    card.Position = GlobalPosition + new Vector2(120, 0);

    // Add it to the same parent as the deck
    GetParent().AddChild(card);

    // Ensure it's face up
    card.FlipCard(); // Since cards start face-down by default now
  }
}