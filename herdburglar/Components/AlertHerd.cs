using Microsoft.Xna.Framework;

using Nez;

class AlertHerd : Component
{
    private float alertTime = 5f;
    private float alertRadius = 150f;
    private float propagationTime = 0.25f;

    private bool alerted = false;

    public override void onAddedToEntity ()
    {
        base.onAddedToEntity ();
    }

    public void alert(int level = 0, int max = 1)
    {
        if (level > max)
            return;

        if (alerted)
            return;

        if (Core.debugRenderEnabled)
            Debug.drawHollowBox(entity.transform.position, (int)alertRadius, Color.PaleVioletRed, alertTime);

        alerted = true;
        Core.schedule(alertTime, timer => alerted = false);

        // TODO: do stuff here when alerted.  move head, etc.

        Collider[] results = new Collider[100];
        int numOverlap = Physics.overlapCircleAll(entity.transform.position, alertRadius, results, -1);
        Debug.log("Alerting!  herdCount={0}, level={1}", numOverlap, level);

        for (int i = 0; i < numOverlap; i++)
        {
            var alerter = results[i].entity.getComponent<AlertHerd>();
            if (results[i].entity != entity && alerter != null)
            {
                Debug.log("Propagating alert to {0}", results[i].entity);
                Core.schedule(propagationTime, timer => alerter.alert(level + 1, max));
            }
        }
    }
}
