using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nez;

namespace herdburglar.Components.Controllers
{
    class IdolController : Component
    {
        private BoxCollider collider;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();

            collider = new BoxCollider(32, 32);
            collider.isTrigger = true;

            entity.addComponent(collider);
        }
    }
}
