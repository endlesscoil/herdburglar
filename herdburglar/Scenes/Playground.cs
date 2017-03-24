using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.UI;
using Nez.Tiled;
using Nez.Timers;
using Nez.Sprites;

using herdburglar.Components.Controllers;

namespace herdburglar.Scenes
{
    class Playground : Scene
    {
        #region Member variables
        herdburglar.Map map;
        #endregion

        #region Events
        public override void initialize()
        {
            base.initialize();

            Core.debugRenderEnabled = true;
			Transform.shouldRoundPosition = false;

            // NOTE: this was an attempt to fix the lag issue when first scheduling something.
            //Pool<ITimer>.warmCache(10);

            // HACK: Fix lag spike associated with launching the first scheduled task
            Core.schedule(0.5f, timer => Nez.Debug.log("Scene started!"));	// NOTE: Does not fix lag spike with just {}

            addRenderer(new DefaultRenderer());
        }

        public override void onStart()
        {
			base.onStart();

            map = new herdburglar.Map() { backgroundRenderLayer = 10, foregroundRenderLayer = -1 };
            map.load(@"maps/test", "tiled-map-entity", "Tile Layer 1", "Tile Layer 2", "collision", "spawns");
        }

        public override void update()
        {
            base.update();
        }
        #endregion

        #region Public interface
        #endregion

        #region Private
        #endregion
    }
}
