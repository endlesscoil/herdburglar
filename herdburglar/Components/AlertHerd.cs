using Microsoft.Xna.Framework;

using Nez;


namespace herdburglar.Components.Controllers
{
    class AlertHerd : Component
    {
        private float alertTime = 2f;
        private float alertRadius = 150f;
        private float propagationTime = 0.25f;

        private bool alerted = false;

        private Cow cow;

        public override void onAddedToEntity ()
        {
            base.onAddedToEntity ();

            cow = (Cow)entity;
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
            Core.schedule(alertTime, timer => alerted = false);

            // NOTE: maybe do something with an event here instead.
            var cow_controller = entity.getComponent<CowController>();
            cow_controller.rotateTowards(target);

            cow.moo();

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