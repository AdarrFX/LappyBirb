using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System;

namespace JustGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        // Textures and sprites
        Texture2D skyBackground;
        Texture2D birbSprite;
        Texture2D grillSprite;
        Texture2D groundSprite;
        Texture2D mountainSprite;

        // Fonts
        SpriteFont TTfont;
        Vector2 TTFposition = new Vector2(200, 100);

        Vector2 birbPosition, grillPosition;
        Vector2 groundPosition = new Vector2(0, 400);
        Rectangle birbRec, birbRecDrawingOffset, birbAnimationWindow, grillRec;

        float birbAlpha, grillAlpha = 1.0f;
        float birbRotation, grillRotation = 0.0f;

        float birbVelocity, gravityAcceleration;

        Vector2 birbOrigin = new Vector2(0, 0);
        Vector2 grillOrigin = new Vector2(100, 100);

        float birbScale = 1.0f;
        float grillScale = 0.5f;

        KeyboardState previousState;

        float zDepth = 0.10f;

        // Barrier list
        List<Rectangle> barrierList = new List<Rectangle>();

        // Parallax scroll lists
        List<Vector2> groundTiles, mountainTiles = new List<Vector2>();

        bool collisionDetected = false;
        bool beginFlapAnimation = false;

        float currentTime = 0f;
        Color testColor = Color.CornflowerBlue;
        
        Animation birbFlap;
        
        //Animation variables
        int[] birbAnimTimings, birbSheetPositions;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            Window.Title = "gaem";

        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            skyBackground = Content.Load<Texture2D>("sky");
            birbSprite = Content.Load<Texture2D>("lape-sheet");
            grillSprite = Content.Load<Texture2D>("grill");
            groundSprite = Content.Load<Texture2D>("ground");
            mountainSprite = Content.Load<Texture2D>("mountain");

            TTfont = Content.Load<SpriteFont>("PressStart2P-Regular");

            // Animation maps
            birbAnimTimings = new int[3]{ 0, 150, 250 };
            birbSheetPositions = new int[3]{ 0, 23, 46 };

            birbRec = new Rectangle(100, 100, 46, 44);
            birbAnimationWindow = new Rectangle(0, 0, 23, 22);

            grillRec = new Rectangle(200, 200, grillSprite.Width/4, grillSprite.Height/4);
            birbOrigin = new Vector2(12, 11);
            barrierList.Add(grillRec);

            birbVelocity = 0;
            gravityAcceleration = 0.25f;

            groundTiles = generateParallaxTilemap(groundSprite, this.GraphicsDevice.Viewport.Width, 3, 400);
            mountainTiles = generateParallaxTilemap(groundSprite, this.GraphicsDevice.Viewport.Width, 2, 300);

            birbFlap = new Animation(birbAnimTimings, birbSheetPositions, false);

        // TODO: use this.Content to load your game content here
    }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            //Input handling

            collisionDetected = false;
            KeyboardState keyState = Keyboard.GetState();

            if (keyState.IsKeyDown(Keys.Right))
                birbRec.X += 5;
            if (keyState.IsKeyDown(Keys.Left))
                birbRec.X -= 5;
            if (keyState.IsKeyDown(Keys.Up))
                birbRec.Y -= 5;
            if (keyState.IsKeyDown(Keys.Down))
                birbRec.Y += 5;

            if (keyState.IsKeyDown(Keys.Space) && !previousState.IsKeyDown(Keys.Space))
            {
                birbVelocity = -7.0f;
                birbRotation = 0;
                previousState = keyState;

                birbFlap.resetAnimation();
            } else
            {
                previousState = keyState;
            }

            // TODO: Add your update logic here

            birbVelocity += gravityAcceleration;
            birbRec.Y += (int)birbVelocity;

            birbRotation = birbVelocity * 0.12f;           


            if (birbRec.Y > 440)
            {
                birbRec.Y = 440;
                birbVelocity = 0;
                birbRotation = 0;
            }

            // Offset positioning fix for Lappy birbsprite to make his collision rectangle match drawn sprite position
            birbRecDrawingOffset = birbRec;
            birbRecDrawingOffset.Y += (int)birbOrigin.Y * 2;
            birbRecDrawingOffset.X += (int)birbOrigin.X * 2;


            // check for collisions
            foreach (Rectangle rect in barrierList)
            {
                if (checkBoxCollision(birbRec, rect))
                {
                     collisionDetected = true;
                }
            }

            // Advance barriers
            for (int i = 0; i < barrierList.Count; i++)
            {
                Rectangle newRect = new Rectangle(barrierList[i].X - 3, barrierList[i].Y, barrierList[i].Width, barrierList[i].Height);
                barrierList[i] = newRect;
            }

            if (barrierList[0].X < -200)
            {
                barrierList.RemoveAt(0);
            }

            if (900 - barrierList[barrierList.Count-1].X > 400)
            {
                addObstacle(grillSprite, barrierList);
            }

            //Update parallax scroll positions
            updateParallaxScroll(groundSprite, groundTiles, 3.0f, 3);
            updateParallaxScroll(mountainSprite, mountainTiles, 0.9f, 2);

            birbFlap.updateTimer(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {

            if (collisionDetected)
            {
                GraphicsDevice.Clear(Color.Red);
            }
            else
            {
                GraphicsDevice.Clear(testColor);
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);

            spriteBatch.Draw(skyBackground, new Vector2(0, 0), Color.White);

            spriteBatch.Draw(skyBackground, new Vector2(), null, Color.White, 0.0f, new Vector2(), 4.0f, SpriteEffects.None, 0.1f);

            spriteBatch.DrawString(TTfont, currentTime.ToString(), new Vector2(-1000, 100), Color.Black);

            foreach (Vector2 mountPos in mountainTiles)
            {
                spriteBatch.Draw(mountainSprite, mountPos, null, Color.White, 0.0f, new Vector2(), 2.0f, SpriteEffects.None, 0.1f);
            }
            foreach (Rectangle obstacle in barrierList)

            {
                spriteBatch.Draw(grillSprite, obstacle, Color.White);
            }

            spriteBatch.Draw(birbSprite, birbRecDrawingOffset, birbFlap.spriteSheetPosition(), Color.White, birbRotation, birbOrigin, SpriteEffects.None, 0.5f);

            foreach (Vector2 groundPos in groundTiles)
            {
                spriteBatch.Draw(groundSprite, groundPos, null, Color.White, 0.0f, new Vector2(), 3.0f, SpriteEffects.None, 0.1f);
            }

            spriteBatch.End();
            base.Draw(gameTime);

            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public static bool checkBoxCollision(Rectangle boxA, Rectangle boxB)
        {
            return boxA.Left < boxB.Right &&
                    boxA.Right > boxB.Left &&
                    boxA.Top < boxB.Bottom &&
                    boxA.Bottom > boxB.Top;
        }

        public static List<Rectangle> addObstacle(Texture2D obstacleSprite, List<Rectangle> barrierList)
        {
            Random r = new Random();

            int rando = r.Next(0, 200);

            Rectangle upperObstacle = new Rectangle(900, -200 + rando, obstacleSprite.Width / 4, obstacleSprite.Height / 4);
            Rectangle lowerObstacle = new Rectangle(900, 200 + rando, obstacleSprite.Width / 4, obstacleSprite.Height / 4);

            barrierList.Add(upperObstacle);
            barrierList.Add(lowerObstacle);

            return barrierList;

        }

        public static List<Vector2> updateParallaxScroll(Texture2D tile, List<Vector2> positionList, float speed, int scaleFactor)
        {
            float largestXPosition = 0;

            for(int i=0; i < positionList.Count; i++)
            {
                Vector2 newVec = new Vector2(positionList[i].X - speed, positionList[i].Y);
                positionList[i] = newVec;
                
                if (positionList[i].X > largestXPosition)
                {
                    largestXPosition = positionList[i].X;
                }
            }

            for (int i = 0; i < positionList.Count; i++)
            {
                if (positionList[i].X <= -(tile.Width - 1) * scaleFactor)
                {
                    Vector2 newReplacementVec = new Vector2(tile.Width * scaleFactor + largestXPosition, positionList[i].Y);
                    positionList[i] = newReplacementVec;
                }
            }

            return positionList;
        }

        public static List<Vector2> generateParallaxTilemap(Texture2D tile, int screenWidth, int scale, int yPosition)
        {
            int numberOfTiles = (int)(screenWidth / (tile.Width * scale)) + 2;
            List<Vector2> tilemap = new List<Vector2>();
            int tilePosition = 0;

            for (int i = 0; i < numberOfTiles; i++)
            {
                tilemap.Add(new Vector2(tilePosition, yPosition));
                tilePosition += tile.Width * scale;
            }
            return tilemap;
        }

        public class Animation
        {
            float currentTime;
            int[] timings;
            int[] sheetPositions;
            bool doesLoop;

            int sheetPosition;

            public Animation(int[] timings, int[] sheetPositions, bool doesLoop)
            {
                this.timings = timings;
                this.sheetPositions = sheetPositions;
                this.doesLoop = doesLoop;
            }

            public void updateTimer(GameTime gameTime)
            {
                currentTime += (float)gameTime.ElapsedGameTime.TotalMilliseconds;

                if (currentTime > timings[timings.Length - 1] && doesLoop)
                {
                    resetAnimation();
                }

                for (int i = 0; i < timings.Length; i++)
                {
                    if (currentTime > timings[i])
                    {
                        sheetPosition = sheetPositions[i];
                    }
                }
            }

            public void resetAnimation()
            {
                currentTime = 0;
                sheetPosition = 0;
            }

            public Rectangle spriteSheetPosition()
            {
                Rectangle returnRectangle = new Rectangle(0,0,23, 22);
                returnRectangle.X = sheetPosition;
                return returnRectangle;
            }

        }

        public class Particle
        {
            float life;
            int lifetime;
            Vector2 particlePosition = new Vector2();
            Vector2 particleVelocity = new Vector2();
            public Particle(int lifetime, Vector2 particlePosition, Vector2 particleVelocity)
            {
                this.lifetime = lifetime;
                this.particlePosition = particlePosition;
                this.particleVelocity = particleVelocity;
                life = 0;
            }
            public float updateParticleLife(GameTime gameTime)
            {
                life += (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                return life;
            }

        }

    }
}
