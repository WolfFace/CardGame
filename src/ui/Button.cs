using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
namespace CardGame
{
	public class Button
	{
		public Action OnClick = () => { };
		public Texture2D Texture;
		public Texture2D HoverTexture;
		public Texture2D PressedTexture;
		public Point pos;
		public Rectangle Bounds
		{
			get { return new Rectangle(pos.X, pos.Y, Texture.Bounds.Width, Texture.Bounds.Height); }
		}
		private bool hover = false;
		private bool pressed = false;
		private ButtonState prevLeftMouseState = ButtonState.Released;

		public Button(String tex, Point p)
		{
			this.Texture = CardResources.Instance.UiTexture(tex);
			this.HoverTexture = CardResources.Instance.UiTexture(tex+"_hover");
			this.PressedTexture = CardResources.Instance.UiTexture(tex+"_pressed");
			pos = p;
		}

		public void Update()
		{
			MouseState mouse = Mouse.GetState();
			Rectangle mouseRect = new Rectangle(mouse.X, mouse.Y, 1, 1);

			if (!hover && Bounds.Contains(mouseRect))
			{
				// TODO: play sound
				hover = true;
			}
			else if (hover && !Bounds.Contains(mouseRect))
			{
				hover = false;
				pressed = false;
			}

			if (mouse.LeftButton == ButtonState.Pressed 
			    && prevLeftMouseState == ButtonState.Released
			    && Bounds.Contains(mouseRect))
			{
				pressed = true;
			}
			if (mouse.LeftButton==ButtonState.Released 
			    && prevLeftMouseState==ButtonState.Pressed 
			    && Bounds.Contains(mouseRect))
			{
				pressed = false;
				OnClick();
			}
			prevLeftMouseState = mouse.LeftButton;
		}

		public void Draw(SpriteBatch batch)
		{
			Texture2D tex = hover ? HoverTexture : Texture;
			tex = pressed ? PressedTexture : tex;
			batch.Draw(tex, Bounds, Color.White);
		}
	}
}
