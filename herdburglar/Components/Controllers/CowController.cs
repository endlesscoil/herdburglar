using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nez;

using Nez.Sprites;
using System.Diagnostics;


namespace herdburglar.Components.Controllers
{
    class CowController : Component, IUpdatable
    {
        private Mover mover;

        public override void onAddedToEntity()
        {
            base.onAddedToEntity();
        }

        void IUpdatable.update()
        {
        }
    }
}
