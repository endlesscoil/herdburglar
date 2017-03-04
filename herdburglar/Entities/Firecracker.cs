using System.Collections.Generic;

using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using Nez;
using Nez.Textures;
using Nez.Sprites;

using herdburglar.Components.Distractions;
using herdburglar.Components.Controllers;

namespace herdburglar
{
    public class Firecracker : Entity
    {
        #region Enums
        public enum Animations
        {
            Fuse = 0,
            Exploding = 1
        }
        #endregion

        public float duration;
        public float delay;
        public float propagationTime;
        public float alertRadius;

        private Sprite<Animations> animation;
        private BoxCollider collider;
        private DelayedDistraction distraction;
        private SoundEffectInstance sound;
        private AlertHerd alerter;

        #region Events
        public override void onAddedToScene()
        {
            base.onAddedToScene();

            tag = (int)Tags.Idol;

            sound = scene.content.Load<SoundEffect>("sound/firecracker").CreateInstance();
            sound.IsLooped = true;
            sound.Volume = 0.25f;

            collider = addComponent<BoxCollider>();
            alerter = addComponent<AlertHerd>();
            distraction = addComponent(new Components.Distractions.DelayedDistraction() { duration = duration, delay = delay });
            distraction.events.addObserver(DelayedDistraction.Events.Started, () => {
                animation.play(Animations.Exploding);
                sound.Play();

                alerter.alert(this);
            });

            distraction.events.addObserver(DelayedDistraction.Events.Finished, () => {
                sound.Stop();

                destroy();
            });

            setupAnimations();
            animation.play(Animations.Fuse);
        }

        public override void onRemovedFromScene()
        {
            base.onRemovedFromScene();
        }
        #endregion

        #region Private
        private void setupAnimations()
        {
            var texture = scene.content.Load<Texture2D>("sprites/flames");
            var subtextures = Subtexture.subtexturesFromAtlas(texture, 16, 18);

            animation = addComponent(new Sprite<Animations>(subtextures[0]));
            animation.addAnimation(Animations.Fuse, new SpriteAnimation(new List<Subtexture>()
                {
                    subtextures[0]
                }));

            animation.addAnimation(Animations.Exploding, new SpriteAnimation(new List<Subtexture>()
                {
                    subtextures[1],
                    subtextures[2],
                    subtextures[3],
                    subtextures[4],
                    subtextures[5],
                    subtextures[6],
                    subtextures[7],
                    subtextures[8],
                    subtextures[9],
                    subtextures[10],
                    subtextures[11]
                }));
        }
        #endregion
    }
}
