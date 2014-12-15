/********************
 * Engine1637: Standard Scene
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

    public class StandardScene {

        protected ICamera _Camera;

        protected List<IActor> _Actors = new List<IActor>();

        public virtual ICamera Camera {

            get { return _Camera; }

            set { _Camera = value; }
        
        }

        public virtual List<IActor> Actors {

            get { return _Actors; }

            set { _Actors = value; } 
        
        }

        public virtual void Draw( GameTime gameTime ) {

            foreach ( IActor actor in Actors ) {

                actor.Model.Animate(gameTime);

            }

        }

    }

}
