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

        Texture2D birbSprite;
        Texture2D grillSprite;
        Texture2D groundSprite;

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

        List<Rectangle> barrierList = new List<Rectangle>();
        List<Vector2> groundTiles = new List<Vector2>();

        bool collisionDetected = false;

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
            birbSprite = Content.Load<Texture2D>("lape-sheet");
            grillSprite = Content.Load<Texture2D>("grill");
            groundSprite = Content.Load<Texture2D>("ground");

            birbRec = new Rectangle(100, 100, 46, 44);
            birbAnimationWindow = new Rectangle(22, 0, 24, 22);

            grillRec = new Rectangle(200, 200, grillSprite.Width/4, grillSprite.Height/4);
            birbOrigin = new Vector2(12, 11);
            barrierList.Add(grillRec);

            birbVelocity = 0;
            gravityAcceleration = 0.25f;

            groundTiles.Add(new Vector2(0, 400));
            groundTiles.Add(new Vector2(groundSprite.Width * 3, 400));
            groundTiles.Add(new Vector2(groundSprite.Width * 3 * 2, 400));

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
                //addObstacle(grillSprite, barrierList);
                birbVelocity = -7.0f;
                birbRotation = 0;
                previousState = keyState;
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

            if (700 - barrierList[barrierList.Count-1].X > 400)
            {
                addObstacle(grillSprite, barrierList);
            }

            //Update parallax scroll positions
            updateParallaxScroll(groundSprite, groundTiles, 3);

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
                GraphicsDevice.Clear(Color.CornflowerBlue);
            }

            spriteBatch.Begin(SpriteSortMode.Deferred, null, SamplerState.PointClamp, null, null, null, null);
            
            spriteBatch.Draw(birbSprite, birbRecDrawingOffset, birbAnimationWindow, Color.White, birbRotation, birbOrigin, SpriteEffects.None, 0.5f);
            
            foreach (Rectangle obstacle in barrierList)
            {
                spriteBatch.Draw(grillSprite, obstacle, Color.White);
            }

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

            Rectangle upperObstacle = new Rectangle(700, -200 + rando, obstacleSprite.Width / 4, obstacleSprite.Height / 4);
            Rectangle lowerObstacle = new Rectangle(700, 200 + rando, obstacleSprite.Width / 4, obstacleSprite.Height / 4);

            barrierList.Add(upperObstacle);
            barrierList.Add(lowerObstacle);

            return barrierList;

        }

        public static List<Vector2> updateParallaxScroll(Texture2D tile, List<Vector2> positionList, int speed)
        {
            for(int i=0; i < positionList.Count; i++)
            {
                Vector2 newVec = new Vector2(positionList[i].X - speed, positionList[i].Y);
                positionList[i] = newVec;

                if (positionList[i].X < -tile.Width * 3)
                {
                    Vector2 newReplacementVec = new Vector2(tile.Width * 3 * 2, positionList[i].Y);
                    positionList[i] = newReplacementVec;
                }

            }

            return positionList;
        }
    }
}
