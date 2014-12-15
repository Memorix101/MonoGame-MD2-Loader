/********************
 * Mouse Bound Camera
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

namespace Engine1637 {

    public class MouseBoundCamera : StandardCamera {

        private bool _Enabled = false;
        private bool _ZoomAlways = true;

        private Keys _EnableKey = Keys.F12;
        private bool _EnableKeyDown = false;

        private Keys _CenterKey = Keys.H;
        private bool _CenterKeyDown = false;

        private int _MouseScroll = 0;
        private int _MouseX = -1;
        private int _MouseY = -1;
       
        public MouseBoundCamera( GraphicsDevice device ) : base( device ) { }

        public MouseBoundCamera( MouseBoundCamera camera ) : base(camera) { }

        public Keys EnableKey {

            get { return _EnableKey; }

            set { _EnableKey = value; }

        }

        public Keys CenterKey {

            get { return _CenterKey; }

            set { _CenterKey = value; }

        }

        public bool ZoomAlways {

            get { return _ZoomAlways; }

            set { _ZoomAlways = value; }

        }

        public override void HandleInput( KeyboardState keyboardState, MouseState mouseState ) {

            if ( keyboardState.IsKeyDown(_EnableKey) ) { _EnableKeyDown = true; return; }

            if ( _EnableKeyDown && keyboardState.IsKeyUp(_EnableKey) ) {

                if ( _Enabled ) { _Enabled = false; } else { _Enabled = true; }

                _EnableKeyDown = false;

                return;

            }

            if ( keyboardState.IsKeyDown(_CenterKey) ) { _CenterKeyDown = true; return; }

            if ( _CenterKeyDown && keyboardState.IsKeyUp(_CenterKey) ) {

                RotationH = 270;
                RotationV = 30;
                Gap = 30;

                _Enabled = false;

                _CenterKeyDown = false;

                return;

            }

            if ( _ZoomAlways || _Enabled ) {

                if ( mouseState.ScrollWheelValue > _MouseScroll ) {

                    Gap -= 3f;

                } else if ( mouseState.ScrollWheelValue < _MouseScroll ) {

                    Gap += 3f;

                }

                _MouseScroll = mouseState.ScrollWheelValue;

            }

            if ( _Enabled && mouseState.RightButton == ButtonState.Pressed ) {

                if ( _MouseX == -1 ) { _MouseX = Device.Viewport.Width / 2; }

                if ( mouseState.X < _MouseX - 10 ) {

                    RotationH -= 5f;

                } else if ( mouseState.X > _MouseX + 10 ) {

                    RotationH += 5f;

                }

                if ( _MouseY == -1 ) { _MouseY = Device.Viewport.Width / 2; }

                if ( mouseState.Y < _MouseY - 5 ) {

                    RotationV -= 2f;

                } else if ( mouseState.Y > _MouseY + 5 ) {

                    RotationV += 2f;

                }

                _MouseX = Device.Viewport.Width / 2;
                _MouseY = Device.Viewport.Height / 2;
                Mouse.SetPosition(Device.Viewport.Width / 2, Device.Viewport.Height / 2);

            } else if ( mouseState.RightButton == ButtonState.Released ) {

                _MouseX = -1;
                _MouseY = -1;

            }

        }
        
    }

}