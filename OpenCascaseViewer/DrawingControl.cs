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

        private Vector3 camera = new Vector3(0, 0, 550);

        public List<VertexPositionNormalTexture[]> vertexData;
        public List<int[]> indexDataTriangles;


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


            vertexData = new List<VertexPositionNormalTexture[]>();
            indexDataTriangles = new List<int[]>();
        }

        public void Clear()
        {
            indexDataTriangles.Clear();
            vertexData.Clear();
        }

        public void Fill(gp.Pnt[] points, gp.Pnt2d[] uvpoints, int[] triangles)
        {
            Vector3 edge1, edge2, normal;
            uint[] normalsCount = new uint[points.Length];
            Vector3[] normals = new Vector3[points.Length];
            VertexPositionNormalTexture[] vertex = new Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture[points.Length];
            
            for (int ind = 0; ind < triangles.Length; ind += 3)
            {
                edge1 = new Vector3(
                    (float)(points[triangles[ind]].x - points[triangles[ind+1]].x),
                    (float)(points[triangles[ind]].x - points[triangles[ind+1]].x),
                    (float)(points[triangles[ind]].x - points[triangles[ind+1]].x));
                edge2 = new Vector3(
                    (float)(points[triangles[ind + 2]].x - points[triangles[ind+1]].x),
                    (float)(points[triangles[ind + 2]].x - points[triangles[ind+1]].x),
                    (float)(points[triangles[ind + 2]].x - points[triangles[ind+1]].x));
                Vector3.Cross(ref edge1, ref edge2, out normal);
                normals[triangles[ind]] += normal;
                ++normalsCount[triangles[ind]];
                normals[triangles[ind + 1]] += normal;
                ++normalsCount[triangles[ind+1]];
                normals[triangles[ind + 2]] += normal;
                ++normalsCount[triangles[ind+2]];
            }
            for (int i = 0; i < points.Length; i++)
            {
                normals[i] /= (float)normalsCount[i];
                Vector3 pos = new Vector3((float)points[i].x, (float)points[i].y, (float)points[i].z);
                //pos *= 5;
                Vector2 tex = new Vector2((float)uvpoints[i].x, (float)uvpoints[i].y);
                vertex[i] = new Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture(
                    pos, normals[i], tex);
            }

            indexDataTriangles.Add(triangles);
            vertexData.Add(vertex);
        }

        protected override void Draw()
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            GraphicsDevice.VertexDeclaration = vertexDeclaration;
            GraphicsDevice.RenderState.DepthBufferEnable = true;

            GraphicsDevice.RenderState.CullMode = CullMode.None;

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

            List<VertexPositionNormalTexture[]>.Enumerator vertexEnumerator = vertexData.GetEnumerator();
            List<int[]>.Enumerator indexDataTrianglesEnumerator = indexDataTriangles.GetEnumerator();

            textureEffect.Begin();

            foreach (EffectPass pass in textureEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                vertexData.GetEnumerator();
                while(vertexEnumerator.MoveNext() && indexDataTrianglesEnumerator.MoveNext())
                {
                    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                    vertexEnumerator.Current, 0, vertexEnumerator.Current.Length, indexDataTrianglesEnumerator.Current, 0, 
                    indexDataTrianglesEnumerator.Current.Length / 3);
                }
                /*GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                    vertexData, 0, vertexData.Length, indexDataTriangles, 0, indexDataTriangles.Length / 3);              */

                pass.End();
            }

            textureEffect.End();

           /* colorEffect.World = World;
            colorEffect.View = View;
            colorEffect.Projection = Projection;

            GraphicsDevice.VertexDeclaration = vertexDeclaration2;

            colorEffect.Begin();

            foreach (EffectPass pass in colorEffect.CurrentTechnique.Passes)
            {
                pass.Begin();

                GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,
              vertexData, 0, vertexData.Length, indexDataTriangles, 0, indexDataTriangles.Length / 3);              

      
                pass.End();
            }

            colorEffect.End();*/
        }
       

#region Mouse


        float xAngle, yAngle;
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
                int xDiff = - lastMouseState.X + newState.X;

                xAngle += yDiff / 2;
                yAngle -= xDiff / 2;

                World *=
                    Matrix.CreateTranslation(xDiff, yDiff, 0);

                Invalidate();
            }
            lastMouseState = newState;
        }

#endregion

    }
}
