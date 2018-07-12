using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Media;

namespace CardGame
{
	/// <summary>
	/// Синглетон. Предоставляет доступ к ресурсам с любой точки приложения.
	/// Требует предварительной инициализации.
	/// </summary>
	public class CardResources
	{

		private Texture2D[] cardTextures;
		public Texture2D Cover;
		public Texture2D Blank;
		public Texture2D DeskBackground;
		public ContentManager contentManager;

		public void Init(ContentManager manager)
		{
			contentManager = manager;
			// Загружаем лицевые текстурки карт.
			cardTextures = new Texture2D[52];
			for (int i = 1; i <= 52; i++)
			{
				cardTextures[i - 1] = manager.Load<Texture2D>("cards/" + i);
			}

			Cover = manager.Load<Texture2D>("cards/cover");
			Blank = manager.Load<Texture2D>("cards/blank");
			DeskBackground = manager.Load<Texture2D>("cards/background");
		}

		public Texture2D CardTextureByNum(int num)
		{
			return cardTextures[num];
		}

		public Texture2D UiTexture(String id)
		{
			return contentManager.Load<Texture2D>("ui/"+id);
		}

		public Song Sound(String id)
		{
			return contentManager.Load<Song>("sound/"+id);
		}

		private static CardResources instance = null;
		public static CardResources Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new CardResources();
				}
				return instance;
			}
		}

	}
	
}
