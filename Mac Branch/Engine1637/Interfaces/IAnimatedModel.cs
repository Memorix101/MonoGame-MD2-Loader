/********************
 * Engine1637: Animated Model Interface
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

    public interface IAnimatedModel : IModel {

        string CurrentAnimation { get; set; }
        
        void Animate( GameTime gameTime );

        void DumpAnimationListToFile( string path );

        void ForceAnimation( string animation );
        

    }

}
