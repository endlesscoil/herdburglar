using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Sprites;
using Nez.Textures;

using herdburglar.Components.Controllers;

namespace herdburglar
{
	public class Cow : Entity
	{
        #region Enums
        public enum Orientation
        {
            Up,
            Left,
            Down,
            Right
        }

        public enum Animations
        {
            FacingUpIdle,
            FacingUpWalk,
            FacingLeftIdle,
            FacingLeftWalk,
            FacingDownIdle,
            FacingDownWalk,
            FacingRightIdle,
            FacingRightWalk
        }
        #endregion

        private CowController controller = null;
        private Sprite<Animations> animation = null;
        private BoxCollider collider = null;
        private Orientation _orientation = Orientation.Right;

        #region Properties
        public Orientation orientation
        {
            get { return _orientation; }

            set
            {
                _orientation = orientation;

                if (animation != null)
                {
                    switch (orientation)
                    {
                        case Orientation.Up:
                            setColliderDetails(-12, -20, 26, 64);
                            animation.play(Animations.FacingUpIdle);
                            break;

                        case Orientation.Left:
                            setColliderDetails(-40, -16, 64, 40);
                            animation.play(Animations.FacingLeftIdle);
                            break;

                        case Orientation.Down:
                            setColliderDetails(-12, -16, 26, 56);
                            animation.play(Animations.FacingDownIdle);
                            break;

                        case Orientation.Right:
                            setColliderDetails(-24, -16, 64, 40);
                            animation.play(Animations.FacingRightIdle);
                            break;

                        default:
                            break;
                    }
                }
            }
        }
        #endregion

        #region Static functions
        public static Orientation getOrientationFromName(string name)
        {
            switch (name)
            {
                case "Up":
                    return Orientation.Up;

                case "Left":
                    return Orientation.Left;

                case "Down":
                    return Orientation.Down;

                case "Right":
                default:
                    return Orientation.Right;
            }
        }
        #endregion

        #region Public
        public Cow(Orientation orientation = Orientation.Right)
		{
            _orientation = orientation;
		}
        #endregion

        #region Events
        public override void onAddedToScene()
		{
			base.onAddedToScene();

			tag = (int)Tags.Cow;
            controller = addComponent<CowController>();
            collider = addComponent<BoxCollider>();
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
            var texture = scene.content.Load<Texture2D>("sprites/cow_walk");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 128, 128);

            animation = addComponent(new Sprite<Animations>(subtextures[0]));
            animation.addAnimation(Animations.FacingUpIdle, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[0*4+2]
            }));

            animation.addAnimation(Animations.FacingUpWalk, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[0*4+0],
                subtextures[0*4+1],
                subtextures[0*4+2],
                subtextures[0*4+3]
            }));

            animation.addAnimation(Animations.FacingLeftIdle, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1*4+2]
            }));

            animation.addAnimation(Animations.FacingLeftWalk, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[1*4+0],
                subtextures[1*4+1],
                subtextures[1*4+2],
                subtextures[1*4+3]
            }));

            animation.addAnimation(Animations.FacingDownIdle, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[2*4+2]
            }));

            animation.addAnimation(Animations.FacingDownWalk, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[2*4+0],
                subtextures[2*4+1],
                subtextures[2*4+2],
                subtextures[2*4+3]
            }));

            animation.addAnimation(Animations.FacingRightIdle, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[3*4+2]
            }));

            animation.addAnimation(Animations.FacingRightWalk, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[3*4+0],
                subtextures[3*4+1],
                subtextures[3*4+2],
                subtextures[3*4+3]
            }));
        }

        private void setColliderDetails(int x, int y, int width, int height)
        {
            collider.setWidth(width);
            collider.setHeight(height);
            collider.localOffset = new Vector2(x + width / 2, y + height / 2);
        }
        #endregion
    }
}
