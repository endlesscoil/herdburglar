using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nez;

namespace herdburglar.Components.Controllers
{
    class IdolController : Component, ITriggerListener
    {
        private BoxCollider collider;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            collider = new BoxCollider(32, 32);
            collider.isTrigger = true;

            entity.addComponent(collider);
        }

        void ITriggerListener.onTriggerEnter(Collider other, Collider self)
        {
            if (other.entity.tag == (int)Tags.Burglar)
            {
                Debug.log("Idol picked up by burglar!");

                // TODO: game win, etc.

                // NOTE: Why does this crash shit?
                //self.entity.destroy();
            }
        }

        void ITriggerListener.onTriggerExit(Collider other, Collider local)
        {

        }
    }
}
