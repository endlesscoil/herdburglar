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
        private BullController controller = null;
        private BullController.Orientation orientation;

        public Bull(BullController.Orientation orientation = BullController.Orientation.Right)
        {
            this.orientation = orientation;
        }

        #region Events
        public override void onAddedToScene()
        {
            base.onAddedToScene();

            tag = (int)Tags.Bull;
            controller = addComponent(new BullController() { orientation = orientation });
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
