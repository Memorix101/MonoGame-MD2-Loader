/********************
 * Engine1637: Standard Camera
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
    
    public class StandardCamera : ICamera {

        protected GraphicsDevice _Device;

        protected Vector3 _Position;
        protected Vector3 _Target;
        protected Vector3 _Offset;
      
        protected float _Gap;
        protected float _MinGap;
        protected float _MaxGap;

        protected float _RotationH;
        protected float _RotationV;

        protected Matrix _Perspective;

        protected IModel _Attachment;

        public StandardCamera( GraphicsDevice device ) {

            _Device = device;

            _Position = new Vector3(0.0f, 0.0f, 0.0f);
            _Target = new Vector3(0.0f, 0.0f, 0.0f);
            _Offset = new Vector3(0.0f, 0.0f, 0.0f);

            _Gap = 0.0f;
            _MinGap = 0.0f;
            _MaxGap = 0.0f;

            _RotationH = 0.0f;
            _RotationV = 0.0f;
            
            _Perspective = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), (float)_Device.Viewport.Width / (float)_Device.Viewport.Height, 1.0f, 100.0f);

            _Attachment = null;

        }

        public StandardCamera( StandardCamera camera ) {

            _Device = camera.Device;
            _Perspective = camera.Perspective;
            _Position = camera.Position;
            _Target = camera.Target;

        }

        public virtual GraphicsDevice Device { get { return _Device; } }

        public virtual int FPS {

            get { return 12; }

            set { }
            
        }

        public virtual Vector3 Position {

            get {

                CalculatePosition();
                return _Position;
            
            }

            set { _Position = value; }
        
        }

        public virtual Vector3 Target {

            get {

                if ( _Attachment == null ) {

                    return _Target;

                } else {

                    return _Attachment.Position;

                }
            
            }

            set { _Target = value; }

        }

        public virtual Matrix Perspective {

            get { return _Perspective; }

            set { _Perspective = value; } 
        
        }

        public virtual Matrix View {

            get { return Matrix.CreateLookAt( Position, Target, Vector3.Up ); }
            
        }

        public Vector3 Offset { 

            get { return new Vector3(0,0,0); }

            set { }

        }

        public float Gap {

            get { return _Gap; }

            set { 
                
                _Gap = value;

                if ( _Gap < MinGap ) { _Gap = MinGap; }
                if ( _Gap > MaxGap ) { _Gap = MaxGap; }

                CalculatePosition();
            
            }

        }

        public float MinGap {

            get { return _MinGap; }

            set { _MinGap = value; }

        }

        public float MaxGap {

            get { return _MaxGap; }

            set { _MaxGap = value; }

        }

        public float RotationH {

            get { return _RotationH; }

            set { 
                
                _RotationH = value;

                CalculatePosition();
            
            }

        }

        public float RotationV {

            get { return _RotationV; }

            set { 
                
                _RotationV = value;

                if ( _RotationV > 89 ) { _RotationV = 89; }
                if ( _RotationV < 0 ) { _RotationV = 0; }

                CalculatePosition();
            
            }

        }

        public void AttachTo( IModel model ) {

            _Attachment = model;

            Target = _Attachment.Position;

            CalculatePosition();

        }

        protected virtual void CalculatePosition() {

            _Position.X = (float)(Gap * Math.Cos(MathHelper.ToRadians(RotationV)) * Math.Cos(MathHelper.ToRadians(RotationH)));
            _Position.Y = (float)(Gap * Math.Sin(MathHelper.ToRadians(RotationV)));
            _Position.Z = (float)(Gap * Math.Cos(MathHelper.ToRadians(RotationV)) * Math.Sin(MathHelper.ToRadians(RotationH)));

            _Position += Target; // +new Vector3(0, 20, 10);


        }

        public virtual void HandleInput( KeyboardState keyboardState, MouseState mouseState ) { }
    
    }
    
}
