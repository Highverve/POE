using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;

namespace Pilgrimage_Of_Embers.Helper_Classes
{
    public class PieChart
    {
        private float rotation = 0.0f;
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }

        private bool isCentered = true;
        public bool IsCentered
        {
            get { return isCentered; }
            set { isCentered = value; }
        }

        private Vector2 position;
        public Vector2 Position
        {
            get { return position; }
            set { position = value; }
        }

        private float radius = 10.0f;
        public float Radius
        {
            get { return radius; }
            set { radius = MathHelper.Clamp(value, 0f, 2000f); }
        }

        private float angle;
        public float Angle
        {
            get { return angle; }
            set
            {
                angle = MathHelper.Clamp(value, 0f, MathHelper.ToRadians(360));
                RebuildVertices();
            }
        }

        private int triangles;
        public int Tesselation
        {
            get { return triangles; }
            set
            {
                triangles = Math.Max(value, 1);
                RebuildVertices();
            }
        }

        Game game;
        BasicEffect be;
        Matrix projectionMatrix;
        Matrix viewMatrix;

        private Color insideColor;
        private Color outsideColor;

        public Color InsideColor { get { return insideColor; } set { insideColor = value; RebuildVertices(); } }
        public Color OutsideColor { get { return outsideColor; } set { outsideColor = value; RebuildVertices(); } }

        private RasterizerState rs = new RasterizerState()
        {
            CullMode = CullMode.None,
            FillMode = FillMode.Solid
        };

        public PieChart(Game game, float startingRotation, float radius, float angle, int tesselation, Color InsideColor, Color OutsideColor, bool centered = true)
        {
            this.rotation = startingRotation;
            this.game = game;
            this.radius = radius;
            this.angle = angle;
            this.triangles = tesselation;
            this.insideColor = InsideColor;
            this.outsideColor = OutsideColor;
            this.isCentered = centered;
        }

        VertexPositionColor[] vertices;
        public void Initialize()
        {
            viewMatrix = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, new Vector3(0, 1, 0));

            projectionMatrix = Matrix.CreateOrthographicOffCenter(0, GameSettings.VectorResolution.X, GameSettings.VectorResolution.Y, 0, 1, 50);
            be = new BasicEffect(game.GraphicsDevice);

            RebuildVertices();
        }

        private void RebuildVertices()
        {
            List<VertexPositionColor> verts = new List<VertexPositionColor>();
            int max = triangles;
            for (int i = 0; i < triangles * 2; i++)
            {
                float ang = Lerp(0, max, 0, angle, i);
                verts.Add(new VertexPositionColor(new Vector3((float)Math.Cos(ang), (float)Math.Sin(ang), 0), insideColor));
                verts.Add(new VertexPositionColor(Vector3.Zero, outsideColor));
            }
            verts.Add(new VertexPositionColor(new Vector3((float)Math.Cos(angle), (float)Math.Sin(angle), 0), insideColor));
            vertices = verts.ToArray();
        }
        private float Lerp(float x0, float x1, float y0, float y1, float x2)
        {
            return y0 * (x2 - x1) / (x0 - x1) + y1 * (x2 - x0) / (x1 - x0);
        }

        public void Draw()
        {
            GraphicsDevice device = game.GraphicsDevice;

            if (isCentered)
                be.World = Matrix.CreateScale(radius) * Matrix.CreateRotationZ(rotation - angle / 2.0f) * Matrix.CreateTranslation(new Vector3(position, 0));
            else
                be.World = Matrix.CreateScale(radius) * Matrix.CreateRotationZ(rotation) * Matrix.CreateTranslation(new Vector3(position, 0));

            device.RasterizerState = rs;

            be.View = viewMatrix;
            be.Projection = projectionMatrix;
            be.VertexColorEnabled = true;
            be.LightingEnabled = false;

            foreach (EffectPass pass in be.CurrentTechnique.Passes)
            {
                pass.Apply();
                device.DrawUserPrimitives<VertexPositionColor>(PrimitiveType.TriangleStrip, vertices, 0, triangles * 2);
            }
        }
    }
}
