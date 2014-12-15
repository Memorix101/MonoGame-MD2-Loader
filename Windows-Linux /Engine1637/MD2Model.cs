/********************
 * MD2 Model Animator
 * 
 * Version: 1.5
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

using Engine1637;
using Engine1637.Interfaces;

namespace Engine1637 {

    public class MD2Model : IAnimatedModel {

        private const int _MD2VALIDID = ('I' + ('D' << 8) + ('P' << 16) + ('2' << 24));

        #region Stored Model Structures

        /***
         * These structures store the 'raw' MD2 model data, prior to the 
         * conversion needed to render the model in an XNA space.
         ***/

        private struct MD2_MD2FileHeader {

            // File identification information.
            public int ID;
            public int Version;

            // Texture dimensions.
            public int SkinWidth;
            public int SkinHeight;
            
            // How many bytes in each frame.
            public int FrameSize;

            // How many of each type of object.
            public int CountVertices; // How many vertices per frame. (Other counts are total for model.)
            public int CountTriangles;
            public int CountFrames;
            public int CountSkins;
            public int CountTextureMaps;
            public int CountOGLCommands;

            // Offsets to where data starts in the model file.
            public int OffsetTriangles;
            public int OffsetFrames;
            public int OffsetSkins;
            public int OffsetTextureMaps;
            public int OffsetOGLCommands;
            public int OffsetEOF;

        }

        private struct MD2Skin {
            public char[] TexturePath; // Path to texture image. Not used here yet, see release notes.
        }

        private struct MD2TextureCoordinate {
            public short X; // Compressed X ('U') texture coordinate.
            public short Y; // Compressed Y ('V') texture coordinate.
        }

        private struct MD2Triangle {
            public ushort[] Vertex; // Index into the model's vertex arrays.
            public ushort[] TextureCoordinate; // Index into the model's texture coordinates array.
        }

        private struct MD2Vertex {
            public byte[] Coordinate; // Compressed vector coordinates.
            public byte LightingNormal; // Index into Quake II Lighting Normals table. Not used here yet, see release notes.
        }

        private struct MD2Frame {
            public char[] Name; // Name of frame, useful for identifying animation sets.
            public float[] Scale; // Scale data for decompression of vertices data.
            public float[] Translation; // Translation data for decompression of vertices data.
            public MD2Vertex[] Vertex; // Compressed vertex data for frame.
            public Vector3[] DecompressedVertices;
        }

        #endregion

        private MD2_MD2FileHeader _MD2FileHeader;
        private MD2Skin[] _MD2Skins;
        private MD2TextureCoordinate[] _MD2TextureMaps;
        private MD2Triangle[] _MD2Triangles;
        private MD2Frame[] _MD2Frames;

        private struct TranslatedFrame {
            public string Name;
            public VertexPositionTexture[] VertexBuffer;
        }

        private TranslatedFrame[] _Frames;

        private ICamera _Camera;

        private BasicEffect _RenderEffect;

        private MD2ModelAnimator _Animator;

        private Vector3 _Position;

        private float _Scale;
        private float _Rotation;
        private float _RotationOffset;
        
        private Matrix _WorldMatrix;
        private Matrix _ScaleMatrix;
        private Matrix _RotationMatrix;

        private Dictionary<string, Texture2D> _Skins;

        private string _CurrentSkin;

        public MD2Model( ICamera camera ) {

            Camera = camera;
            Animator = new MD2ModelAnimator();

            RenderEffect = new BasicEffect(Camera.Device);

            Position = new Vector3(0, 0, 0);

            Scale = 100;
            RotationOffset =  0;
            Rotation = 0;

            Skins = new Dictionary<string, Texture2D>();

        }

        public MD2Model( MD2Model model ) {

            _MD2FileHeader = model._MD2FileHeader;
            _MD2Skins = model._MD2Skins;
            _MD2TextureMaps = model._MD2TextureMaps;
            _MD2Triangles = model._MD2Triangles;
            _MD2Frames = model._MD2Frames;
            _Frames = model._Frames;
            _Camera = model._Camera;
            
           // _RenderEffect = (BasicEffect)model._RenderEffect.Clone(_Camera.Device);
            _RenderEffect = new BasicEffect(_Camera.Device); // (model._RenderEffect.Clone(_Camera.Device));

            _Animator = new MD2ModelAnimator();
            _Animator.Animations = new Dictionary<string,int[]>(model._Animator.Animations);
            _Animator.FPS = model._Animator.FPS;
            _Animator.CurrentAnimation = model._Animator.CurrentAnimation;
            
            _Position = model._Position;
            _Scale = model._Scale;
            RotationOffset = model._RotationOffset;
            Rotation = model._Rotation;
            _WorldMatrix = model._WorldMatrix;
            _ScaleMatrix = model._ScaleMatrix;
            _RotationMatrix = model._RotationMatrix;
            _Skins = new Dictionary<string,Texture2D>(model._Skins);

            if ( model._CurrentSkin != null ) {
                CurrentSkin = model._CurrentSkin;
            } else {
                _CurrentSkin = null;
            }

        }

        public bool LoadFromFile( string path ) {

            System.IO.FileStream stream = null;
            System.IO.BinaryReader reader = null;

            try {

                // Open the specified file as a stream.
                stream = new System.IO.FileStream(path, FileMode.Open, FileAccess.Read);

                // Pipe the stream in to a binary reader so we can work at the byte-by-byte level.
                reader = new System.IO.BinaryReader(stream);

                /*** LOAD FILE HEADER ***/

                this._MD2FileHeader = new MD2_MD2FileHeader();

                // If the file is a valid MD2 model the first 4B of data will be an ID.
                // The ID is a standard integer which should match _MD2VALIDID.

                this._MD2FileHeader.ID = reader.ReadInt32();

                if ( this._MD2FileHeader.ID == _MD2VALIDID ) {

                    /*** LOAD HEADER DATA ***/

                    // After the magic number the next 64B of data contains header information.
                    // For more information see the Struct MD2Header definition.

                    this._MD2FileHeader.Version = reader.ReadInt32();

                    this._MD2FileHeader.SkinWidth = reader.ReadInt32();
                    this._MD2FileHeader.SkinHeight = reader.ReadInt32();

                    this._MD2FileHeader.FrameSize = reader.ReadInt32();

                    this._MD2FileHeader.CountSkins = reader.ReadInt32();
                    this._MD2FileHeader.CountVertices = reader.ReadInt32();
                    this._MD2FileHeader.CountTextureMaps = reader.ReadInt32();
                    this._MD2FileHeader.CountTriangles = reader.ReadInt32();
                    this._MD2FileHeader.CountOGLCommands = reader.ReadInt32();
                    this._MD2FileHeader.CountFrames = reader.ReadInt32();

                    this._MD2FileHeader.OffsetSkins = reader.ReadInt32();
                    this._MD2FileHeader.OffsetTextureMaps = reader.ReadInt32();
                    this._MD2FileHeader.OffsetTriangles = reader.ReadInt32();
                    this._MD2FileHeader.OffsetFrames = reader.ReadInt32();
                    this._MD2FileHeader.OffsetOGLCommands = reader.ReadInt32();
                    this._MD2FileHeader.OffsetEOF = reader.ReadInt32();

                    /*** LOAD SKIN DEFINITIONS ***/

                    // Initialise data array.
                    this._MD2Skins = new MD2Skin[this._MD2FileHeader.CountSkins];

                    // Jump to the position in file where data starts.
                    // This is defined in the header data.
                    reader.BaseStream.Seek(this._MD2FileHeader.OffsetSkins, SeekOrigin.Begin);

                    // Loop for each entry.
                    for ( int i = 0; i < this._MD2FileHeader.CountSkins; i++ ) {

                        this._MD2Skins[i].TexturePath = reader.ReadChars(64);

                    }

                    /*** LOAD TEXTURE MAPS ***/

                    this._MD2TextureMaps = new MD2TextureCoordinate[this._MD2FileHeader.CountTextureMaps];

                    reader.BaseStream.Seek(this._MD2FileHeader.OffsetTextureMaps, SeekOrigin.Begin);

                    for ( int i = 0; i < this._MD2FileHeader.CountTextureMaps; i++ ) {

                        this._MD2TextureMaps[i].X = reader.ReadInt16();
                        this._MD2TextureMaps[i].Y = reader.ReadInt16();

                    }

                    /*** LOAD TRIANGLE DATA ***/

                    this._MD2Triangles = new MD2Triangle[this._MD2FileHeader.CountTriangles];

                    reader.BaseStream.Seek(this._MD2FileHeader.OffsetTriangles, SeekOrigin.Begin);

                    for ( int i = 0; i < this._MD2FileHeader.CountTriangles; i++ ) {

                        this._MD2Triangles[i].Vertex = new ushort[3];
                        this._MD2Triangles[i].Vertex[0] = reader.ReadUInt16();
                        this._MD2Triangles[i].Vertex[1] = reader.ReadUInt16();
                        this._MD2Triangles[i].Vertex[2] = reader.ReadUInt16();

                        this._MD2Triangles[i].TextureCoordinate = new ushort[3];
                        this._MD2Triangles[i].TextureCoordinate[0] = reader.ReadUInt16();
                        this._MD2Triangles[i].TextureCoordinate[1] = reader.ReadUInt16();
                        this._MD2Triangles[i].TextureCoordinate[2] = reader.ReadUInt16();

                    }

                    /*** LOAD FRAMES ***/

                    this._MD2Frames = new MD2Frame[this._MD2FileHeader.CountFrames];

                    for ( int i = 0; i < this._MD2FileHeader.CountFrames; i++ ) {

                        // Unlike other data, frame data is of a defined size not fixed.
                        // As such the jump to the start of the data must happen for each frame.
                        reader.BaseStream.Seek((this._MD2FileHeader.OffsetFrames + (i * this._MD2FileHeader.FrameSize)), SeekOrigin.Begin);

                        this._MD2Frames[i].Scale = new float[3];
                        this._MD2Frames[i].Translation = new float[3];

                        // The binary reader does not read floats; 'scale' and 'translate' are foat[]..
                        // We need to read the data to a byte[] and convert it.
                        byte[] buffer;

                        buffer = reader.ReadBytes(4);
                        this._MD2Frames[i].Scale[0] = System.BitConverter.ToSingle(buffer, 0);
                        buffer = reader.ReadBytes(4);
                        this._MD2Frames[i].Scale[1] = System.BitConverter.ToSingle(buffer, 0);
                        buffer = reader.ReadBytes(4);
                        this._MD2Frames[i].Scale[2] = System.BitConverter.ToSingle(buffer, 0);

                        buffer = reader.ReadBytes(4);
                        this._MD2Frames[i].Translation[0] = System.BitConverter.ToSingle(buffer, 0);
                        buffer = reader.ReadBytes(4);
                        this._MD2Frames[i].Translation[1] = System.BitConverter.ToSingle(buffer, 0);
                        buffer = reader.ReadBytes(4);
                        this._MD2Frames[i].Translation[2] = System.BitConverter.ToSingle(buffer, 0);

                        this._MD2Frames[i].Name = reader.ReadChars(16);

                        /*** LOAD VERTEX DATA (FOR FRAME) ***/

                        this._MD2Frames[i].Vertex = new MD2Vertex[this._MD2FileHeader.CountVertices];
                        this._MD2Frames[i].DecompressedVertices = new Vector3[this._MD2FileHeader.CountVertices];

                        for ( int j = 0; j < this._MD2FileHeader.CountVertices; j++ ) {

                            this._MD2Frames[i].Vertex[j].Coordinate = reader.ReadBytes(3);
                            this._MD2Frames[i].Vertex[j].LightingNormal = reader.ReadByte();

                        }

                    }

                    /*** DECOMPRESS AND TRANSLATE FRAMES ***/

                    this._Frames = new TranslatedFrame[this._MD2FileHeader.CountFrames];

                    for ( int i = 0; i < this._MD2FileHeader.CountFrames; i++ ) {

                        this._Frames[i].Name = new string(this._MD2Frames[i].Name);

                        this._Frames[i].VertexBuffer = new VertexPositionTexture[this._MD2FileHeader.CountTriangles * 3];

                        int j = 0; // position in vertex buffer

                        for ( int k = 0; k < this._MD2FileHeader.CountTriangles; k++ ) {

                            // We need to invert the vertices of the triangle,
                            // switch the Y and Z coordinates of the vertices,
                            // decompress the vertices coordinates (coord * scale + translation) and
                            // convert the texture coordinates (x / width, y / height).

                            this._Frames[i].VertexBuffer[j].Position.X = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[2]].Coordinate[0] * this._MD2Frames[i].Scale[0] + this._MD2Frames[i].Translation[0];
                            this._Frames[i].VertexBuffer[j].Position.Y = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[2]].Coordinate[2] * this._MD2Frames[i].Scale[2] + this._MD2Frames[i].Translation[2];
                            this._Frames[i].VertexBuffer[j].Position.Z = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[2]].Coordinate[1] * this._MD2Frames[i].Scale[1] + this._MD2Frames[i].Translation[1];

                            this._Frames[i].VertexBuffer[j].TextureCoordinate.X = (float)this._MD2TextureMaps[this._MD2Triangles[k].TextureCoordinate[2]].X / (float)this._MD2FileHeader.SkinWidth;
                            this._Frames[i].VertexBuffer[j].TextureCoordinate.Y = (float)this._MD2TextureMaps[this._MD2Triangles[k].TextureCoordinate[2]].Y / (float)this._MD2FileHeader.SkinHeight;

                            j += 1;

                            this._Frames[i].VertexBuffer[j].Position.X = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[1]].Coordinate[0] * this._MD2Frames[i].Scale[0] + this._MD2Frames[i].Translation[0];
                            this._Frames[i].VertexBuffer[j].Position.Y = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[1]].Coordinate[2] * this._MD2Frames[i].Scale[2] + this._MD2Frames[i].Translation[2];
                            this._Frames[i].VertexBuffer[j].Position.Z = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[1]].Coordinate[1] * this._MD2Frames[i].Scale[1] + this._MD2Frames[i].Translation[1];

                            this._Frames[i].VertexBuffer[j].TextureCoordinate.X = (float)this._MD2TextureMaps[this._MD2Triangles[k].TextureCoordinate[1]].X / (float)this._MD2FileHeader.SkinWidth;
                            this._Frames[i].VertexBuffer[j].TextureCoordinate.Y = (float)this._MD2TextureMaps[this._MD2Triangles[k].TextureCoordinate[1]].Y / (float)this._MD2FileHeader.SkinHeight;

                            j += 1;

                            this._Frames[i].VertexBuffer[j].Position.X = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[0]].Coordinate[0] * this._MD2Frames[i].Scale[0] + this._MD2Frames[i].Translation[0];
                            this._Frames[i].VertexBuffer[j].Position.Y = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[0]].Coordinate[2] * this._MD2Frames[i].Scale[2] + this._MD2Frames[i].Translation[2];
                            this._Frames[i].VertexBuffer[j].Position.Z = this._MD2Frames[i].Vertex[this._MD2Triangles[k].Vertex[0]].Coordinate[1] * this._MD2Frames[i].Scale[1] + this._MD2Frames[i].Translation[1];

                            this._Frames[i].VertexBuffer[j].TextureCoordinate.X = (float)this._MD2TextureMaps[this._MD2Triangles[k].TextureCoordinate[0]].X / (float)this._MD2FileHeader.SkinWidth;
                            this._Frames[i].VertexBuffer[j].TextureCoordinate.Y = (float)this._MD2TextureMaps[this._MD2Triangles[k].TextureCoordinate[0]].Y / (float)this._MD2FileHeader.SkinHeight;

                            j += 1;

                        }

                    }

                    string animationName = "";
                    string frameName = "";
                    int currentIndex = 1;

                    int start = 1;
                    int finish = 1;

                    for ( int i = 0; i < _Frames.Length; i++ ) {

                        frameName = _Frames[i].Name;
                        if ( frameName.IndexOf("\0") > 0 ) { frameName = frameName.Substring(0, frameName.IndexOf("\0")); }

                        if ( frameName.EndsWith("0") && !currentIndex.ToString().EndsWith("0") ) { frameName = frameName.Substring(0, frameName.Length - 1); }

                        frameName = frameName.Substring(0, frameName.Length - currentIndex.ToString().Length);

                        if ( frameName.EndsWith("0") ) { frameName = frameName.Substring(0, frameName.Length - 1); }

                        if ( frameName != animationName ) {

                            if ( animationName != "" && frameName != "" ) {
                                
                                finish = i;
                                this._Animator.AddFrames(animationName, start, finish);

                            }

                            currentIndex = 1;

                            frameName = _Frames[i].Name;
                            if ( frameName.IndexOf("\0") > 0 ) { frameName = frameName.Substring(0, frameName.IndexOf("\0")); }

                            if ( frameName.EndsWith("0") && !currentIndex.ToString().EndsWith("0") ) { frameName = frameName.Substring(0, frameName.Length - 1); }

                            frameName = frameName.Substring(0, frameName.Length - currentIndex.ToString().Length);

                            if ( frameName.EndsWith("0") ) { frameName = frameName.Substring(0, frameName.Length - 1); }

                            animationName = frameName;
                            start = i + 1;
                            finish = i + 1;

                            currentIndex += 1;                            

                        } else {

                            currentIndex += 1;

                        }

                        if ( i == _Frames.Length - 1 && animationName != "" && frameName != "" ) {

                            finish = i + 1;
                            this._Animator.AddFrames(animationName, start, finish);

                        }

                    }

                    return true;

                }

            } catch ( Exception ex ) {

                Console.Out.WriteLine("Error while loading MD2 model from definition file ( " + path + " ).");
                Console.Out.WriteLine(ex.Message.ToString());

            } finally {

                if ( reader != null ) { reader.Close(); }
                if ( stream != null ) {
                    stream.Close();
                    stream.Dispose();
                }

            }

            return false;

        }

        public ICamera Camera {

            get { return _Camera; }

            set { _Camera = value; }

        }

        public BasicEffect RenderEffect {

            get { return _RenderEffect; }

            set { _RenderEffect = value; }

        }

        public MD2ModelAnimator Animator {

            get { return _Animator; }

            set { _Animator = (MD2ModelAnimator)value; }

        }

        public Vector3 Position {

            get { return _Position; }

            set {

                _Position = value;
                _WorldMatrix = _ScaleMatrix * _RotationMatrix * Matrix.CreateTranslation(_Position);

            }

        }       
        public float Scale {

            get { return this._Scale; }

            set {

                this._Scale = value;
                this._ScaleMatrix = Matrix.CreateScale(Scale / 100f);
                _WorldMatrix = _ScaleMatrix * _RotationMatrix * Matrix.CreateTranslation(_Position);

            }

        }

        public float RotationOffset {

            get { return this._RotationOffset; }

            set {

                _RotationOffset = value;
                Rotation = 0;

            }

        }

        public float Rotation {

            get { return this._Rotation; }

            set {

                _Rotation = value;

                if ( _Rotation < 0 ) { _Rotation = 360; }
                if ( _Rotation > 360 ) { _Rotation = 0; }

                _RotationMatrix = Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(_Rotation)) * Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(-_RotationOffset));
                _WorldMatrix = _ScaleMatrix * _RotationMatrix * Matrix.CreateTranslation(_Position);

            }

        }

        public Dictionary<string, Texture2D> Skins {

            get { return _Skins; }

            set { _Skins = value; }

        }

        public string CurrentSkin {

            get { return _CurrentSkin; }

            set {

                _CurrentSkin = value;

                if ( Skins.ContainsKey(_CurrentSkin) ) {

                    RenderEffect.Texture = Skins[_CurrentSkin];

                } else {

                    _CurrentSkin = "none";
                    RenderEffect.Texture = null;

                }
            
            }

        }

        public void DefineSkin( string name, string file ) {

            if ( Skins.ContainsKey(name) ) {

                using (FileStream stream = File.OpenRead(file))
                {
                    Skins[name] = Texture2D.FromStream(Camera.Device, stream);
                }

            } else {

                using (FileStream stream = File.OpenRead(file))
                {

                    Skins.Add(name, Texture2D.FromStream(Camera.Device, stream));
                }

            }

        }
                
        public void Render( int frame ) {

            //RenderEffect.LightingEnabled = true;
            //RenderEffect.DirectionalLight0.Enabled = true;
            //RenderEffect.DirectionalLight0.Direction = new Vector3(1, 10, 1);
            //RenderEffect.DirectionalLight0.DiffuseColor = new Vector3(0.6f, 0.6f, 0.6f);
            //RenderEffect.AmbientLightColor = new Vector3(0.65f, 0.65f, 0.65f);


            

            RenderEffect.World = _WorldMatrix;
            RenderEffect.View = Camera.View;
            RenderEffect.Projection = Camera.Perspective;
            RenderEffect.TextureEnabled = true;

            foreach ( EffectPass pass in RenderEffect.CurrentTechnique.Passes ) {
                pass.Apply();

             //   Camera.Device.VertexDeclaration = new VertexDeclaration(Camera.Device, VertexPositionTexture.VertexElements);
                Camera.Device.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleList, this._Frames[frame].VertexBuffer, 0, this._MD2FileHeader.CountTriangles);

            }

           // RenderEffect.End();

        }

        public void Animate( GameTime gameTime ) {

            // MD2 Frames typically start at 1, where as C# arrays are zero base
            // hence the "this._AnimationController.CurrentFrame - 1" 
            Render(Animator.GetFrame(gameTime) - 1);

        }

        public void DefineAnimation( string name, int start, int finish ) {

            Animator.AddFrames(name, start, finish);

        }

        public string CurrentAnimation {

            get { return Animator.CurrentAnimation; }

            set { Animator.CurrentAnimation = value; }

        }

        public void DumpAnimationListToFile( string path ) {

            using ( System.IO.StreamWriter stream = new System.IO.StreamWriter(path, false, System.Text.Encoding.ASCII) ) {

                try {

                    foreach ( KeyValuePair<string, int[]>  kvp in _Animator.Animations ) {

                        stream.WriteLine(kvp.Key);

                    }

                } catch (Exception ex) {

                    Console.Out.WriteLine("Error while writting animation list to file.");
                    Console.Out.WriteLine(ex.Message.ToString());

                }

            } 

        }

        public void Move( Vector3 axis, float distance ) {

            if ( axis.X > 0 && axis.Y == 0 && axis.Z == 0 ) {

            } else if ( axis.X > 0 && axis.Y == 0 && axis.Z == 0 ) {

                Position += (Matrix.CreateTranslation(distance, 0, 0) * (Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(_Rotation)))).Translation;

            } else if ( axis.X < 0 && axis.Y == 0 && axis.Z == 0 ) {

                Position -= (Matrix.CreateTranslation(distance, 0, 0) * (Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(_Rotation)))).Translation;

            } else if ( axis.Y > 0 && axis.X == 0 && axis.Z == 0 ) {

                Position += (Matrix.CreateTranslation(0, distance, 0) * (Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(_Rotation)))).Translation;

            } else if ( axis.Y < 0 && axis.X == 0 && axis.Z == 0 ) {

                Position -= (Matrix.CreateTranslation(0, distance, 0) * (Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(_Rotation)))).Translation;

            } else if ( axis.Z > 0 && axis.Y == 0 && axis.X == 0 ) {

                Position += (Matrix.CreateTranslation(0, 0, distance) * (Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(_Rotation))) ).Translation;

            } else if ( axis.Z < 0 && axis.Y == 0 && axis.X == 0 ) {

                Position -= (Matrix.CreateTranslation(0, 0, distance) * (Matrix.CreateFromAxisAngle(Vector3.Up, MathHelper.ToRadians(_Rotation)))).Translation;

            }

        }

        public void ForceAnimation( string animation ) {

            _Animator.ForceAnimation(animation);

        }

    }


}