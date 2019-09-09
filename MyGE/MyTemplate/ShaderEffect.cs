using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    public abstract class ShaderEffect
    {
        /// ShaderEffect class store the effect with it's iwn parameters and update
        /// 
        /// M. Le Thiec
        /// 22/07/2019
        /// 

        protected string Name;
        public Effect Shader { get; protected set; }
        protected Timer timer;

        public ShaderEffect(string pShaderName)
        {
            Name = pShaderName.ToUpper();
            Load(pShaderName);
        }

        private void Load(string pName)
        {
            Shader = AssetManager.LoadEffect(pName);
        }

        public virtual void Update(GameTime gameTime) { }
        public virtual void Update(GameTime gameTime, Texture2D mask) { }

    }
}
