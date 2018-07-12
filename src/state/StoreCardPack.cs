using System;
using System.Collections.Generic;
namespace CardGame
{
	
	/// <summary>
	/// Колода-хранилище без ограничений по добавлению, удалению.
	/// </summary>
	public class StoreCardPack : CardPack
	{
		public StoreCardPack() : base()
		{
		}

		public StoreCardPack(List<int> cards) : base(cards, CardPack.DisplayMode.Hidden)
		{
		}

		public static StoreCardPack FullDeck()
		{
			List<int> cards = new List<int>();
			for (int i = 0; i < 52; i++)
			{
				cards.Add(i);
			}
			return new StoreCardPack(cards);
		}

		/// <summary>
		/// Перетасовывает стопку карт.
		/// </summary>
		/// <param name="rnd">Генератор рандомных чисел.</param>
		public void Shuffle(Random rnd)
		{
			int n = cards.Count;
			while (n > 1)
			{
				n--;
				int k = rnd.Next(n + 1);
				int c = cards[k];
				cards[k] = cards[n];
				cards[n] = c;
			}
		}

		public override bool CanAccept(CardPack cp)
		{
			return true;
		}

		public override bool CanGive(int count)
		{
			return count <= this.Count;
		}

		public override CardPack Copy()
		{
			StoreCardPack pack = new StoreCardPack(cards);
			return pack;
		}
	}
}
