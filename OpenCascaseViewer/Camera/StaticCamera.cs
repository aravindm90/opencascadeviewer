using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace OpenCascaseViewer
{
    public class StaticCamera
    {
        /*float xAngle, yAngle;
        MouseState lastMouseState;

        protected override void OnMouseWheel(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseWheel(e);
            MouseState newState = Mouse.GetState();

            if (newState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                float zDiff = (float)e.Delta / 4;

                World *= Matrix.CreateTranslation(0.0f, 0.0f, zDiff);

                Invalidate();
            }

            lastMouseState = newState;
        }

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            MouseState newState = Mouse.GetState();
            if (newState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                int yDiff = lastMouseState.Y - newState.Y;
                int xDiff = lastMouseState.X - newState.X;

                xAngle = yDiff / 2;
                yAngle = xDiff / 2;

                Vector3 trans = World.Translation;

                World *= Matrix.CreateTranslation(-trans) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians((yAngle)))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(MathHelper.Clamp(xAngle, -90, 90)))
                    * Matrix.CreateTranslation(trans);

                Invalidate();
            }
            if (newState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                int yDiff = lastMouseState.Y - newState.Y;
                int xDiff = -lastMouseState.X + newState.X;

                xAngle += yDiff / 2;
                yAngle -= xDiff / 2;

                World *= Matrix.CreateTranslation(xDiff, yDiff, 0);

                Invalidate();
            }
            lastMouseState = newState;
        }*/
    }
}
