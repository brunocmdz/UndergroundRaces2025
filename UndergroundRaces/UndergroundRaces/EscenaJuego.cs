using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Content;
using System;

namespace UndergroundRaces
{
    public class EscenaJuego : IEscena
    {
        private Texture2D _fondoAtlas;
        private List<Rectangle> _framesFondo = new();
        private int _frameActual = 0;
        private int _totalFrames = 14;
        private float _timerFrame = 0f;
        private float _tiempoPorFrame = 0.02f;
        private bool _avanzando = false;

        private Texture2D _corsaAtlas;
        private List<Rectangle> _framesCorsa = new();
        private int _frameCorsaActual = 0;
        private float _timerCorsa = 0f;
        private float _tiempoPorFrameCorsa = 0.08f;
        private bool _usandoAtlas = true;
        private Vector2 _corsaPosition;
        private SpriteEffects _spriteEffect = SpriteEffects.None;

        private Texture2D _corsaDoblandoAtlas;
        private List<Rectangle> _framesDoblando = new();
        private int _frameDoblandoActual = 0;
        private float _timerDoblando = 0f;
        private float _tiempoPorFrameDoblando = 0.1f;

        private SoundEffect _motorSound;
        private SoundEffectInstance _motorInstance;
        private float _motorVolume = 0f;
        private const float _volumenMaximo = 0.5f;
        private const float _velocidadCambioVolumen = 0.01f;

        public Action OnPausaSolicitada;

        private List<Texture2D> _carteles = new();
        private int _indiceCartelIzq = 0;
        private int _indiceCartelDer = 0;
        private Texture2D _cartelIzqActual;
        private Texture2D _cartelDerActual;
        private Vector2 _posCartelIzq;
        private Vector2 _posCartelDer;
        private float _velocidadCartel = 3.5f;
        private float _tiempoCartel = 3f;
        private float _timerCartelIzq = 0f;
        private float _timerCartelDer = 0f;


        private GraphicsDevice _graphicsDevice;
        private ContentManager _content;

        public void LoadContent(Game game)
        {
            _graphicsDevice = game.GraphicsDevice;
            _content = game.Content;

            _fondoAtlas = _content.Load<Texture2D>("images/backgroundPLANTILLA2");
            GenerarFramesFondo(_fondoAtlas, 1024, 576);

            _corsaAtlas = _content.Load<Texture2D>("images/corsaPLANTILLA");
            GenerarFramesCorsa(_corsaAtlas, _corsaAtlas.Width, _corsaAtlas.Height / 2);

            _corsaDoblandoAtlas = _content.Load<Texture2D>("images/corsaDoblandoPLANTILLA");
            GenerarFramesDoblando(_corsaDoblandoAtlas, _corsaDoblandoAtlas.Width, _corsaDoblandoAtlas.Height / 2);

            _motorSound = _content.Load<SoundEffect>("audio/motor-corsa");
            _motorInstance = _motorSound.CreateInstance();
            _motorInstance.IsLooped = true;
            _motorInstance.Volume = 0f;
            _motorInstance.Play();

            for (int i = 1; i <= 8; i++)
            {
                _carteles.Add(_content.Load<Texture2D>($"images/cartel{i}"));
            }

            _cartelIzqActual = _carteles[_indiceCartelIzq];
            _cartelDerActual = _carteles[_indiceCartelDer];
            _posCartelIzq = new Vector2(30, 180); 
            _posCartelDer = new Vector2(880, 180);


            int screenWidth = _graphicsDevice.Viewport.Width;
            _corsaPosition = new Vector2(screenWidth / 2f, 500);
        }

        public void Update(GameTime gameTime)
        {
            var state = Keyboard.GetState();
            float velocidad = 3.5f;

            int screenWidth = _graphicsDevice.Viewport.Width;
            float corsaAncho = _corsaAtlas.Width * 3f;
            float corsaMitad = corsaAncho / 2f;

            float rutaMargenIzquierdo = 200f;
            float rutaMargenDerecho = screenWidth - 200f;
            float limiteIzquierdo = rutaMargenIzquierdo - corsaMitad;
            float limiteDerecho = rutaMargenDerecho + corsaMitad;

            if (state.IsKeyDown(Keys.Escape))
                OnPausaSolicitada?.Invoke();

            if (state.IsKeyDown(Keys.D))
            {
                _usandoAtlas = false;
                _spriteEffect = SpriteEffects.None;
                if (_corsaPosition.X + velocidad < limiteDerecho)
                    _corsaPosition.X += velocidad;

                _timerDoblando += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timerDoblando >= _tiempoPorFrameDoblando)
                {
                    _timerDoblando = 0f;
                    _frameDoblandoActual = (_frameDoblandoActual + 1) % _framesDoblando.Count;
                }
            }
            else if (state.IsKeyDown(Keys.A))
            {
                _usandoAtlas = false;
                _spriteEffect = SpriteEffects.FlipHorizontally;
                if (_corsaPosition.X - velocidad > limiteIzquierdo)
                    _corsaPosition.X -= velocidad;

                _timerDoblando += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timerDoblando >= _tiempoPorFrameDoblando)
                {
                    _timerDoblando = 0f;
                    _frameDoblandoActual = (_frameDoblandoActual + 1) % _framesDoblando.Count;
                }
            }
            else
            {
                _usandoAtlas = true;
                _spriteEffect = SpriteEffects.None;
                _frameDoblandoActual = 0;
            }

