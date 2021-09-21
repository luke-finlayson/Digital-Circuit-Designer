using System.Collections.Generic;
using System.Drawing;

namespace Circuits
{
    public class PadGate : Gate
    {
        public PadGate(int x, int y) : base(x, y)
        {
            // Create pins
            OneInOneOut(x, y);
        }

        public override void Draw(Graphics paper)
        {
            base.Draw(paper);

            // Simply just a small rectangle
            paper.FillRectangle(brush, left, top + HEIGHT / 4, 
                WIDTH, HEIGHT / 2);
        }

        public override void MoveTo(int x, int y)
        {
            // Move the main body
            base.MoveTo(x, y);

            // Move the pins
            pins[0].X = x - GAP;
            pins[0].Y = y + HEIGHT / 2;
            pins[1].X = x + WIDTH + GAP;
            pins[1].Y = y + HEIGHT / 2;
        }

        /// <summary>
        /// Returns true if the owner of the wire connected to the input
        /// pin returns true
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            // Only evaluate if the input pin is connected to a gate
            if (CheckPin(pins[0]))
            {
                // Get owner of wire connected to input pin
                Gate gate = pins[0].InputWire.FromPin.Owner;
                return gate.Evaluate();
            }
            else
            {
                return false;
            }
        }

        public override Gate Clone()
        {
            // Create new pad gate
            Gate newGate = new PadGate(left, top);
            // Return the new input source
            return newGate;
        }
    }
}
