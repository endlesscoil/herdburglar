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

				var c = new BoxCollider(o.x, o.y, o.width, o.height);
				c.physicsLayer = tiledmapComponent.physicsLayer;
				c.entity = tiledEntity;

				Physics.addCollider(c);
			}

            var spawn_layer = (TiledObjectGroup)tiledmap.getObjectGroup("spawns");
            for (var i = 0; i < spawn_layer.objects.Length; i++)
            {
                var o = spawn_layer.objects[i];
                
                switch (o.type)
                {
                    case "BurglarSpawn":
                        var burglar = new Burglar();
                        burglar.transform.position = new Vector2(o.position.X + o.width / 2, o.position.Y + o.height / 2);
                        addEntity(burglar);
                        break;

                    case "CowSpawn":
                        var orientation = Cow.Orientation.Right;
                        if (o.properties.ContainsKey("orientation"))
                            orientation = Cow.getOrientationFromName(o.properties["orientation"]);

                        var cow = new Cow(orientation: orientation);
                        cow.transform.position = new Vector2(o.position.X + o.width / 2, o.position.Y + o.height / 2);
                        addEntity(cow);
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
