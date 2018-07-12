using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace CardGame
{
	
	/// <summary>
	/// Данный класс является главным классом всей бизнес логики игры.
	/// Представляет собой хранилище состояния игры и предоставляет интерфейс для управления
	/// игровым процессом со стороны ui.
	/// Будет полем GameScene.
	/// </summary>
	public class State
	{
		private const int BACK_STEP_SCORE = -5;

		private Dictionary<String, CardPack> cardPackTable;
		private HashSet<Tuple<CardPack, CardPack>> disabledMoves;
		private List<Tuple<CardPack, CardPack, int>> movesHistory = new List<Tuple<CardPack, CardPack, int>>();
		public Action OnWin = () => {};
		private int gameTimeScore = 0;
		private int steps = 0;

		/// <summary>
		/// Инициализация нового экземляра состояния логики игры.
		/// </summary>
		public State()
		{
			cardPackTable = new Dictionary<String, CardPack>();
			disabledMoves = new HashSet<Tuple<CardPack, CardPack>>();

			// Полная колода
			StoreCardPack deck = StoreCardPack.FullDeck();
			Random rnd = new Random();
			deck.Shuffle(rnd);
			cardPackTable.Add("deck", deck);

			// "Секретная" колода
			cardPackTable.Add("secret", CreateSecretPack());

			// Базовые колоды
			cardPackTable.Add("base1", CreateBasePack());
			cardPackTable.Add("base2", CreateBasePack());
			cardPackTable.Add("base3", CreateBasePack());
			cardPackTable.Add("base4", CreateBasePack());

			// Игровые колоды
			cardPackTable.Add("game1", CreateGamePack());
			cardPackTable.Add("game2", CreateGamePack());
			cardPackTable.Add("game3", CreateGamePack());
			cardPackTable.Add("game4", CreateGamePack());

			// Закрытая колода
			cardPackTable.Add("closed", CreateClosedPack());

			// Открытая рядом с закрытой
			cardPackTable.Add("openedClosed", CreateOpenedClosedPack());

			// Расскидываем карты
			ScatterCards();

			InitLimits();

			// Количество совершенных пользователем шагов
			steps = 0;
		}

		private void ScatterCards()
		{
			ConfirmMove(cardPackTable["deck"], cardPackTable["secret"], 13);
			ConfirmMove(cardPackTable["deck"], cardPackTable["game1"], 1);
			ConfirmMove(cardPackTable["deck"], cardPackTable["game2"], 1);
			ConfirmMove(cardPackTable["deck"], cardPackTable["game3"], 1);
			ConfirmMove(cardPackTable["deck"], cardPackTable["game4"], 1);
			ConfirmMove(cardPackTable["deck"], cardPackTable["closed"], 52-18);
			ConfirmMove(cardPackTable["deck"], cardPackTable["openedClosed"], 1);
			movesHistory.Clear();
		}

		private void InitLimits()
		{
			foreach (KeyValuePair<String,CardPack> entry in cardPackTable)
			{
				if (entry.Key != "openedClosed") disabledMoves.Add(Tuple.Create(entry.Value, cardPackTable["closed"]));
				if (entry.Key != "closed") disabledMoves.Add(Tuple.Create(entry.Value, cardPackTable["openedClosed"]));
				disabledMoves.Add(Tuple.Create(entry.Value, cardPackTable["secret"]));
			}
		}

		/// <summary>
		/// Совершает перемещение карт с одной колоды на другую. 
		/// Желательно проводить все операции с колодами через этот метод.
		/// </summary>
		/// <returns><c>true</c>, перемещение возможно и совершено, <c>false</c> в ином случае.</returns>
		/// <param name="from">С какой колоды.</param>
		/// <param name="to">На какую.</param>
		/// <param name="count">Количество карт.</param>
		public bool ConfirmMove(CardPack from, CardPack to, int count)
		{
			if (!disabledMoves.Contains(Tuple.Create(from, to))
			    && from.CanGive(count)
			    && to.CanAccept(from.LastCards(count)))
			{
				to.AddCards(from.LastCards(count));
				from.RemoveLastCards(count);
				OnMoveConfirmed(from, to, count);
				movesHistory.Add(Tuple.Create(from, to, count));
				return true;
			}
			return false;
		}

		/// <summary>
		/// Вызывается всякий раз, когда совершено успешное перемещение.
		/// </summary>
		/// <param name="from">С какой стопки.</param>
		/// <param name="to">На какую.</param>
		/// <param name="count">Сколько карт.</param>
		private void OnMoveConfirmed(CardPack from, CardPack to, int count)
		{
			// Перемещена карты с открытой колоды.
			if (from == cardPackTable["openedClosed"])
			{
				// Перемещаем на открытую колоду одну карту с закрытой.
				ConfirmMove(cardPackTable["closed"], cardPackTable["openedClosed"], 1);
			}

			// Перемещение с игрового поля.
			// В случае, если карт не осталось в игровом поле - блокировать перемещение на нее
			// с открытой колоды
			if (CardPackId(from).StartsWith("game", StringComparison.OrdinalIgnoreCase)
			    && from.Count == 0)
			{
				foreach (KeyValuePair<String,CardPack> kv in cardPackTable)
				{
					if (kv.Key != "secret" && !kv.Key.StartsWith("game", StringComparison.Ordinal))
					{
						disabledMoves.Add(Tuple.Create(kv.Value, from));
					}
				}
			}

			// Разрешаем добавление карт на игровое поле с открытой колоды.
			if (CardPackId(to).StartsWith("game", StringComparison.Ordinal))
			{
				disabledMoves.RemoveWhere(i => i.Item2 == to);
			}

			// Проверяем выиграл ли игрок, при перемещении на базовую колоду.
			if (CardPackId(to).StartsWith("base", StringComparison.Ordinal))
			{
				CheckIfWin();
			}

			steps++;
		}

		/// <summary>
		/// Проверяет выиграл ли игрок, вызывает делегат OnWin в случае успеха.
		/// </summary>
		public void CheckIfWin()
		{
			if (cardPackTable["base1"].Count == 13
				&& cardPackTable["base2"].Count == 13
				&& cardPackTable["base3"].Count == 13
				&& cardPackTable["base4"].Count == 13)
			{
				OnWin();
			}
		}

		public void BackStep()
		{
			if (movesHistory.Count < 1) return;
			Tuple<CardPack, CardPack, int> move = movesHistory[movesHistory.Count - 1];
			move.Item1.AddCards(move.Item2.LastCards(move.Item3));
			move.Item2.RemoveLastCards(move.Item3);
			movesHistory.RemoveAt(movesHistory.Count - 1);
			gameTimeScore -= BACK_STEP_SCORE;
		}

		public int Score
		{
			get
			{
				return cardPackTable["base1"].Count
									   + cardPackTable["base2"].Count
									   + cardPackTable["base3"].Count
									   + cardPackTable["base4"].Count
									   + gameTimeScore;
			}
		}

		public CardPack CardPackById(String id)
		{
			return cardPackTable[id];
		}

		public String CardPackId(CardPack cp)
		{
			return cardPackTable.FirstOrDefault(i => i.Value == cp).Key;
		}

		public CardPack CreateBasePack()
		{
			CustomCardPack pack = new CustomCardPack();

			pack.displayMode = CardPack.DisplayMode.OnlyLast;

			pack.canAccept = (cp) =>
			{
				if (pack.Count == 0) return cp.Count == 1 && Card.Dignity(cp.FirstCard) == 0;
				return cp.Count == 1 
					     && Card.Suit(cp.FirstCard) == Card.Suit(pack.LastCard)
					     && Card.Dignity(cp.FirstCard)-Card.Dignity(pack.LastCard)==1;
			};

			pack.canGive = (count) =>
			{
				return false;
			};

			return pack;
		}

		public CardPack CreateSecretPack()
		{
			CustomCardPack pack = new CustomCardPack();

			pack.displayMode = CardPack.DisplayMode.OnlyLast;

			pack.canAccept = (cp) =>
			{
				return true;
			};

			// TODO: написать реализацию
			pack.canGive = (count) =>
			{
				return count==1 && count <= pack.Count;
			};

			return pack;
		}

		public CardPack CreateGamePack()
		{
			CustomCardPack pack = new CustomCardPack();

			pack.displayMode = CardPack.DisplayMode.Full;

			// TODO: написать реализацию
			pack.canAccept = (cp) =>
			{
				if (pack.Count == 0) return true;
				if (Card.CardColor(cp.FirstCard) == Card.CardColor(pack.LastCard)
				    || Card.Dignity(cp.FirstCard) >= Card.Dignity(pack.LastCard))
				{
					return false;
				}
				return true;
			};

			pack.canGive = (count) =>
			{
				return count <= pack.Count;
			};

			return pack;
		}

		public CardPack CreateClosedPack()
		{
			CustomCardPack pack = new CustomCardPack();
			Boolean canGiveFlag = false;

			pack.displayMode = CardPack.DisplayMode.Hidden;

			pack.canAccept = (cp) =>
			{
				return true;
			};

			pack.canGive = (count) =>
			{
				return canGiveFlag;
			};

			pack.OnActivate = () =>
			{
				if (cardPackTable["closed"].Count == 0) return;
				canGiveFlag = true;
				ConfirmMove(cardPackTable["closed"], cardPackTable["openedClosed"], 1);
				canGiveFlag = false;
			};

			return pack;
		}

		public CardPack CreateOpenedClosedPack()
		{
			CustomCardPack pack = new CustomCardPack();

			pack.displayMode = CardPack.DisplayMode.OnlyLast;

			pack.canAccept = (cp) =>
			{
				return true;
			};

			pack.canGive = (count) =>
			{
				return count == 1 && count <= pack.Count;
			};

			pack.OnActivate = () =>
			{
				if (cardPackTable["closed"].Count == 0)
				{
					int c = pack.Count - 1;
					for (int i = 0; i<c; i++)
					{
						ConfirmMove(cardPackTable["openedClosed"], cardPackTable["closed"], 1);
					}
				}
			};

			return pack;
		}

	}

}
