using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Circuits
{
    public class Compound : Gate
    {
        // Contains all the gates in the compound gate
        List<Gate> gates = new List<Gate>();

        /// <summary>
        /// Contains all the wires in the compound gate
        /// </summary>
        List<Wire> wires = new List<Wire>();

        private bool firstMove = true;

        public Compound(int x, int y) : base(x, y)
        {
            // Nothing else to initialise
        }

        /// <summary>
        /// Takes a given gate and adds it to the collection of gates
        /// </summary>
        /// <param name="gate">The gate to add to the collection</param>
        public void AddGate(Gate gate)
        {
            // Check if the gate to add is a compound gate
            if (gate is Compound)
            {
                // Move the gates from the compound gate to the list of gates
                foreach (Gate g in ((Compound)gate).Gates)
                {
                    gates.Add(g);
                }
            }
            else
            {
                // Add the gate to the list of gates
                gates.Add(gate);
            }
            // Turn on the gates selection indicator
            gate.Selected = true;
        }

        /// <summary>
        /// Adds a given wire to the compound gate
        /// </summary>
        public void AddWire(Wire wire)
        {
            // Add the given wire to the list of wires
            wires.Add(wire);
        }

        /// <summary>
        /// Contains all the gate objects in the compound gate
        /// </summary>
        public List<Gate> Gates {
            get { return gates; }
            set { gates = value; }
        }

        /// <summary>
        ///
        /// </summary>
        public override bool Selected {
            get
            {
                return selected;
            }
            set
            {
                foreach(Gate gate in gates)
                {
                    // Select or deselect all the gates in the compound gate
                    gate.Selected = value;
                }
                // Select or delselect the compound gate
                selected = value;
            }
        }

        public override bool IsMouseOn(int x, int y)
        {
            foreach (Gate gate in gates)
            {
                // If the current gate was selected, return true
                if (gate.IsMouseOn(x, y))
                {
                    return true;
                }
            }
            // Otherwise, return false
            return false;
        }

        public override void Draw(Graphics paper)
        {
            // Draw each gate
            foreach(Gate gate in gates)
            {
                gate.Draw(paper);
            }
            // Draw each wire
            foreach(Wire wire in wires)
            {
                wire.Draw(paper);
            }
        }

        public override void MoveTo(int x, int y)
        {
            if(!firstMove)
            {
                // Move each gate with offset to the compound gates position
                foreach (Gate gate in gates)
                {
                    gate.MoveTo(gate.Left - left + x, gate.Top - top + y);
                }
            }
            else
            {
                // Move each gate without offsetting (to avoid zero error)
                foreach (Gate gate in gates)
                {
                    gate.MoveTo(gate.Left, gate.Top);
                }
            }
            // Move the compound gate
            base.MoveTo(x, y);

            if(left != 0 && top != 0)
            {
                firstMove = false;
            }
        }

        public override bool Evaluate()
        {
            // Always return true on evaluate as it will never get called
            return true;
        }

        public override Gate Clone()
        {
            List<Gate> newGates = new List<Gate>();
            int sumX = 0;
            int sumY = 0;
            int x, y;
            int count = 0;

            // Get sum of positions and clone gates
            foreach (Gate gate in gates)
            {
                // Clone the gate
                Gate newGate = gate.Clone();
                newGates.Add(newGate);

                // Add the x and y position to the total sums
                sumX += newGate.Left;
                sumY += newGate.Top;
                count++;
            }
            // Calculate average position
            x = sumX / count;
            y = sumY / count;

            // Create new compound gate
            Compound newCompound = new Compound(x, y);

            // Clone wires after all the gates have been cloned
            foreach (Gate gate in gates)
            {
                // Clone only the internal wires of the compound gate
                foreach (Pin pin in gate.Pins)
                {
                    if (CheckPin(pin))
                    {
                        // If the pin is attached to a input wire, check
                        // to see if it connects to another gate inside the collection
                        if (gates.Contains(pin.InputWire.FromPin.Owner))
                        {
                            // Find the output pin
                            Pin output = newGates[gates.IndexOf(pin.InputWire.FromPin.Owner)].OutputPin;

                            // Find the input pin
                            Pin input = newGates[gates.IndexOf(gate)].Pins[gate.Pins.IndexOf(pin)];

                            // Create a new wire
                            Wire newWire = new Wire(output, input);
                            // Add the new wire to the approriate pin
                            input.InputWire = newWire;
                            // Add the wire to the list of wires in the new compound gate
                            newCompound.AddWire(newWire);
                        }
                    }
                }
            }

            // Add gates to new compound gate
            newCompound.Gates = newGates;

            // Return the new compound
            return newCompound;
        }
    }
}
