using System;
using System.Collections.Generic;

namespace CardGame
{
	/// <summary>
	/// Вспомогательный класс для операций с картами, представленных в виде целых чисел.
	/// </summary>
	public class Card
	{

		/// <summary>
		/// Находит масть карты в диапазоне [0, ...].
		/// </summary>
		public static int Suit(int card)
		{
			return card / 13;
		}

		/// <summary>
		/// Находит достоинство карты в диапазоне [0, 12].
		/// </summary>
		public static int Dignity(int card)
		{
			return card % 13;
		}

		public enum Color { Red, Black };
		public static Color CardColor(int card)
		{
			return Suit(card) == 0 || Suit(card) == 3 ? Color.Black : Color.Red;
		}
	}
	
	/// <summary>
	/// Абстракция. Упорядоченное множество карт, способное принимать в свой состав другие множества карт,
	/// согласно правилам конкретной реализации. Имеет место также и извлечение подмножества.
	/// Используется со стороны ui, связываясь с прямоугольной областью игрового поля.
	/// </summary>
	abstract public class CardPack
	{

		/// <summary>
		/// Карты хранятся в числовом виде.
		/// Масть карты - [c/13] - [0, 3]
		/// Достоинство - c%13 - [0, 12]
		/// </summary>
		protected List<int> cards;

		/// <summary>
		/// Ожидаемый от ui режим отображения.
		/// </summary>
		public enum DisplayMode { Full, OnlyLast, Hidden };
		public DisplayMode displayMode;

		public delegate void OnActivateDelegate();
		public OnActivateDelegate OnActivate = () => {};

		protected CardPack()
		{
			this.cards = new List<int>();
			displayMode = DisplayMode.Full;
		}

		protected CardPack(List<int> cards, DisplayMode dm)
		{
			this.cards = new List<int>(cards);
			this.displayMode = dm;
		}

		public int this[int i]
		{
			get { return cards[i]; }
		}

		public int Count
		{
			get { return cards.Count; }
		}
		/// <summary>
		/// WARNING: Only for debug!!!
		/// </summary>
		public void ForceAdd(int card)
		{
			cards.Add(card);
		}

		public bool AddCards(CardPack cp)
		{
			if (cp == null) return false;

			if (!CanAccept(cp))
			{
				return false;
			}

			cards.AddRange(cp.cards);
			return true;
		}

		public bool RemoveLastCards(int count)
		{
			cards.RemoveRange(cards.Count - count, count);
			return true;
		}

		public void RemoveAllCards()
		{
			cards = new List<int>();
		}

		public CardPack FirstCards(int count)
		{
			CardPack cp = this.Copy();
			if (count <= Count) cp.cards = cards.GetRange(0, count);
			return cp;
		}

		public CardPack LastCards(int count)
		{
			CardPack copy = this.Copy();
			copy.cards = cards.GetRange(cards.Count - count, count);
			return copy;
		}

		public int FirstCard
		{
			get { return cards.Count>0 ? cards[0] : -1; }
		}

		public int LastCard
		{
			get { return cards.Count>0 ? cards[cards.Count - 1] : -1; }
		}

		/// <summary>
		/// Копирует стопку карт.
		/// </summary>
		/// <returns>Новый экземпляр (копия).</returns>
		abstract public CardPack Copy();

		/// <summary>
		/// Определяет может ли данная стопка карт принять другую. Для задания правил.
		/// </summary>
		/// <returns><c>true</c>, если может принять, <c>false</c> иначе.</returns>
		/// <param name="cp">Стопка для принятия.</param>
		abstract public bool CanAccept(CardPack cp);

		/// <summary>
		/// Определяет может ли данная стопка карт отдать count карт. Для задания правил.
		/// </summary>
		/// <returns><c>true</c>, если может отдать, <c>false</c> иначе.</returns>
		/// <param name="count">Количество карт для отдачи.</param>
		abstract public bool CanGive(int count);

	}

}
