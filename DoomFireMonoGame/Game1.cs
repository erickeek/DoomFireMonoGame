using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace DoomFireMonoGame
{
	public class Game1 : Game
	{
		private const int PixelSize = 5;

		private static Texture2D _pixel;

		private readonly Random _random = new Random();
		private readonly Color[] _fireColorsPalette = { new Color(7, 7, 7), new Color(31, 7, 7), new Color(47, 15, 7), new Color(71, 15, 7), new Color(87, 23, 7), new Color(103, 31, 7), new Color(119, 31, 7), new Color(143, 39, 7), new Color(159, 47, 7), new Color(175, 63, 7), new Color(191, 71, 7), new Color(199, 71, 7), new Color(223, 79, 7), new Color(223, 87, 7), new Color(223, 87, 7), new Color(215, 95, 7), new Color(215, 95, 7), new Color(215, 103, 15), new Color(207, 111, 15), new Color(207, 119, 15), new Color(207, 127, 15), new Color(207, 135, 23), new Color(199, 135, 23), new Color(199, 143, 23), new Color(199, 151, 31), new Color(191, 159, 31), new Color(191, 159, 31), new Color(191, 167, 39), new Color(191, 167, 39), new Color(191, 175, 47), new Color(183, 175, 47), new Color(183, 183, 47), new Color(183, 183, 55), new Color(207, 207, 111), new Color(223, 223, 159), new Color(239, 239, 199), new Color(255, 255, 255) };

		private readonly GraphicsDeviceManager _graphics;
		private SpriteBatch _spriteBatch;
		private Rectangle _tracedSize;
		private int[] _pixels;
		private int _numberOfPixels;
		private WindDirection _windDirection;

		public Game1()
		{
			_graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		protected override void Initialize()
		{
			var windowSize = GraphicsDevice.PresentationParameters.Bounds;

			_tracedSize = new Rectangle { Width = windowSize.Width / PixelSize, Height = windowSize.Height / PixelSize };
			_numberOfPixels = _tracedSize.Width * _tracedSize.Height;
			_pixels = new int[_numberOfPixels];

			_windDirection = WindDirection.Right;

			CreateFireDataStructure();
			CreateFireSource();

			base.Initialize();
		}

		protected override void LoadContent()
		{
			_spriteBatch = new SpriteBatch(GraphicsDevice);
			FontManager.LoadContent(Content);
		}

		protected override void Update(GameTime gameTime)
		{
			InputManager.Update();

			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();

			if (InputManager.OnKeyUp(Keys.N))
				_windDirection = WindDirection.None;
			else if (InputManager.OnKeyUp(Keys.Left))
				_windDirection = WindDirection.Left;
			else if (InputManager.OnKeyUp(Keys.Right))
				_windDirection = WindDirection.Right;
			else if (InputManager.OnKeyUp(Keys.Up))
				IncreaseFireSource();
			else if (InputManager.OnKeyUp(Keys.Down))
				DecreaseFireSource();

			CalculateFirePropagation();

			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime)
		{
			GraphicsDevice.Clear(Color.CornflowerBlue);

			RenderFire();

			_spriteBatch.Begin();
			_spriteBatch.DrawString(FontManager.Arial14, "PRESS 'N' TO NONE WIND\n'LEFT' OR 'RIGHT' TO CHANGE THE WIND DIRETION\n'UP' OR 'DOWN'		TO INCREASE AND DEACRESE", new Vector2(20, 20), Color.White);
			_spriteBatch.End();

			base.Draw(gameTime);
		}

		private void RenderFire()
		{
			_spriteBatch.Begin();

			for (var y = 0; y < _tracedSize.Height; y++)
			{
				for (var x = 0; x < _tracedSize.Width; x++)
				{
					var pixelIndex = x + (_tracedSize.Width * y);
					var fireIntensity = _pixels[pixelIndex];
					var color = _fireColorsPalette[fireIntensity];
					DrawRectangle(new Rectangle(x * PixelSize, y * PixelSize, PixelSize, PixelSize), color);
				}
			}

			_spriteBatch.End();
		}

		private void CreateFireDataStructure()
		{
			for (var i = 0; i < _numberOfPixels; i++)
			{
				_pixels[i] = 0;
			}
		}

		private void CreateFireSource()
		{
			for (var column = 0; column < _tracedSize.Width; column++)
			{
				var pixelIndex = (_numberOfPixels - _tracedSize.Width) + column;

				_pixels[pixelIndex] = 36;
			}
		}

		private void CalculateFirePropagation()
		{
			for (var i = 0; i < _numberOfPixels; i++)
			{
				UpdateFireIntensityPerPixel(i);
			}
		}

		private void UpdateFireIntensityPerPixel(int currentPixelIndex)
		{
			var belowPixelIndex = currentPixelIndex + _tracedSize.Width;

			if (belowPixelIndex >= _tracedSize.Width * _tracedSize.Height)
				return;

			var decay = _random.Next(0, 3);
			var belowPixelFireIntensity = _pixels[belowPixelIndex];
			var newFireIntensity = Math.Max(belowPixelFireIntensity - decay, 0);

			switch (_windDirection)
			{
				case WindDirection.None:
					_pixels[currentPixelIndex] = newFireIntensity;
					break;
				case WindDirection.Left:
					_pixels[Math.Max(currentPixelIndex - decay, 0)] = newFireIntensity;
					break;
				case WindDirection.Right:
					_pixels[currentPixelIndex + decay] = newFireIntensity;
					break;
			}
		}

		private void IncreaseFireSource()
		{
			for (var column = 0; column < _tracedSize.Width; column++)
			{
				var pixelIndex = (_numberOfPixels - _tracedSize.Width) + column;
				var currentFireIntensity = _pixels[pixelIndex];

				if (currentFireIntensity >= 36) continue;

				var increase = _random.Next(0, 14);
				var newFireIntensity = Math.Min(currentFireIntensity + increase, 36);
				_pixels[pixelIndex] = newFireIntensity;
			}
		}

		private void DecreaseFireSource()
		{
			for (var column = 0; column < _tracedSize.Width; column++)
			{
				var pixelIndex = (_numberOfPixels - _tracedSize.Width) + column;
				var currentFireIntensity = _pixels[pixelIndex];

				if (currentFireIntensity <= 0) continue;

				var decay = _random.Next(0, 14);
				var newFireIntensity = Math.Max(currentFireIntensity - decay, 0);
				_pixels[pixelIndex] = newFireIntensity;
			}
		}

		private void DrawRectangle(Rectangle coords, Color color)
		{
			if (_pixel == null)
			{
				_pixel = new Texture2D(_graphics.GraphicsDevice, 1, 1);
				_pixel.SetData(new[] { Color.White });
			}
			_spriteBatch.Draw(_pixel, coords, color);
		}
	}
}