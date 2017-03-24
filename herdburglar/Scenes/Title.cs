using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using Nez;
using Nez.UI;
using Nez.Sprites;

namespace herdburglar.Scenes
{
    class Title : Scene
    {
        #region Member variables
        private UICanvas canvas;
        #endregion

        #region Events
        public override void initialize()
        {
            base.initialize();

            Core.debugRenderEnabled = true;
            Transform.shouldRoundPosition = false;

            clearColor = Color.Black;

            canvas = createEntity("ui").addComponent(new UICanvas());
            canvas.isFullScreen = true;
            canvas.renderLayer = 0;

            var table = canvas.stage.addElement(new Table());


            table.setFillParent(true).center();

            table.add("HerdBurglar");
            table.row().setPadTop(50);
            table.add("Press space to start..");


            addRenderer(new DefaultRenderer());
        }

        public override void onStart()
        {
            base.onStart();
        }

        public override void update()
        {
            base.update();

            if (Input.isKeyPressed(Keys.Space))
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
