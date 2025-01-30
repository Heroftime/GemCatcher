using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace GemCatcher
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private bool _gameOver;

        private int _score;
        private string _scoreText;

        private SpriteFont _font;

        private Texture2D _bgTexture;

        private Texture2D _paddleTexture;
        private Vector2 _paddlePosition;

        private Texture2D _gemTexture;
        private List<Vector2> _gemPositions;
        private Random _random;

        private double _gemSpawnTimer = 0;
        private const double GemSpawnInterval = 2.0;

        private double _scoreUpdateTime;
        private const double ScoreHighlightDuration = 0.5;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.PreferredBackBufferWidth = 1152;
            _graphics.PreferredBackBufferHeight = 648;
            _graphics.ApplyChanges();
        }

        protected override void Initialize()
        {
            _random = new Random();
            _gameOver = false;
            _score = 0;
            _scoreText = "0000";
            _scoreUpdateTime = -ScoreHighlightDuration;
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>("Font");
            _bgTexture = Content.Load<Texture2D>("GameBg");
            _paddleTexture = Content.Load<Texture2D>("paddleBlu");
            _gemTexture = Content.Load<Texture2D>("element_red_diamond");
            _paddlePosition = new Vector2(_graphics.PreferredBackBufferWidth / 2 - _paddleTexture.Width / 2, _graphics.PreferredBackBufferHeight - 50);
            _gemPositions = new List<Vector2>();
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (_gameOver)
            {
                if (Keyboard.GetState().IsKeyDown(Keys.Enter))
                {
                    Initialize();
                }
                else
                {
                    return;
                }
            }

            var keyboardState = Keyboard.GetState();
            if (keyboardState.IsKeyDown(Keys.Left))
            {
                _paddlePosition.X -= 5f;
            }

            if (keyboardState.IsKeyDown(Keys.Right))
            {
                _paddlePosition.X += 5f;
            }

            _paddlePosition.X = MathHelper.Clamp(_paddlePosition.X, 0, _graphics.PreferredBackBufferWidth - _paddleTexture.Width);

            _gemSpawnTimer += gameTime.ElapsedGameTime.TotalSeconds;
            if (_gemSpawnTimer >= GemSpawnInterval)
            {
                _gemSpawnTimer = 0;
                float x = _random.Next(0, _graphics.PreferredBackBufferWidth - _gemTexture.Width);
                _gemPositions.Add(new Vector2(x, -100));
            }

            var gemSpeed = 1.5f;
            for (int i = _gemPositions.Count - 1; i >= 0; i--)
            {
                _gemPositions[i] = new Vector2(_gemPositions[i].X, _gemPositions[i].Y + gemSpeed);

                if (_gemPositions[i].Y >= (_paddlePosition.Y - _paddleTexture.Height) &&
                    _gemPositions[i].X >= _paddlePosition.X && _gemPositions[i].X <= _paddlePosition.X + _paddleTexture.Width)
                {
                    _gemPositions.RemoveAt(i);
                    _score++;
                    _scoreText = $"{_score:0000}";
                    _scoreUpdateTime = gameTime.TotalGameTime.TotalSeconds;

                }

                if (_gemPositions[i].Y >= _graphics.PreferredBackBufferHeight)
                {
                    _gemPositions.RemoveAt(i);
                    _gameOver = true;
                }
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            _spriteBatch.Begin();

            float scaleX = (float)_graphics.PreferredBackBufferWidth / _bgTexture.Width;
            float scaleY = (float)_graphics.PreferredBackBufferHeight / _bgTexture.Height;

            _spriteBatch.Draw(_bgTexture, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, new Vector2(scaleX, scaleY), SpriteEffects.None, 0f);

            foreach (var gemPosition in _gemPositions)
            {
                _spriteBatch.Draw(_gemTexture, gemPosition, Color.White);
            }

            _spriteBatch.Draw(_paddleTexture, _paddlePosition, Color.White);

            Color scoreColor = (gameTime.TotalGameTime.TotalSeconds - _scoreUpdateTime) < ScoreHighlightDuration ? Color.Green : Color.DeepPink;
            Vector2 scorePosition = new Vector2(10, 10);
            Color shadowColor = Color.White;
            _spriteBatch.DrawString(_font, _scoreText, scorePosition + new Vector2(1, 0), shadowColor); // Right shadow
            _spriteBatch.DrawString(_font, _scoreText, scorePosition + new Vector2(-1, 0), shadowColor); // Left shadow
            _spriteBatch.DrawString(_font, _scoreText, scorePosition + new Vector2(0, 1), shadowColor); // Bottom shadow
            _spriteBatch.DrawString(_font, _scoreText, scorePosition + new Vector2(0, -1), shadowColor); // Top shadow

            _spriteBatch.DrawString(_font, _scoreText, scorePosition, scoreColor);

            if (_gameOver)
            {
                Vector2 gameOverPosition = new Vector2(
                    (_graphics.PreferredBackBufferWidth - _font.MeasureString("Game Over!").X) / 2,
                    (_graphics.PreferredBackBufferHeight - _font.MeasureString("Game Over!").Y) / 2
                );
                _spriteBatch.DrawString(_font, "Game Over!", gameOverPosition, Color.DeepPink);
                _spriteBatch.DrawString(_font, "Enter To Restart", gameOverPosition + new Vector2(0, _font.MeasureString("Enter To Restart").Y), Color.DeepPink);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
