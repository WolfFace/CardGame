using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace CardGame
{
	/// <summary>
	/// Абстракция сцены приложения. 
	/// Представляет собой один экран приложения (меню, настройки, игровое поле, etc)
	/// </summary>
	abstract public class Scene
	{
		public ContentManager Content;

		public Scene(ContentManager content)
		{
			this.Content = content;
		}

		/// <summary>
		/// Время обновлять логику сцены!
		/// </summary>
		virtual public void Update()
		{

		}

		/// <summary>
		/// Рендерит сцену.
		/// </summary>
		/// <param name="spriteBatch">Контекст рисования сцены.</param>
		virtual public void DrawScene(SpriteBatch spriteBatch)
		{

		}

	}
}
