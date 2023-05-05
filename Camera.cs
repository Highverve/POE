using Microsoft.Xna.Framework;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Microsoft.Xna.Framework.Graphics;
using System;
using Pilgrimage_Of_Embers.Helper_Classes;

namespace Pilgrimage_Of_Embers
{
    public class Camera
    {
        public Vector2 currentPosition, targetPosition;

        Controls controls = new Controls();
        private Random random = new Random();

        public enum CameraState
        {
            Current,
            Cinematic,
            Debug,
        }
        public CameraState state = CameraState.Current;

        private bool isZooming = false;

        private Vector2 lastPosition = Vector2.Zero;
        public Vector2 Position
        {
            get
            {
                return currentPosition;
            }
            set
            {
                currentPosition = value;
                ValidatePosition();
            }
        }
        public float PositionChange { get; private set; }

        public Vector2 Origin { get; set; }
        public float Rotation { get; set; }

        private float currentZoom;
        public float Zoom
        {
            get { return currentZoom; }
            set
            {
                currentZoom = MathHelper.Clamp(value, .25f, 16f);

                ValidateZoom();
                ValidatePosition();
                ValidateListenerRadius();
            }
        }

        public float DelaySpeed { get; set; }

        public Matrix World { get; protected set; }
        public Matrix Projection { get; protected set; }
        public Matrix View(Vector2 parallax)
        {
            return Matrix.CreateTranslation(new Vector3(-Position * parallax, 0.0f)) *
                Matrix.CreateTranslation(new Vector3(-Origin, 0.0f)) *
                Matrix.CreateRotationZ(Rotation) *
                Matrix.CreateScale(Zoom, Zoom, 1) *
                Matrix.CreateTranslation(new Vector3(Origin, 0.0f));
        }

        private Viewport viewport; 

        private Rectangle? limits;
        public Rectangle? Limits
        {
            get { return limits; }
            set
            {
                if (value != null)
                {
                    limits = new Rectangle
                    {
                        X = value.Value.X,
                        Y = value.Value.Y,
                        Width = value.Value.Width,
                        Height = value.Value.Height
                    };

                    ValidateZoom();
                    ValidatePosition();
                }
                else
                    limits = null;
            }
        }

        //Audio-related
        private Vector2 listenerPosition;

        public float ListenerRadius { get; set; }
        public Vector2 ListenerPosition { get { return listenerPosition; } }
        private void ValidateListenerRadius()
        {
            ListenerRadius = (GameSettings.WindowResolution.X / 3) * MathHelper.Clamp((1f - (currentZoom / 18f)), .05f, 1f);
        }

        public Camera(Viewport Viewport)
        {
            viewport = Viewport;

            Origin = new Vector2(viewport.Width / 2, viewport.Height / 2);
            Zoom = 1.0f;

            ReloadMatrices();

            ListenerRadius = viewport.Width / 3;
        }
        public void ReloadMatrices()
        {
            //View = Matrix.CreateLookAt(new Vector3(1, 0.75f, 1)) * Matrix.CreateTranslation(new Vector3(-Position, 1));
            World = Matrix.Invert(View(Vector2.One));
            Projection = Matrix.CreateOrthographic(GameSettings.VectorResolution.X, GameSettings.VectorResolution.Y, -1, 1);
            Matrix halfPixelOffset = Matrix.CreateTranslation(-0f, -0f, 0);
            Projection *= halfPixelOffset;
        }

        public void LookAt(Vector2 position) { targetPosition = position - Origin; }
        public void ForceLookAt(Vector2 position) { LookAt(position); Position = targetPosition; }

        public Vector2 WorldToScreen(Vector2 position) { return Vector2.Transform(position, View(Vector2.One)); }
        public float WorldXToScreen(float x) { return Vector2.Transform(new Vector2(x, 0), View(Vector2.One)).X; }
        public float WorldYToScreen(float y) { return Vector2.Transform(new Vector2(y, 0), View(Vector2.One)).Y; }

        public Vector2 ScreenToWorld(Vector2 position) { return Vector2.Transform(position, Matrix.Invert(View(Vector2.One))); }

        private void ValidateZoom()
        {
            if (limits.HasValue)
            {
                float minZoomX = MathHelper.Clamp((float)GameSettings.WindowResolution.X / limits.Value.Width, .5f, 4f);
                float minZoomY = MathHelper.Clamp((float)GameSettings.WindowResolution.Y / limits.Value.Height, .5f, 4f);
                currentZoom = MathHelper.Max(currentZoom, MathHelper.Max(minZoomX, minZoomY));
            }
        }
        private void ValidatePosition()
        {
            if (limits.HasValue)
            {
                //Limit the screen from moving beyond map ONLY IF the map size is greater than the viewport
                Vector2 cameraWorldMin = Vector2.Transform(Vector2.Zero, Matrix.Invert(View(Vector2.One)));
                Vector2 cameraSize = new Vector2(GameSettings.VectorResolution.X, GameSettings.VectorResolution.Y) / Zoom;

                Vector2 limitWorldMin = new Vector2(limits.Value.Left, limits.Value.Top);
                Vector2 limitWorldMax = new Vector2(limits.Value.Right, limits.Value.Bottom);

                Vector2 posOffset = currentPosition - cameraWorldMin;
                currentPosition = Vector2.Clamp(cameraWorldMin, limitWorldMin, limitWorldMax - cameraSize) + posOffset;

                if (float.IsNaN(currentPosition.X) == true || float.IsNaN(currentPosition.Y) == true)
                    currentPosition = Vector2.Zero;
            }
        }

