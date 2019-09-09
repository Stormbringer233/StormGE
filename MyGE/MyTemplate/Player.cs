using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Media;
using Newtonsoft.Json;
using MyGE;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework.Graphics;

namespace MyTemplate
{
    public class Player : GameObject
    {
        readonly int Scale;
        readonly int CellDim;
        public enum Directions { UP, RIGHT, DOWN, LEFT };
        Directions direction;
        Tween MoveTween;
        Vector2 NextPosition;
        int MaxLife = 5;
        Stack<Sprite> Life;
        Song TankSound;

        public delegate void PlayerDestroyEventHandler(object sender, EventArgs e);
        public event PlayerDestroyEventHandler PlayerDestroy;

        public enum Rotations { LEFT, RIGHT}
        public Rotations Rotation;
        public Point Position;
        public Dictionary<Directions, int[]> RotateCount;

        public Player(Vector2 pPosition)
            // pPosition [col, line]
        {
            //Debug.WriteLine("player position at (col, line) : " + pPosition);
            Scale = (int)MainConfig.ConfigDatas.Scale;
            //string fullpath = MainConfig.ConfigDatas.GameObjectsFolder + "Player.json";
            //string animJSONFile = File.ReadAllText(fullpath);
            //SpriteSheetDatasReader Datas = JsonConvert.DeserializeObject<SpriteSheetDatasReader>(animJSONFile);

            //CellDim = Datas.SpriteDatas.FrameWidth;
            //float spriteX = (Position.X + 1 ) * Scale * CellDim;
            //float spriteY = (Position.Y + 1 ) * Scale * CellDim;
            //Sprite = new Sprite(Datas.SpriteDatas, new Vector2(spriteX, spriteY));
            Sprite = new Sprite("Axel_idle-sheet", pPosition, Sprite.Anchors.CENTER);
            Sprite.AddAnimation("IDLE", new int[] { 1, 2, 3, 2 }, 0.1f, 0, -1, new Vector2(43, 77), false, false);
            Sprite.SetAnimation("IDLE");
            Sprite.PlayAnimation();
            Sprite.SetScale(4);

            direction = Directions.RIGHT;
            RotateCount = new Dictionary<Directions, int[]>()
            {
                // up, right, down, left
                {Directions.UP, new int[] {0, 1, 2, 3} },
                {Directions.RIGHT, new int[] {3, 0, 1, 2 } },
                {Directions.DOWN, new int[] {2, 3, 0, 1} },
                {Directions.LEFT, new int[] {1, 2, 3, 0} }
            };

            MoveTween = new Tween(EaseFunc.Linear, 1, OnMoveFinish);
            //MaxLife = 10; // the number of units in player's army
            Life = new Stack<Sprite>();
        }

        public void Initialize(Point pPosition)
            // For re-initializing during game
        {

        }

        public Vector2 GetSpritePosition()
        {
            return Sprite.Position;
        }

        public void Rotate(Rotations pRotation)
        {
            if (pRotation == Rotations.LEFT)
                Sprite.RotateAntiClock();
            else
                Sprite.RotateClock();
        }

        public void PointTo(Directions pNewDir)
        {
            if (!MoveTween.InTween)
            {
                int count = RotateCount[direction][(int)pNewDir];
                for (int i = 0; i < count; i++)
                    Sprite.RotateClock();
                direction = pNewDir;
            }
        }

        public Vector2 GetScreenPosition()
        {
            return Sprite.Position;
        }

        public void Hit(int pLostLife)
        {
            bool alive = true;
            if (pLostLife > 0)
            {
                if (pLostLife < Life.Count)
                {
                    Life.Pop();
                }
                else
                {
                    Life.Clear();
                    alive = false;
                }
            }
            if (!alive)
                PlayerDestroy?.Invoke(this, EventArgs.Empty);
        }

        public void Move(int pDeltaCol, int pDeltaLine)
            // Get delta in term of column and line.
            // one of them must be 0
        {
            if (MoveTween.InTween == false)
            {
                // engage tweening
                InitializeMove(pDeltaCol, pDeltaLine);
            }
        }

        private void InitializeMove(int pCol, int pLine)
        {
            if (pCol != 0)
                MoveTween.Initialize(Sprite.Position.X, pCol * CellDim * Scale);

            else if (pLine != 0)
                MoveTween.Initialize(Sprite.Position.Y, pLine * CellDim * Scale);
        }

        public void OnMoveFinish(object sender, EventArgs e)
        {
        }

        public override void Update(GameTime gameTime)
        {
            Sprite.Update(gameTime);
            if (MoveTween.InTween == true)
            {
                if (direction == Directions.LEFT || direction == Directions.RIGHT)
                {
                    var x = (float)MoveTween.DeltaValue;
                    Sprite.Move(new Vector2(x, 0));
                }
                else
                {
                    var y = (float)MoveTween.DeltaValue;
                    Sprite.Move(new Vector2(0, y));
                }
                MoveTween.Update(gameTime);
            }
        }

        public override void Draw(SpriteBatch sb, GameTime gameTime)
        {
            Sprite.Draw(sb, gameTime);
        }
    }
}
