using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace CardGame
{

	/// <summary>
	/// Сцена игрового поля игры.
	/// </summary>
	public class GameScene : Scene
	{

		private Point offset = new Point(0, 0);
		private State state = new State();
		private List<VisibleCardPack> visiblePacks = new List<VisibleCardPack>();
		public Button BackBtn = new Button("backBtn", new Point(10, 660));
		public Button StepBackBtn = new Button("stepBackBtn", new Point(50, 194 + 95 - 50));
		public Action OnWin
		{
			get { return state.OnWin; }
			set
			{
				state.OnWin = value;
			}
		}
		public int Score
		{
			get { return state.Score; }
		}

		// Клики и drag'n'drop
		private const int DRAG_HOLD_POW_2 = 100;
		private VisibleCardPack mouseTarget = null;
		private Point startTargetingPoint;

		// Колода, находящаяся в полете
		private VisibleCardPack draggedPack = null;
		private Point draggedOriginalPoint;

		private MouseState prevMouseState = Mouse.GetState();

		public GameScene(Microsoft.Xna.Framework.Content.ContentManager content) : base(content)
		{
			visiblePacks.Add(new VisibleCardPack(state.CardPackById("closed"), new Point(50, 50)));
			visiblePacks.Add(new VisibleCardPack(state.CardPackById("openedClosed"), new Point(145, 50)));

			visiblePacks.Add(new VisibleCardPack(state.CardPackById("base1"), new Point(290, 50)));
			visiblePacks.Add(new VisibleCardPack(state.CardPackById("base2"), new Point(385, 50)));
			visiblePacks.Add(new VisibleCardPack(state.CardPackById("base3"), new Point(480, 50)));
			visiblePacks.Add(new VisibleCardPack(state.CardPackById("base4"), new Point(575, 50)));

			visiblePacks.Add(new VisibleCardPack(state.CardPackById("secret"), new Point(145, 194)));

			visiblePacks.Add(new VisibleCardPack(state.CardPackById("game1"), new Point(290, 194)));
			visiblePacks.Add(new VisibleCardPack(state.CardPackById("game2"), new Point(385, 194)));
			visiblePacks.Add(new VisibleCardPack(state.CardPackById("game3"), new Point(480, 194)));
			visiblePacks.Add(new VisibleCardPack(state.CardPackById("game4"), new Point(575, 194)));

			StepBackBtn.OnClick = () =>
			{
				state.BackStep();
			};
		}

		private VisibleCardPack nearestCardPack(Rectangle rect)
		{
			Rectangle line = new Rectangle(rect.X, rect.Y, rect.Width, 50);
			int centerX = line.Center.X;
			VisibleCardPack pack = visiblePacks
				.Where(i => i.GetRectangle().Intersects(line))
				.OrderByDescending(i => Math.Abs(centerX - i.GetRectangle().Center.X)).FirstOrDefault();
			return pack;
		}

		public override void Update()
		{
			MouseUpdate(Mouse.GetState());
			BackBtn.Update();
			StepBackBtn.Update();
		}

		public void MouseUpdate(MouseState mouse)
		{
			visiblePacks.ForEach(i => i.Light = false);

			if (prevMouseState.LeftButton == ButtonState.Released
				&& mouse.LeftButton == ButtonState.Pressed)
			{
				OnLeftMousePressed(mouse);
			}
			else if (prevMouseState.LeftButton == ButtonState.Pressed
					 && mouse.LeftButton == ButtonState.Released)
			{
				OnLeftMouseReleased(mouse);
			}
			else if (mouseTarget != null
					 && draggedPack != null
					 && mouse.LeftButton == ButtonState.Pressed)
			{
				OnMouseMoveWithCard(mouse);
			}
			else if (mouseTarget != null
					 && mouse.LeftButton == ButtonState.Pressed)
			{
				OnLeftPressedOverCard(mouse);
			}

			if (draggedPack != null)
				draggedPack.Move(mouse.Position - (startTargetingPoint - draggedOriginalPoint), true);

			prevMouseState = mouse;
		}

		public void OnLeftMousePressed(MouseState mouse)
		{
			mouseTarget = visiblePacks.FirstOrDefault(i => i.GetRectangle().Contains(mouse.Position));
			startTargetingPoint = mouse.Position;
		}

		public void OnLeftMouseReleased(MouseState mouse)
		{
			if (draggedPack != null)
			{
				VisibleCardPack under = nearestCardPack(draggedPack.GetRectangle());
				if (under != null && under.pack.CanAccept(draggedPack.pack))
				{
					state.ConfirmMove(mouseTarget.pack, under.pack, draggedPack.pack.Count);
				}
				draggedPack = null;
			}
			else if (mouseTarget != null && draggedPack == null)
			{
				mouseTarget.Activate();
			}
			if (mouseTarget != null) mouseTarget.InvisibleCards = 0;
			mouseTarget = null;
		}

		public void OnLeftPressedOverCard(MouseState mouse)
		{
			Point shift = startTargetingPoint - mouse.Position;
			if ((shift.X * shift.X + shift.Y * shift.Y) > DRAG_HOLD_POW_2)
			{
				int gap = mouseTarget.GapCount(startTargetingPoint);
				if (gap != -1)
				{
					draggedPack = new VisibleCardPack(mouseTarget.pack.LastCards(gap), startTargetingPoint);
					draggedOriginalPoint = mouseTarget.GapLeftTopPoint(mouse.Position);
					mouseTarget.InvisibleCards = gap;
				}
			}
		}

		public void OnMouseMoveWithCard(MouseState mouse)
		{
			VisibleCardPack under = nearestCardPack(draggedPack.GetRectangle());
			if (under != null && under.pack.CanAccept(draggedPack.pack)) under.Light = true;
		}

		/// <summary>
		/// Рисует игровое поле.
		/// </summary>
		/// <param name="spriteBatch">Контекст рисования.</param>
		public override void DrawScene(SpriteBatch spriteBatch)
		{
			spriteBatch.Begin();

			// Рисуем кнопки
			BackBtn.Draw(spriteBatch);
			StepBackBtn.Draw(spriteBatch);

			// Рисуем карты
			foreach (VisibleCardPack vpack in visiblePacks)
			{
				vpack.Draw(spriteBatch, offset);
			}

			// Рисуем стопку карт, учавствующую в drag'n'drop
			if (draggedPack != null) draggedPack.Draw(spriteBatch, offset);
			spriteBatch.End();
		}

	}
}
