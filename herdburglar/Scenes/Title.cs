using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.UI;
using Nez.Tiled;
using Nez.Timers;
using Nez.Sprites;

using herdburglar.Components.Controllers;

namespace herdburglar
{
    class Title : Scene
    {
        #region Member variables
        #endregion

        #region Events
        public override void initialize()
        {
            base.initialize();

            Core.debugRenderEnabled = true;
            Transform.shouldRoundPosition = false;

            clearColor = Color.Black;

            addRenderer(new DefaultRenderer());
        }

        public override void onStart()
        {
            base.onStart();
        }

        public override void update()
        {
            base.update();

            if (Input.isKeyPressed(Microsoft.Xna.Framework.Input.Keys.Space))
            {
                Core.startSceneTransition(new WindTransition(() => new Playground()));
            }
        }
        #endregion

        #region Public interface
        #endregion

        #region Private
        #endregion
    }
}
