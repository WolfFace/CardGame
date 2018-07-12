using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGame
{
	/// <summary>
	/// Сцена главного меню игры.
	/// </summary>
	public class MainMenuScene : Scene
	{

		public Button StartGameBtn = new Button("startGameBtn", new Point(65, 20));
		public Button RecordsBtn = new Button("recordsBtn", new Point(85, 100));
		public Button RulesBtn = new Button("rulesBtn", new Point(65, 180));
		public Button AboutBtn = new Button("aboutBtn", new Point(65, 260));
		public Button ExitBtn = new Button("exitBtn", new Point(65, 330));

		public MainMenuScene(Microsoft.Xna.Framework.Content.ContentManager content) : base(content)
		{
		}

		public override void Update()
		{
			base.Update();
			StartGameBtn.Update();
			RecordsBtn.Update();
			RulesBtn.Update();
			AboutBtn.Update();
			ExitBtn.Update();
		}

		public override void DrawScene(SpriteBatch spriteBatch)
		{
			base.DrawScene(spriteBatch);
			spriteBatch.Begin();
			spriteBatch.Draw(CardResources.Instance.UiTexture("menuBackground"), new Vector2(0, 396), Color.White);
			StartGameBtn.Draw(spriteBatch);
			RecordsBtn.Draw(spriteBatch);
			RulesBtn.Draw(spriteBatch);
			AboutBtn.Draw(spriteBatch);
			ExitBtn.Draw(spriteBatch);
			spriteBatch.End();
		}

	}
}
