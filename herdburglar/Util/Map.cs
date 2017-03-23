using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Nez;
using Nez.Tiled;

using herdburglar.Components.Controllers;

namespace herdburglar
{
    class Map
    {
        public int backgroundRenderLayer = 0;
        public int foregroundRenderLayer = 0;

        private TiledMap map;
        private Entity mapEntity;
        private Component backgroundLayer;
        private Component foregroundLayer;

        #region Public
        public bool load(string mapResource, string targetEntity, 
                         string backgroundLayerName = null, string foregroundLayerName = null, 
                         string collisionLayerName = null, string spawnLayerName = null)
        {
            mapEntity = Core.scene.createEntity(targetEntity);
            map = Core.content.Load<TiledMap>(mapResource);

            if (backgroundLayerName != null)
                backgroundLayer = loadLayer(backgroundLayerName, backgroundRenderLayer);

            if (foregroundLayerName != null)
                foregroundLayer = loadLayer(foregroundLayerName, foregroundRenderLayer);

            if (collisionLayerName != null)
                addMapColliders(collisionLayerName);

            if (spawnLayerName != null)
                spawn(spawnLayerName);

            return true;
        }

        public TiledTile getTileAt(Vector2 pos)
        {
            TiledTile ret = null;

            var tile_pos = map.worldToTilePosition(pos, false);

            if (tile_pos.X > 0 && tile_pos.X < map.width && tile_pos.Y > 0 && tile_pos.Y < map.height)
            {
                var tilelayer = (TiledTileLayer)map.layers[0];  // FIXME
                ret = tilelayer.getTile(tile_pos.X, tile_pos.Y);
            }

            return ret;
        }

        public Vector2 getTileCenterAt(Vector2 pos)
        {
            var tile = getTileAt(pos);
            var world_pos = tile.getWorldPosition(map);
            var offset = new Vector2(map.tileWidth / 2, map.tileHeight / 2);

            return new Vector2(world_pos.X + offset.X, world_pos.Y + offset.Y);
        }

        public bool hasCollider(Vector2 pos)
        {
            var tile = getTileAt(pos);
            var tile_rect = tile.getTileRectangle(map);
            var tile_rectf = new RectangleF(tile_rect.X, tile_rect.Y, tile_rect.Width, tile_rect.Height);

            var objectlayer = (Nez.Tiled.TiledTileLayer)map.layers[1];  // FIXME
            var collision_rectangles = objectlayer.getCollisionRectangles();
            var emptyRect = new RectangleF();

            foreach (var rect in collision_rectangles)
            {
                if (RectangleF.intersect(tile_rectf, new RectangleF(rect.X, rect.Y, rect.Width, rect.Height)) != emptyRect)
                    return true;
            }

            return false;
        }

        public List<Point> getOpenTiles()
        {
            List<Point> openTiles = new List<Point>();

            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    var p = new Point(x, y);

                    if (!hasCollider(map.tileToWorldPosition(p)))
                        openTiles.Add(p);
                }
            }

            return openTiles;
        }
        #endregion

        #region Private
        private Component loadLayer(string layerName, int renderLayer = 0)
        {
            var component = mapEntity.addComponent(new TiledMapComponent(map));
            component.setLayersToRender(new string[] { layerName });
            component.renderLayer = renderLayer;

            return component;
        }

        private void addMapColliders(string collisionLayerName)
        {
            var col_layer = map.getObjectGroup(collisionLayerName);

            for (var i = 0; i < col_layer.objects.Length; i++)
            {
                var obj = col_layer.objects[i];
                var c = new BoxCollider(obj.x, obj.y, obj.width, obj.height);       // TODO: add support for collider shapes that aren't boxes.

                mapEntity.addComponent(c);
            }
        }

        private void spawn(string spawnLayerName)
        {
            var spawn_layer = map.getObjectGroup(spawnLayerName);
            for (var i = 0; i < spawn_layer.objects.Length; i++)
            {
                var obj = spawn_layer.objects[i];
                var objPosition = new Vector2(obj.position.X + obj.width / 2, obj.position.Y + obj.height / 2);

                switch (obj.type)
                {
                    case "BurglarSpawn": {
                        var burglar = new Burglar();
                        burglar.transform.position = objPosition;
                        Core.scene.addEntity(burglar);
                        break;
                    }

                    case "CowSpawn": {
                        var orientation = CowController.Orientation.Right;
                        if (obj.properties.ContainsKey("orientation"))
                            orientation = CowController.getOrientationFromName(obj.properties["orientation"]);

                        var cow = new Cow(orientation: orientation);
                        cow.transform.position = objPosition;
                        cow.name = obj.name;
                        Core.scene.addEntity(cow);
                        break;
                    }

                    case "BullSpawn": {
                        var orientation = CowController.Orientation.Right;
                        if (obj.properties.ContainsKey("orientation"))
                            orientation = CowController.getOrientationFromName(obj.properties["orientation"]);

                        var bull = new Bull(orientation: orientation);
                        bull.transform.position = objPosition;
                        bull.name = obj.name;
                        Core.scene.addEntity(bull);
                        break;
                    }

                    case "IdolSpawn": {
                        var idol = new Idol();
                        idol.transform.position = objPosition;
                        Core.scene.addEntity(idol);
                        break;
                    }

                    default:
                        break;
                }
            }
        }
        #endregion
    }
}
