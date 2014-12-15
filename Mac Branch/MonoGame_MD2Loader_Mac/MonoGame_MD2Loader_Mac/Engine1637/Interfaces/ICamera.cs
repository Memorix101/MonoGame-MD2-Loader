/********************
 * Engine1637: Camera Interface
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

namespace Engine1637.Interfaces {
    
    public interface ICamera {

        /// <summary>
        /// The graphical device handling the scene in which the camera is to be placed.
        /// </summary>
        GraphicsDevice Device { get; }

        int FPS { get; set; }

        /// <summary>
        /// The XYZ position in the scene to place camera.
        /// Manually setting this property will detach the object from the model it is currently attached to.
        /// </summary>
        Vector3 Position { get; set; }

        /// <summary>
        /// The XYZ position in the scene the camera is pointing at.
        /// Manually setting this property will detach the object from the model it is currently attached to.
        /// </summary>
        Vector3 Target { get; set; }

        /// <summary>
        /// How far offset the camera should be from the Camera.Target. 
        /// Camera.Position will be calculate using Camera.Target + Camera.Offset. 
        /// Setting this property will not affect attachment; however it will have an affect on rotation.
        /// </summary>
        Vector3 Offset { get; set; }

        /// <summary>
        /// The gap between the camera and the Camera.Target. 
        /// The gap is applied during rotation calculations. 
        /// If a Camera.Offset exists the following order is applied: Camera.Target + Camera.Offset + Camera.Gap. 
        /// </summary>
        float Gap { get; set; }

        float MinGap { get; set; }

        float MaxGap { get; set; }

        float RotationH { get; set; }

        float RotationV { get; set; }

        void AttachTo( IModel model );
        
        Matrix Perspective { get; set; }

        Matrix View { get; }

        void HandleInput(KeyboardState keyboardState, MouseState mouseState);
        
    }

}
