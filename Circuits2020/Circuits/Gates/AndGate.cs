using System.Collections.Generic;
using System.Drawing;

namespace Circuits
{
    /// <summary>
    /// This class implements an AND gate with two inputs
    /// and one output.
    /// </summary>
    public class AndGate : Gate
    {
        public AndGate(int x, int y) : base(x, y)
        {
            // Create pins
            TwoInOneOut(x, y);
        }
        
        public override void Draw(Graphics paper)
        {
            base.Draw(paper);

            // AND is simple, so we can use a circle plus a rectange.
            // An alternative would be to use a bitmap.
            paper.FillEllipse(brush, left, top, WIDTH, HEIGHT);
            paper.FillRectangle(brush, left, top, WIDTH/2, HEIGHT);
        }

        public override void MoveTo(int x, int y)
        {
            // Move the main body
            base.MoveTo(x, y);

            // Move the pins
            Move2In1Out(x, y);
        }
        
        /// <summary>
        /// Gets the output of the two gates connected to its input pins
        /// and returns true if they are both true
        /// </summary>
        /// <returns></returns>
        public override bool Evaluate()
        {
            // Only evaluate if both pins are connected to a gate
            if(CheckPin(pins[0]) && CheckPin(pins[1]))
            {
                // Get the two gates connected to the input pin
                Gate gate1 = pins[0].InputWire.FromPin.Owner;
                Gate gate2 = pins[1].InputWire.FromPin.Owner;
                // Return true if they are both true
                return gate1.Evaluate() && gate2.Evaluate();
            }
            else
            {
                return false;
            }
        }

        public override Gate Clone()
        {
            // Create a new AND gate
            Gate newGate = new AndGate(left, top);
            // Return the new gate
            return newGate;
        }
    }
}
