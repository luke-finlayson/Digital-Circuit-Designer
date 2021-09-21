using System;
using System.Collections.Generic;
using System.Drawing;

namespace Circuits
{
    class OutputLamp : Gate
    {
        private bool highVoltage = false;
        
        /// <summary>
        /// Stores the on/off value of the output lamp, true == high voltage
        /// </summary>
        public bool HighVoltage { 
            get { return highVoltage; }
            set { highVoltage = value; } 
        }

        public OutputLamp(int x, int y) : base(x, y)
        {
            // Add the input pin
            pins.Add(new Pin(this, true, 20));

            // Move the main body to the given position
            MoveTo(x, y);
        }

        public override void Draw(Graphics paper)
        {
            base.Draw(paper);

            // Just a small square
            int smallLeft = left + WIDTH / 4;
            int smallTop = top + HEIGHT / 4;

            // Determine which colour to use for the lamp
            if (highVoltage)
            {
                // Use a yellow colour for the lamp
                indicatorBrush = SystemBrushes.ActiveCaption;
                // Use white text
                textBrush = Brushes.White;
                indicatorText = "1";
            }
            else
            {
                // Use the default colour for a gate
                indicatorBrush = brush;
                // Use black text
                textBrush = Brushes.Black;
                indicatorText = "0";
            }

            // Draw a larger circle behind lamp to show highlight when lamp is on
            if(selected && highVoltage)
            {
                paper.FillEllipse(brush, left + WIDTH / 4 - 2, top + HEIGHT / 4 - 2, 
                    WIDTH / 2 + 4, HEIGHT / 2 + 4);
            }

            // Shown as a small circle
            paper.FillEllipse(indicatorBrush, left + WIDTH / 4,
                top + HEIGHT / 4, WIDTH / 2,
                HEIGHT / 2);
            // Add text to clarify which state is on & which is off
            paper.DrawString(indicatorText, SystemFonts.DefaultFont, textBrush,
                left + 15, top + 14);
        }

        public override void MoveTo(int x, int y)
        {
            base.MoveTo(x, y);

            // Move the input pin
            pins[0].X = x;
            pins[0].Y = y + HEIGHT / 2;
        }

        /// <summary>
        /// Gets the input from the owner of the wire connected to the input pin
        /// and updates the voltage to match
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            // Only evaluate if the pin is connected to another gate
            if(CheckPin(pins[0]))
            {
                // Get the owner of the wire connected to the input pin
                Gate gate = pins[0].InputWire.FromPin.Owner;
                // Update the voltage to match input
                highVoltage = gate.Evaluate();
            }

            return true;
        }

        public override Gate Clone()
        {
            // Create new output lamp
            Gate newLamp = new OutputLamp(left, top);
            // Return the new lamp
            return newLamp;
        }
    }
}