        private float startingZoom, targetZoom, zoomSpeed, zoomLerp;
        private int zoomWaitTime = 0;
        private bool isOneWayZoom = true, isIncrease = true;
        public void SmoothZoom(float targetZoom, float speed, bool isOneWay, int waitTime)
        {
            this.targetZoom = targetZoom;
            zoomSpeed = speed;
            isOneWayZoom = isOneWay;
            zoomWaitTime = waitTime;
            isZooming = true;
            isIncrease = true;

            startingZoom = currentZoom;
            zoomLerp = 0f;
        }

        public void Update(GameTime gt)
        {
            listenerPosition = Position + Origin;
            PositionChange = Vector2.Distance(lastPosition, Position);
            lastPosition = Position;

            container = new Rectangle((int)Position.X - margins.X, (int)Position.Y - margins.Y, GameSettings.WindowResolution.X + (margins.X * 2), GameSettings.WindowResolution.Y + (margins.Y * 2));

            UpdateZoom(gt);
            UpdateDelay(gt);
            UpdateShake(gt);
        }
        private void UpdateZoom(GameTime gt)
        {
            if (isZooming == true)
            {
                if (isIncrease == true)
                    zoomLerp += zoomSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
                else
                    zoomLerp -= zoomSpeed * (float)gt.ElapsedGameTime.TotalSeconds;
            }

            zoomLerp = MathHelper.Clamp(zoomLerp, 0f, 1f);

            if (isZooming == true)
                Zoom = MathHelper.SmoothStep(startingZoom, targetZoom, zoomLerp);

            if (isOneWayZoom == true)
            {
                if (zoomLerp >= 1f || zoomLerp <= 0f)
                    isZooming = false;
            }
            else
            {
                if (zoomLerp <= 0f)
                    isZooming = false;

                if (isIncrease == true && zoomLerp >= 1f) //At peak
                {
                    zoomWaitTime -= gt.ElapsedGameTime.Milliseconds;

                    if (zoomWaitTime <= 0)
                        isIncrease = false;
                }
            }
        }
        private void UpdateDelay(GameTime gt)
        {
            if (Vector2.Distance(targetPosition, currentPosition) >= 5)
            {
                float cameraSpeed = MathHelper.Clamp(Vector2.Distance(targetPosition, currentPosition) * DelaySpeed, 0f, 750f);
                Vector2 direction = targetPosition - currentPosition;

                if (direction != Vector2.Zero)
                    direction.Normalize();

                Position += direction * (cameraSpeed * (float)gt.ElapsedGameTime.TotalSeconds);
            }
        }

        private Vector2 shakeDirection;
        private float shakeRadius = 5, shakeMultiplier = 50;
        public bool IsShaking { get; private set; }
        private void UpdateShake(GameTime gt)
        {
            if (shakeMultiplier > 0)
            {
                shakeDirection = new Vector2(random.NextFloat(-shakeRadius, shakeRadius), random.NextFloat(-shakeRadius, shakeRadius));
                Position += (shakeDirection * (shakeMultiplier * (float)gt.ElapsedGameTime.TotalSeconds)) * GameSettings.ShakeMultiplier;

                shakeMultiplier -= Math.Max(shakeMultiplier * 2, 3f) * (float)gt.ElapsedGameTime.TotalSeconds;
                IsShaking = true;
            }
            else
                IsShaking = false;

            /*
            controls.UpdateCurrent();

            if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.D1))
                Shake(5, 50);
            if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.D2))
                Shake(10, 100);
            if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.D3))
                Shake(20, 200);

            controls.UpdateLast();*/
        }
        public void Shake(float radius, float multiplier)
        {
            if (IsShaking == false)
            {
                shakeRadius = 0;
                shakeMultiplier = 0;
            }

            shakeRadius += radius;
            shakeMultiplier += multiplier;
        }

        private Point margins = new Point(128);
        private Rectangle container;
        public bool IsOnScreen(Vector2 position)
        {
            return container.Contains(position);
        }
        public bool IsOnScreen(Vector2 position, Vector2 clipMargins)
        {
            return new Rectangle((int)(Position.X - clipMargins.X),
                                 (int)(Position.Y - clipMargins.Y),
                                 (int)(GameSettings.WindowResolution.X + (clipMargins.X * 2)),
                                 (int)(GameSettings.WindowResolution.Y + (clipMargins.Y * 2))).Contains(position);
        }
        public bool IsOnScreen(Vector2 position, Rectangle customMargins)
        {
            Rectangle custom = new Rectangle((int)Position.X - customMargins.X, (int)Position.Y - customMargins.Y, (int)Position.X + GameSettings.WindowResolution.X + customMargins.Width, (int)Position.Y + GameSettings.WindowResolution.Y + customMargins.Height);
            return custom.Contains(position);
        }

        /// <summary>
        /// Added for Camera.cs completeness. DO NOT use "followRotation" in this game!
        /// </summary>
        /// <param name="displacement"></param>
        /// <param name="followRotation"></param>
        public void Move(Vector2 displacement, bool followRotation = false)
        {
            if (followRotation == true)
                displacement = Vector2.Transform(displacement, Matrix.CreateRotationZ(-Rotation));

            Position += displacement;
        }

        public void SetCameraState(CameraState state) { this.state = state; }
        public void MoveDebugCamera(GameTime gt, Vector2 moveTo)
        {
            if (state == CameraState.Debug)
            {
                Vector2 direction = moveTo - GameSettings.VectorCenter;
                float speed = Vector2.Distance(moveTo, GameSettings.VectorCenter) * 3f;

                if (direction != Vector2.Zero)
                    direction.Normalize();

                Position += (direction * speed) * (float)gt.ElapsedGameTime.TotalSeconds;
                targetPosition = Position;
            }
        }
    }
}