            _avanzando = state.IsKeyDown(Keys.W);
            if (_avanzando)
            {
                _timerFrame += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timerFrame >= _tiempoPorFrame)
                {
                    _timerFrame = 0f;
                    _frameActual = (_frameActual + 1) % _framesFondo.Count;
                }

                _timerCorsa += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timerCorsa >= _tiempoPorFrameCorsa)
                {
                    _timerCorsa = 0f;
                    _frameCorsaActual = (_frameCorsaActual + 1) % _framesCorsa.Count;
                }

                Vector2 direccionIzq = new Vector2(-1.5f, 1f); 
                Vector2 direccionDer = new Vector2(1.5f, 1f);  

                direccionIzq.Normalize();
                direccionDer.Normalize();

                _posCartelIzq += direccionIzq * _velocidadCartel;
                _posCartelDer += direccionDer * _velocidadCartel;

                _timerCartelIzq += (float)gameTime.ElapsedGameTime.TotalSeconds;
                _timerCartelDer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (_timerCartelIzq >= _tiempoCartel)
                {
                    _indiceCartelIzq = (_indiceCartelIzq + 1) % _carteles.Count;
                    _cartelIzqActual = _carteles[_indiceCartelIzq];
                    _posCartelIzq = new Vector2(30, 200);
                    _timerCartelIzq = 0f;
                }

                if (_timerCartelDer >= _tiempoCartel)
                {
                    _indiceCartelDer = (_indiceCartelDer + 1) % _carteles.Count;
                    _cartelDerActual = _carteles[_indiceCartelDer];
                    _posCartelDer = new Vector2(screenWidth - 130, 200);
                    _timerCartelDer = 0f;
                }
            }
            else
            {
                _frameCorsaActual = 0;
            }

            if (_avanzando)
            {
                _motorVolume += _velocidadCambioVolumen;
                if (_motorVolume > _volumenMaximo)
                    _motorVolume = _volumenMaximo;
            }
            else
            {
                _motorVolume -= _velocidadCambioVolumen;
                if (_motorVolume < 0f)
                    _motorVolume = 0f;
            }
            _motorInstance.Volume = _motorVolume;
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            int screenWidth = _graphicsDevice.Viewport.Width;
            int screenHeight = _graphicsDevice.Viewport.Height;

            Rectangle frameRect = _framesFondo[_frameActual];
            spriteBatch.Draw(_fondoAtlas, new Rectangle(0, 0, screenWidth, screenHeight), frameRect, Color.White);

            if (_usandoAtlas)
            {
                Rectangle corsaRect = _framesCorsa[_frameCorsaActual];
                Vector2 origin = new Vector2(corsaRect.Width / 2f, corsaRect.Height / 2f);
                spriteBatch.Draw(_corsaAtlas, _corsaPosition, corsaRect, Color.White, 0f, origin, 3f, _spriteEffect, 0f);
            }
            else
            {
                Rectangle corsaRect = _framesDoblando[_frameDoblandoActual];
                Vector2 origin = new Vector2(corsaRect.Width / 2f, corsaRect.Height / 2f);
                spriteBatch.Draw(_corsaDoblandoAtlas, _corsaPosition, corsaRect, Color.White, 0f, origin, 3f, _spriteEffect, 0f);
            }

            spriteBatch.Draw(_cartelIzqActual, _posCartelIzq, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);
            spriteBatch.Draw(_cartelDerActual, _posCartelDer, null, Color.White, 0f, Vector2.Zero, 2f, SpriteEffects.None, 0f);


            spriteBatch.End();
        }


        private void GenerarFramesFondo(Texture2D atlas, int anchoFrame, int altoFrame)
        {
            int columnas = atlas.Width / anchoFrame;
            int filas = atlas.Height / altoFrame;

            for (int y = 0; y < filas; y++)
            {
                for (int x = 0; x < columnas; x++)
                {
                    _framesFondo.Add(new Rectangle(x * anchoFrame, y * altoFrame, anchoFrame, altoFrame));
                }
            }

            if (_framesFondo.Count > _totalFrames)
                _framesFondo = _framesFondo.GetRange(0, _totalFrames);
        }

        private void GenerarFramesCorsa(Texture2D atlas, int anchoFrame, int altoFrame)
        {
            int filas = atlas.Height / altoFrame;
            for (int y = 0; y < filas; y++)
            {
                _framesCorsa.Add(new Rectangle(0, y * altoFrame, anchoFrame, altoFrame));
            }
        }

        private void GenerarFramesDoblando(Texture2D atlas, int anchoFrame, int altoFrame)
        {
            int filas = atlas.Height / altoFrame;
            for (int y = 0; y < filas; y++)
            {
                _framesDoblando.Add(new Rectangle(0, y * altoFrame, anchoFrame, altoFrame));
            }
        }
    }
}
