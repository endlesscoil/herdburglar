﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Sprites;

using System;
using System.Collections.Generic;
using Nez.UI;
using Nez.Tiled;

namespace herdburglar
{
    class Playground : Scene
    {
        #region Member variables
        #endregion

        #region Events
        public override void initialize()
        {
            base.initialize();

            Core.debugRenderEnabled = true;
			Transform.shouldRoundPosition = false;

			load_tiled_map();

			var burglar = new Burglar();
			burglar.transform.position = new Vector2(Screen.width / 2, Screen.height / 2);
			addEntity(burglar);

            addRenderer<DefaultRenderer>(new DefaultRenderer());
        }

        public override void onStart()
        {
            base.onStart();
        }

        public override void update()
        {
            base.update();
        }
        #endregion

        #region Public interface
        #endregion

        #region Private
		private void load_tiled_map()
        {
            var tiledEntity = createEntity("tiled-map-entity");

            //content.RootDirectory = "Content";
            var tiledmap = content.Load<TiledMap>(@"maps/test");
            var tiledmapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledmap, "collision"));
            tiledmapComponent.physicsLayer = 8;
            tiledmapComponent.setLayersToRender(new string[] { "Tile Layer 1", "Tile Layer 2" });
        }
        #endregion
    }
}
