using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

using Nez;

namespace template
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Nez.Core
	{
		public Game1() : base(windowTitle: "") { }

		protected override void Initialize() 
		{
			base.Initialize();

			Window.AllowUserResizing = true;
		}
	}
}

