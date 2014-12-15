using System;
using MonoMac.AppKit;
using MonoMac.Foundation;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using Engine1637;

namespace MonoGame_MD2Loader_Mac
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        ContentManager content;

        TutorialScene Scene;
        ControlledActor Player;

        public Game1() {
          //  graphics = new GraphicsDeviceManager(this);
          //  content = new ContentManager(Services);

			graphics = new GraphicsDeviceManager (this);

			Content.RootDirectory = "Assets";

			graphics.IsFullScreen = false;

        }
        protected override void Initialize() {

            base.Initialize();

            graphics.PreferredBackBufferWidth = 640;
            graphics.PreferredBackBufferHeight = 480;
            graphics.IsFullScreen = false;
            graphics.ApplyChanges();

        }

     //   protected override void LoadContent( bool loadAllContent ) {

        protected override void LoadContent()
        {
  
            if ( true ) {

                /* ***
                 * Set-up a camera so that we can see what is going on.
                 * This camera is a mouse bound camera so you can zoom in and out 
                 * and rotate around for a better view.
                 * 
                 * ***/
                MouseBoundCamera Camera = new MouseBoundCamera(graphics.GraphicsDevice);

                Camera.RotationH = 270;         // How many degrees to rotate the camera around. 270 degrees puts it straight on the z-axis.
                Camera.RotationV = 30;          // How many degrees to rotate/move the camera up.

                Camera.MinGap = 10;             // The minimum distance the camera can be from the target.
                Camera.MaxGap = 40;             // The maximum distance the camera can be from the target.
                Camera.Gap = 30;                // How far the camera should start from target.

                Camera.EnableKey = Keys.F12;    // Which key should enable/disable camera movement?
                Camera.ZoomAlways = true;       // Should the zoom-in/zoom-out (scroll wheel) camera movement always be enabled.
                Camera.CenterKey = Keys.H;      // Which key should center the camera on its target?

                /* ***
                 * Initialize the scene in which everything will take place.
                 * 
                 * ***/
                Scene = new TutorialScene();

                Scene.Camera = Camera;  // Which camera are we viewing the scene through?

                /* ***
                 * Load and set-up our Goblin model.
                 * 
                 * ***/
                MD2Model Goblin = new MD2Model(Camera);

                // Load the model from a file; easy enough.
                Goblin.LoadFromFile("Assets/Models/Goblin/model.md2");

                // Lets find out what animations our goblin model has . . .
                // (check the project directory for 'GOBLIN-ANIMATIONS.TXT')
                // Make sure to call this function after you load the model from file.
                Goblin.DumpAnimationListToFile(@"..\..\..\GOBLIN-ANIMATIONS.TXT");

                // Set-up the skins for the model.
                // Basically give the skin a name, and point at a valid skin file.
                Goblin.DefineSkin("Stone Goblin", "Assets/Models/Goblin/Skins/stone-goblin.jpg");
                Goblin.DefineSkin("Ice Goblin", "Assets/Models/Goblin/Skins/ice-goblin.jpg");
                Goblin.DefineSkin("Fire Goblin", "Assets/Models/Goblin/Skins/red-goblin.jpg");

                // IMPORTANT: The 'RotationOffset' of a model is the number of degrees you must rotate the model to make it look at positive-z.
                // Most MD2 models will be looking towards negative-x or negative-z by default.
                // The Goblin model is facing negative-x so we need to rotate him 90 degrees.
                // This number may not make sense, it may seem backwards; but that is because MD2 models are, simply, designed backwards . . . while it is looking to negative-x, the the mesh is actually facing towards positive-x.
                // If this made your head spin, don't worry. When loading a model for the first time, play with this number until the models back is to you . . .
                // You only need to set this number while setting up a model, if you need for some reason to manually rotate an actor, think of positive-z as 0-degrees in a clockwise rotating system.
                Goblin.RotationOffset = 90;
                
                // Set the starting / default animation so that the goblin won't just be frozen.
                Goblin.CurrentAnimation = "Idle";

                /* ***
                 * Create a new actor that will be a goblin.
                 * 
                 * ***/
                StandardActor npcIceGoblin = new StandardActor();

                npcIceGoblin.Model = new MD2Model(Goblin);              // Inherit the settings from our already set-up goblin model.
                npcIceGoblin.Model.CurrentSkin = "Ice Goblin";          // Make this guy an ice goblin.
                npcIceGoblin.Model.Scale = 20;                          // Make this guy 20% the default size. Our goblin model, by default, is huge.

                npcIceGoblin.SetPrimaryAnimation("Idle", "stand");       // Set the primary animation for the 'Idle' action to be the 'stand' animation.

                npcIceGoblin.AddOptionalAnimation("Idle", "salute");    // Make it so that some times while performing the 'Idle' action, the actor does the 'salute' animation.
                npcIceGoblin.AddOptionalAnimation("Idle", "taunt");     // Make it so that some times while performing the 'Idle' action, the actor does the 'taunt' animation.

                npcIceGoblin.Position = new Vector3(0, 5, 40);          // Position the actor at X:0, Y:5, Z:40. Y is 5 because the goblin models origin is not the base of its feet, so we need to lift it slightly so it doesn't sink in to ground.

                // Make the actor face us. 
                // By default (after 'RotationOffset') the actor will be facing away from us, because 'north' is positive-z, or 0 degrees. 
                // To face us we basically need the actor to do a 180, so . . . make them do a 180 degree turn (=
                npcIceGoblin.Rotation = 180;                            

                Scene.Actors.Add(npcIceGoblin);                         // Add the actor to the scene.

                /* ***
                 * Create another goblin actor.
                 * 
                 * ***/
                StandardActor npcStoneGoblin = new StandardActor();

                npcStoneGoblin.Model = new MD2Model(Goblin);
                npcStoneGoblin.Model.CurrentSkin = "Stone Goblin";      // Make this guy a stone goblin.
                npcStoneGoblin.Model.Scale = 22;                        // Make this guy 22% the default size. Slightly bigger than the last.

                npcStoneGoblin.SetPrimaryAnimation("Idle", "stand");

                npcStoneGoblin.AddOptionalAnimation("Idle", "salute");  // This guy can salute while idle, but he does not taunt, he is more refined (=

                npcStoneGoblin.Position = new Vector3(-5, 5, 30);       // Position the actor at X:-5, Y:5, Z:30. Infront of and to the left of our last goblin.
                npcStoneGoblin.Rotation = 210;                          // Make this goblin face towards us, but slightly to the west.

                Scene.Actors.Add(npcStoneGoblin);

                /* ***
                 * Create yet another goblin actor.
                 * You could make all these goblins in loop, or make a goblin spawner, 
                 * but that is a bit ahead of the purpose of this tutorial.
                 * 
                 * ***/
                StandardActor npcFireGoblin = new StandardActor();

                npcFireGoblin.Model = new MD2Model(Goblin);
                npcFireGoblin.Model.CurrentSkin = "Fire Goblin";        // Make this guy a fire goblin.
                npcFireGoblin.Model.Scale = 18;                         // Make this guy 18% the default size. Slightly smaller than the first goblin.

                npcFireGoblin.SetPrimaryAnimation("Idle", "stand");
                npcFireGoblin.AddOptionalAnimation("Idle", "taunt");    // This guy can taunt while idle, but he does not think to salute, he is crude /=

                npcFireGoblin.Position = new Vector3(5, 5, 20);         // Position the actor at X:5, Y:5, Z:20. Infront of and to the right of our other goblins.
                npcFireGoblin.Rotation = 150;                          // Make this goblin face towards us, but slightly to the east.

                Scene.Actors.Add(npcFireGoblin);

                /* ***
                 * Load and set-up our Knight model.
                 * 
                 * ***/
                MD2Model Knight = new MD2Model(Camera);

                Knight.LoadFromFile("Assets/Models/Knight/model.md2");
                Knight.DumpAnimationListToFile(@"..\..\..\KNIGHT-ANIMATIONS.TXT");

                Knight.DefineSkin("Light Knight", "Assets/Models/Knight/Skins/standard-knight.jpg");
                Knight.DefineSkin("Dark Knight", "Assets/Models/Knight/Skins/dark-knight.jpg");

                // The knight model is also facing negative-x by default.
                // See the 'RotationOffset' comment for the goblin model (above) for more info.
                Knight.RotationOffset = 90;

                /* ***
                 * Create a controllable actor to represent the player.
                 * 
                 * ***/
                Player = new ControlledActor();                 // The actual 'Player' variable is declared as a member of class, cause we need to access it in update function.

                Player.Model = new MD2Model(Knight);
                Player.Model.CurrentSkin = "Dark Knight";       // Lets make our player a dark knight, muwahaha (=
                Player.Model.Scale = 20;                        // By default the knight model is also rather big, so scale the player down to 20% of original size.

                Player.SetPrimaryAnimation("Idle", "stand");

                Player.AddOptionalAnimation("Idle", "taunt");   
                Player.AddOptionalAnimation("Idle", "wave");    // Player can taunt and wave while idle.

                Player.SetPrimaryAnimation("Run", "run");       // The primary animation for the "Run" action is the "run" animation.

                Player.MovementSpeed = 0.45f;                   // Make the player move at 0.45 units per update while moving.
                
                // Set-up which keys control the character.
                Player.FowardKey = Keys.W;
                Player.LeftKey = Keys.A;
                Player.RightKey = Keys.D;

                Player.Position = new Vector3(0, 5, 0);         // Position the actor at X:0, Y:5, Z:0. In front of all the goblins.

                Scene.Actors.Add(Player);

                Camera.AttachTo(Player.Model);                  // IMPORTANT: Attach the camera to the player. The camera will center on the player and follow the player as it moves.

                /* ***
                 * Load and set-up our Mushroom model.
                 * 
                 * ***/
                MD2Model Mushroom = new MD2Model(Camera);

                Mushroom.LoadFromFile("Assets/Models/Mushroom/model.md2");
                Mushroom.DumpAnimationListToFile(@"..\..\..\MUSHROOM-ANIMATIONS.TXT");

                Mushroom.DefineSkin("Bland", "Assets/Models/Mushroom/Skins/standard.jpg");
                
                /* ***
                 * Create some mushroom models, to spruce the place up.
                 * They don't do much, just for decoration (to prove non-animated MD2s work as well).
                 * 
                 * ***/

                StandardActor Shroom;
                
                Shroom = new StandardActor();
                Shroom.Model = new MD2Model(Mushroom);
                Shroom.Model.CurrentSkin = "Bland";
                Shroom.Model.Scale = 10;
                Shroom.Position = new Vector3(-50, 0, -50);
                Scene.Actors.Add(Shroom);

                Shroom = new StandardActor();
                Shroom.Model = new MD2Model(Mushroom);
                Shroom.Model.CurrentSkin = "Bland";
                Shroom.Model.Scale = 10;
                Shroom.Position = new Vector3(-50, 0, 50);
                Scene.Actors.Add(Shroom);

                Shroom = new StandardActor();
                Shroom.Model = new MD2Model(Mushroom);
                Shroom.Model.CurrentSkin = "Bland";
                Shroom.Model.Scale = 10;
                Shroom.Position = new Vector3(50, 0, 50);
                Scene.Actors.Add(Shroom);

                Shroom = new StandardActor();
                Shroom.Model = new MD2Model(Mushroom);
                Shroom.Model.Scale = 10;
                Shroom.Model.CurrentSkin = "Bland";
                Shroom.Position = new Vector3(50, 0, -50);
                Scene.Actors.Add(Shroom);
 
            }

        }

        protected override void Draw( GameTime gameTime ) {


            float frameRate = 1;

            this.Window.Title = "MonoGame MD2 Loader " + frameRate / (float)gameTime.ElapsedGameTime.TotalSeconds;

            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            Scene.Draw(gameTime);


            base.Draw(gameTime);

        }

        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        protected override void Update( GameTime gameTime ) {

            KeyboardState keyState = Keyboard.GetState();
            MouseState mouseState = Mouse.GetState();

            if (keyState.IsKeyDown(Keys.Escape))
                this.Exit();

            Player.HandleInput(keyState, mouseState);
            Scene.Camera.HandleInput(keyState, mouseState);

            foreach ( StandardActor actor in Scene.Actors ) {

                actor.Idle();

            }

            base.Update(gameTime);
        }

    }
}
