using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MehGame.View;

namespace MehGame.Model
{
	public class Player
	{
		private int score;
		private bool active;
		private int health;
		public Animation PlayerAnimation;

		// Animation representing the player
		public Texture2D PlayerTexture;

		// Position of the Player relative to the upper left side of the screen
		public Vector2 Position;

		// State of the player
		public bool Active
		{
			get { return active; }
			set { active = value; }
		}

		// Amount of hit points that player has
		public int Health;

		// Get the width of the player ship

		public int Width
		{
			get { return PlayerAnimation.FrameWidth; }
		}

		// Get the height of the player ship
		public int Height
		{
			get { return PlayerAnimation.FrameHeight; }
		}

		public int Score
		{
			get {return score; }
			set { score = value; }
		}
		public void Initialize(Texture2D texture, Vector2 position)
		{
			PlayerTexture = texture; 

			// Set the starting position of the player around the middle of the screen and to the back
			Position = position;

			// Set the player to be active
			Active = true;

			// Set the player health
			this.health = 100;

			this.score = 0;
		}

		// Initialize the player
		public void Initialize(Animation animation, Vector2 position)
		{
			PlayerAnimation = animation;

			// Set the starting position of the player around the middle of the screen and to the back
			Position = position;

			// Set the player to be active
			Active = true;

			// Set the player health
			Health = 100;
		}

		// Draw the player
		public void Draw(SpriteBatch spriteBatch)
		{
			PlayerAnimation.Draw(spriteBatch);
		}
		// Update the player animation
		public void Update(GameTime gameTime)
		{
			PlayerAnimation.Position = Position;
			PlayerAnimation.Update(gameTime);
		}
	}
}

