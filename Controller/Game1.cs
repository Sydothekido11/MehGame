using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using MehGame.Model;
using MehGame.View;

namespace MehGame
{

	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Game
	{
		//explosion animations.
		private Texture2D explosionTexture;
		private List<Animation> explosions;

		// The sound that is played when a laser is fired
		private SoundEffect laserSound;

		// The sound used when the player or an enemy dies
		private SoundEffect explosionSound;

		// The music played during gameplay
		private Song gameplayMusic;


		// Keyboard states used to determine key presses
		private KeyboardState currentKeyboardState;
		private KeyboardState previousKeyboardState;

		// Gamepad states used to determine button presses
		private GamePadState currentGamePadState;
		private GamePadState previousGamePadState; 

		// A movement speed for the player
		private float playerMoveSpeed;
		private GraphicsDeviceManager graphics;
		private SpriteBatch spriteBatch;
		private Player player;

		// Image used to display the static background
		private Texture2D mainBackground;

		// Parallaxing Layers
		private ParallaxingBackground bgLayer1;
		private ParallaxingBackground bgLayer2;

		// Enemies
		private Texture2D enemyTexture;
		private List<Enemy> enemies;


		// The rate at which the enemies appear
		TimeSpan enemySpawnTime;
		TimeSpan previousSpawnTime;

		// A random number generator
		Random random;

		private Texture2D scatterShotTexture;
		private List<Projectile> scatterShot;

		// The rate of fire of the player laser
		private TimeSpan fireTime;
		private TimeSpan previousFireTime;

		private Texture2D beamTexture;
		private List<RapidFire> rapidFire;

		// The rate of fire of the player laser
		private TimeSpan shotRate;
		private TimeSpan previousShotTime;

		//Number that holds the player score
		private int score;

		// The font used to display UI elements
		private SpriteFont font;


		public Game1 ()
		{

			graphics = new GraphicsDeviceManager (this);
			Content.RootDirectory = "Content";
		}

		private void AddExplosion(Vector2 position)
		{
			Animation explosion = new Animation();
			explosion.Initialize(explosionTexture,position, 134, 134, 12, 45, Color.White, 1f,false);
			explosions.Add(explosion);
		}

		protected override void Initialize ()
		{
			bgLayer1 = new ParallaxingBackground();
			bgLayer2 = new ParallaxingBackground();

			enemies = new List<Enemy> ();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator

			// Initialize the enemies list

			enemies = new List<Enemy> ();

			// Set the time keepers to zero
			previousSpawnTime = TimeSpan.Zero;

			// Used to determine how fast enemy respawns
			enemySpawnTime = TimeSpan.FromSeconds(1.0f);

			// Initialize our random number generator

			random = new Random();

			player = new Player();

			playerMoveSpeed = 8.0f;

			//initalizes the projectile ArrayList
			scatterShot = new List<Projectile>();

			//initalizes the beam ArrayList;
			rapidFire = new List<RapidFire>();

			// Set the laser to fire every quarter second for the projectile
			fireTime = TimeSpan.FromSeconds(.15f);

			shotRate = TimeSpan.FromSeconds(.08f);

			//initializing explosions
			explosions = new List<Animation>();

			//Set player's score to zero
			score = 0;

			base.Initialize ();
		}


		protected override void LoadContent ()
		{
			//TODO: use this.Content to load your game content here 

			// Load the player resources
			Animation playerAnimation = new Animation();
			Texture2D playerTexture = Content.Load<Texture2D>("Animation/shipAnimation");
			playerAnimation.Initialize(playerTexture, Vector2.Zero, 115, 69, 8, 30, Color.White, 1f, true);

			Vector2 playerPosition = new Vector2 (GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y
				+ GraphicsDevice.Viewport.TitleSafeArea.Height / 2);
			player.Initialize(playerAnimation, playerPosition);
			// Create a new SpriteBatch, which can be used to draw textures.
			spriteBatch = new SpriteBatch (GraphicsDevice);

			bgLayer1.Initialize(Content, "Textures/bgLayer1", GraphicsDevice.Viewport.Width, -1);
			bgLayer2.Initialize(Content, "Textures/bgLayer2", GraphicsDevice.Viewport.Width, -2);

			enemyTexture = Content.Load<Texture2D>("Animation/mineAnimation");



			mainBackground = Content.Load<Texture2D>("Texture/mainbackground");

			scatterShotTexture = Content.Load<Texture2D>("Texture/laser");

			beamTexture = Content.Load<Texture2D> ("Texture/laser");

			//textures for the explosions
			explosionTexture = Content.Load<Texture2D>("Animation/explosion");

			// Load the music
			gameplayMusic = Content.Load<Song>("Sound/gameMusic");

			// Load the laser and explosion sound effect
			laserSound = Content.Load<SoundEffect>("Sound/laserFire");
			explosionSound = Content.Load<SoundEffect>("Sound/explosion");

			// Start the music right away
			PlayMusic(gameplayMusic);

			// Load the score font
			font = Content.Load<SpriteFont>("Font/gameFont");

		}


		#region update Region
		private void UpdatePlayer(GameTime gameTime)
		{
			player.Update (gameTime);
			// Get Thumbstick Controls
			player.Position.X += currentGamePadState.ThumbSticks.Left.X *playerMoveSpeed;
			player.Position.Y -= currentGamePadState.ThumbSticks.Left.Y *playerMoveSpeed;

			// Use the Keyboard / Dpad
			if (currentKeyboardState.IsKeyDown(Keys.Left) ||
				currentGamePadState.DPad.Left == ButtonState.Pressed)
			{
				player.Position.X -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Right) ||
				currentGamePadState.DPad.Right == ButtonState.Pressed)
			{
				player.Position.X += playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Up) ||
				currentGamePadState.DPad.Up == ButtonState.Pressed)
			{
				player.Position.Y -= playerMoveSpeed;
			}
			if (currentKeyboardState.IsKeyDown(Keys.Down) ||
				currentGamePadState.DPad.Down == ButtonState.Pressed)
			{
				player.Position.Y += playerMoveSpeed;
			}
			#endregion
			// Make sure that the player does not go out of bounds
			player.Position.X = MathHelper.Clamp(player.Position.X, 0,GraphicsDevice.Viewport.Width - player.Width);
			player.Position.Y = MathHelper.Clamp(player.Position.Y, 0,GraphicsDevice.Viewport.Height - player.Height);

