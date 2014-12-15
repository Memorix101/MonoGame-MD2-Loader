/********************
 * Engine1637: Standard Actor
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
    
    public class StandardActor : IActor {

        protected IAnimatedModel _Model;

        protected Dictionary<string, string> _PrimaryAnimations = new Dictionary<string, string>();
        protected Dictionary<string, List<string>> _OptionalAnimations = new Dictionary<string, List<string>>();

        protected bool _Active = false;
        protected bool _Running = false;

        protected float _MovementSpeed = 0.25f;

        public virtual IAnimatedModel Model {

            get { return _Model; }

            set { _Model = value; }

        }

        public Vector3 Position {

            get { return _Model.Position; }

            set { _Model.Position = value; }

        }

        public float Rotation {

            get { return _Model.Rotation; }

            set { _Model.Rotation = value; }

        }

        public float MovementSpeed {

            get { return _MovementSpeed; }

            set { _MovementSpeed = value; }

        }

        public void SetPrimaryAnimation( string action, string animation ) {

            if ( _PrimaryAnimations.ContainsKey(action) ) {

                _PrimaryAnimations[action] = animation;

            } else {

                _PrimaryAnimations.Add(action, animation);

            }

        }

        public void AddOptionalAnimation( string action, string animation ) {

            if ( _OptionalAnimations.ContainsKey(action) ) {

                if ( !_OptionalAnimations[action].Contains(animation) ) {

                    _OptionalAnimations[action].Add(animation);

                }

            } else {

                _OptionalAnimations.Add(action, new List<string>());
                _OptionalAnimations[action].Add(animation);

            }

        }

        public void RemoveOptionalAnimation( string action, string animation ) {

            if ( _OptionalAnimations.ContainsKey(action) ) {

                if ( _OptionalAnimations[action].Contains(animation) ) {

                    _OptionalAnimations[action].Remove(animation);

                }

            }

        }

        public void Run() {

            _Active = true;
            
            string primaryAnimation = "none";
            int optionalAnimations = 0;

            if ( _PrimaryAnimations.ContainsKey("Run") ) { primaryAnimation = _PrimaryAnimations["Run"]; }

            if ( _OptionalAnimations.ContainsKey("Run") ) { optionalAnimations = _OptionalAnimations["Run"].Count; }

            if ( optionalAnimations > 0 ) {

                Random rnd = new Random();
                int check = rnd.Next(0, 50000 + (optionalAnimations * 100));

                if ( check < 50000 ) {

                    primaryAnimation = primaryAnimation;

                } else {

                    int ani = (check - 50000) / 100;

                    primaryAnimation = _OptionalAnimations["Run"][ani];

                }

            }

            if ( _Running ) {
                Model.CurrentAnimation = primaryAnimation;
            } else {
                Model.ForceAnimation(primaryAnimation);
                _Running = true;
            }
            
            Model.Move(new Vector3(0, 0, 1), _MovementSpeed);

        }

        public void StopRunning() {

            if ( _Running ) {

                Model.ForceAnimation("stand");
                _Running = false;
                _Active = false;

            }

        }

        public void Idle() {

            if ( _Active ) { return; }

            string primaryAnimation = "none";
            int optionalAnimations = 0;

            if ( _PrimaryAnimations.ContainsKey("Idle") ) { primaryAnimation = _PrimaryAnimations["Idle"]; }

            if ( _OptionalAnimations.ContainsKey("Idle") ) { optionalAnimations = _OptionalAnimations["Idle"].Count; }

            if ( optionalAnimations > 0 ) {

                Random rnd = new Random();
                int check = rnd.Next(0, 50000 + (optionalAnimations * 100));

                if ( check < 50000 ) {

                    primaryAnimation = primaryAnimation;

                } else {

                    int ani = (check - 50000) / 100;

                    primaryAnimation = _OptionalAnimations["Idle"][ani];

                }

            }

            Model.CurrentAnimation = primaryAnimation;

        }

        public virtual void HandleInput( KeyboardState keyboardState, MouseState mouseState ) { }

    }

}
