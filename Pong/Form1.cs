/*
 * Description:     A basic PONG simulator
 * Author:   Justin Morrison        
 * Date:  February 3, 2023         
 */

#region libraries

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.Media;
using System.Diagnostics;

#endregion

namespace Pong
{
    public partial class Form1 : Form
    {
        #region global values

        //Global random generator
        Random random = new Random();   

        //Global stopwatch for intervals
        Stopwatch stopWatch = new Stopwatch();
        Stopwatch animationStopWatch = new Stopwatch();


        //graphics objects for drawing
        SolidBrush whiteBrush = new SolidBrush(Color.White);
        SolidBrush blueBrush = new SolidBrush(Color.Blue);
        SolidBrush redBrush = new SolidBrush(Color.Red);
        SolidBrush yellowBrush = new SolidBrush(Color.Yellow);

        Font drawFont = new Font("Courier New", 10);

        // Sounds for game
        SoundPlayer scoreSound = new SoundPlayer(Properties.Resources.score);
        SoundPlayer collisionSound = new SoundPlayer(Properties.Resources.collision);

        //determines whether a key is being pressed or not
        Boolean wKeyDown, sKeyDown, upKeyDown, downKeyDown;

        // check to see if a new game can be started
        Boolean newGameOk = true;

        //ball values
        Boolean ballMoveRight = true;
        Boolean ballMoveDown = true;
        int ballSpeed = 4;
        const int BALL_WIDTH = 20;
        const int BALL_HEIGHT = 20; 
        Rectangle ball;

        //player values
        int paddleSpeed = 4;
        const int PADDLE_EDGE = 20;  // buffer distance between screen edge and paddle            
        const int PADDLE_WIDTH = 10;
        int paddleHeight1 = 40;
        int paddleHeight2 = 40;
        Rectangle player1, player2;

        //animate values
        const int ANIMATE_SPEED = 10;
        Rectangle animateLine1;
        Rectangle animateLine2;
        Boolean isAnimate = false;
        Boolean player1Scored;

        //Target shape
        Rectangle yellowPowerUp;
        Boolean player1PowerUp = false;
        Boolean player2PowerUp = false;
        int targetSpeed = 1;



        //player and game scores
        int player1Score = 0;
        int player2Score = 0;
        int gameWinScore = 2;  // number of points needed to win game
        int counter = 0;

        int randNum;

