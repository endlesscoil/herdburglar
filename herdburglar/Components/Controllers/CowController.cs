﻿using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nez;

using Nez.Sprites;
using System.Collections.Generic;

using static herdburglar.Cow;

namespace herdburglar.Components.Controllers
{
    class CowController : Component, IUpdatable
    {
		public static Dictionary<Cow.Orientation, Vector2> orientationToFacingDirection = new Dictionary<Cow.Orientation, Vector2>
        {
			{ Cow.Orientation.Up, new Vector2(0, -1) },
			{ Cow.Orientation.Left, new Vector2(-1, 0) },
			{ Cow.Orientation.Down, new Vector2(0, 1) },
			{ Cow.Orientation.Right, new Vector2(1, 0) }
        };

		public enum HeadRotation {
			None = 0,
			Left = 1,
			Right = 2,
			Down = 1,
			Up = 2
		}

        private Cow cow = null;
        private float fovAngle = MathHelper.Pi / 8; // 90 degrees
        private float _computedAngle;
        private float alertDistance = 250f;
        private float dangerDistance = 175;
		private Vector2 headDirection;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            cow = (Cow)entity;
            _computedAngle = Mathf.cos(fovAngle);

			headDirection = orientationToFacingDirection[cow.orientation];
        }

        void IUpdatable.update()
        {
            var facingDirection = orientationToFacingDirection[cow.orientation];

			// Draw facing indicator
			if (Core.debugRenderEnabled)
				Debug.drawLine(entity.transform.position, entity.transform.position + ((facingDirection + headDirection) * 100), Color.Blue, 0.5f);
            
        	// Find the burglar
            var burglar = entity.scene.findEntitiesWithTag((int)Tags.Burglar);
            if (burglar.Count > 0)
            {
            	// Are they within alert distance?
                var distance = Vector2.Distance(entity.transform.position, burglar[0].transform.position);
                if (distance < alertDistance)
                {
					var vector_to_burglar = Vector2.Normalize(burglar[0].transform.position - entity.transform.position);
                    var dot = Vector2.Dot(facingDirection + headDirection, vector_to_burglar);

                    // Are they in front of us?
                    if (dot > _computedAngle)
                    {
                        var raycastHit = Physics.linecast(entity.transform.position, burglar[0].transform.position);

                        // And can we actually see them?
                        if (raycastHit.collider.entity.tag == (int)Tags.Burglar)
                        {
                            var color = distance < dangerDistance ? Color.Red : Color.Yellow;
							if (Core.debugRenderEnabled)
                            	Debug.drawLine(entity.transform.position, burglar[0].transform.position, color, 0.5f);

                            rotateTowards(burglar[0]);
                            entity.getComponent<AlertHerd>().alert();

                            // TODO: Do something here for alert/danger.
                        }
                    }
                }
            }
        }

		private Vector2 getHeadDirection(Entity target)
		{
			Vector2 newDirection;

			var facingDirection = orientationToFacingDirection[cow.orientation];
			var rotation = HeadRotation.None;
			var target_pos = target.transform.position;
			var pos = entity.transform.position;

			// Determine which direction the head should rotate given our position relative to the target.
			switch (cow.orientation) {
				case Cow.Orientation.Up:
					rotation = target_pos.X < pos.X ? HeadRotation.Left : HeadRotation.Right;
					break;

				case Cow.Orientation.Left:
					rotation = target_pos.Y < pos.Y ? HeadRotation.Right : HeadRotation.Left;
					break;

				case Cow.Orientation.Down:
					rotation = target_pos.X < pos.X ? HeadRotation.Right : HeadRotation.Left;
					break;

				case Cow.Orientation.Right:
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
            var new_orientation = cow.orientation;

            var x_dist = target_pos.X - pos.X;
            var y_dist = target_pos.Y - pos.Y;

            // Default to turning to the left or right side, depending on x distance.
            if (x_dist < 0)
            {
                new_orientation = Cow.Orientation.Left;

                // If y distance is greater, prefer up or down orientations
                if (Math.Abs(y_dist) > Math.Abs(x_dist))
                {
                    if (y_dist < 0)
                        new_orientation = Cow.Orientation.Up;                    
                    else
                        new_orientation = Cow.Orientation.Down;
                }                    
            }
            else
            {
                new_orientation = Cow.Orientation.Right;

                // If y distance is greater, prefer up or down orientations
                if (Math.Abs(y_dist) > Math.Abs(x_dist))
                {
                    if (y_dist < 0)
                        new_orientation = Cow.Orientation.Up;                    
                    else
                        new_orientation = Cow.Orientation.Down;
                }
            }

            if (cow.orientation != new_orientation)
                cow.orientation = new_orientation;
            
            // NOTE: not sure if I like the head rotating too.  it makes sense
            // but feels like the mechanic is too ... "tight"?
            headDirection = getHeadDirection(target);
        }
    }
}
