using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Sprites;

using System;
using System.Collections.Generic;
using Nez.UI;
using Nez.Tiled;

namespace playground.Scenes
{
    class Test : Scene
    {
        #region Member variables
        #endregion

        #region Events
        public override void initialize()
        {
            base.initialize();

            Core.debugRenderEnabled = true;
			Transform.shouldRoundPosition = false;

            addRenderer<DefaultRenderer>(new DefaultRenderer());
        }

        public override void onStart()
        {
            base.onStart();
        }

        public override void update()
        {
            base.update();
        }
        #endregion

        #region Public interface
        #endregion

        #region Private
        #endregion
    }
}
