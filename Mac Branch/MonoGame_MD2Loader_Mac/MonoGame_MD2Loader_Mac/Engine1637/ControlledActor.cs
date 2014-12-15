/********************
 * Engine1637: Controlled Actor
 * 
 * Version: 1.0
 * Author(s): Matthew Lynch
 * 
 * Copyright:
 * 
 * This class is released under the 
 * Creative Commons Attribution-Share Alike 3.0 License. 
 * 
 * For more information please see:
 * http://creativecommons.org/licenses/by-sa/3.0/
 * 
 ********************/

#region Standard Using Statements
using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

using Engine1637.Interfaces;

namespace Engine1637 {

    public class ControlledActor : StandardActor {

        protected Keys _LeftKey = Keys.A;
        protected Keys _RightKey = Keys.D;
        protected Keys _FowardKey = Keys.W;
        protected bool _FowardKeyDown = false;

        public Keys LeftKey {

            get { return _LeftKey; }

            set { _LeftKey = value; }

        }

        public Keys RightKey {

            get { return _RightKey; }

            set { _RightKey = value; }

        }

        public Keys FowardKey {

            get { return _FowardKey; }

            set { _FowardKey = value; }

        }

        public override void HandleInput( KeyboardState keyboardState, MouseState mouseState ) {

            if ( _FowardKeyDown && keyboardState.IsKeyUp(_FowardKey) ) {

                _FowardKeyDown = false;
                StopRunning();

            }

            if ( keyboardState.IsKeyDown(_FowardKey) ) {

                _FowardKeyDown = true;
                Run();

            }

            if ( keyboardState.IsKeyDown(_LeftKey) ) {

                Rotation += 4;

            }

            if ( keyboardState.IsKeyDown(_RightKey) ) {

                Rotation -= 4;

            }

        }

    }

}
