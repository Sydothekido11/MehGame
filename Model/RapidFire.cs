
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MehGame.Model
{
	public class RapidFire
	{
		// Image representing the Projectile
		public Texture2D RapidTexture;

		// Position of the Projectile relative to the upper left side of the screen
		public Vector2 Position;

		// State of the Projectile
		public bool Active;

		// The amount of damage the projectile can inflict to an enemy
		public int Damage;

		// Represents the viewable boundary of the game
		Viewport viewport;

		// Determines how fast the projectile moves
		float projectileMoveSpeed;


		// Get the width of the projectile ship
		public int Width
		{
			get { return RapidTexture.Width; }
		}

		// Get the height of the projectile ship
		public int Height
		{
			get { return RapidTexture.Height; }
		}

		public void Initialize(Viewport viewport, Texture2D beamTexture, Vector2 position)
		{
			this.RapidTexture = beamTexture;
			Position = position;
			this.viewport = viewport;

			Active = true;

			Damage = 1;

			projectileMoveSpeed = 20f;
		}
		public void Update()
		{
			// Projectiles always move to the right
			Position.X += projectileMoveSpeed;

			// Deactivate the bullet if it goes out of screen
			if (Position.X + RapidTexture.Width / 2 > viewport.Width)
				Active = false;
		}
		public void Draw(SpriteBatch spriteBatch)
		{
			spriteBatch.Draw(RapidTexture, Position, null, Color.White, 0f,
				new Vector2(Width / 2, Height / 2), 1f, SpriteEffects.None, 0f);
		}

	}
}

