using System.Collections.Generic;
using System.Drawing;

namespace Circuits
{
    public class NotGate : Gate
    {
        // Shorter width for NOT gate
        private int width = 30;
        // Offset for main body circle and output pin
        private int offset = 5;

        public NotGate(int x, int y) : base(x, y)
        {
            // Create pins
            OneInOneOut(x, y);
        }

        public override void Draw(Graphics paper)
        {
            base.Draw(paper);

            // Triangle + Small circle
            int triangleRight = left + 3 * (width / 4);

            paper.FillPolygon(brush, new Point[] { 
                new Point(left, top + HEIGHT / 4), 
                new Point(left, top + HEIGHT - HEIGHT / 4),
                new Point(triangleRight, top + (HEIGHT / 2)) });
            paper.FillEllipse(brush, triangleRight - offset, top + 3 * (HEIGHT / 8),
                HEIGHT / 4, HEIGHT / 4);
        }

        public override void MoveTo(int x, int y)
        {
            base.MoveTo(x, y);

            // Move the input pin x position
            pins[0].X = x - GAP;
            // Move the output pin x position
            pins[1].X = x + WIDTH - offset;
            // Move the pins y position
            foreach(Pin pin in pins)
            {
                pin.Y = y + HEIGHT / 2;
            }
        }

        /// <summary>
        /// Override default IsMouseOn method to account for smaller dimensions
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public override bool IsMouseOn(int x, int y)
        {
            if (left <= x && x < left + width
                && top + HEIGHT / 4 <= y && y < top + HEIGHT - HEIGHT / 4)
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

        /// <summary>
        /// Returns false if the owner of the wire connected to the input 
        /// pin returns true
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            // Only evaluate if the input pin is connected to a gate
            if(CheckPin(pins[0]))
            {
                // Get the owner of the wire connected to the input pin
                Gate gate = pins[0].InputWire.FromPin.Owner;
                return !gate.Evaluate();
            }
            else
            {
                return false;
            }
        }

        public override Gate Clone()
        {
            // Create new pad gate
            Gate newGate = new NotGate(left, top);
            // Return the new input source
            return newGate;
        }
    }
}
