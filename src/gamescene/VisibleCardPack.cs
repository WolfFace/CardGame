using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGame
{

	/// <summary>
	/// Привязка колоды карт к пространству и графике.
	/// </summary>
	public class VisibleCardPack
	{
		public const int CARD_WIDTH = 70;
		public const int CARD_HEIGHT = 94;
		public const int CARD_IMPOSE_HEIGHT = 20;

		public CardPack pack;
		public Point point;
		public bool Light = false;
		public int InvisibleCards = 0;

		public bool isMouseTarget = false;
		public Point targetPoint;

		public VisibleCardPack(CardPack p, Point pt)
		{
			pack = p;
			point = pt;
		}

		public void Move(Point p, bool absolute)
		{
			point = p;
		}

		public void SetAsTarget(Point p)
		{
			isMouseTarget = true;
			targetPoint = p;
		}

		public void SetAsNotTarget()
		{
			isMouseTarget = false;
		}

		public void Activate()
		{
			pack.OnActivate();
		}

		// TODO: Переписать!
		public CardPack Gap(Point p)
		{
			if (pack.Count == 0)
			{
				return null;
			}
			CardPack gapPack;
			if (pack.displayMode == CardPack.DisplayMode.Full)
			{
				int a = (int)Math.Floor((double)(p.Y - point.Y) / CARD_IMPOSE_HEIGHT);
				a = a < pack.Count ? pack.Count - a : 1;
				if (!pack.CanGive(a)) return null; 
				gapPack = pack.LastCards(a);
				pack.RemoveLastCards(a);
				return gapPack;
			}

			if (!pack.CanGive(1)) return null;
			gapPack = pack.LastCards(1);
			pack.RemoveLastCards(1);
			return gapPack;
		}

		public int GapCount(Point p)
		{
			if (pack.Count == 0)
			{
				return -1;
			}
			if (pack.displayMode == CardPack.DisplayMode.Full)
			{
				int a = (int)Math.Floor((double)(p.Y - point.Y) / CARD_IMPOSE_HEIGHT);
				a = a < pack.Count ? pack.Count - a : 1;
				if (!pack.CanGive(a)) return -1;
				return a;
			}

			if (!pack.CanGive(1)) return -1;
			return 1;
		}

		public Point GapLeftTopPoint(Point p)
		{
			if (pack.displayMode != CardPack.DisplayMode.Full)
			{
				return point;
			}

			int a = (p.Y - point.Y) / CARD_IMPOSE_HEIGHT;
			int y = a < pack.Count 
			                ? point.Y + a * CARD_IMPOSE_HEIGHT
			                : point.Y + pack.Count * CARD_IMPOSE_HEIGHT;
			return new Point(point.X, y);
		}

		public Rectangle GetFirstCardRectangle()
		{
			return new Rectangle(point.X, point.Y, CARD_WIDTH, CARD_HEIGHT);
		}

		public Rectangle GetRectangle()
		{
			if (pack.Count == 0)
			{
				return GetFirstCardRectangle();
			}

			if (pack.displayMode == CardPack.DisplayMode.Full)
			{
				int h = CARD_HEIGHT + CARD_IMPOSE_HEIGHT * (pack.Count - 1);
				return new Rectangle(point.X, point.Y, CARD_WIDTH, h);
			}
			return GetFirstCardRectangle();
		}

		public void Draw(SpriteBatch spriteBatch, Point offset)
		{
			CardPack toDraw = pack.FirstCards(pack.Count-InvisibleCards);
			if (toDraw.Count == 0)
			{
				spriteBatch.Draw(CardResources.Instance.Blank, GetFirstCardRectangle(), Color.White);
				return;
			}

			if (pack.displayMode == CardPack.DisplayMode.Full)
			{
				Rectangle cardRect = GetFirstCardRectangle();
				for (int i = 0; i < toDraw.Count; i++)
				{
					Rectangle rect = new Rectangle(
						cardRect.X + offset.X,
						cardRect.Y + offset.Y + CARD_IMPOSE_HEIGHT * i,
						cardRect.Width,
						cardRect.Height
					);
					Texture2D texture = CardResources.Instance.CardTextureByNum(toDraw[i]);
					spriteBatch.Draw(texture, OffsetRectangle(rect, offset), Color.White);
				}
			}
			else if (pack.displayMode == CardPack.DisplayMode.OnlyLast)
			{
				Texture2D texture = CardResources.Instance.CardTextureByNum(toDraw[toDraw.Count - 1]);
				spriteBatch.Draw(texture, OffsetRectangle(GetRectangle(), offset), Color.White);
			}
			else if (pack.displayMode == CardPack.DisplayMode.Hidden)
			{
				spriteBatch.Draw(CardResources.Instance.Cover, OffsetRectangle(GetRectangle(), offset), Color.White);
			}
		}

		private static Rectangle OffsetRectangle(Rectangle rect, Point offset)
		{
			return new Rectangle(
				rect.X + offset.X,
				rect.Y + offset.Y,
				rect.Width, rect.Height
			);
		}

	}
	
}
