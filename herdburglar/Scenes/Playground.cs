﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Sprites;

using System;
using System.Collections.Generic;
using Nez.UI;
using Nez.Tiled;
using Nez.Timers;

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

            // NOTE: this was an attempt to fix the lag issue when first scheduling something.
            //Pool<ITimer>.warmCache(10);

            // HACK: Fix lag spike associated with launching the first scheduled task
            Core.schedule(0.5f, timer => Nez.Debug.log("Scene started!"));	// NOTE: Does not fix lag spike with just {}

            addRenderer(new DefaultRenderer());
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
            var mapEntity = createEntity("tiled-map-entity");
            var map = content.Load<TiledMap>(@"maps/test");

            // Load background layer
            var mapComponent = mapEntity.addComponent(new TiledMapComponent(map));
            mapComponent.setLayersToRender(new string[] { "Tile Layer 1" });
            mapComponent.renderLayer = 10;

            // Load foreground layer
            var mapDetailComponent = mapEntity.addComponent(new TiledMapComponent(map));
            mapDetailComponent.setLayersToRender(new string[] { "Tile Layer 2" });
            mapDetailComponent.renderLayer = -1;

            // Add colliders from collider layer
            add_map_colliders(map, mapEntity);

            // Spawn entities at spawn points.
            spawn_stuff(map);
        }

        private void add_map_colliders(TiledMap map, Entity entity)
        {
            var col_layer = map.getObjectGroup("collision");

            for (var i = 0; i < col_layer.objects.Length; i++)
            {
                var obj = col_layer.objects[i];
                var c = new BoxCollider(obj.x, obj.y, obj.width, obj.height);       // TODO: add support for collider shapes that aren't boxes.

                entity.addCollider(c);
            }
        }

        private void spawn_stuff(TiledMap map)
        {
            var spawn_layer = map.getObjectGroup("spawns");
            for (var i = 0; i < spawn_layer.objects.Length; i++)
            {
                var obj = spawn_layer.objects[i];
                var objPosition = new Vector2(obj.position.X + obj.width / 2, obj.position.Y + obj.height / 2);

                switch (obj.type)
                {
                    case "BurglarSpawn":
                        var burglar = new Burglar();
                        burglar.transform.position = objPosition;
                        addEntity(burglar);
                        break;

                    case "CowSpawn":
                        var orientation = Cow.Orientation.Right;
                        if (obj.properties.ContainsKey("orientation"))
                            orientation = Cow.getOrientationFromName(obj.properties["orientation"]);

                        var cow = new Cow(orientation: orientation);
                        cow.transform.position = objPosition;
                        cow.name = obj.name;
                        addEntity(cow);
                        break;

                    case "IdolSpawn":
                        var idol = new Idol();
                        idol.transform.position = objPosition;
                        addEntity(idol);
                        break;

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
