using System;
using System.Timers;
using System.Linq;
using System.Collections.Generic;
using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGame
{
	public class SaveRecordScene : Scene
	{
		public const String RECORDS_FILE = "records";

		public Action OnSaved = () => { };
		Button continueBtn = new Button("continueBtn", new Point(100, 300));

		SpriteFont font, fontSmaller;

		int score;

		public String userName = "";
		bool postName = true;

		Timer fieldTimer = new Timer();

		KeyboardState prevKeyState;

		public SaveRecordScene(int score, Microsoft.Xna.Framework.Content.ContentManager content) : base(content)
		{
			font = Content.Load<SpriteFont>("MenuFont");
			fontSmaller = Content.Load<SpriteFont>("MenuFontSmaller");
			this.score = score;

			fieldTimer = new Timer();
			fieldTimer.Interval = 500;
			fieldTimer.Elapsed += FieldTimerTick;
			fieldTimer.Start();

			prevKeyState = Keyboard.GetState();

			continueBtn.OnClick = Save;
		}

		private void Save()
		{
			String nameToWrite = userName.Length > 0 ? userName.Trim() : "ANONYMOUS";
			if (File.Exists(RECORDS_FILE))
			{
				List<String> lines = File.ReadAllLines(RECORDS_FILE).Where(i => i.Length>0).ToList();
				lines.Add(nameToWrite + " " + score);
				File.WriteAllLines(RECORDS_FILE, lines.ToArray());
			}
			else
			{
				File.WriteAllText(RECORDS_FILE, nameToWrite + " " + score);
			}
			OnSaved();
		}

		public void FieldTimerTick(object sender, EventArgs e)
		{
			postName = !postName;
		}

		public override void Update()
		{
			base.Update();
			KeyboardUpdate();
			continueBtn.Update();
		}

		public void KeyboardUpdate()
		{
			KeyboardState state = Keyboard.GetState();
			foreach (Keys key in state.GetPressedKeys())
			{
				if (key.ToString().Length == 1 
				    && !prevKeyState.GetPressedKeys().Contains(key)
				    && userName.Length<18)
				{
					userName += key.ToString();
				}
				if (key == Keys.Space 
				    && !prevKeyState.IsKeyDown(Keys.Space) 
				    && userName.Length < 18)
				{
					userName += " ";
				}
				if (key == Keys.Back && !prevKeyState.IsKeyDown(Keys.Back))
				{
					userName = userName.Length>0 
					                   ? userName.Substring(0, userName.Length-1) 
					                   : userName;
				}
				if (key == Keys.Enter && !prevKeyState.IsKeyDown(Keys.Enter))
				{
					Save();
				}
			}
			prevKeyState = state;
		}

		public override void DrawScene(SpriteBatch spriteBatch)
		{
			base.DrawScene(spriteBatch);
			spriteBatch.Begin();
			spriteBatch.Draw(CardResources.Instance.UiTexture("menuBackground"), new Vector2(0, 396), Color.White);
			spriteBatch.DrawString(font, "Ваш результат: "+score, new Vector2(20, 20), Color.Black);
			spriteBatch.DrawString(fontSmaller, 
			                       "Вбейте свое имя, чтобы\nзаписать себя в историю рекордов", 
			                       new Vector2(20, 100), Color.Black);

			spriteBatch.DrawString(font, userName+(postName ? "_" : ""), new Vector2(20, 200), Color.Black);

			continueBtn.Draw(spriteBatch);

			spriteBatch.End();
		}

	}
}
