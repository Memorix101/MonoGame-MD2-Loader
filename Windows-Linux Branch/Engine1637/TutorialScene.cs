/********************
 * Engine1637: Tutorial Scene
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

    class TutorialScene : StandardScene {

        public override void Draw( GameTime gameTime ) {

            base.Draw(gameTime);

            VertexPositionTexture[] vertexBuffer = new VertexPositionTexture[600];

            List<VertexPositionTexture> vb = new List<VertexPositionTexture>();

            int WIDTH = 22;
            int HEIGHT = 22;

            for ( int x = 0; x < WIDTH; x++ ) {
                for ( int y = 0; y < HEIGHT; y++ ) {
                        vb.Add(new VertexPositionTexture(new Vector3(x + 1, 0, y + 1), new Vector2(0, 0)));
                        vb.Add(new VertexPositionTexture(new Vector3(x + 1, 0, y), new Vector2(0, 1)));
                        vb.Add(new VertexPositionTexture(new Vector3(x, 0, y), new Vector2(1, 1)));

                        vb.Add(new VertexPositionTexture(new Vector3(x, 0, y), new Vector2(1, 1)));
                        vb.Add(new VertexPositionTexture(new Vector3(x, 0, y + 1), new Vector2(1, 0)));
                        vb.Add(new VertexPositionTexture(new Vector3(x + 1, 0, y + 1), new Vector2(0, 0)));
                }
            }

            vertexBuffer = vb.ToArray();

          // Camera.Device.RenderState.CullMode = CullMode.None;

            RasterizerState rasterizerState1 = new RasterizerState();
            rasterizerState1.CullMode = CullMode.None;

            BasicEffect effect = new BasicEffect(Camera.Device);

            effect.World = Matrix.CreateScale(5) * Matrix.CreateTranslation(new Vector3(-55,0,-55));
            effect.View = Camera.View;
            effect.Projection = Camera.Perspective;
            effect.TextureEnabled = true;

            using (FileStream stream = File.OpenRead("Content/Textures/basictile.jpg"))
            {
                effect.Texture = Texture2D.FromStream(Camera.Device, stream);
            }



            foreach ( EffectPass pass in effect.CurrentTechnique.Passes ) {
                pass.Apply();

              //  Camera.Device.VertexDeclaration = new VertexDeclaration(Camera.Device, VertexPositionTexture.VertexElements);
                Camera.Device.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, vertexBuffer, 0, vertexBuffer.Length / 3);

            }

           // effect.End();

        }
        

    }

}
