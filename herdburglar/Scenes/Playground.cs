using Microsoft.Xna.Framework;
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

            addRenderer<DefaultRenderer>(new DefaultRenderer());
        }

        public override void onStart()
        {
			base.onStart();

			load_tiled_map();

			var burglar = new Burglar();
			burglar.transform.position = new Vector2(Screen.width / 3, Screen.height / 3);
			addEntity(burglar);

            var cow = new Cow(orientation: Cow.Orientation.Up);
            cow.transform.position = new Vector2(300, 200);
            addEntity(cow);
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
            var tiledmapComponent = tiledEntity.addComponent(new TiledMapComponent(tiledmap, "collisionxxxxxxx"));
            tiledmapComponent.physicsLayer = 8;
            tiledmapComponent.setLayersToRender(new string[] { "Tile Layer 1" });
            tiledmapComponent.renderLayer = 10;

            var tiledmapComponentDetail = tiledEntity.addComponent(new TiledMapComponent(tiledmap));
            tiledmapComponentDetail.setLayersToRender(new string[] { "Tile Layer 2" });
            tiledmapComponentDetail.renderLayer = -1;

			var col_layer = (TiledObjectGroup)tiledmap.getObjectGroup("collision");

			for(var i = 0; i < col_layer.objects.Length; i++)
			{
				var o = col_layer.objects[i];
				Debug.log("o={0}", col_layer.objects[i]);

				var c = new BoxCollider(o.x, o.y, o.width, o.height);
				c.physicsLayer = tiledmapComponent.physicsLayer;
				c.entity = tiledEntity;

				Physics.addCollider(c);
			}
        }
        #endregion
    }
}
