﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nez;

using Nez.Sprites;


namespace herdburglar.Components.Controllers
{
    class BurglarController : Component, IUpdatable
    {
        private Mover mover;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            mover = entity.getOrCreateComponent<Mover>();
        }

        void IUpdatable.update()
        {
            // Movement input
            var movement = Vector2.Zero;
            if (Input.isKeyDown(Keys.A))
                movement.X = -1f;
            else if (Input.isKeyDown(Keys.D))
                movement.X = 1f;

            if (Input.isKeyDown(Keys.W))
                movement.Y = -1f;
            else if (Input.isKeyDown(Keys.S))
                movement.Y = 1f;

            // Make sure our movement is valid
            if (movement.Length() == 0)
                return;

            // Move!
            CollisionResult result;
            if (mover.move(movement * Time.deltaTime * 150f, out result))
            {
                Debug.log("Collided.");
            }
        }
    }
}
