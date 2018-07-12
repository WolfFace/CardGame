using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGame
{
	/// <summary>
	/// Сцена страницы об авторах.
	/// </summary>
	public class AboutScene : Scene
	{

		public Button BackBtn = new Button("backBtn", new Point(10, 660));

		SpriteFont font;

		public AboutScene(Microsoft.Xna.Framework.Content.ContentManager content) : base(content)
		{
			font = Content.Load<SpriteFont>("MenuFont");
		}

		public override void Update()
		{
			base.Update();
			BackBtn.Update();
		}

		public override void DrawScene(SpriteBatch spriteBatch)
		{
			base.DrawScene(spriteBatch);
			spriteBatch.Begin();
			spriteBatch.DrawString(font, "Разработчики: ", new Vector2(20, 20), Color.Black);
			spriteBatch.DrawString(font, "Николаевич Максим, ЭК-11", new Vector2(20, 120), Color.Black);
			spriteBatch.DrawString(font, "Клопот Александр, ЭК-11", new Vector2(20, 220), Color.Black);
			BackBtn.Draw(spriteBatch);
			spriteBatch.End();
		}

	}
}
