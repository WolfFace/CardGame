using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGame
{
	/// <summary>
	/// Сцена страницы об авторах.
	/// </summary>
	public class RulesScene : Scene
	{

		public Button BackBtn = new Button("backBtn", new Point(10, 660));

		public RulesScene(Microsoft.Xna.Framework.Content.ContentManager content) : base(content)
		{
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
			spriteBatch.Draw(CardResources.Instance.UiTexture("rulesBackground"), new Vector2(0, 0), Color.White);
			BackBtn.Draw(spriteBatch);
			spriteBatch.End();
		}

	}
}
