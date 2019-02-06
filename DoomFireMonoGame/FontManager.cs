using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DoomFireMonoGame
{
	public static class FontManager
	{
		public static SpriteFont Arial14 { get; private set; }

		public static void LoadContent(ContentManager content)
		{
			Arial14 = content.Load<SpriteFont>("Arial");
		}
	}
}