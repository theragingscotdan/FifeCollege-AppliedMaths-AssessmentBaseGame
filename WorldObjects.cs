﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;

namespace Assessment
{
    public struct directionalLightSource
    {
        public Vector3 diffuseColor; // the main color of the light
        public Vector3 specularColor; // highlight color
        public Vector3 direction; // direction the light is shining in
    }
    public class camera3d
    {
        public Vector3 position;
        public Vector3 offset = Vector3.Zero;
        public Vector3 target;
        public Vector3 whichWayIsUp = Vector3.Up;
        public float fieldOfView = MathHelper.ToRadians(45);
        public float aspectRatio = 4f / 3f;
        public float nearPlane = 1f;
        public float farPlane = 10000f;
        public Matrix ViewMatrix()
        { // Set the position of the camera and tell it what to look at
            return Matrix.CreateLookAt(position, target, whichWayIsUp);
        }
        public Matrix ProjectionMatrix()
        { // Set up a matrix to project the camera image onto the viewport
            return Matrix.CreatePerspectiveFieldOfView(fieldOfView, aspectRatio,
            nearPlane, farPlane);
        }
        public BoundingFrustum viewarea()
        {
            return new BoundingFrustum(ViewMatrix() * ProjectionMatrix());
        }
    }
    public class object3d
    {
        public Vector3 rotation;
        public Vector3 position;
        public Vector3 velocity;
        public Vector3 acceleration;
        public bool visible = false;
        public Model mesh;
        public Matrix[] transforms;
        public float Alpha = 1;
        public Vector3 collisionScale = Vector3.One;
        public float scale = 1;
        public bool Lit = true;
        public Vector3 storedPos;
        public BoundingBox hitBox
        {
            get
            {
                BoundingBox b = new BoundingBox();
                ///////////////////////////////////////////////////////////////////
                //
                // CODE FOR TASK 3 SHOULD BE ENTERED HERE
                //
                ///////////////////////////////////////////////////////////////////
                return b;
            }
        }
        internal void LoadModel(ContentManager content, string modelName)
        {
            mesh = content.Load<Model>(modelName);
            transforms = new Matrix[mesh.Bones.Count];
            mesh.CopyAbsoluteBoneTransformsTo(transforms);
            visible = true;
        }
        public void Draw(camera3d cam, directionalLightSource light)
        {
            if (!visible) return; // dont render hidden meshes
            foreach (ModelMesh mesh in mesh.Meshes) // loop through the mesh in the 3d model, drawing each one in turn.
             {
                foreach (BasicEffect effect in mesh.Effects) // This loop then goes through every effect in each mesh.
                {   
                    effect.World = transforms[mesh.ParentBone.Index]; // begin dealing with transforms to render the object into the game world
                                                                      // The following effects allow the object to be drawn in the correct place, with the correct rotation and scale.
                                                                      ///////////////////////////////////////////////////////////////////
                                                                      //

                    // CODE FOR TASK 1 SHOULD BE ENTERED HERE
                    //
                    ///////////////////////////////////////////////////////////////////
                    ///
                    // world matrix

                    // 1. Scale
                    effect.World *= Matrix.CreateScale(scale);
                    // 2. Rotation
                    effect.World *= Matrix.CreateRotationX(rotation.X);
                    effect.World *= Matrix.CreateRotationY(rotation.Y);
                    effect.World *= Matrix.CreateRotationZ(rotation.Z);

                    // 3. Translation/position
                    effect.World *= Matrix.CreateTranslation(position);

                    // view matrix
                    effect.View = Matrix.CreateLookAt(cam.target + cam.offset, cam.target, Vector3.Up);

                    // projection matrix
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(cam.fieldOfView, cam.aspectRatio,
                    cam.nearPlane, cam.farPlane);
                    //effect.Projection = Matrix.CreateOrthographic(1600, 900, 1f, 10000f);


                    // the following effects are related to lighting and texture  settings, feel free to tweak them to see what happens.
                    effect.LightingEnabled = true;
                    effect.Alpha = Alpha; //  amount of transparency
                    effect.AmbientLightColor = new Vector3(0.25f); // fills in dark areas with a small amount of light
                    effect.DiffuseColor = new Vector3(0.1f);
                    // Diffuse is the standard colour method
                    effect.DirectionalLight0.Enabled = true; // allows a directional light
                    effect.DirectionalLight0.DiffuseColor = light.diffuseColor; // the directional light's main colour
                    effect.DirectionalLight0.SpecularColor = light.specularColor; // the directional light's colour used for highlights
                    effect.DirectionalLight0.Direction = light.direction; // the direction of the light
                    effect.EmissiveColor = new Vector3(0.15f);
                }
                mesh.Draw(); // draw the current mesh using the effects.
            }
        }
    }

