using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGame
{
	/// <summary>
	/// Сцена страницы об авторах.
	/// </summary>
	public class RecordListScene : Scene
	{

		public Button BackBtn = new Button("backBtn", new Point(10, 660));
		private List<Tuple<String, int>> records = new List<Tuple<String, int>>();

		SpriteFont font, fontSmaller;

		public RecordListScene(Microsoft.Xna.Framework.Content.ContentManager content) : base(content)
		{

			font = Content.Load<SpriteFont>("MenuFont");
			fontSmaller = Content.Load<SpriteFont>("MenuFontSmaller");

			try
			{
				String[] ss = File.ReadAllLines(SaveRecordScene.RECORDS_FILE).Where(i => i.Length>0).ToArray();
				foreach (String recordString in ss)
				{
					String[] recordSplitted = recordString.Split(' ');
					String scoreString = recordSplitted[recordSplitted.Length - 1];
					int score = Int32.Parse(scoreString);
					string name = recordString.Substring(0, recordString.Length-scoreString.Length);
					records.Add(Tuple.Create(name, score));
				}
			}
			catch (Exception)
			{
				records.Clear();
			}
			records.Sort((x, y) => y.Item2.CompareTo(x.Item2));
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
			spriteBatch.DrawString(font, "Доска лучших: ", new Vector2(20, 20), Color.Black);

			for (int i = 0; i < records.Count && i < 14; i++)
			{
				spriteBatch.DrawString(fontSmaller, i+1+".", new Vector2(20, 80+40*i), Color.Black);
				spriteBatch.DrawString(fontSmaller, records[i].Item1, new Vector2(70, 80 + 40 * i), Color.Black);
				spriteBatch.DrawString(fontSmaller, ""+records[i].Item2, new Vector2(500, 80 + 40 * i), Color.Black);
			}

			BackBtn.Draw(spriteBatch);
			spriteBatch.End();
		}

	}
}