			// Fire only every interval we set as the fireTime
			if (gameTime.TotalGameTime - previousFireTime > fireTime)
			{
				// Reset our current time
				previousFireTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddProjectile(player.Position + new Vector2(player.Width / 2, 0));

				// Play the laser sound
				laserSound.Play();
			}

			// Fire our beam constantly and checking for a button press
			if (gameTime.TotalGameTime - previousShotTime > shotRate && currentKeyboardState.IsKeyDown(Keys.Space))
			{
				// Reset our current time
				previousShotTime = gameTime.TotalGameTime;

				// Add the projectile, but add it to the front and center of the player
				AddBeam(player.Position + new Vector2(player.Width / 2, 0));

				// Play the laser sound
				laserSound.Play();
			}



			// reset score if player health goes to zero
			if (player.Health <= 0)
			{
				player.Health = 100;
				score = 0;
			}

		}

		private void UpdateExplosions(GameTime gameTime)
		{
			for (int i = explosions.Count - 1; i >= 0; i--)
			{
				explosions[i].Update(gameTime);
				if (explosions[i].Active == false)
				{
					explosions.RemoveAt(i);
				}
			}
		}

		protected override void Update (GameTime gameTime)
		{

			// Update the parallaxing background
			bgLayer1.Update();
			bgLayer2.Update();

			// Update the enemies
			UpdateEnemies(gameTime);

			// Update the collision
			UpdateCollision();

			// Update the projectiles
			UpdateProjectiles();

			//update the beam
			UpdateShot();

			// Update the explosions
			UpdateExplosions(gameTime);

			//Update the player

			// For Mobile devices, this logic will close the Game when the Back button is pressed
			// Exit() is obsolete on iOS
			#if !__IOS__ &&  !__TVOS__
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState ().IsKeyDown (Keys.Escape))
				Exit ();
			#endif

			// TODO: Add your update logic here
			previousGamePadState = currentGamePadState;
			previousKeyboardState = currentKeyboardState;

			// Read the current state of the keyboard and gamepad and store it
			currentKeyboardState = Keyboard.GetState();
			currentGamePadState = GamePad.GetState(PlayerIndex.One);


			UpdatePlayer(gameTime);

			base.Update (gameTime);
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.DeepPink);

			//TODO: Add your drawing code here
			// Start drawing
			spriteBatch.Begin();

			spriteBatch.Draw(mainBackground, Vector2.Zero, Color.White);

			// Draw the moving background
			bgLayer1.Draw(spriteBatch);
			bgLayer2.Draw(spriteBatch);

			// Draw the Enemies
			for (int i = 0; i < enemies.Count; i++)
			{
				enemies[i].Draw(spriteBatch);
			}

			// Draw the Projectiles
			for (int i = 0; i < scatterShot.Count; i++)
			{
				scatterShot[i].Draw(spriteBatch);
			}

			// Draw the Projectiles
			for (int i = 0; i < rapidFire.Count; i++)
			{
				rapidFire[i].Draw(spriteBatch);
			}

			// Draw the Player
			player.Draw(spriteBatch);


			// Draw the explosions
			for (int i = 0; i < explosions.Count; i++)
			{
				explosions[i].Draw(spriteBatch);
			}

			// Draw the score
			spriteBatch.DrawString(font, "score: " + score, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y), Color.White);

			// Draw the player health
			spriteBatch.DrawString(font, "health: " + player.Health, new Vector2(GraphicsDevice.Viewport.TitleSafeArea.X, GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.White);

			base.Draw(gameTime);

			// Stop drawing
			spriteBatch.End();

		}

		private void AddEnemy()
		{ 
			// Create the animation object
			Animation enemyAnimation = new Animation();

			// Initialize the animation with the correct animation information
			enemyAnimation.Initialize(enemyTexture, Vector2.Zero, 47, 61, 8, 30,Color.White, 1f, true);

			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height -100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(enemyAnimation, position); 

			// Add the enemy to the active enemies list
			enemies.Add(enemy);
		}

		private void AddNewEnemy()
		{ 
			// Create the animation object
			Animation newEnemyAnimation = new Animation();

			// Initialize the animation with the correct animation information


			// Randomly generate the position of the enemy
			Vector2 position = new Vector2(GraphicsDevice.Viewport.Width +enemyTexture.Width / 2, random.Next(100, GraphicsDevice.Viewport.Height -100));

			// Create an enemy
			Enemy enemy = new Enemy();

			// Initialize the enemy
			enemy.Initialize(newEnemyAnimation, position); 

		}

		private void UpdateEnemies(GameTime gameTime)
		{
			// Spawn a new enemy enemy every 1.5 seconds
			if (gameTime.TotalGameTime - previousSpawnTime > enemySpawnTime) 
			{
				previousSpawnTime = gameTime.TotalGameTime;

				// Add an Enemy
				AddEnemy();
			}

			// Update the Enemies
			for (int i = enemies.Count - 1; i >= 0; i--) 
			{
				enemies[i].Update(gameTime);

				if (enemies[i].Active == false)
				{
					// If not active and health <= 0
					if (enemies[i].Health <= 0)
					{
						// Add an explosion
						AddExplosion(enemies[i].Position);

						//Add to the player's score
						score += 1;

						// Play the explosion sound
						explosionSound.Play();


					}

					enemies.RemoveAt(i);
				} 
			}
			// Update the collision
			UpdateCollision();
		}

		private void UpdateCollision()
		{
			// Use the Rectangle's built-in intersect function to 
			// determine if two objects are overlapping
			Rectangle rectangle1;
			Rectangle rectangle2;

			// Only create the rectangle once for the player
			rectangle1 = new Rectangle((int)player.Position.X,
				(int)player.Position.Y,
				player.Width,
				player.Height);

			// Do the collision between the player and the enemies
			for (int i = 0; i <enemies.Count; i++)
			{
				rectangle2 = new Rectangle((int)enemies[i].Position.X,
					(int)enemies[i].Position.Y,
					enemies[i].Width,
					enemies[i].Height);

				// Determine if the two objects collided with each
				// other
				if(rectangle1.Intersects(rectangle2))
				{
					// Subtract the health from the player based on
					// the enemy damage
					player.Health -= enemies[i].Damage;

					// Since the enemy collided with the player
					// destroy it
					enemies[i].Health = 0;

					// If the player health is less than zero we died
					if (player.Health <= 0)
						player.Active = false; 
				}

			}

			// Projectile vs Enemy Collision
			for (int i = 0; i < scatterShot.Count; i++)
			{
				for (int j = 0; j < enemies.Count; j++)
				{
					// Create the rectangles we need to determine if we collided with each other
					rectangle1 = new Rectangle((int)scatterShot[i].Position.X - 
						scatterShot[i].Width / 2,(int)scatterShot[i].Position.Y - 
						scatterShot[i].Height / 2,scatterShot[i].Width, scatterShot[i].Height);

					rectangle2 = new Rectangle((int)enemies[j].Position.X - enemies[j].Width / 2,
						(int)enemies[j].Position.Y - enemies[j].Height / 2,
						enemies[j].Width, enemies[j].Height);

					// Determine if the two objects collided with each other
					if (rectangle1.Intersects(rectangle2))
					{
						enemies[j].Health -= scatterShot[i].Damage;
						scatterShot[i].Active = false;
					}
				}
			}


		}

		private void UpdateProjectiles()
		{
			// Update the Projectiles
			for (int i = scatterShot.Count - 1; i >= 0; i--) 
			{
				scatterShot[i].Update();

				if (scatterShot[i].Active == false)
				{
					scatterShot.RemoveAt(i);
				} 
			}
		}

		private void UpdateShot()
		{
			// Update the Projectiles
			for (int i = rapidFire.Count - 1; i >= 0; i--) 
			{
				rapidFire[i].Update();

				if (rapidFire[i].Active == false)
				{
					rapidFire.RemoveAt(i);
				} 
			}
		}


		private void AddBeam(Vector2 position)
		{
			RapidFire beam = new RapidFire();
			beam.Initialize (GraphicsDevice.Viewport, beamTexture, position);
			rapidFire.Add(beam);
		}

		private void AddProjectile(Vector2 position)
		{
			Projectile projectile = new Projectile(); 
			projectile.Initialize(GraphicsDevice.Viewport, scatterShotTexture,position); 
			scatterShot.Add(projectile);
		}


		private void PlayMusic(Song song)
		{
			// Due to the way the MediaPlayer plays music,
			// we have to catch the exception. Music will play when the game is not tethered
			try
			{
				// Play the music
				MediaPlayer.Play(song);

				// Loop the currently playing song
				MediaPlayer.IsRepeating = true;
			}
			catch { }
		}
	}
}

