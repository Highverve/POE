using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Pilgrimage_Of_Embers.TileEngine;
using Pilgrimage_Of_Embers.ScreenEngine;
using Pilgrimage_Of_Embers.ScreenEngine.Options;
using Pilgrimage_Of_Embers.SaveTypes;
using Pilgrimage_Of_Embers.Helper_Classes;
using Pilgrimage_Of_Embers.LightEngine;
using Pilgrimage_Of_Embers.Entities.Types;
using Pilgrimage_Of_Embers.Debugging;
using Pilgrimage_Of_Embers.Culture;
using Pilgrimage_Of_Embers.ScreenEngine.Main;
using Pilgrimage_Of_Embers.Skills;
using Pilgrimage_Of_Embers.ScreenEngine.InventoryScreen.ItemTypes;

namespace Pilgrimage_Of_Embers
{
    public class WorldManager : Microsoft.Xna.Framework.DrawableGameComponent
    {
        Effect lightEffect;
        List<ShaderType> shaders = new List<ShaderType>();
        List<ShaderType> activeShaders = new List<ShaderType>();

        RenderTarget2D objectTarget, lightTarget, finalTarget, shadedTarget;

        WorldLight ambient;

        GameAssetsManager GAM;
        Camera camera;
        public TileMap tm;
        //public static Player player; //Remove soon
        private PlayerEntity playerEntity;
        public ScreenManager sm;
        public DebugManager debug;
        private DataManager dataManager;

        private CultureManager culture;

        public GraphicsDevice g;

        private MainScreen mainScreen;

        Controls controls = new Controls();
        private ContentManager mapContent;

        int mapTimeToSave = 0, playerTimeToSave = 0, worldTimeToSave = 0;

        ToolTip toolTip = new ToolTip();

        private Random random;

        private string playerName = "";
        public string PlayerName { get { return playerName; } }

        public WorldManager(Game game, GraphicsDeviceManager graphics) : base(game)
        {
            g = game.GraphicsDevice;
            camera = new Camera(g.Viewport);

            mainScreen = new MainScreen();

            controls.SetDefaultControls();

            AssignObjects(game, graphics);

            random = new Random(Guid.NewGuid().GetHashCode());
        }
        private void AssignObjects(Game game, GraphicsDeviceManager graphics)
        {
            mapContent = new ContentManager(Game.Services, "MapContent");
            culture = new CultureManager();

            GAM = new GameAssetsManager();

            tm = new TileMap(game, game.Content, mapContent);
            sm = new ScreenManager(game, graphics, this);

            GameSettings.ReadData();
            GameSettings.LoadSettings();
            GameSettings.ApplyChanges(this, graphics, game);

            debug = new DebugManager();
            dataManager = new DataManager();

            ambient = new WorldLight();
        }

        public void LoadContent(ContentManager content)
        {
            LoadShaders(content);

            GAM.SetReferences(debug);
            GAM.LoadAssets(content, mapContent);

            SetObjectReferences();
            debug.LoadContent(content);

            mainScreen.Load(content);

            //Initialize random maps, and load one with main menu settings.
            InitializeMaps();
#if !DEBUG
            GoToMainMenu(); //Skip loading two different maps on game start
#endif
            sm.Load(content);
            toolTip.LoadContent(content);

            ambient.LoadGradients(content);

#if DEBUG
            LoadSaveFile("Shyy");
#endif
        }
        private void SetObjectReferences()
        {
            mainScreen.SetReferences(camera, this, sm, tm);

            tm.SetReferences(sm, camera, debug, this, dataManager, culture); tm.LoadOnce(); playerEntity = tm.Player;
            sm.SetReferences(camera, tm, debug, tm.ControlledEntity, ambient, culture, this, mainScreen);

            debug.SetReferences(Game, this, tm, sm, playerEntity, camera);
            dataManager.SetReferences(tm, sm, playerEntity, debug, culture);

            //dataManager.ReadWorldData(PlayerName);
            //dataManager.ReadPlayerData(PlayerName);

            ambient.SetReferences(debug, culture);
        }

