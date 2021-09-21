using System;
using System.Collections.Generic;
using System.Drawing;

namespace Circuits
{
    class InputSource : Gate
    {
        // Boolean to store on/off value of input source
        private bool highVoltage = false;

        public bool HighVoltage { 
            get { return highVoltage; }
            set { highVoltage = value; } 
        }

        public InputSource(int x, int y) : base(x, y)
        {
            // Add the output pin
            pins.Add(new Pin(this, false, 20));
            outputPin = pins[0];
            // Move the main body to the given position
            MoveTo(x, y);
        }

        public override void Draw(Graphics paper)
        {
            base.Draw(paper);

            // Just a small square
            int smallLeft = left + WIDTH / 4;
            int smallTop = top + HEIGHT / 4;

            paper.FillRectangle(brush, smallLeft, smallTop, WIDTH / 2, HEIGHT / 2);
            
            // Determine which colour to use for the high voltage indicator
            if(!highVoltage)
            {
                // Use a white circle
                indicatorBrush = Brushes.White;
                textBrush = Brushes.Black;
                indicatorText = "0";
            }
            else
            {
                // Use a black circle
                indicatorBrush = SystemBrushes.ActiveCaption;
                textBrush = Brushes.White;
                indicatorText = "1";
            }

            // Add the high voltage indicator
            paper.FillEllipse(indicatorBrush, smallLeft + 4,
            smallTop + 4, (WIDTH / 2) - 8, (HEIGHT / 2) - 8);
            // Add text to clarify which state is on & which is off
            paper.DrawString(indicatorText, SystemFonts.DefaultFont, textBrush, 
                left + 15, top + 14);
        }

        /// <summary>
        /// Returns true if the input source has been toggled on
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            return highVoltage;
        }

        public override void MoveTo(int x, int y)
        {
            base.MoveTo(x, y);

            // Move the output pin
            pins[0].X = x + WIDTH;
            pins[0].Y = y + HEIGHT / 2;
        }

        /// <summary>
        /// Override default IsMouseOn method to 
        /// account for smaller dimensions
        /// </summary>
        public override bool IsMouseOn(int x, int y)
        {
            int smallLeft = left + WIDTH / 4;
            int smallTop = top + HEIGHT / 4;

            if (smallLeft <= x && x < smallLeft + WIDTH / 2
                && smallTop <= y && y < smallTop + HEIGHT / 2)
            {
                // Mouse is ontop of the gate
                return true;
            }
            else
            {
                // Mouse is not ontop of the gate
                return false;
            }
        }

        public override Gate Clone()
        {
            // Create new input source
            Gate newSource = new InputSource(left, top);
            // Return the new input source
            return newSource;
        }
    }
}
