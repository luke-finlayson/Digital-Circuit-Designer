using System.Collections.Generic;
using System.Drawing;

namespace Circuits
{
    public class OrGate : Gate
    {
        public OrGate(int x, int y) : base(x, y)
        {
            // Create pins
            TwoInOneOut(x, y);
        }

        public override void Draw(Graphics paper)
        {
            // Determine brush colour
            base.Draw(paper);

            // Define points for the main body
            Point topLeft = new Point(left, top);
            Point bottomLeft = new Point(left, top + HEIGHT);
            Point right = new Point(left + WIDTH, top + HEIGHT / 2);
            Point midLeft = new Point(left + WIDTH / 8, top + HEIGHT / 2);
            Point[] points = { topLeft, midLeft, bottomLeft, right };

            // Draw main body
            paper.FillClosedCurve(brush, points);
        }

        public override void MoveTo(int x, int y)
        {
            // Move the main body
            base.MoveTo(x, y);

            // Move the pins
            Move2In1Out(x, y);
        }

        public override bool Evaluate()
        {
            // Only evaluate if both pins are connected to gates
            if(CheckPin(pins[0]) && CheckPin(pins[1]))
            {
                // Get the output of the two gates connected to the input pins
                Gate gate1 = pins[0].InputWire.FromPin.Owner;
                Gate gate2 = pins[1].InputWire.FromPin.Owner;
                // Return true if either one of them is true
                return gate1.Evaluate() || gate2.Evaluate();
            }
            else
            {
                return false;
            }
        }

        public override Gate Clone()
        {
            // Create a new AND gate
            Gate newGate = new OrGate(left, top);
            // Return the new gate
            return newGate;
        }
    }
}
