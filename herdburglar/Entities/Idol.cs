using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Textures;
using Nez.Sprites;

using herdburglar.Components.Controllers;

namespace herdburglar
{
    public class Idol : Entity
    {
        private Sprite sprite;
        private BoxCollider collider;

        public override void onAddedToScene()
        {
            base.onAddedToScene();

            var texture = scene.content.Load<Texture2D>("sprites/buddhas");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 32, 35);
            sprite = addComponent(new Sprite(subtextures[0]));
            sprite.renderLayer = 1;

            collider = new BoxCollider(32, 32);
            collider.isTrigger = true;
        }
    }
}
