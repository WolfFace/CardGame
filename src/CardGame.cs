using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace CardGame
{
	/// <summary>
	/// Главный класс игры.
	/// Рисует ту сцену, которая активна.
	/// Декларирует переход от одной сцены к другой.
	/// </summary>
	public class CardGame : Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;
		//Song song;

		public Scene currentScene;

		public CardGame()
		{
			graphics = new GraphicsDeviceManager(this);
			Content.RootDirectory = "Content";
		}

		/// <summary>
		/// Инициализация, загрузка неграфических ресурсов.
		/// </summary>
		protected override void Initialize()
		{
			this.IsMouseVisible = true;
			this.graphics.PreferredBackBufferWidth = 700;
			this.graphics.PreferredBackBufferHeight = 720;
			this.graphics.ApplyChanges();
			base.Initialize();

			//song = Content.Load<Song>("sound/soundtrack");
			// рАЗДРАЖАЕТ
			// MediaPlayer.Play(song);
			MediaPlayer.IsRepeating = true;
		}

		/// <summary>
		/// Загрузка ресурсов.
		/// </summary>
		protected override void LoadContent()
		{
			// Создали SpriteBatch, который может быть использован для рисования текстур.
			spriteBatch = new SpriteBatch(GraphicsDevice);
			CardResources.Instance.Init(this.Content);
			SceneToMainMenu();
		}

		public void SceneToMainMenu()
		{
			MainMenuScene scene = new MainMenuScene(Content);
			scene.StartGameBtn.OnClick = () =>
			{
				GameScene gameScene = new GameScene(Content);
				currentScene = gameScene;
				gameScene.BackBtn.OnClick = () =>
				{
					if (gameScene.Score != 0) SceneToSaveRecord(gameScene.Score);
					else SceneToMainMenu();
				};
				gameScene.OnWin = () =>
				{
					currentScene = new WinScene(Content);
					(currentScene as WinScene).BackBtn.OnClick = () =>
					{
						SceneToSaveRecord(gameScene.Score);
					};
				};
			};
			scene.RecordsBtn.OnClick = SceneToRecords;
			scene.RulesBtn.OnClick = () =>
			{
				currentScene = new RulesScene(Content);
				(currentScene as RulesScene).BackBtn.OnClick = SceneToMainMenu;
			};
			scene.AboutBtn.OnClick = () =>
			{
				currentScene = new AboutScene(Content);
				(currentScene as AboutScene).BackBtn.OnClick = SceneToMainMenu;
			};
			scene.ExitBtn.OnClick = () =>
			{
				this.Exit();
			};
			currentScene = scene;
		}

		public void SceneToRecords()
		{
			currentScene = new RecordListScene(Content);
			(currentScene as RecordListScene).BackBtn.OnClick = SceneToMainMenu;
		}

		public void SceneToSaveRecord(int score)
		{
			SaveRecordScene saveRecordScene = new SaveRecordScene(score, Content);
			currentScene = saveRecordScene;
			saveRecordScene.OnSaved = SceneToRecords;
		}

		/// <summary>
		/// Обновляем мир тут.
		/// </summary>
		/// <param name="gameTime">Временная информация игры.</param>
		protected override void Update(GameTime gameTime)
		{
#if !__IOS__ && !__TVOS__
			if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed
				|| Keyboard.GetState().IsKeyDown(Keys.Escape))
				Exit();
#endif
			currentScene.Update();
			base.Update(gameTime);
		}

		/// <summary>
		/// Вызывается, когда игра решает себя перерисовать.
		/// </summary>
		/// <param name="gameTime">Временная информация игры.</param>
		protected override void Draw(GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear(Color.White);

			//TODO: Добавить рендеринг чего-либо.
			currentScene.DrawScene(spriteBatch);

			base.Draw(gameTime);
		}
	}
}
