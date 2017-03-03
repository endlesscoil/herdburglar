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
