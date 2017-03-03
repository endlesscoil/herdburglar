using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nez;
using Nez.Sprites;

using static herdburglar.Cow;

namespace herdburglar.Components.Controllers
{
    class CowController : Component, IUpdatable
    {
        #region Static
		public static Dictionary<Cow.Orientation, Vector2> orientationToFacingDirection = new Dictionary<Cow.Orientation, Vector2>
        {
			{ Cow.Orientation.Up, new Vector2(0, -1) },
			{ Cow.Orientation.Left, new Vector2(-1, 0) },
			{ Cow.Orientation.Down, new Vector2(0, 1) },
			{ Cow.Orientation.Right, new Vector2(1, 0) }
        };
        #endregion

        #region Enums
		public enum HeadRotation {
			None = 0,
			Left = 1,
			Right = 2,
			Down = 1,
			Up = 2
		}
        #endregion

        public float fovAngle = MathHelper.Pi / 8; // 90 degrees
        public float alertDistance = 250f;
        public float dangerDistance = 175;

        private Cow cow = null;
        private float computedFOVAngle = 0f;
        private Vector2 headDirection = Vector2.Zero;

        #region Events
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            cow = (Cow)entity;
            computedFOVAngle = Mathf.cos(fovAngle);
			headDirection = orientationToFacingDirection[cow.orientation];
        }

        void IUpdatable.update()
        {
            var facingDirection = orientationToFacingDirection[cow.orientation];        // FIXME: I don't like how this is currently done.

			// Draw facing indicator
			if (Core.debugRenderEnabled)
				Debug.drawLine(entity.transform.position, entity.transform.position + ((facingDirection + headDirection) * 100), Color.Blue, 0.5f);

            // Scan for any threats
            var threat = scanForThreats(facingDirection);
        }
        #endregion

        #region Private
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
        #endregion
    }
}