        private void LoadShaders(ContentManager cm)
        {
            lightEffect = cm.Load<Effect>("Shaders/Light");

            shaders.Add(new ShaderType("radialBlur", 5, "Shaders/RadialBlur", (Effect x) =>
            {
                x.Parameters["BlurWidth"].SetValue(MathHelper.SmoothStep(0f, .05f, timer));
            }));

            shaders.Add(new ShaderType("Blur", 6, "Shaders/GaussianBlur", (Effect x) =>
            {
            }));

            shaders.Add(new ShaderType("Test", 7, "Shaders/TestingEffect", (Effect x) =>
            {
            }));

            shaders.Add(new ShaderType("wiggle", 10, "Shaders/Wiggle", (Effect x) =>
            {
                x.Parameters["value"].SetValue(wiggleValue);
                x.Parameters["zoom"].SetValue(wiggleZoom);

                x.Parameters["World"].SetValue(camera.World);
                x.Parameters["View"].SetValue(camera.View(Vector2.One));
                x.Parameters["Projection"].SetValue(camera.Projection);
            }));

            shaders.Add(new ShaderType("SaturationGamma", 997, "Shaders/SaturationGamma", (Effect x) =>
            {
                x.Parameters["saturation"].SetValue(1.5f);//ambient.saturationIntensity);
                x.Parameters["gamma"].SetValue(.5f + GameSettings.Gamma);
            }));

            //shaders.Add(new ShaderType("HSV", 998, "Shaders/HSV", (Effect x) =>
            //{
            //    x.Parameters["toMultiply"].SetValue(new Vector3(1f, 1f, 1f));
            //    x.Parameters["toAdd"].SetValue(new Vector3(.25f, -.5f, 0f));
            //}));

            shaders.Add(new ShaderType("Heat", 50, "Shaders/HeatWaves", (Effect x) =>
            {
                x.Parameters["matrixTransform"].SetValue(camera.Projection);
            }));

            shaders.Add(new ShaderType("Vignette", 999, "Shaders/Vignette", (Effect x) =>
            {
            }));

            PresentationParameters pp = g.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;
            Point resolution = new Point((int)GameSettings.WindowResolution.X, (int)GameSettings.WindowResolution.Y);

            objectTarget = new RenderTarget2D(g, resolution.X, resolution.Y, true, format, DepthFormat.None); //All objects that are to be drawn behind the light go to this RenderTarget.
            lightTarget = new RenderTarget2D(g, resolution.X, resolution.Y, true, format, DepthFormat.None);  //For light masks only.
            finalTarget = new RenderTarget2D(g, resolution.X, resolution.Y, true, format, DepthFormat.None);
            shadedTarget = new RenderTarget2D(g, resolution.X, resolution.Y, true, format, DepthFormat.None);

            for (int i = 0; i < shaders.Count; i++)
            {
                shaders[i].CurrentState = ShaderType.State.End;
                shaders[i].IsActive = false;
                shaders[i].Load(cm);
                shaders[i].AssignRenderTarget(g, resolution.X, resolution.Y, format);
            }

            ActivateShader("Vignette");
            ActivateShader("SaturationGamma");
            //ActivateShader("Test");
        }

        public void ResetRenderTargets()
        {
            PresentationParameters pp = g.PresentationParameters;
            SurfaceFormat format = pp.BackBufferFormat;
            Point resolution = new Point((int)GameSettings.WindowResolution.X, (int)GameSettings.WindowResolution.Y);

            objectTarget = new RenderTarget2D(g, resolution.X, resolution.Y, true, format, DepthFormat.None); //All objects that are to be drawn behind the light go to this RenderTarget.
            lightTarget = new RenderTarget2D(g, resolution.X, resolution.Y, true, format, DepthFormat.None);  //For light masks only.
            finalTarget = new RenderTarget2D(g, resolution.X, resolution.Y, true, format, DepthFormat.None);
            shadedTarget = new RenderTarget2D(g, resolution.X, resolution.Y, true, format, DepthFormat.None);

            for (int i = 0; i < shaders.Count; i++)
                shaders[i].AssignRenderTarget(g, resolution.X, resolution.Y, format);

            camera.ReloadMatrices();
            camera.Origin = GameSettings.VectorCenter;
        }

        private void SetReferences()
        {

        }

        public void UnloadWorldContent()
        {
            //Don't save empty players
            if (mainScreen.IsActive == false)
            {
                ForceSavePlayerData();
                ForceSaveMapData();
                ForceSaveWorldData();
            }

            tm.UnloadAll();
        }

