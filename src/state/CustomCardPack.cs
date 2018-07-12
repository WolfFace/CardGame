using System;
using System.Collections.Generic;

namespace CardGame
{

	/// <summary>
	/// Реализация CardPack на делегатах.
	/// </summary>
	public class CustomCardPack : CardPack
	{

		public delegate bool CanAcceptDelegate(CardPack cp);
		public delegate bool CanGiveDelegate(int count);

		public CanAcceptDelegate canAccept = cp => true;
		public CanGiveDelegate canGive = count => true;

		public CustomCardPack() : base()
		{}

		public CustomCardPack(List<int> cards, DisplayMode dm) : base(cards, dm)
		{}

		public override bool CanAccept(CardPack cp)
		{
			return canAccept(cp);
		}

		public override bool CanGive(int count)
		{
			return canGive(count);
		}

		public override CardPack Copy()
		{
			var pack = new CustomCardPack(this.cards, this.displayMode);
			pack.canAccept = this.canAccept;
			pack.canGive = this.canGive;
			return pack;
		}
	}
}
