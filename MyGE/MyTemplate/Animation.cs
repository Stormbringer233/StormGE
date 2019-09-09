using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MyTemplate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTemplate
{
    /// <summary>
    /// Classe d'animation de texture 2D. Cette classe "découpe" en rectangles identiques uns sprite sheet
    /// Animation génère et gère UNE SEULE LIGNE D'ANIMATION dans une sprite sheet.
    /// 
    /// La classe gère 3 types d'animations :
    ///  - FORWARD => lit l'animation de la 1ere à la derniere frame
    ///  - BACWARD => lit l'animation de la derniere à la premiere frame
    ///  - AUTOREVERSE => lit l'animation de la 1ere à la dernière frame puis à la 1ère
    ///  
    /// Auteur : M. Le Thiec
    /// creation date : 18/03/2018
    /// V : 1.20
    /// 
    /// Correct the bug in update that double time between last frame to 1st frame
    /// </summary>

    public class Animation
    {
        List<Rectangle> quads; // liste de liste des frames
        Timer AnimationTimer; // manage the time between 2 frames
        Timer WaitTimer; // manage the time between 2 animations
        readonly int maxLoop; // nombre de fois que l'animation est à jouée (-1 = infini)
        int currentLoopCount; // compteur de boucle d'animation
        int totalFrameCount; // nombre total de frame dans l'animation

        public string Name { get; private set; }
        public List<Rectangle> ListHitbox { get; private set; } // list of hitbox. An animation can have multiple hitbox
        public Rectangle CurrentQuad { get; private set; } // rectangle délimitant la frame en cours
        public Vector2 Origin { get; private set; }
        public bool OnAnimation { get; protected set; }
        public bool RestartLoop { get; set; }
        public int FrameCount { get; private set; } // N° de frame en cours
        public enum Animations { NONE, FORWARD, BACKWARD, PINGPONG }
        public Animations Way;

        public Animation(int pFrameWidth, int pFrameHeight)
            // Build a single quad for the entire image
        {
            CurrentQuad = new Rectangle(0, 0, pFrameWidth, pFrameHeight);
            Way = Animations.NONE;
        }

        public Animation(AnimationDatas pDatas, int pFrameWidth, int pFrameHeight)
        // Build animation from datas contains in json file
        // FrameWith is the same for all of frame. Idem for height
        // AnimationDatas is the main class of SpriteSheetManager
        {
            Name = pDatas.Name;
            var mode = pDatas.Mode.ToUpper();
            if (mode == "FORWARD")
                Way = Animations.FORWARD;
            else if (mode == "BACKWARD")
                Way = Animations.BACKWARD;
            else if (mode == "PINGPONG")
                Way = Animations.PINGPONG;
            else
                Way = Animations.NONE; // default

            if (pDatas.Duration <= 0)
                AnimationTimer = new Timer(0.2);
            else
                AnimationTimer = new Timer(pDatas.Duration);

            if (pDatas.Delay <= 0)
                WaitTimer = new Timer();
            else
                WaitTimer = new Timer(pDatas.Delay, OnWaitTimeEnded);

            if (pDatas.Loop == -1) // -1 for constant animation or > 0 for a define number of loop
            {
                maxLoop = pDatas.Loop;
                AnimationTimer.ProcessFinish += OnContinousAnimationUpdate;
            }
            else if (pDatas.Loop > 0)
            {
                maxLoop = pDatas.Loop;
                AnimationTimer.ProcessFinish += OnCountAnimationUpdate;
            }
            else
            {
                AnimationTimer.ProcessFinish += OnContinousAnimationUpdate;
                maxLoop = -1;
            }
            totalFrameCount = pDatas.Frames;
            Origin = new Vector2(pDatas.OriginX, pDatas.OriginY);
            // Add a hitbox
            ListHitbox.Add(new Rectangle(pDatas.HitBox[0], pDatas.HitBox[1], pDatas.HitBox[2], pDatas.HitBox[3]));
            Initialize();
            //SheetCutting(pDatas.Xinit, pDatas.Yinit, pDatas.Width, pDatas.Height);
            SheetCutting(pDatas.Xinit, pDatas.Yinit, pFrameWidth, pFrameHeight);
        }

        public Animation(int[] pFrameOrder, float pFrameRate, float pDelayAtEnd, int pLoop, Vector2 pImgDims, Vector2 pFrameDims, bool pFlipH, bool pFlipV)
        {
            Way = Animations.FORWARD; // in this constructor, the FrameOrder give the entire way of animation
            double finalTimer = 0;
            if (pFrameRate <= 0)
                finalTimer = 0.2;
            else
                finalTimer = pFrameRate;

            if (pDelayAtEnd <= 0)
                WaitTimer = new Timer();
            else
                WaitTimer = new Timer(pDelayAtEnd + finalTimer, OnWaitTimeEnded);

            if (pLoop == -1 || pLoop > 0)  // -1 for constant animation or > 0 for a define number of loop
                maxLoop = pLoop;
            else
                maxLoop = -1; // si le client met un loop = 0, on joue à l'infini

            if (pLoop == -1) // -1 for constant animation or > 0 for a define number of loop
            {
                AnimationTimer = new Timer(finalTimer, OnContinousAnimationUpdate);
            }
            else if (pLoop > 0)
            {
                AnimationTimer = new Timer(finalTimer, OnCountAnimationUpdate);
            }
            else
            {
                AnimationTimer = new Timer(finalTimer, OnContinousAnimationUpdate);
            }
            totalFrameCount = pFrameOrder.Length;
            Initialize();
            // build quads
            for (int i = 0; i < totalFrameCount; i++)
            {
                int x = (pFrameOrder[i] - 1) * (int)pFrameDims.X;
                quads.Add(new Rectangle(x, 0, (int)pFrameDims.X, (int)pFrameDims.Y));
            }
            CurrentQuad = quads[0];
        }

        private void Initialize()
        {
            ListHitbox = new List<Rectangle>();
            quads = new List<Rectangle>(); // prépare la liste de quads
            currentLoopCount = 0; // commence à la 1ere boucle
            OnAnimation = false; // l'état courant de l'animation est à jouer l'animation. Doit etre false ici
            RestartLoop = true;
            FrameCount = 0;
        }

        public void AddHitBox(Rectangle pHitbox)
        {
            ListHitbox.Add(pHitbox);
        }

        private void SheetCutting(int pXinit, int pYinit, int pFrameWidth, int pFrameHeight)
        // Découpe la sprite sheet en frames et les stockes dans une liste de Rectangles
        // la spriteSheet sera découpée en rectangles de tailles identiques
        {
            //Console.WriteLine("width x height : "+pWidth+" x "+pHeight+"\tpTblframes[1] et [2] : " + pTblFrames[0] + " " + pTblFrames[1]);

            List<Rectangle> fullSheet = new List<Rectangle>();
            //Console.WriteLine("colCount x lineCount : " + columnCount + " x " + lineCount);
            for (int col = 0; col < totalFrameCount; col++)
            {
                int x = pXinit + col * pFrameWidth;
                fullSheet.Add(new Rectangle(x, pYinit, pFrameWidth, pFrameHeight));
            }
            RebuildQuads(fullSheet);
        }

        private void RebuildQuads(List<Rectangle> pFullLine)
        // traitement de la liste des frames suivant les 3 cas possibles
        // cas 1 : FORWARD => aucun traitement supplémentaire, les quads sont dans le sens frame 1 -> dernière frame
        // cas 2 : BACKWARD => inversion des quads
        // cas 3 : PINGPONG => construction de la liste - 1 frame
        // exemple : liste base 1 2 3 4  devient : 1 2 3 4 3 2 
        {
            if (Way == Animations.BACKWARD)
            {
                for (int i = 0; i < pFullLine.Count; i++)
                {
                    pFullLine.Reverse();
                }
            }
            else if (Way == Animations.PINGPONG)
            {
                List<Rectangle> endQuads = new List<Rectangle>(new List<Rectangle>(pFullLine));
                // crée une liste temporaire de rectangle dans le sens inverse
                // supprime le dernier élément qui serait sinon ensuite le même que le dernier élément de quads
                endQuads.RemoveAt(endQuads.Count - 1);
                endQuads.Reverse();
                // suppression du dernier élément
                endQuads.RemoveAt(endQuads.Count - 1);
                // contatenation de la liste inverséee au bout de la liste de base
                pFullLine.AddRange(endQuads);
                totalFrameCount = pFullLine.Count();
            }
            quads = pFullLine;
            // Le 1er élément de la liste devient le quad courant
            CurrentQuad = quads[0];
        }

        public void ResetAnimation()
        {
            FrameCount = 0;
            currentLoopCount = 0;
        }

        public void Play()
        {
            OnAnimation = true;
        }

        public void Stop()
        // TODO : ensure the animation is ended before stopping it
        {
            OnAnimation = false;
        }

        private void OnContinousAnimationUpdate(object sender, EventArgs e)
        {
            FrameCount++;
            if (FrameCount >= totalFrameCount)
            {
                FrameCount = 0;
                OnAnimation = false;
            }
            else
            {
                CurrentQuad = quads[FrameCount];
                //Console.WriteLine("CurrentQuad is " + CurrentQuad);
            }
        }

        private void OnCountAnimationUpdate(object sender, EventArgs e)
        {
            FrameCount++;
            if (FrameCount >= totalFrameCount)
            {
                FrameCount = 0;
                currentLoopCount++;
                OnAnimation = false;
            }
            else
            {
                CurrentQuad = quads[FrameCount];
            }
            if (currentLoopCount >= maxLoop)
            {
                OnAnimation = false;
                RestartLoop = false;
                ResetAnimation();
            }
        }

        private void OnWaitTimeEnded(object sender, EventArgs e)
        {
            OnAnimation = true;
            CurrentQuad = quads[FrameCount];
        }

        public void Update(GameTime gameTime)
        // Update les animations
        // on update si le temps entre chaque frame est écoulé ou si le delay est écoulé
        {
            if (Way != Animations.NONE)
            {
                if (OnAnimation)
                {
                    AnimationTimer.Update(gameTime);
                }
                if (OnAnimation == false && RestartLoop)
                {
                    if (WaitTimer.Duration > 0)
                    {
                        WaitTimer.Update(gameTime);
                    }
                    else
                    {
                        OnAnimation = true;
                        CurrentQuad = quads[FrameCount];
                    }
                }
            }
        }
    }
}