        private bool firstIteration = true;
        public override void Update(GameTime gt)
        {
            controls.UpdateCurrent();

            debug.Update(gt);

            mainScreen.Update(gt);

            if (firstIteration == false)
            {
                if (mainScreen.IsActive == false)
                {
                    if (Game.IsActive != true && GameSettings.IsDebugging == false)
                    {
                        sm.Pause();
                    }
                }
            }

            camera.Update(gt);
            sm.UpdatePause(gt);

            toolTip.Update(gt);

            if (controls.IsKeyPressedOnce(controls.CurrentControls.TakeScreenshot))
                TakeScreenshot();

            if (!sm.IsPaused())
            {
                sm.Update(gt, g);
                tm.Update(gt);

                ApplyShaderCode(gt);

                ambient.UpdateColor(gt);
            }

            CheckOutputGuide();
            SaveAllData(gt);

            controls.UpdateLast();

            if (firstIteration == true)
                firstIteration = false;
        }

        private void CheckOutputGuide()
        {
            if (GameSettings.IsDebugging == true && MAIN_IsActive == false) //Only allow this when not in the main screen, unfortunately. =(
            {
                if (controls.IsKeyPressedOnce(Microsoft.Xna.Framework.Input.Keys.F10))
                {
                    GAM_OutputCreationGuide();
                }
            }
        }
        public void GAM_OutputCreationGuide()
        {
            try
            {
                GAM.OutputCreationGuide("Creation Handbook.txt", sm.OutputExtraCreationGuide());
            }
            catch (Exception e)
            {
                Logger.AppendLine("Error saving creation guide to text file. " + e.Message);
            }
        }

        private void SaveAllData(GameTime gt)
        {
            //Keep the game from saving empty characters
            if (mainScreen.IsActive == false)
            {
                mapTimeToSave += gt.ElapsedGameTime.Milliseconds;
                playerTimeToSave += gt.ElapsedGameTime.Milliseconds;
                worldTimeToSave += gt.ElapsedGameTime.Milliseconds;

                if (mapTimeToSave > GameSettings.MapSaveSpeed() * 1000)
                {
                    dataManager.WriteMapData(PlayerName, tm.MapName);
                    mapTimeToSave = 0;
                }

                if (playerTimeToSave > GameSettings.PlayerSaveSpeed() * 1000)
                {
                    dataManager.WritePlayerData(PlayerName);
                    tm.CHARACTERS_Save(PlayerName);
                    playerTimeToSave = 0;
                }

                if (worldTimeToSave > GameSettings.MapSaveSpeed() * 1000)
                {
                    dataManager.WriteWorldData(PlayerName);
                    worldTimeToSave = 0;
                }
            }
        }
        public void ForceSavePlayerData() { if (!string.IsNullOrEmpty(playerName)) { dataManager.WritePlayerData(PlayerName); tm.CHARACTERS_Save(PlayerName); } }
        public void ForceSaveMapData() { if (!string.IsNullOrEmpty(playerName)) { dataManager.WriteMapData(PlayerName, tm.MapName); } }
        public void ForceSaveWorldData() { if (!string.IsNullOrEmpty(playerName)) { dataManager.WriteWorldData(PlayerName); } }

        private Texture2D sceneTexture;
        public void DrawTargets(SpriteBatch sb)
        {
            RenderLightTarget(sb);
            RenderObjectTarget(sb);
            RenderFinal(sb);
            
            sceneTexture = (Texture2D)finalTarget;

            AddShaders();

            if (activeShaders.Count > 0)
            {
                for (int i = 0; i < activeShaders.Count; i++)
                {
                    g.SetRenderTarget(activeShaders[i].renderTarget);
                    g.Clear(Color.Transparent);

                    sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

                    for (int p = 0; p < activeShaders[i].Effect.CurrentTechnique.Passes.Count; p++)
                    {
                        activeShaders[i].Effect.CurrentTechnique.Passes[p].Apply();

                        if (i == 0)
                            sb.Draw(sceneTexture, GameSettings.VectorCenter, new Rectangle(0, 0, sceneTexture.Width, sceneTexture.Height), Color.White, 0f, sceneTexture.Center(), 1f, SpriteEffects.None, 1f);
                        else
                            sb.Draw(activeShaders[i - 1].renderTarget, GameSettings.VectorCenter, new Rectangle(0, 0, (int)activeShaders[i - 1].renderTarget.Width, (int)activeShaders[i - 1].renderTarget.Height), Color.White, 0f, activeShaders[i - 1].renderTarget.Center(), 1f, SpriteEffects.None, 1f);
                    }

                    sb.End();
                }
                
                g.SetRenderTarget(shadedTarget);
                g.Clear(ClearOptions.Target, Color.Transparent, 1.0f, 0);

                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);

                for (int i = 0; i < activeShaders.Count; i++)
                    sb.Draw(activeShaders[i].renderTarget, GameSettings.VectorCenter, new Rectangle(0, 0, activeShaders[i].renderTarget.Width, activeShaders[i].renderTarget.Height), Color.White, 0f, activeShaders[i].renderTarget.Center(), 1f, SpriteEffects.None, 1f);

                sb.End();
            }
            else if (activeShaders.Count == 0)
            {
                g.SetRenderTarget(shadedTarget);
                g.Clear(Color.Transparent);

                sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
                sb.Draw(finalTarget, GameSettings.VectorCenter, new Rectangle(0, 0, finalTarget.Width, finalTarget.Height), Color.White, 0f, finalTarget.Center(), 1f, SpriteEffects.None, 1f);
                sb.End();
            }

