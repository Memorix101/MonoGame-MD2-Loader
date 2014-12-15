/********************
 * Engine1637: MD2 Model Animator
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

    public class MD2ModelAnimator {

        private Dictionary<string, int[]> _Animations;

        private string _CurrentAnimation;
        private string _NextAnimation;
        private int _CurrentPointer;
        private int _PauseCountDown;
        private int _TimeLapsed;
        private int _FPS;

        public MD2ModelAnimator() {

            _Animations = new Dictionary<string, int[]>();

            _CurrentAnimation = null;
            _NextAnimation = null;
            _CurrentPointer = 0;
            _PauseCountDown = 0;
            _TimeLapsed = 0;
            _FPS = 12;

        }

        public Dictionary<string, int[]> Animations {

            get { return _Animations; }

            set { _Animations = value; }

        }

        public int FPS {

            get { return _FPS; }

            set { _FPS = value; }

        }

        public void AddFrames( string name, int start, int finish ) {

            List<int> frames = new List<int>();

            if ( Animations.ContainsKey(name) ) {

                for ( int i = 0; i < Animations[name].Length; i++ ) {

                    frames.Add(Animations[name][i]);

                }

            } else {

                Animations.Add(name, new int[0]);

            }

            if ( start < finish ) {

                for ( int i = start; i <= finish; i++ ) {

                    frames.Add(i);

                }

            } else {

                for ( int i = start; i >= finish; i-- ) {

                    frames.Add(i);

                }

            }

            Animations[name] = frames.ToArray();

        }

        public void AddPause( string name, int ms ) {

            if ( Animations.ContainsKey(name) ) {

                List<int> frames = new List<int>();

                for ( int i = 0; i < Animations[name].Length; i++ ) {

                    frames.Add(Animations[name][i]);

                }

                frames.Add(ms * -1);

                Animations[name] = frames.ToArray();

            }

        }

        public void ClearFrames( string name ) {

            if ( Animations.ContainsKey(name) ) {

                Animations[name] = new int[0];

            }

        }

        public string CurrentAnimation {

            get { return _CurrentAnimation; }

            set {

                if ( _CurrentAnimation != value ) {

                    if ( Animations.ContainsKey(value) ) {

                        if ( _CurrentAnimation == null ) {
                            
                            _CurrentAnimation = value;
                        
                        } else {

                            _NextAnimation = value;

                        }

                    } else {

                        _NextAnimation = null;

                    }

                }

            }

        }

        public void ForceAnimation( string animation ) {

            if ( animation != _CurrentAnimation ) {
                if ( Animations.ContainsKey(animation) ) {
                    _CurrentAnimation = animation;
                    _NextAnimation = null;
                    _CurrentPointer = 0;
                }
            }

        } 

        public int GetFrame( GameTime gameTime ) {

            int frame = 1;

            if ( _CurrentAnimation != null ) {

                if ( Animations.ContainsKey(_CurrentAnimation) ) {

                    if ( Animations[_CurrentAnimation].Length > 0 ) {

                        if ( _PauseCountDown > 0 ) {

                            _PauseCountDown -= gameTime.ElapsedGameTime.Milliseconds;

                            frame = Animations[_CurrentAnimation][_CurrentPointer - 1];

                        } else {

                            _TimeLapsed += gameTime.ElapsedGameTime.Milliseconds;

                            if ( _TimeLapsed > (int)(1000 / FPS) ) {

                                _TimeLapsed = 0;

                                _CurrentPointer++;

                            }

                            if ( _CurrentPointer > Animations[_CurrentAnimation].Length - 1 ) {

                                if ( _NextAnimation != null ) {

                                    _CurrentAnimation = _NextAnimation;
                                    _NextAnimation = null;

                                }

                                _CurrentPointer = 0;

                            }


                            frame = Animations[_CurrentAnimation][_CurrentPointer];

                            if ( frame < 0 ) {

                                _PauseCountDown = frame * -1;
                                _TimeLapsed = (int)(1000 / FPS) + 1;

                                frame = Animations[_CurrentAnimation][_CurrentPointer - 1];

                            }

                        }

                    }

                }

            }

            if ( frame < 1 ) { frame = 1; }

            return frame;

        }

    }

}
