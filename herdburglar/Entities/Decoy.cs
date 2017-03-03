using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Textures;
using Nez.Sprites;

using herdburglar.Components.Distractions;

namespace herdburglar
{
    public class Decoy : Entity
    {
        #region Enums
        public enum Animations
        {
            Waiting,
            Walking,
            FellOver
        }
        #endregion

        public float duration;
        public float delay;
        public Vector2 velocity;

        private Sprite<Animations> animation;
        private BoxCollider collider;
        private MovingDistraction distraction;
        private SoundEffectInstance sound;

        #region Events
        public override void onAddedToScene()
        {
            base.onAddedToScene();

            /*          
            sound = scene.content.Load<SoundEffect>("sound/firecracker").CreateInstance();
            sound.IsLooped = true;
            sound.Volume = 0.25f;
            */

            tag = (int)Tags.Distraction;
            collider = addComponent<BoxCollider>();

            distraction = addComponent(new Components.Distractions.MovingDistraction() { duration = duration, delay = delay, velocity = velocity });
            distraction.events.addObserver(MovingDistraction.Events.Started, () => animation.play(Animations.Walking));
            //distraction.events.addObserver(MovingDistraction.Events.Finished, () => { });

            setupAnimations();

            animation.play(Animations.Waiting);
        }
        #endregion

        #region Private
        private void setupAnimations()
        {
            var texture = scene.content.Load<Texture2D>("sprites/kit_from_firefox");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 56, 80);

            animation = addComponent(new Sprite<Animations>(subtextures[0]));
            animation.transform.scale = new Vector2(0.5f, 0.5f);
            animation.color = Color.LightSlateGray;

            animation.addAnimation(Animations.Waiting, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[0],
                subtextures[1],
                subtextures[2]
            }).setFps(3));

            animation.addAnimation(Animations.Walking, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[4],
                subtextures[5],
                subtextures[6]
            }).setFps(7));

            animation.addAnimation(Animations.FellOver, new SpriteAnimation(new List<Subtexture>()
            {
                subtextures[0]
            }));
        }
        #endregion
    }
}
