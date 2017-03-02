using System.Collections.Generic;

using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Textures;
using Nez.Sprites;

using herdburglar.Components.Controllers;

namespace herdburglar
{
	public class Burglar : Entity
	{
        #region Enums
        public enum Animations
		{
			Idle,
			Walk
		}
        #endregion

        private BurglarController controller;
		private BoxCollider collider;
		private Sprite<Animations> animation;

        #region Public
        public Burglar()
		{

		}
        #endregion

        #region Events
        public override void onAddedToScene()
		{
			base.onAddedToScene();

			tag = (int)Tags.Burglar;

            controller = addComponent(new BurglarController() { speed = 150f });
            collider = addComponent(new BoxCollider(-16, -24, 32, 64));

            setupAnimations();
			animation.play(Animations.Idle);
		}

		public override void onRemovedFromScene()
		{
			base.onRemovedFromScene();
		}

		public override void update()
		{
			base.update();
		}
        #endregion

        #region Private
        private void setupAnimations()
        {
            var texture = scene.content.Load<Texture2D>("sprites/kit_from_firefox");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 56, 80);

            animation = addComponent(new Sprite<Animations>(subtextures[0]));
            animation.addAnimation(Animations.Idle, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[0],
                subtextures[1],
                subtextures[2]
            }).setFps(3));

            animation.addAnimation(Animations.Walk, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[4],
                subtextures[5],
                subtextures[6]
            }).setFps(7));
        }
        #endregion
    }
}