            g.SetRenderTarget(null);
            g.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
            sb.Draw(shadedTarget, GameSettings.VectorCenter, new Rectangle(0, 0, shadedTarget.Width, shadedTarget.Height), Color.White, 0f, shadedTarget.Center(), 1f, SpriteEffects.None, 1f);
            sb.End();
        }
        private void AddShaders()
        {
            activeShaders.Clear();
            for (int i = 0; i < shaders.Count; i++)
            {
                if (shaders[i].IsActive == true)
                    activeShaders.Add(shaders[i]);
            }
            activeShaders.Sort((s1, s2) => s1.Order.CompareTo(s2.Order));
        }
        private void ApplyShaderCode(GameTime gt)
        {
            UpdateShaderVariables(gt);

            for (int i = 0; i < activeShaders.Count; i++)
                activeShaders[i].ContinueShading(activeShaders[i].Effect);
        }

        private void UpdateShaderVariables(GameTime gt)
        {
            UpdateSaturationGamma(gt);
            //UpdateHSV(gt);
            UpdateRadial(gt);
            UpdateWiggle(gt);
        }

        bool moveTimerUp = true; float timer = 0f;
        private void UpdateRadial(GameTime gt)
        {
            ShaderType radial = GetShader("radialBlur");

            timer = MathHelper.Clamp(timer, -1f, 1f);

            if (radial.CurrentState == ShaderType.State.Started && radial.IsActive == true)
            {
                if (timer <= -1f)
                    moveTimerUp = true;
                else if (timer >= 1f)
                    moveTimerUp = false;

                if (moveTimerUp == true)
                    timer += (float)gt.ElapsedGameTime.Milliseconds / 250;
                else
                    timer -= (float)gt.ElapsedGameTime.Milliseconds / 500;
            }
            else if (radial.CurrentState == ShaderType.State.End)
            {
                timer -= (float)gt.ElapsedGameTime.Milliseconds / 500;

                if (timer <= -1f)
                {
                    radial.IsActive = false;
                    radial.CurrentState = ShaderType.State.Started;
                }
            }
        }
        float wiggleValue = 0f, wiggleZoom = 0f; ShaderType wiggle;
        private void UpdateWiggle(GameTime gt)
        {
            wiggle = GetShader("wiggle");

            wiggleValue += (float)gt.ElapsedGameTime.Milliseconds / 500;

            if (wiggle.CurrentState == ShaderType.State.Started && wiggle.IsActive == true)
            {
                wiggleZoom += (float)gt.ElapsedGameTime.Milliseconds / 500;
            }
            else if (wiggle.CurrentState == ShaderType.State.End)
            {
                wiggleZoom -= (float)gt.ElapsedGameTime.Milliseconds / 500;

                if (wiggleZoom <= 0f)
                {
                    wiggle.IsActive = false;
                    wiggle.CurrentState = ShaderType.State.Started;
                    wiggleValue = 0f;
                }
            }

            wiggleZoom = (float)MathHelper.Clamp(wiggleZoom, 0f, 1f);
        }
        private void UpdateHSV(GameTime gt)
        {
            ShaderType hsv = GetShader("HSV");
            if (hsv.CurrentState == ShaderType.State.End)
            {
                hsv.IsActive = false;
                hsv.CurrentState = ShaderType.State.Started;
            }
        }
        private void UpdateSaturationGamma(GameTime gt)
        {
            ShaderType sc = GetShader("SaturationGamma");
            if (sc.CurrentState == ShaderType.State.End)
            {
                sc.IsActive = false;
                sc.CurrentState = ShaderType.State.Started;
            }
            else
            {
            }
        }

