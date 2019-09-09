using Content.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public class SpritesRenderer
    {
        /// <summary>
        /// This class is usefull to rendering all of the sprites contains into a scene.
        /// It's also apply some shaders by batches
        /// </summary>
        /// M. Le Thiec
        /// 22/07/2019
        /// 

        List<GameObject> ListObjects;
        Dictionary<string, ShaderEffect> ListShaders;
        ShaderEffect CurrentEffect;

        public SpritesRenderer()
        {
            ListShaders = new Dictionary<string, ShaderEffect>();
            ListShaders = new Dictionary<string, ShaderEffect>()
            {
                {"DEFAULT", new DefaultEffect() },
            };
            CurrentEffect = ListShaders["DEFAULT"]; // load simple basic effect
            ListObjects = new List<GameObject>();
        }

        public void AddEffect(string pName, ShaderEffect pEffect)
        {
            pName = pName.ToUpper();
            if (ListShaders.ContainsKey(pName))
                return;
            ListShaders.Add(pName, pEffect);
        }

        public void AssignEffect(string pName)
        {
            pName = pName.ToUpper();
            if (ListShaders.ContainsKey(pName))
                CurrentEffect = ListShaders[pName];
        }

        public void ResetShader()
        {
            CurrentEffect = ListShaders["DEFAULT"];
        }

        public void AddSprite(GameObject pSprite)
        {
            // do not allow to add the same sprite twice
            if (ListObjects.Contains(pSprite))
                return;
            ListObjects.Add(pSprite);
        }

        public void RemoveSprite(GameObject pSprite)
        {
            if (ListObjects.Contains(pSprite))
            {
                ListObjects.Remove(pSprite);
            }
        }

        public void Update(GameTime gameTime)
        {
            CurrentEffect.Update(gameTime);
            foreach (GameObject sprite in ListObjects)
            {
                sprite.Update(gameTime);
            }
        }

        public void Draw(SpriteBatch sb, GameTime gameTime)
        {
            // First close the in progress SpriteBatch
            CurrentEffect.Shader.CurrentTechnique.Passes[0].Apply();
            // now open a new spritebatch to apply current effect for sprites
            //sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, effect: CurrentEffect.Shader);
            // course the list of sprites
            foreach (GameObject sprite in ListObjects)
            {
                sprite.Draw(sb, gameTime);
            }
            //sb.End();
        }

        public void DrawWithGlobalEffect(SpriteBatch sb, GameTime gameTime)
        {
            foreach (GameObject sprite in ListObjects)
            {
                sprite.Draw(sb, gameTime);
            }
        }

    }
}
