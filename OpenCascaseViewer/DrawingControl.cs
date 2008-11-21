using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using System.ComponentModel;

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

        private Vector3 camera = new Vector3(0, 0, 100);

        public List<VertexPositionNormalTexture[]> vertexData = new List<VertexPositionNormalTexture[]>();
        public List<int[]> indexData = new List<int[]>();

        [EditorBrowsable(EditorBrowsableState.Never), Bindable(BindableSupport.No), Browsable(false)]
        public ModelData Model { get; set; }

        public void SyncBuffers()
        {
            vertexData.Clear();
            indexData.Clear();
            foreach (TessalatedShape shape in Model.Tessations)
                GenerateBuffers(shape.points, shape.uvpoints, shape.triangles);          
        }

        private void GenerateBuffers(gp.Pnt[] points, gp.Pnt2d[] uvpoints, int[] triangles)
        {
            Vector3 edge1, edge2, normal;
            uint[] normalsCount = new uint[points.Length];
            Vector3[] normals = new Vector3[points.Length];
            VertexPositionNormalTexture[] vertex = new Microsoft.Xna.Framework.Graphics.VertexPositionNormalTexture[points.Length];

            for (int ind = 0; ind < triangles.Length; ind += 3)
            {
                edge1 = new Vector3(
                    (float)(points[triangles[ind]].x - points[triangles[ind + 1]].x),
                    (float)(points[triangles[ind]].y - points[triangles[ind + 1]].y),
                    (float)(points[triangles[ind]].z - points[triangles[ind + 1]].z));
                edge2 = new Vector3(
                    (float)(points[triangles[ind + 2]].x - points[triangles[ind + 1]].x),
                    (float)(points[triangles[ind + 2]].y - points[triangles[ind + 1]].y),
                    (float)(points[triangles[ind + 2]].z - points[triangles[ind + 1]].z));
                Vector3.Cross(ref edge1, ref edge2, out normal);
                edge1 = new Vector3(
                    (float)(points[triangles[ind]].x),
                    (float)(points[triangles[ind]].y),
                    (float)(points[triangles[ind]].z));
                if (Vector3.Dot(normal, edge1) < 0)
                    normal *= -1.0f;
                normal.Normalize();
                normals[triangles[ind]] += normal;
                ++normalsCount[triangles[ind]];
                normals[triangles[ind + 1]] += normal;
                ++normalsCount[triangles[ind + 1]];
                normals[triangles[ind + 2]] += normal;
                ++normalsCount[triangles[ind + 2]];
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

            indexData.Add(triangles);
            vertexData.Add(vertex);
        }


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

            textureEffect.Begin();

            foreach (EffectPass pass in textureEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                for (int i = 0; i < vertexData.Count; i++)
                {
                    GraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList,
                    vertexData[i], 0, vertexData[i].Length, indexData[i], 0, indexData[i].Length / 3);
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

            float zDiff = (float)e.Delta / 120.0f;
               
            World *= Matrix.CreateScale((zDiff<0.0f?1.0f/-zDiff:zDiff));
            
            Invalidate();
            
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
