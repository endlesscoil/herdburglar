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
        public static Dictionary<Orientation, Vector2> orientationToFacingDirection = new Dictionary<Orientation, Vector2>
        {
            { Orientation.Up, new Vector2(0, 1) },
            { Orientation.Left, new Vector2(1, 0) },
            { Orientation.Down, new Vector2(0, -1) },
            { Orientation.Right, new Vector2(-1, 0) }
        };

        private Cow cow = null;
        private float fovAngle = MathHelper.Pi / 4; // 45deg
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
            var burglar = entity.scene.findEntitiesWithTag((int)Tags.Burglar);
            if (burglar.Count > 0)
            {
                var distance = Vector2.Distance(entity.transform.position, burglar[0].transform.position);
                if (distance < alertDistance)
                {
                    var facingDirection = orientationToFacingDirection[cow.orientation];
                    var vector_to_burglar = Vector2.Normalize(entity.transform.position - burglar[0].transform.position);
                    var dot = Vector2.Dot(facingDirection, vector_to_burglar);

                    if (dot > _computedAngle)
                    {
                        var raycastHit = Physics.linecast(entity.transform.position, burglar[0].transform.position);

                        if (raycastHit.collider.entity.tag == (int)Tags.Burglar)
                        {
                            var color = distance < dangerDistance ? Color.Red : Color.Yellow;
                            Debug.drawLine(entity.transform.position, burglar[0].transform.position, color, 0.5f);

                            // TODO: Do something here for alert/danger.
                        }
                    }
                }
            }
        }
    }
}
