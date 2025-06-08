using Godot;

/// <summary>
/// Represents a single playing card in the game.
/// This script handles the visual display of the card (rank, suit)
/// and enables dragging functionality.
/// </summary>
public partial class Card : Control
{
    // Exported properties allow you to set the card's rank and suit
    // directly from the Godot editor's Inspector panel.
    [Export] public string Rank { get; set; } = "A"; // e.g., "A", "K", "Q", "J", "10", "9", etc.
    [Export] public string Suit { get; set; } = "Spades"; // e.g., "Spades", "Hearts", "Diamonds", "Clubs"
    [Export] public bool IsFaceUp { get; private set; } = true; // Whether the card is face up or face down

    // Internal state variables for dragging
    private bool _isDragging = false;
    private Vector2 _dragOffset; // Stores the offset from the card's origin to the mouse click point

    // References to child nodes for visual elements
    private ColorRect _cardBackground;
    private ColorRect _cardBack;
    private Label _rankLabel;
    private Label _suitLabel;

    /// <summary>
    /// Called when the node enters the scene tree for the first time.
    /// Used for initial setup, getting node references, and setting card appearance.
    /// </summary>
    public override void _Ready()
    {
        GD.Print("Card ready!");
        // Get references to the child nodes defined in the .tscn scene file.
        _cardBackground = GetNode<ColorRect>("CardBackground");
        _cardBack = GetNode<ColorRect>("CardBack");
        _rankLabel = GetNode<Label>("RankLabel");
        _suitLabel = GetNode<Label>("SuitLabel");

        // Set the initial size of the card.
        // Using CustomMinimumSize and then setting Size ensures the Control node
        // has a defined size, which is important for UI elements.
        CustomMinimumSize = new Vector2(100, 150); // A common playing card aspect ratio
        Size = CustomMinimumSize;
        // SetAnchorsPreset(LayoutPreset.Custom) is not strictly necessary if Size is set directly,
        // but it can be useful if you're working with Godot's layout system.

        // Update the card's visual display based on its current properties.
        UpdateCardDisplay();
    }

    /// <summary>
    /// Called every frame. 'delta' is the elapsed time since the previous frame.
    /// This method is used to continuously update the card's position if it's being dragged.
    /// </summary>
    public override void _Process(double delta)
    {
        if (_isDragging)
        {
            // If the card is being dragged, update its global position.
            // GlobalPosition is set to the current mouse position minus the initial drag offset.
            // This ensures the point where the user clicked on the card remains under the mouse cursor.
            GlobalPosition = GetGlobalMousePosition() - _dragOffset;
        }
    }

    /// <summary>
    /// Handles GUI input events specifically for this Control node.
    /// This is where we detect mouse clicks to start and stop dragging.
    /// </summary>
    /// <param name="@event">The input event that occurred.</param>
    public override void _GuiInput(InputEvent @event)
    {
        if (@event.IsActionPressed("click"))
        {
            AcceptEvent();
            // If the left mouse button is pressed down on the card:
            _isDragging = true; // Set dragging state to true.
                                // Calculate the offset: the difference between the mouse's global position
                                // and the card's global position. This offset is maintained during dragging.
            _dragOffset = GetGlobalMousePosition() - GlobalPosition;
            // Bring the card to the front by setting a high ZIndex.
            // Nodes with higher ZIndex values are drawn on top of nodes with lower ZIndex values.
            ZIndex = 100; // A sufficiently high value to ensure it's on top of other cards.
        }
        else if (@event.IsActionReleased("click"))
        {
            AcceptEvent();
            _isDragging = false;
            ZIndex = 0;
        }
        else if (@event.IsActionPressed("right_click"))
        {
            AcceptEvent();
            FlipCard();
        }
    }

    /// <summary>
    /// Flips the card between face-up and face-down states.
    /// </summary>
    public void FlipCard()
    {
        IsFaceUp = !IsFaceUp;
        UpdateCardDisplay();
    }

    /// <summary>
    /// Updates the visual appearance of the card (rank text, suit symbol, colors).
    /// This method is called in _Ready and can be called again if card properties change.
    /// </summary>
    private void UpdateCardDisplay()
    {
        // Show/hide card face elements based on IsFaceUp
        _cardBackground.Visible = IsFaceUp;
        _rankLabel.Visible = IsFaceUp;
        _suitLabel.Visible = IsFaceUp;
        _cardBack.Visible = !IsFaceUp;

        if (IsFaceUp)
        {
            // Set the text for the rank label.
            _rankLabel.Text = Rank;
            // Get the appropriate Unicode symbol for the suit and set the suit label text.
            _suitLabel.Text = GetSuitSymbol(Suit);

            // Determine the text color based on the suit.
            // Hearts and Diamonds are typically red, Spades and Clubs are black.
            Color textColor = (Suit == "Hearts" || Suit == "Diamonds") ? Colors.Red : Colors.Black;
            // Apply the calculated text color to both labels.
            _rankLabel.AddThemeColorOverride("font_color", textColor);
            _suitLabel.AddThemeColorOverride("font_color", textColor);

            // Set the background color of the card (e.g., white for the card face).
            _cardBackground.Color = Colors.White;
        }
    }

    /// <summary>
    /// Helper method to convert a suit name string into its corresponding Unicode symbol.
    /// </summary>
    /// <param name="suitName">The name of the suit (e.g., "Spades").</param>
    /// <returns>The Unicode symbol for the suit.</returns>
    private string GetSuitSymbol(string suitName)
    {
        return suitName switch
        {
            "Spades" => "♠",
            "Hearts" => "♥",
            "Diamonds" => "♦",
            "Clubs" => "♣",
            _ => "" // Return empty string for unknown suits
        };
    }
}
