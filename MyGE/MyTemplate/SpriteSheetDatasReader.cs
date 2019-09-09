using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft;

namespace MyTemplate
{
    public class SpriteSheetDatasReader
    {
        public CharacterDatas CharacterDatas;
        public SpriteDatas SpriteDatas;
    }

    public class SpriteDatas
    {
        public string SpriteSheet;
        public string Orientation;
        public float VelocityX;
        public float VelocityY;
        public int FrameWidth;
        public int FrameHeight;
        public byte[] TextureColor = new byte[4];
        public List<AnimationDatas> AnimDatas = new List<AnimationDatas>();

        public AnimationDatas GetDatas(string pName)
        {
            foreach (AnimationDatas datas in AnimDatas)
            {
                if (datas.Name.ToUpper() == pName.ToUpper())
                {
                    return datas;
                }
            }
            return null;
        }

        public IEnumerator<AnimationDatas> GetEnumerator()
        {
            List<AnimationDatas> datas = new List<AnimationDatas>();
            foreach (AnimationDatas d in AnimDatas)
            {
                datas.Add(d);
            }
            return datas.GetEnumerator();
        }
    }

    public class CharacterDatas
    {
        public string SpriteType;
       
        public int HealthPoint;
    }

    public class AnimationDatas
    {
        public string Name;
        public string Mode;
        public double Duration;
        public double Delay;
        public int Loop;
        public int OriginX;
        public int OriginY;
        public int Xinit;
        public int Yinit;
        public int Frames;
        //public List<List<int>> HitBoxes = new List<List<int>>(); // 1 hitbox possible par frame
        public List<int> HitBox = new List<int>(); // single rectangle for all of the frames

        //public List<List<int>> GetHitBoxes(int pFrameNum)
        //{
        //    if (pFrameNum >= 0 && pFrameNum < HitBoxes.Count)
        //    {
        //        return HitBoxes[]
        //    }
        //    return null;
        //}

    }

    public class TextureDatas
    {
        public string Name;
        public int[] Quad = new int[4];
    }
}
