using Microsoft.Xna.Framework;

using Nez;
using Nez.Systems;

namespace herdburglar.Components.Controllers
{
    class AlertHerd : Component
    {
        public enum Events
        {
            Alerted,
            Settled
        }

        private float alertTime = 2f;
        private float alertRadius = 150f;
        private float propagationTime = 0.25f;

        private bool alerted = false;

        public Emitter<Events,Entity> events = new Emitter<Events,Entity>();

        public override void onAddedToEntity ()
        {
            base.onAddedToEntity ();
        }

        public void alert(Entity target, int level = 0, int max = 1)
        {
            if (level > max)
                return;

            if (alerted)
                return;

            if (Core.debugRenderEnabled)
                Debug.drawHollowBox(entity.transform.position, (int)alertRadius*2, Color.PaleVioletRed, alertTime);

            alerted = true;
            events.emit(Events.Alerted, target);

            Core.schedule(alertTime, timer => {
                alerted = false;
                events.emit(Events.Settled, null);
            });

            alertNearby(target, level, max);
        }

        private void alertNearby(Entity target, int level = 0, int max = 1)
        {
            Collider[] results = new Collider[100];
            int numOverlap = Physics.overlapCircleAll(entity.transform.position, alertRadius, results, -1);

            for (int i = 0; i < numOverlap; i++)
            {
                var alerter = results[i].entity.getComponent<AlertHerd>();

                if (results[i].entity != entity && alerter != null)
                    Core.schedule(propagationTime, timer => alerter.alert(target, level + 1, max));
            }
        }
    }
}