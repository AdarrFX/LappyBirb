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

        SpriteFont TTfont;
        Vector2 TTFposition = new Vector2(200, 100);

        Vector2 birbPosition, grillPosition;
        Rectangle birbRec, grillRec;

        float birbAlpha, grillAlpha = 1.0f;
        float birbRotation, grillRotation = 0.0f;

        float birbVelocity, gravityAcceleration;

        Vector2 birbOrigin = new Vector2(0, 0);
        Vector2 grillOrigin = new Vector2(100, 100);

        float birbScale = 1.0f;
        float grillScale = 0.5f;

        SpriteEffects spriteEffect = SpriteEffects.None;

        KeyboardState previousState;

        float zDepth = 0.10f;

        List<Rectangle> barrierList = new List<Rectangle>();
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
            birbSprite = Content.Load<Texture2D>("lape");
            grillSprite = Content.Load<Texture2D>("grill");

            birbRec = new Rectangle(100, 100, birbSprite.Width*2, birbSprite.Height*2);
            grillRec = new Rectangle(200, 200, grillSprite.Width/4, grillSprite.Height/4);

            barrierList.Add(grillRec);

            birbVelocity = 0;
            gravityAcceleration = 0.25f;


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

            birbRotation = birbVelocity * 0.07f;           


            if (birbRec.Y > 420)
            {
                birbVelocity = 0;
                birbRec.Y = 420;
            }

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
            //spriteBatch.Draw(birbSprite, birbRec, Color.White);
            spriteBatch.Draw(birbSprite, birbRec, null, Color.White, birbRotation, birbOrigin, SpriteEffects.None, 0.5f);

            foreach (Rectangle obstacle in barrierList)
            {
                spriteBatch.Draw(grillSprite, obstacle, Color.White);
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

    }
}