        private void RenderLightTarget(SpriteBatch sb)
        {
            g.SetRenderTarget(lightTarget);
            g.Clear(WorldLight.AmbientColor);

            sb.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, null, null, camera.View(Vector2.One));

            tm.DrawLights(sb);

            sb.End();

            //Screenshot.TakePicture(lightTarget, "lights.png");
        }
        private void RenderObjectTarget(SpriteBatch sb)
        {
            g.SetRenderTarget(objectTarget);
            g.Clear(Color.Black);

            sb.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, camera.View(Vector2.One));

            tm.Draw(sb);

            sb.End();
        }
        private void RenderFinal(SpriteBatch sb)
        {
            g.SetRenderTarget(finalTarget);
            g.Clear(Color.Transparent);

            sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            lightEffect.CurrentTechnique = lightEffect.Techniques["LightTechnique"];
            lightEffect.Parameters["lightMask"].SetValue(lightTarget);

            lightEffect.CurrentTechnique.Passes[0].Apply();
            sb.Draw(objectTarget, GameSettings.VectorCenter, new Rectangle(0, 0, objectTarget.Width, objectTarget.Height), Color.White, 0f, objectTarget.Center(), 1f, SpriteEffects.None, 1f);

            sb.End();

            //Screenshot.TakePicture(finalTarget, "final.png");
        }

        public void DrawAboveLight(SpriteBatch sb)
        {
            sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);

            tm.DrawAboveLight(sb);
            debug.Draw(sb);

            sb.End();

            tm.DrawEditor(sb);
            debug.DrawConsole(sb);
            sm.Draw(sb);

            //Draw mainscreen over everything except tooltip and cursor.
            mainScreen.Draw(sb);

            sb.Begin(samplerState: SamplerState.PointClamp);

            mainScreen.DrawScreen(sb);
            sm.DrawTransition(sb);

            //Draw tooltip
            toolTip.Draw(sb);

            //Draw cursor above everything else
            sm.DrawCursor(sb);

            sb.End();
        }

        public void TakeScreenshot()
        {
            if (Screenshot.IsPictureTaken() == true)
            {
                try
                {
                    DateTime dateTime = DateTime.Now;
                    Screenshot.TakePicture(shadedTarget, "Screenshots/", playerEntity.Name + dateTime.ToString("yy-MM-dd_H;mm;ss") + ".png");
                }
                catch (Exception e)
                {
                    debug.OutputError("SCREENSHOT: Error taking screenshot: " + e.Message);
                    Screenshot.isPictureTaken = true; //reset to true just in case...
                }
            }
        }
        private void TakeRawScreenshot()
        {
            if (Screenshot.IsPictureTaken() == true)
            {
                if (controls.IsKeyPressedOnce(controls.CurrentControls.TakeScreenshot))
                {
                    try { Screenshot.TakeRawPicture(GraphicsDevice); }
                    catch (Exception e)
                    {
                        debug.OutputError("SCREENSHOT: Error taking screenshot: " + e.Message);
                        Screenshot.isPictureTaken = true; //reset to true just in case...
                    }
                }
            }
        }

        /// <summary>
        /// Not working! Screw you
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public int DarknessLevel(Vector2 position)
        {
            Color objectPoint = objectTarget.SelectColor(position.ToPoint());
            Color finalPoint = finalTarget.SelectColor(position.ToPoint());

            int objectCombined = objectPoint.R + objectPoint.G + objectPoint.B + objectPoint.A;
            int finalCombined = finalPoint.R + finalPoint.G + finalPoint.B + finalPoint.A;

            int returnValue = objectCombined - finalCombined;

            return (int)((returnValue));
        }
        public Color LightColor(Vector2 position)
        {
            return lightTarget.SelectColor(position.ToPoint());
        }

        // [Encapsulate] Shader methods
        public ShaderType GetShader(string name)
        {
            ShaderType shader = null;
            for (int i = 0; i < shaders.Count; i++)
            {
                if (shaders[i].Name.ToUpper().Equals(name.ToUpper()))
                    shader = shaders[i];
            }
            return shader;
        }

        public void ActivateShader(string name)
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                if (shaders[i].Name.ToUpper().Equals(name.ToUpper()))
                {
                    shaders[i].IsActive = true;
                    shaders[i].CurrentState = ShaderType.State.Started;
                }
            }
        }
        public void DeactivateShader(string name)
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                if (shaders[i].Name.ToUpper().Equals(name.ToUpper()))
                    shaders[i].CurrentState = ShaderType.State.End;
            }
        }
        public void SetShaderState(string name, ShaderType.State state)
        {
            for (int i = 0; i < shaders.Count; i++)
            {
                if (shaders[i].Name.ToUpper().Equals(name.ToUpper()))
                {
                    shaders[i].CurrentState = state;

                    if (state == ShaderType.State.Started)
                        shaders[i].IsActive = true;
                }
            }
        }

        // [Encapsulate] Menu methods
        public bool MAIN_IsActive { get { return mainScreen.IsActive; } }

        private List<Action> randomMaps = new List<Action>();
        public void RandomMainMap(string map, Vector2 mainOffset, Vector2 newOffset, Vector2 loadOffset, Vector2 configOffset, Vector2 exitOffset)
        {
            tm.LoadMap(map, new Point(-20, -20));
            mainScreen.SetScreenPositions(mainOffset, newOffset, loadOffset, configOffset, exitOffset);
        }

        private void InitializeMaps()
        {
            Vector2 main = new Vector2(3000, 3000);
            randomMaps.Add(() => RandomMainMap("MainMenu.map", main, main + new Vector2(0, -GameSettings.VectorResolution.Y), main + new Vector2(GameSettings.VectorResolution.X, 0),
                                                                            new Vector2(1200, 2000), main + new Vector2(0, GameSettings.VectorResolution.Y)));
        }

        public void StartNewGame(string name, string characterClass, Skillset skills, List<BaseItem> items, string mapName, Point tileLocation, string pathway, int pathwayID, string birthplace)
        {
            mainScreen.IsActive = false;

            playerName = name;
            playerEntity.SetPlayerData(name, characterClass, skills, items, pathway, pathwayID, birthplace);

            camera.SetCameraState(Camera.CameraState.Current);
            playerEntity.SetPlayerControlled(true);
            camera.ForceLookAt(playerEntity.Position);

            sm.OPTIONS_ResetPosition();
            sm.OPTIONS_IsActive = false;

            tm.LoadMap(mapName, tileLocation);
        }
        public void LoadSaveFile(string name)
        {
            mainScreen.IsActive = false;

            //Prepare the player entity for the next save file load
            playerEntity.ResetPlayerData();
            sm.RUMORS_Reset();
            sm.OFFERINGS_Reset();
            sm.STONEHOLD_Reset();

            //Reset Options UI
            sm.OPTIONS_ResetPosition();
            sm.OPTIONS_IsActive = false;

            //Assign name
            playerName = name;
            playerEntity.Name = name;

            //Load data to player and world
            dataManager.ReadWorldData(playerName);
            dataManager.ReadPlayerData(playerName);
            tm.CHARACTERS_Load(playerName);

            //Ensure the player entity is being controlled, and send references to their respective interfaces
            playerEntity.SetPlayerControlled(true);

            //Set camera state to default
            camera.SetCameraState(Camera.CameraState.Current);
            camera.ForceLookAt(playerEntity.Position);

            //Load map of player
            tm.LoadMap(playerEntity.SavedMap, new Point(-1, -1), false);
        }

        public void QuitGame()
        {
            Logger.AppendLine("Preparing for game exit.");

            //Unload all content first
            UnloadWorldContent();
            Logger.AppendLine("Game content has been unloaded.");

            //Game will now exit
            Logger.AppendLine("Exiting...");
            Game.Exit();
        }

        public void GoToMainMenu()
        {
            ForceSaveMapData();
            ForceSavePlayerData();
            ForceSaveWorldData();

            randomMaps[random.Next(0, randomMaps.Count)].Invoke();

            mainScreen.ForceMainScreen();
            mainScreen.IsActive = true;

            playerName = string.Empty;

            //Prepare the player entity for the next save file load
            playerEntity.ResetPlayerData();
            sm.RUMORS_Reset();
            sm.OFFERINGS_Reset();
            sm.STONEHOLD_Reset();

            //Reset Options UI
            sm.OPTIONS_IsActive = true;

            camera.SetCameraState(Camera.CameraState.Cinematic);
        }
    }
}