        #endregion

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            //check to see if a key is pressed and set is KeyDown value to true if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = true;
                    break;
                case Keys.S:
                    sKeyDown = true;
                    break;
                case Keys.Up:
                    upKeyDown = true;
                    break;
                case Keys.Down:
                    downKeyDown = true;
                    break;
                case Keys.Y:
                case Keys.Space:
                    if (newGameOk)
                    {
                        SetParameters();
                    }
                    break;
                case Keys.Escape:
                    if (newGameOk)
                    {
                        Close();
                    }
                    break;
            }
        }
        
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            //check to see if a key has been released and set its KeyDown value to false if it has
            switch (e.KeyCode)
            {
                case Keys.W:
                    wKeyDown = false;
                    break;
                case Keys.S:
                    sKeyDown = false;
                    break;
                case Keys.Up:
                    upKeyDown = false;
                    break;
                case Keys.Down:
                    downKeyDown = false;
                    break;
            }
        }

        /// <summary>
        /// sets the ball and paddle positions for game start
        /// </summary>
        private void SetParameters()
        {
            if (newGameOk)
            {
                ballSpeed = 4;

                player1Score = player2Score = 0;
                player1ScoreLabel.Text = $"{player1Score}";
                player2ScoreLabel.Text = $"{player2Score}";

                newGameOk = false;
                startLabel.Visible = false;
                gameUpdateLoop.Start();
            }

            //player start positions
            player1 = new Rectangle(PADDLE_EDGE, this.Height / 2 - paddleHeight1 / 2, PADDLE_WIDTH, paddleHeight1);
            player2 = new Rectangle(this.Width - PADDLE_EDGE - PADDLE_WIDTH, this.Height / 2 - paddleHeight2 / 2, PADDLE_WIDTH, paddleHeight2);

            // TODO create a ball rectangle in the middle of screen
            ball = new Rectangle(this.Width/2, this.Height/2, 10, 10);

            //animation lines
            animateLine1 = new Rectangle(0, -300, 800, 40);
            animateLine2 = new Rectangle(0, -100, 800, 40);

            //random powerups
            yellowPowerUp = new Rectangle(this.Width / 2, -50, 50, 50);

            //set up random number
            randNum = random.Next(25, this.Height - 25);
        }

        /// <summary>
        /// This method is the game engine loop that updates the position of all elements
        /// and checks for collisions.
        /// </summary>
        private void gameUpdateLoop_Tick(object sender, EventArgs e)
        {
            counter++;

            if (yellowPowerUp.Y == randNum)
            {

            }
            else
            {
                yellowPowerUp.Y += targetSpeed;
            }



            //if ((player1PowerUp == true || player2PowerUp == true) && counter % 1000 == 0)
            //{

            //}

            if (player1PowerUp == true && animationStopWatch.ElapsedMilliseconds < 5000)
            {
                paddleHeight1 = 80;
                player1 = new Rectangle(player1.X, player1.Y, PADDLE_WIDTH, paddleHeight1);
            }
            else
            {
                paddleHeight1 = 40;
                player1 = new Rectangle(player1.X, player1.Y, PADDLE_WIDTH, paddleHeight1);
            }
            if (player2PowerUp == true && animationStopWatch.ElapsedMilliseconds < 5000)
            {
                paddleHeight2 = 80;
                player2 = new Rectangle(player2.X, player2.Y, PADDLE_WIDTH, paddleHeight2);
            }
            else
            {
                paddleHeight2 = 40;
                player2 = new Rectangle(player2.X, player2.Y, PADDLE_WIDTH, paddleHeight2);
            }            



            #region update ball position



            // TODO create code to move ball either left or right based on ballMoveRight and usingballSpeed
            if (ballMoveRight == true)
            {
                ball.X +=ballSpeed;
            }
            else
            {
                ball.X -=ballSpeed;
            }

            // TODO create code move ball either down or up based on ballMoveDown and usingballSpeed
            if (ballMoveDown == true)
            {
                ball.Y +=ballSpeed;
            }
            else
            {
                ball.Y -=ballSpeed;
            }
            #endregion

            #region update paddle positions

            // TODO create code to move player 1 up
            if (wKeyDown == true && player1.Y > 5)
            {
                player1.Y -= paddleSpeed;
            }

            // TODO create an if statement and code to move player 1 down 
            if (sKeyDown == true && player1.Y < this.Height - paddleHeight1-5)
            {
                // TODO create code to move player 1 up
                player1.Y += paddleSpeed;
            }

            // TODO create an if statement and code to move player 2 up
            if (upKeyDown == true && player2.Y > 5)
            {
                // TODO create code to move player 1 up
                player2.Y -= paddleSpeed;
            }

            // TODO create an if statement and code to move player 2 down
            if (downKeyDown == true && player2.Y < this.Height - paddleHeight2-5)
            {
                // TODO create code to move player 1 up
                player2.Y += paddleSpeed;
            }
            #endregion

            #region ball collision with top and bottom lines

            if (ball.Y < 0) // if ball hits top line
            {
                // TODO use ballMoveDown boolean to change direction
                // TODO play a collision sound

                ballMoveDown = true;
                collisionSound.Play();
            }
            // TODO In an else if statement check for collision with bottom line
            // If true use ballMoveDown boolean to change direction
            else if (ball.Y > this.Height - ball.Height)
            {
                ballMoveDown = false;
                collisionSound.Play();

            }

            #endregion

            #region ball collision with paddles

            // TODO create if statment that checks if player1 collides with ball and if it does
            // --- play a "paddle hit" sound and
            // --- use ballMoveRight boolean to change direction
          
            // TODO create if statment that checks if player2 collides with ball and if it does
            // --- play a "paddle hit" sound and
            // --- use ballMoveRight boolean to change direction
           

            /*  ENRICHMENT
             *  Instead of using two if statments as noted above see if you can create one
             *  if statement with multiple conditions to play a sound and change direction
             */
            if (ball.IntersectsWith(player1) || ball.IntersectsWith(player2))
            {
                collisionSound.Play();
                ballMoveRight = !ballMoveRight;
                ballSpeed++;
                paddleSpeed++;
            }

            #endregion

            #region ball collision with side walls (point scored)

            if (ball.X < 0)  // ball hits left wall logic
            {
                // TODO
                // --- play score sound
                player1Scored = false;

                // --- update player 2 score and display it to the label
                player2Score++;
                player2ScoreLabel.Text = $"{player2Score}";

                ScoreMethod();


                // TODO use if statement to check to see if player 2 has won the game. If true run 
                // GameOver() method. Else change direction of ball and call SetParameters() method.
            }

            // TODO same as above but this time check for collision with the right wall
            if (ball.X > this.Width - ball.Width)  // ball hits right wall logic
            {
                // TODO
                // --- play score sound
                player1Scored = true;

                // --- update player 1 score and display it to the label
                player1Score++;
                player1ScoreLabel.Text = $"{player1Score}";

                ScoreMethod();


                // TODO use if statement to check to see if player 1 has won the game. If true run 
                // GameOver() method. Else change direction of ball and call SetParameters() method.

            }

            #endregion

            if (ball.IntersectsWith(yellowPowerUp))
            {
                yellowPowerUp.Y += 10;
                //targetSpeed = 10;

                animationStopWatch.Reset();
                animationStopWatch.Start();

                if (ballMoveRight)
                {
                    player1PowerUp = true;
                }
                else
                {
                    player2PowerUp = true;
                }
            }
        
            if (stopWatch.ElapsedMilliseconds > 1000)
            {
                GoalGif.Enabled = false;
                GoalGif.Visible = false;
                animateLine1.Y += ANIMATE_SPEED;
                animateLine2.Y += ANIMATE_SPEED;
            }

            if(stopWatch.ElapsedMilliseconds > 2500)
            {
                paddleSpeed = 4;
                ballSpeed = 4;
                stopWatch.Stop();
                stopWatch.Reset();
                targetSpeed = 1;
            }

            //refresh the screen, which causes the Form1_Paint method to run
            this.Refresh();
        }
        
        /// <summary>
        /// Displays a message for the winner when the game is over and allows the user to either select
        /// to play again or end the program
        /// </summary>
        /// <param name="winner">The player name to be shown as the winner</param>
        /// 


        private void ScoreMethod()
        {
            scoreSound.Play();

            targetSpeed = 0;    

            ballSpeed = 0;

            GoalGif.Location = new Point(ball.X - 80, ball.Y - 35);

            GoalGif.Enabled = true;
            GoalGif.Visible = true;

            stopWatch.Reset();
            stopWatch.Start();

            if (player2Score == gameWinScore)
            {
                GameOver("Player 2");
            }
            if (player1Score == gameWinScore)
            {
                GameOver("Player 1");
            }
            else
            {
                ballMoveRight = !ballMoveRight;
                SetParameters();
            }
        }

        private void GameOver(string winner)
        {
            newGameOk = true;

            // TODO create game over logic
            // --- stop the gameUpdateLoop
            gameUpdateLoop.Enabled = false;

            // --- show a message on the startLabel to indicate a winner, (may need to Refresh).
            startLabel.Text = $"{winner} Has Won";
            startLabel.Visible = true;
            this.Refresh();

            // --- use the startLabel to ask the user if they want to play again
            startLabel.Text += $"\n\n Press Space To Play Again";

        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            // TODO draw player2 using FillRectangle
            e.Graphics.FillRectangle(blueBrush, player1);
            e.Graphics.FillRectangle(redBrush, player2);


            // TODO draw ball using FillRectangle
            e.Graphics.FillRectangle(whiteBrush, ball);

            if(player1Scored == true)
            {
                e.Graphics.FillRectangle(blueBrush, animateLine1);
                e.Graphics.FillRectangle(blueBrush, animateLine2);
            }
            else
            {
                e.Graphics.FillRectangle(redBrush, animateLine1);
                e.Graphics.FillRectangle(redBrush, animateLine2);
            }

            e.Graphics.FillRectangle(yellowBrush, yellowPowerUp);

        }

    }
}
