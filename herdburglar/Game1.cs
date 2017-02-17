using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Input;

using Nez;

namespace herdburglar
{
	/// <summary>
	/// This is the main type for your game.
	/// </summary>
	public class Game1 : Nez.Core
	{
		public Game1() : base(windowTitle: "herdburglar") { }

		protected override void Initialize() 
		{
			base.Initialize();

			Window.AllowUserResizing = true;
			Core.exitOnEscapeKeypress = false;	// HACK: Because mono on linux is obnoxious.

			//Content.RootDirectory = "Content";
			scene = new Playground();
		}
	}
}

