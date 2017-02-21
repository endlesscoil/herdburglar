﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nez;

using Nez.Sprites;
using System.Diagnostics;


namespace herdburglar.Components.Controllers
{
    class BurglarController : Component, IUpdatable, ITriggerListener
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

			if (Input.isKeyPressed(Keys.Escape))
			{
				#if __MonoCS__
				Process.GetCurrentProcess().Kill();		// HACK: Linux and escape keypress doesn't work right.
				#else
				Core.exit();
				#endif
			}

			var anim = entity.getComponent<Sprite<Burglar.Animations>>();
		
            // Make sure our movement is valid
            if (movement.Length() == 0)
            {
				if (anim.isAnimationPlaying(Burglar.Animations.Walk))
					anim.play(Burglar.Animations.Idle);

                return;
            }

            // Update animation
			anim.flipX = movement.X < 0;
            if (!anim.isAnimationPlaying(Burglar.Animations.Walk))
				anim.play(Burglar.Animations.Walk);

            // Move!
            CollisionResult result;
            if (mover.move(movement * Time.deltaTime * 150f, out result))
            {
                Nez.Debug.log("Collided.");
            }
        }


        void ITriggerListener.onTriggerEnter(Collider other, Collider self)
        {
            if (other.entity.tag == (int)Tags.Idol)
            {
                Nez.Debug.log("Got golden idol!");

				// NOTE: Works okay, weird jumpiness, though.
				Core.schedule(0.5f, timer => other.entity.destroy());
            }
        }

        void ITriggerListener.onTriggerExit(Collider other, Collider local)
        {

        }
    }
}
