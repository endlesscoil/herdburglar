using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace herdburglar.Components.Controllers
{
    public class CowController : Component, IUpdatable
    {
        #region Static
		public readonly static Dictionary<Orientation, Vector2> orientationToFacingDirection = new Dictionary<Orientation, Vector2>
        {
			{ Orientation.Up, new Vector2(0, -1) },
			{ Orientation.Left, new Vector2(-1, 0) },
			{ Orientation.Down, new Vector2(0, 1) },
			{ Orientation.Right, new Vector2(1, 0) }
        };

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

        #region Enums
		public enum HeadRotation {
			None = 0,
			Left = 1,
			Right = 2,
			Down = 1,
			Up = 2
		}

        public enum Orientation
        {
            Up,
            Right,
            Down,
            Left
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

        public float fovAngle = MathHelper.Pi / 8; // 90 degrees
        private float computedFOVAngle = 0f;

        public float alertDistance = 250f;
        public float dangerDistance = 175;

        private Sprite<Animations> animation = null;
        private AlertHerd alerter = null;
        private BoxCollider collider = null;
        private SoundEffectInstance sound;

        private Vector2 headDirection = Vector2.Zero;
        private Orientation _orientation;

        #region Properties
        public Orientation orientation
        {
            get { return _orientation; }

            set
            {
                _orientation = value;

                refreshOrientation();
            }
        }
        #endregion

        #region Events
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            computedFOVAngle = Mathf.cos(fovAngle);
            headDirection = orientationToFacingDirection[orientation];

            sound = entity.scene.content.Load<SoundEffect>("sound/cow").CreateInstance();
            collider = entity.getOrCreateComponent<BoxCollider>();
            alerter = entity.getOrCreateComponent<AlertHerd>();
            alerter.events.addObserver(AlertHerd.Events.Alerted, (target) => {
                rotateTowards(target);
                moo();
            });

            setupAnimations();
            refreshOrientation();
        }

        void IUpdatable.update()
        {
            var facingDirection = orientationToFacingDirection[orientation];        // FIXME: I don't like how this is currently done.

			// Draw facing indicator
			if (Core.debugRenderEnabled)
				Debug.drawLine(entity.transform.position, entity.transform.position + ((facingDirection + headDirection) * 50), Color.Blue, 0.5f);

            // Scan for any threats
            scanForThreats(facingDirection);
        }
        #endregion

        #region Private
        private void moo()
        {
            sound.Play();
        }

        private Entity scanForThreats(Vector2 facingDirection)
        {
            var threats = new List<Entity>();
            Entity foundThreat = null;

            // Find the burglar
            var burglar = entity.scene.findEntitiesWithTag((int)Tags.Burglar);
            if (burglar.Count > 0)
                threats.InsertRange(0, burglar);

            // .. and his distractions
            var distractions = entity.scene.findEntitiesWithTag((int)Tags.Distraction);
            if (distractions.Count > 0)
                threats.InsertRange(0, distractions);

            if (threats.Count > 0)
            {
                foreach (var threat in threats) 
                {
                    // Are they within alert distance?
                    var distance = Vector2.Distance(entity.transform.position, threat.transform.position);
                    if (distance < alertDistance)
                    {
                        var vector_to_threat = Vector2.Normalize(threat.transform.position - entity.transform.position);
                        var dot = Vector2.Dot(facingDirection + headDirection, vector_to_threat);

                        // Are they in front of us?
                        if (dot > computedFOVAngle)
                        {
                            var raycastHit = Physics.linecast(entity.transform.position, threat.transform.position);

                            // And can we actually see them?
                            if (raycastHit.collider != null && 
                                (raycastHit.collider.entity.tag == (int)Tags.Burglar || raycastHit.collider.entity.tag == (int)Tags.Distraction))
                            {
                                var color = distance < dangerDistance ? Color.Red : Color.Yellow;
                                if (Core.debugRenderEnabled)
                                    Debug.drawLine(entity.transform.position, threat.transform.position, color, 0.5f);

                                rotateTowards(threat);
                                var alertHerd = entity.getComponent<AlertHerd>();
                                if (alertHerd != null)
                                    alertHerd.alert(raycastHit.collider.entity);

                                // Break out on first threat found.
                                foundThreat = threat;
                                break;
                            }
                        }
                    }
                }
            }

            return foundThreat;
        }

		private Vector2 getHeadDirection(Entity target)
		{
			Vector2 newDirection;

			var facingDirection = orientationToFacingDirection[orientation];
			var rotation = HeadRotation.None;
			var target_pos = target.transform.position;
			var pos = entity.transform.position;

			// Determine which direction the head should rotate given our position relative to the target.
			switch (orientation) {
				case Orientation.Up:
					rotation = target_pos.X < pos.X ? HeadRotation.Left : HeadRotation.Right;
					break;

				case Orientation.Left:
					rotation = target_pos.Y < pos.Y ? HeadRotation.Right : HeadRotation.Left;
					break;

				case Orientation.Down:
					rotation = target_pos.X < pos.X ? HeadRotation.Right : HeadRotation.Left;
					break;

				case Orientation.Right:
					rotation = target_pos.Y < pos.Y ? HeadRotation.Left : HeadRotation.Right;
					break;

				default:
					rotation = HeadRotation.None;
					break;
			}

			// Rotate the head direction.
			if (rotation == HeadRotation.Right || rotation == HeadRotation.Up)
				newDirection = new Vector2(-facingDirection.Y, facingDirection.X);
			else if (rotation == HeadRotation.Left || rotation == HeadRotation.Down)
				newDirection = new Vector2(facingDirection.Y, -facingDirection.X);
			else
				newDirection = facingDirection;

			return newDirection;
		}

        public void rotateTowards(Entity target)
        {
            var pos = entity.transform.position;
            var target_pos = target.transform.position;
            var new_orientation = orientation;

            var x_dist = target_pos.X - pos.X;
            var y_dist = target_pos.Y - pos.Y;

            // Default to turning to the left or right side, depending on x distance.
            if (x_dist < 0)
            {
                new_orientation = Orientation.Left;

                // If y distance is greater, prefer up or down orientations
                if (Math.Abs(y_dist) > Math.Abs(x_dist))
                {
                    if (y_dist < 0)
                        new_orientation = Orientation.Up;                    
                    else
                        new_orientation = Orientation.Down;
                }                    
            }
            else
            {
                new_orientation = Orientation.Right;

                // If y distance is greater, prefer up or down orientations
                if (Math.Abs(y_dist) > Math.Abs(x_dist))
                {
                    if (y_dist < 0)
                        new_orientation = Orientation.Up;                    
                    else
                        new_orientation = Orientation.Down;
                }
            }

            if (orientation != new_orientation)
                orientation = new_orientation;
            
            // NOTE: not sure if I like the head rotating too.  it makes sense
            // but feels like the mechanic is too ... "tight"?
            headDirection = getHeadDirection(target);
        }


        private void setColliderDetails(int x, int y, int width, int height)
        {
            collider.setWidth(width);
            collider.setHeight(height);
            collider.localOffset = new Vector2(x + width / 2, y + height / 2);
        }

        private void refreshOrientation()
        {
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

        private void setupAnimations()
        {
            var texture = entity.scene.content.Load<Texture2D>("sprites/cow_walk");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 128, 128);

            animation = entity.addComponent(new Sprite<Animations>(subtextures[0]));
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
        #endregion
    }
}
