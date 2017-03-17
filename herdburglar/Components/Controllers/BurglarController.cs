using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Sprites;
using Nez.Textures;

namespace herdburglar.Components.Controllers
{
    class BurglarController : Component, IUpdatable, ITriggerListener
    {
        public float speed = 150f;

        private Mover mover;
        private Sprite<Burglar.Animations> animation;

        #region Events
        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            mover = entity.getOrCreateComponent<Mover>();
            animation = entity.getComponent<Sprite<Burglar.Animations>>();
        }

        void IUpdatable.update()
        {
            var movement = processInput();

            updateAnimation(movement);
		
            // Move!
            CollisionResult result;
            if (mover.move(movement * Time.deltaTime * speed, out result))
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
                other.entity.enabled = false;
				Core.schedule(1f, timer => other.entity.destroy());
            }
        }

        void ITriggerListener.onTriggerExit(Collider other, Collider local)
        {

        }
        #endregion

        #region Private
        private Vector2 processInput() 
        {
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
                Process.GetCurrentProcess().Kill();     // HACK: Linux and escape keypress doesn't work right.
                #else
                Core.exit();
                #endif
            }

            // TEMP
            if (Input.isKeyPressed(Keys.Space))
            {
                var firecracker = new Firecracker() { duration = 5f, delay = 2f, propagationTime = 0.25f, alertRadius = 200f };
                firecracker.transform.position = Input.mousePosition;

                entity.scene.addEntity(firecracker);
            }

            // TEMP
            if (Input.isKeyPressed(Keys.V))
            {
                var decoyVelocity = new Vector2(2, 0);
                if (animation.flipX)
                    decoyVelocity.X = -decoyVelocity.X;
                
                var decoy = new Decoy() { duration = 5f, delay = 2f, velocity = decoyVelocity };
                decoy.transform.position = entity.transform.position + new Vector2(animation.width * 0.75f * (decoyVelocity.X < 0 ? -1 : 1), 0);

                entity.scene.addEntity(decoy);
            }

            return movement;
        }

        private void updateAnimation(Vector2 movement)
        {
            if (movement == Vector2.Zero)
            {
                if (animation.isAnimationPlaying(Burglar.Animations.Walk))
                    animation.play(Burglar.Animations.Idle);
            }
            else
            {
                animation.flipX = movement.X < 0;   // flip depending on movement direction.

                if (!animation.isAnimationPlaying(Burglar.Animations.Walk))
                    animation.play(Burglar.Animations.Walk);
            }
        }
        #endregion
    }
}
