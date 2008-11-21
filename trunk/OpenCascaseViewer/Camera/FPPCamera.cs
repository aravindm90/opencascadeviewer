using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace OpenCascaseViewer
{
    public class Camera
    {
        private Vector3 position;
        private float a, b;
        private int cX, cY;

        private MouseState prevMouseState;

        public float XAngle
        {
            get { return a; }
        }

        public float YAngle
        {
            get { return b; }
        }

        public Vector3 CamPosition
        {
            get { return position; }
        }

        public Camera(int centerX, int centerY)
        {
            cX = centerX;
            cY = centerY;
            //prevMouseState = Mouse.GetState();
            b = MathHelper.Pi;
            position = new Vector3(0, 0, 5);
            SetInitialValues();
        }

        private void SetInitialValues()
        {
            Mouse.SetPosition(cX, cY);
            prevMouseState = Mouse.GetState();
        }

        private void ProcessInput(float amountOfMovement)
        {
            Vector3 moveVector = new Vector3();

            KeyboardState keys = Keyboard.GetState();
            if (keys.IsKeyDown(Keys.Right) || keys.IsKeyDown(Keys.D))
                moveVector.X -= amountOfMovement;
            if (keys.IsKeyDown(Keys.Left) || keys.IsKeyDown(Keys.A))
                moveVector.X += amountOfMovement;
            if (keys.IsKeyDown(Keys.Down) || keys.IsKeyDown(Keys.S))
                moveVector.Z -= amountOfMovement;
            if (keys.IsKeyDown(Keys.Up) || keys.IsKeyDown(Keys.W))
                moveVector.Z += amountOfMovement;
            if (keys.IsKeyDown(Keys.PageUp) || keys.IsKeyDown(Keys.R))
                moveVector.Y += amountOfMovement;
            if (keys.IsKeyDown(Keys.PageDown) || keys.IsKeyDown(Keys.F))
                moveVector.Y -= amountOfMovement;

            Matrix cameraRotation = Matrix.CreateRotationX(a) * Matrix.CreateRotationY(b);
            position += Vector3.Transform(moveVector, cameraRotation);

            MouseState currentMouseState = Mouse.GetState();
            if (currentMouseState.X != prevMouseState.X)
                b -= amountOfMovement / 80.0f * (currentMouseState.X - prevMouseState.X);
            if (currentMouseState.Y != prevMouseState.Y)
                a += amountOfMovement / 80.0f * (currentMouseState.Y - prevMouseState.Y);
            Mouse.SetPosition(cX, cY);
        }

        public void Update(ref Matrix view, float elapsedGameTime)
        {
            ProcessInput(elapsedGameTime / 120.0f);
            //view = Translate(ref view) * Rotate(ref view);
            Matrix cameraRotation = Matrix.CreateRotationX(a) * Matrix.CreateRotationY(b);
            Vector3 targetPos = position + Vector3.Transform(new Vector3(0, 0, 1), cameraRotation);
            Vector3 upVector = Vector3.Transform(new Vector3(0, 1, 0), cameraRotation);

            view = Matrix.CreateLookAt(position, targetPos, upVector);
        }
        /*
        private Matrix Rotate(ref Matrix view)
        {
            MouseState ms = Mouse.GetState();

            int dx = prevMouseState.X - ms.X, dy = prevMouseState.Y - ms.Y;

            float speed = 0.1f;

            a += speed * dx;
            b += speed * dy;

            prevMouseState = ms;

            return Matrix.CreateFromYawPitchRoll(a, b, 0);
        }

        private Matrix Translate(ref Matrix view)
        {
            float speed = 0.1f;

            KeyboardState ks = Keyboard.GetState();

            if (ks.IsKeyDown(Keys.W))
                position += view.Forward * speed;
            if (ks.IsKeyDown(Keys.S))
                position += view.Backward * speed;
            if (ks.IsKeyDown(Keys.A))
                position += view.Left * speed;
            if (ks.IsKeyDown(Keys.D))
                position += view.Right * speed;

            return Matrix.CreateTranslation(position);
        }*/
    }
}
