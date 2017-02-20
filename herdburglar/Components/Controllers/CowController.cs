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

        private Cow cow = null;
        private float fovAngle = MathHelper.Pi / 4; // 90 degrees
        private float _computedAngle;
        private float alertDistance = 250f;
        private float dangerDistance = 175;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            cow = (Cow)entity;
            _computedAngle = Mathf.cos(fovAngle);
        }

        void IUpdatable.update()
        {
            var facingDirection = orientationToFacingDirection[cow.orientation];

			// Draw facing indicator
			if (Core.debugRenderEnabled)
				Debug.drawLine(entity.transform.position, entity.transform.position + (facingDirection * 100), Color.Blue, 0.5f);

        	// Find the burglar
            var burglar = entity.scene.findEntitiesWithTag((int)Tags.Burglar);
            if (burglar.Count > 0)
            {
            	// Are they within alert distance?
                var distance = Vector2.Distance(entity.transform.position, burglar[0].transform.position);
                if (distance < alertDistance)
                {
					var vector_to_burglar = Vector2.Normalize(burglar[0].transform.position - entity.transform.position);
                    var dot = Vector2.Dot(facingDirection, vector_to_burglar);

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

                            // TODO: Do something here for alert/danger.
                        }
                    }
                }
            }
        }
    }
}
