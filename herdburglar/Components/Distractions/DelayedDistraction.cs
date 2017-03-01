using Microsoft.Xna.Framework;

using Nez;
using Nez.Systems;


namespace herdburglar.Components.Distractions
{
    class DelayedDistraction : Component, IUpdatable
    {
        public enum Events 
        {
            Started = 0,
            Finished = 1
        }
        public Emitter<Events> events = new Emitter<Events>();

        public float duration = 5f;
        public float delay = 0f;

        private float startTime = 0f;
        private bool distracting = false;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

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

                if (Time.time >= startTime + delay + duration)
                {
                    events.emit(Events.Finished);

                    entity.destroy();
                }
            }
        }
    }
}