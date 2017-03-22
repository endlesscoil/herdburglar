using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Sprites;
using Nez.Textures;

using herdburglar.Components.Controllers;

namespace herdburglar
{
    public class Bull : Entity
    {
        private CowController controller = null;
        private CowController.Orientation orientation;

        public Bull(CowController.Orientation orientation = CowController.Orientation.Right)
        {
            this.orientation = orientation;
        }

        #region Events
        public override void onAddedToScene()
        {
            base.onAddedToScene();

            tag = (int)Tags.Bull;
            controller = addComponent(new CowController() { orientation = orientation, watchForDistractions = false });
        }

        public override void onRemovedFromScene()
        {
            base.onRemovedFromScene();
        }

        public override void update()
        {
            base.update();
        }
        #endregion
    }
}