    public static class BoundingRenderer
    {
        static GraphicsDevice gfx;
        static VertexBuffer sphereVertexBuffer;
        static VertexBuffer cubeVertexBuffer;
        static BasicEffect effect;
        static int tessellation = 16;
        public static void InitializeGraphics(GraphicsDevice graphicsDevice)
        {
            gfx = graphicsDevice;
            effect = new BasicEffect(gfx);
            effect.LightingEnabled = false;
            effect.VertexColorEnabled = false;
            VertexPositionColor[] sphereVerts = new VertexPositionColor[tessellation *
            3 + 2];
            int index = 0;
            float step = MathHelper.TwoPi / (float)tessellation;
            for (float a = 0; a <= MathHelper.TwoPi; a += step) //create circle on the XY plane first
                sphereVerts[index++] = new VertexPositionColor(new
                Vector3((float)Math.Cos(a), (float)Math.Sin(a), 0f), Color.White);
            for (float a = 0; a <= MathHelper.TwoPi; a += step) //next the XZ  circle
                sphereVerts[index++] = new VertexPositionColor(new
                Vector3((float)Math.Cos(a), 0f, (float)Math.Sin(a)), Color.White);
            sphereVerts[index++] = sphereVerts[index - tessellation - 1]; // close the circle
        for (float a = 0; a <= MathHelper.TwoPi; a += step) //finally the YZ circle
        sphereVerts[index++] = new VertexPositionColor(new Vector3(0f,
        (float)Math.Cos(a), (float)Math.Sin(a)), Color.White);

            sphereVerts[index++] = sphereVerts[index - tessellation - 1]; // close the circle
            sphereVertexBuffer = new VertexBuffer(graphicsDevice,
            typeof(VertexPositionColor), sphereVerts.Length, BufferUsage.None);
            sphereVertexBuffer.SetData(sphereVerts);
            int[] boxpos = new int[] { 0, 0, 0, 0, 0, 1, 0, 1, 1, 0, 1, 0, 0, 0, 0, 1,
0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 0, 1, 0, 0, 1, 0, 1, 0, 0, 1, 0, 1, 1, 1, 1, 1, 1, 1, 0,
0, 1, 0 };
            VertexPositionColor[] boxVerts = new VertexPositionColor[17];
            for (index = 0; index < boxpos.Length; index += 3)
                boxVerts[index / 3] = new VertexPositionColor(new
                Vector3(boxpos[index], boxpos[index + 1], boxpos[index + 2]), Color.White);
            cubeVertexBuffer = new VertexBuffer(graphicsDevice,
            typeof(VertexPositionColor), boxVerts.Length, BufferUsage.None);
            cubeVertexBuffer.SetData(boxVerts);
        }
        public static void RenderBox(BoundingBox box, Matrix view, Matrix projection,
        Color wireColour)
        {
            gfx.SetVertexBuffer(cubeVertexBuffer);
            effect.World =
            Matrix.CreateScale(box.Max - box.Min) *
            Matrix.CreateTranslation(box.Min);
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = wireColour.ToVector3();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gfx.DrawPrimitives(PrimitiveType.LineStrip, 0, 16);
                // draw first circle
            }
        }
        public static void RenderSphere(BoundingSphere sphere, Matrix view, Matrix
        projection, Color wireColour)
        {
            gfx.SetVertexBuffer(sphereVertexBuffer);
            effect.World = Matrix.CreateScale(sphere.Radius) *
            Matrix.CreateTranslation(sphere.Center);
            effect.View = view;
            effect.Projection = projection;
            effect.DiffuseColor = wireColour.ToVector3();
            foreach (EffectPass pass in effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                gfx.DrawPrimitives(PrimitiveType.LineStrip, 0, tessellation);
                // draw first circle
                gfx.DrawPrimitives(PrimitiveType.LineStrip, tessellation,
                tessellation); // draw second circle
                gfx.DrawPrimitives(PrimitiveType.LineStrip, (tessellation) * 2 + 1,
                tessellation); // draw third circle
            }
        }
    }
}
