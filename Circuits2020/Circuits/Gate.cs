using System.Collections.Generic;
using System.Drawing;

namespace Circuits
{
    public abstract class Gate
    {
        // The left-hand edge of the main body of the gate
        protected int left = 0;
        // The top of the whole gate
        protected int top = 0;

        // The width and height of the main part of the gate
        protected const int WIDTH = 40;
        protected const int HEIGHT = 40;
        // length of the connector legs sticking out left and right
        protected const int GAP = 10;

        protected Pin outputPin = null;
        // The various brushes used when drawing the gate
        protected Brush selectedBrush = Brushes.LightCoral;
        protected Brush normalBrush = Brushes.LightGray;
        protected Brush brush;
        protected Brush indicatorBrush;
        protected Brush textBrush;
        protected string indicatorText;
        // Used to check if the gate has been selected
        protected bool selected = false;

        // The list of all the pins of the gate
        protected List<Pin> pins = new List<Pin>();

        public Gate(int x, int y)
        {

        }

        /// <summary>
        /// Moves the main gate body and the pins to a new position
        /// </summary>
        /// <param name="x">The new X position of the gate</param>
        /// <param name="y">The new Y position of the gate</param>
        public virtual void MoveTo(int x, int y)
        {
            // Update the position of the main gate body
            left = x;
            top = y;
        }
        /// <summary>
        /// Draw the gate and the pins in the gate's position
        /// </summary>
        /// <param name="paper">The graphics object to draw the gate on</param>
        public virtual void Draw(Graphics paper)
        {
            // Detemine which brush to use
            if (selected)
            {
                brush = selectedBrush;
            }
            else
            {
                brush = normalBrush;
            }
            // Draw each pin
            foreach (Pin pin in pins)
            {
                pin.Draw(paper);
            }
        }

        /// <summary>
        /// Returns true if the conditions of the gate are met
        /// </summary>
        /// <returns></returns>
        public abstract bool Evaluate();

        /// <summary>
        /// Returns a new copy of the gate
        /// </summary>
        /// <returns></returns>
        public abstract Gate Clone();

        /// <summary>
        /// Returns true if the given mouse position is inside gate boundary
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public virtual bool IsMouseOn(int x, int y)
        {
            if (left <= x && x < left + WIDTH
                && top <= y && y < top + HEIGHT)
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
        /// Creates two input pins and one output pins
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void TwoInOneOut(int x, int y)
        {
            // Create pins
            pins.Add(new Pin(this, true, 20));
            pins.Add(new Pin(this, true, 20));
            pins.Add(new Pin(this, false, 20));
            outputPin = pins[2];
            // Move the gate to the given position
            MoveTo(x, y);
        }
        /// <summary>
        /// Moves two input pins and one output pin
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void Move2In1Out(int x, int y)
        {
            // Move the input pins
            pins[0].X = x - GAP;
            pins[0].Y = y + GAP;
            pins[1].X = x - GAP;
            pins[1].Y = y + HEIGHT - GAP;
            // Move the output pin
            pins[2].X = x + WIDTH + GAP;
            pins[2].Y = y + HEIGHT / 2;
        }

        /// <summary>
        /// Creates one input pin and one output pin
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        protected void OneInOneOut(int x, int y)
        {
            // Create pins
            pins.Add(new Pin(this, true, 20));
            pins.Add(new Pin(this, false, 20));
            outputPin = pins[1];
            // Move the main body to the given position
            MoveTo(x, y);
        }

        /// <summary>
        /// Indicates whether this gate is the current one selected
        /// </summary>
        public virtual bool Selected
        {
            get { return selected; }
            set { selected = value; }
        }

        /// <summary>
        /// Returns the output pin of the gate
        /// </summary>
        public Pin OutputPin
        {
            get
            {
                return outputPin;
            }
        }

        /// <summary>
        /// Returns the x position of the gate
        /// </summary>
        public int Left
        {
            get { return left; }
        }
        /// <summary>
        /// Returns the y position of the gate
        /// </summary>
        public int Top
        {
            get { return top; }
        }
        /// <summary>
        /// The list of pins conatined in the gate
        /// </summary>
        public List<Pin> Pins
        {
            get { return pins; }
            set { pins = value; }
        }

        /// <summary>
        /// Returns true if the given pin is connected to a wire
        /// </summary>
        /// <param name="pin"></param>
        /// <returns></returns>
        protected bool CheckPin(Pin pin)
        {
            if(pin.InputWire != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
