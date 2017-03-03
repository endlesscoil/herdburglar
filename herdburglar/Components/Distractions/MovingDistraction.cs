using Microsoft.Xna.Framework;

using Nez;
using Nez.Systems;

namespace herdburglar.Components.Distractions
{
    class MovingDistraction : Component, IUpdatable
    {
        public enum Events 
        {
            Started = 0,
            Finished = 1
        }
        public Emitter<Events> events = new Emitter<Events>();

        public float duration = 5f;
        public float delay = 0f;
        public Vector2 velocity = Vector2.Zero;

        private float startTime = 0f;
        private bool distracting = false;
        private bool collided = false;
        private Mover mover;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            mover = entity.getOrCreateComponent<Mover>();

            startTime = Time.time;
            entity.tag = (int)Tags.Distraction;
        }

        void IUpdatable.update()
        {
            if (Time.time > startTime + delay)
            {
                if (!distracting)
                {
                    distracting = true;
                    events.emit(Events.Started);
                }

                CollisionResult result;
                if (mover.move(velocity, out result))
                {
                    collided = true;
                }

                if (collided || Time.time >= startTime + delay + duration)
                {
                    events.emit(Events.Finished);

                    entity.destroy();
                }
            }
        }
    }
}