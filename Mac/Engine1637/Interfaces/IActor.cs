/********************
 * Engine1637: Actor Interface
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

    public interface IActor {

        IAnimatedModel Model { get; set; }

        Vector3 Position { get; set; }

        void HandleInput( KeyboardState keyboardState, MouseState mouseState );
        
    }

}
