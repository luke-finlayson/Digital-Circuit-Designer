using System.Drawing;

namespace Circuits
{
    /// <summary>
    /// A wire connects between two pins.
    /// That is, it connects the output pin FromPin 
    /// to the input pin ToPin.
    /// </summary>
    public class Wire
    {
        protected bool selected = false;

        protected Pin fromPin, toPin;

        private Color color = Color.DarkGray;

        public Wire(Pin from, Pin to)
        {
            fromPin = from;
            toPin = to;
        }

        /// <summary>
        /// Indicates whether this gate is the current one selected.
        /// </summary>
        public bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// The output pin that this wire is connected to.
        /// </summary>
        public Pin FromPin
        {
            get { return fromPin; }
        }

        /// <summary>
        /// The input pin that this wire is connected to.
        /// </summary>
        public Pin ToPin
        {
            get { return toPin; }
        }

        public void Draw(Graphics paper)
        {
            // Determine which brush to use
            if(!FromPin.Owner.Evaluate())
            {
                color = Color.DarkGray;
            }
            else
            {
                color = SystemColors.ActiveCaption;
            }

            Pen wire = new Pen(selected ? Color.Red : color, 3);
            paper.DrawLine(wire, fromPin.X, fromPin.Y, toPin.X, toPin.Y);
        }
    }
}
