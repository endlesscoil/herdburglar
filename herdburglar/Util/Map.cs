using System.Collections.Generic;
using Microsoft.Xna.Framework;

using Nez;
using Nez.Tiled;

namespace herdburglar
{
    class Map
    {
        public static TiledTile getTileAt(TiledMap map, Vector2 pos)
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

        public static Vector2 getTileAt_Center(TiledMap map, Vector2 pos)
        {
            var tile = getTileAt(map, pos);
            var world_pos = tile.getWorldPosition(map);
            var offset = new Vector2(map.tileWidth / 2, map.tileHeight / 2);
            
            return new Vector2(world_pos.X + offset.X, world_pos.Y + offset.Y);
        }

        public static bool hasCollider(TiledMap map, Vector2 pos)
        {
            var tile = getTileAt(map, pos);
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

        public static List<Point> getOpenTiles(TiledMap map)
        {
            List<Point> openTiles = new List<Point>();

            for (int x = 0; x < map.width; x++)
            {
                for (int y = 0; y < map.height; y++)
                {
                    var p = new Point(x, y);
                    if (!hasCollider(map, map.tileToWorldPosition(p)))
                    {
                        openTiles.Add(p);
                    }
                }
            }

            return openTiles;
        }
    }
}
