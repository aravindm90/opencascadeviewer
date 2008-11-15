using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;

namespace OpenCascaseViewer
{
    public class DrawingControl : WinFormsGraphicsDevice.GraphicsDeviceControl
    {
        private ContentManager contentManager;

        private VertexDeclaration vertexDeclaration, vertexDeclaration2;
        private BasicEffect textureEffect, colorEffect;
        private Texture2D glassTexture;

        public Matrix View { get; set; }
        public Matrix World { get; set; }
        public Matrix Projection { get; set; }

        private Vector3 camera = new Vector3(0, 300, 150);

        private VertexPositionNormalTexture[] vertexData;
        private int[] indexDataTriangles;


        protected override void Initialize()
        {
            contentManager = new ContentManager(Services, "Content");

            View = Matrix.CreateLookAt(camera, Vector3.Zero, Vector3.Up);
            World = Matrix.Identity;
            Projection = Matrix.CreatePerspectiveFieldOfView(1, GraphicsDevice.Viewport.AspectRatio, 1, 10000);

            vertexDeclaration = new VertexDeclaration(GraphicsDevice, VertexPositionNormalTexture.VertexElements);

            textureEffect = new BasicEffect(GraphicsDevice, null);

            textureEffect.TextureEnabled = true;
            textureEffect.Texture = glassTexture;

            textureEffect.DirectionalLight0.Enabled = true;
            textureEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            textureEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, -1.0f, -1.0f));
            textureEffect.DirectionalLight0.SpecularColor = Vector3.One;

            textureEffect.DirectionalLight1.Enabled = true;
            textureEffect.DirectionalLight1.DiffuseColor = new Vector3(0.5f, 0.5f, 0.5f);
            textureEffect.DirectionalLight1.Direction = Vector3.Normalize(new Vector3(-1.0f, -1.0f, 1.0f));
            textureEffect.DirectionalLight1.SpecularColor = new Vector3(0.5f, 0.5f, 0.5f);

            glassTexture = contentManager.Load<Texture2D>("Glass");

            vertexDeclaration2 = new VertexDeclaration(GraphicsDevice, VertexPositionColor.VertexElements);

            colorEffect = new BasicEffect(GraphicsDevice, null);
            colorEffect.VertexColorEnabled = true;
            colorEffect.DiffuseColor = new Vector3(1.0f, 1.0f, 1.0f);

            vertexData = new VertexPositionNormalTexture[3];
            indexDataTriangles = new int[3];
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.Black);

            GraphicsDevice.VertexDeclaration = vertexDeclaration;
            GraphicsDevice.RenderState.DepthBufferEnable = true;

            GraphicsDevice.RenderState.CullMode = CullMode.CullCounterClockwiseFace;

            textureEffect.World = World;
            textureEffect.View = View;
            textureEffect.Projection = Projection;

            textureEffect.EnableDefaultLighting();

            textureEffect.TextureEnabled = true;
            textureEffect.Texture = glassTexture;

            textureEffect.DirectionalLight0.Enabled = true;
            textureEffect.DirectionalLight0.DiffuseColor = Vector3.One;
            textureEffect.DirectionalLight0.Direction = Vector3.Normalize(new Vector3(1.0f, 1.0f, -1.0f));
            textureEffect.DirectionalLight0.SpecularColor = Vector3.One;

            textureEffect.Begin();

            foreach (EffectPass pass in textureEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                    vertexData, 0, vertexData.Length, indexDataTriangles, 0, indexDataTriangles.Length / 3);              

                pass.End();
            }

            textureEffect.End();

            colorEffect.World = World;
            colorEffect.View = View;
            colorEffect.Projection = Projection;

            GraphicsDevice.VertexDeclaration = vertexDeclaration2;

            colorEffect.Begin();

            foreach (EffectPass pass in colorEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

      
                pass.End();
            }

            colorEffect.End();
        }
       

#region Mouse


        float xAngle, yAngle;
        MouseState lastMouseState;

        protected override void OnMouseMove(System.Windows.Forms.MouseEventArgs e)
        {
            base.OnMouseMove(e);

            MouseState newState = Mouse.GetState();
            if (newState.LeftButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
            {
                int yDiff = lastMouseState.Y - newState.Y;
                int xDiff = lastMouseState.X - newState.X;

                xAngle += yDiff / 2;
                yAngle -= xDiff / 2;

                World =
                    Matrix.CreateRotationZ(MathHelper.ToRadians((yAngle)))
                    * Matrix.CreateRotationX(MathHelper.ToRadians(MathHelper.Clamp(xAngle, -90, 90)));

                Invalidate();
            }
            lastMouseState = newState;
        }

#endregion

    }
}
