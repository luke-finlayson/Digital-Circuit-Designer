using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace Circuits
{
    /// <summary>
    /// The main GUI for the COMP104 digital circuits editor.
    /// This has a toolbar, containing buttons called buttonAnd, buttonOr, etc.
    /// The contents of the circuit are drawn directly onto the form.
    ///
    /// REVIEW QUESTIONS
    ///
    /// (1) Is it a better idea to fully document the Gate class or the AndGate subclass?
    ///
    ///     - It is better to fully document the Gate superclass, as the AndGate subclass
    ///       will inherit the comments from the superclass and thus you only need to add
    ///       comments for the parts that are unique to the AndGate class.
    ///
    /// (2) What is the advantage of making a method abstract in the superclass rather
    ///     than just writing a virtual method with no code in the body of the method
    ///
    ///     - Making a method abstract means that every subclass must override it with their own implementation,
	///       which can be useful if you need every subclass to have its own version of the method.However,
    ///       in cases where its better to have a default version of the method to fall back on if multiple
    ///       subclasses don't vary in how it works, then its better to use virtual classes as then you're not
    ///       repeated code.
    ///
    /// (3) If a class has an abstract method in it, does the class have to be abstract?
    ///
    ///     - Yes it does.
    ///
    /// (4) What would happen in your program if one of the gates added to your Compound gate is another Compound
    ///     gate?
    ///
    ///     - It works as expected, as when a compound gate is added to another compound gate instead of adding the compound
    ///       gate to the compound gates list of gates directly, which cause issues when copying and pasting (due to the wires),
    ///       the list of gates from the first compound gate is merged with the list from the new compound gate. Thus everything
    ///       works properly without too much extra code needing to be added.
    ///
    ///
    /// NOTE: You will notice that some aspects of this program don't follow
    /// the instructions exactly. This was to improve the users experience,
    /// i.e: automatically updating the output lamps instead of needing the user
    /// to press a button.
    ///
    /// If these changes are an issue, I can always just update the program so that it
    /// behaves exactly how the instructions describe it.
    ///
    /// </summary>
    public partial class Form1 : Form
    {
        /// <summary>
        /// The (x,y) mouse position of the last MouseDown event.
        /// </summary>
        protected int startX, startY;

        /// <summary>
        /// If this is non-null, we are inserting a wire by
        /// dragging the mouse from startPin to some output Pin.
        /// </summary>
        protected Pin startPin = null;

        /// <summary>
        /// The (x,y) position of the current gate, just before we started dragging it.
        /// </summary>
        protected int currentX, currentY;

        /// <summary>
        /// The set of gates in the circuit
        /// </summary>
        protected List<Gate> gates = new List<Gate>();

        /// <summary>
        /// The set of connector wires in the circuit
        /// </summary>
        protected List<Wire> wires = new List<Wire>();

        /// <summary>
        /// The currently selected gate, or null if no gate is selected.
        /// </summary>
        protected Gate current = null;

        /// <summary>
        /// The new gate that is about to be inserted into the circuit
        /// </summary>
        protected Gate newGate = null;

        /// <summary>
        /// Ensures voltage of input source isn't toggled after its been moved around
        /// </summary>
        protected bool toggleVoltage = true;

        /// <summary>
        /// Used to ignore most other mouse events when copying a gate
        /// </summary>
        protected bool copyMode = false;

        /// <summary>
        /// Acts as a timeline of all gate and wire creations (so that objects
        /// are removed from their respective lists in order)
        /// </summary>
        protected List<string> creationHistory = new List<string>();

        /// <summary>
        /// The new compound gate being created, null if there isn't one
        /// </summary>
        protected Compound newCompound = null;

        public Form1()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }

        /// <summary>
        /// Finds the pin that is close to (x,y), or returns
        /// null if there are no pins close to the position.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public Pin findPin(int x, int y)
        {
            foreach (Gate g in gates)
            {
                if(!(g is Compound))
                {
                    // If the current gate isn't a compound gate
                    foreach (Pin p in g.Pins)
                    {
                        // Check if the given position overlaps any pins in
                        // any of the gates
                        if (p.IsMouseOn(x, y))
                            return p;
                    }
                }
                else
                {
                    // If it is, go through each gate in the compound gate
                    foreach(Gate c in ((Compound)g).Gates)
                    {
                        foreach (Pin p in c.Pins)
                        {
                            // And check if the given position overlaps any pins
                            // in the gates
                            if (p.IsMouseOn(x, y))
                                return p;
                        }
                    }
                }
            }
            return null;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            foreach (Gate g in gates)
            {
                g.Draw(e.Graphics);
            }
            foreach (Wire w in wires)
            {
                w.Draw(e.Graphics);
            }
            if (startPin != null)
            {
                e.Graphics.DrawLine(Pens.White,
                    startPin.X, startPin.Y,
                    currentX, currentY);
            }
            if (newGate != null)
            {
                // show the gate that we are dragging into the circuit
                newGate.MoveTo(currentX, currentY);
                newGate.Draw(e.Graphics);
            }
            if(newCompound != null)
            {
                newCompound.Draw(e.Graphics);
            }
            if(creationHistory.Count == 0 || newCompound != null)
            {
                // Disable undo button
                buttonUndo.Enabled = false;
            }
            else
            {
                // Enable undo button
                buttonUndo.Enabled = true;
            }
        }

        private void Form1_MouseClick(object sender, MouseEventArgs e)
        {
            if(newCompound != null)
            {
                // Check to see if a gate has been selected
                foreach(Gate gate in gates)
                {
                    if(gate.IsMouseOn(e.X, e.Y))
                    {
                        // Add the gate to the compound gate
                        newCompound.AddGate(gate);
                        // Remove the gate from the list
                        gates.Remove(gate);
                        this.Invalidate();
                        break;
                    }
                }
            }
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if(!copyMode && newCompound == null)
            {
                if (current == null)
                {
                    // Determine if the user wants to drag a gate around, or attach a wire
                    bool moveGate = false;
                    foreach (Gate g in gates)
                    {
                        if (g.IsMouseOn(e.X, e.Y))
                        {
                            // Make the selected gate the current gate
                            g.Selected = true;
                            current = g;
                            moveGate = true;

                            // start dragging the current object around
                            startX = e.X;
                            startY = e.Y;
                            currentX = current.Left;
                            currentY = current.Top;

                            this.Invalidate();
                        }
                    }

                    // If the mouse wasn't over a gate, attach a wire
                    if (!moveGate)
                    {
                        // try to start adding a wire
                        startPin = findPin(e.X, e.Y);
                    }
                }
            }
        }

        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            if (startPin != null)
            {
                currentX = e.X;
                currentY = e.Y;
                this.Invalidate();  // this will draw the line
            }
            else if (startX >= 0 && startY >= 0 && current != null)
            {
                //Debug.WriteLine("mouse move to " + e.X + "," + e.Y);
                current.MoveTo(currentX + (e.X - startX), currentY + (e.Y - startY));
                // Check if current gate is an input source, so that upon mouse up
                // the voltage of the input source won't be toggled
                if(current is InputSource)
                {
                    toggleVoltage = false;
                }
                this.Invalidate();
            }
            else if (newGate != null)
            {
                currentX = e.X;
                currentY = e.Y;
                this.Invalidate();
            }
        }

        // Pretty much everything from the MouseClick event is now here, as having those things
        // in the MouseClick method was causing some bugs
        private void Form1_MouseUp(object sender, MouseEventArgs e)
        {
            if(!copyMode && newCompound == null)
            {
                if (startPin != null)
                {
                    // see if we can insert a wire
                    Pin endPin = findPin(e.X, e.Y);
                    if (endPin != null)
                    {
                        Pin input, output;
                        if (startPin.IsOutput)
                        {
                            input = endPin;
                            output = startPin;
                        }
                        else
                        {
                            input = startPin;
                            output = endPin;
                        }
                        if (input.IsInput && output.IsOutput)
                        {
                            if (input.InputWire == null)
                            {
                                Wire newWire = new Wire(output, input);
                                input.InputWire = newWire;
                                wires.Add(newWire);
                                creationHistory.Add("wire");
                            }
                            else
                            {
                                MessageBox.Show("That input is already used.");
                            }
                        }
                        else
                        {
                            MessageBox.Show("Error: you must connect an output pin to an input pin.");
                        }
                    }
                    startPin = null;
                    this.Invalidate();
                }
                if (current != null)
                {
                    // If there is a gate selected then deselected the gate
                    current.Selected = false;
                    current = null;
                    this.Invalidate();
                }
                // See if we are inserting a new gate
                if (newGate != null)
                {
                    // Move the new gate to the mouse position
                    newGate.MoveTo(e.X, e.Y);
                    // Add the gate to the list of gates
                    gates.Add(newGate);
                    // Update the creation history
                    creationHistory.Add("gate");
                    // Reset the newGate variable
                    newGate = null;
                    this.Invalidate();
                }
                // Check to see if the user clicked on a input source
                if (toggleVoltage)
                {
                    foreach (Gate gate in gates)
                    {
                        // For each gate, check if its an input source
                        if (gate is InputSource)
                        {
                            // Use the method to check if the input source has
                            // been clicked
                            CheckInputSource((InputSource)gate, e.X, e.Y);
                        }
                        // If it happens to be a compound gate
                        else if (gate is Compound)
                        {
                            // Go through each gate in the compound and check for inputs
                            foreach (Gate c in ((Compound)gate).Gates)
                            {
                                if (c is InputSource)
                                {
                                    // Use the method to check if the input source has
                                    // been clicked
                                    CheckInputSource((InputSource)c, e.X, e.Y);
                                }
                            }
                        }
                    }
                }
                // Change this back to true after the mouse has been released
                toggleVoltage = true;

                // We have finished moving/dragging
                startX = -1;
                startY = -1;
                currentX = 0;
                currentY = 0;

                // Loop through all the output lamps
                foreach (Gate g in gates)
                {
                    if (g is OutputLamp)
                    {
                        // Evaluate g update lamp to display voltage
                        g.Evaluate();
                    }
                    else if (g is Compound)
                    {
                        // Also need to loop through all the compound gates
                        foreach(Gate c in ((Compound)g).Gates)
                        {
                            if(c is OutputLamp)
                            {
                                // If the gate is an output lamp, then evaluate it
                                c.Evaluate();
                            }
                        }
                    }
                }
            }
            if(copyMode)
            {
                // Check to see if a gate was clicked after the copy button was pressed
                foreach (Gate g in gates)
                {
                    if (g.IsMouseOn(e.X, e.Y))
                    {
                        // If it was then clone the selected gate
                        newGate = g.Clone();
                    }
                }
                // Turn off copy mode even if no gate was clicked on
                copyMode = false;
                buttonCopy.Enabled = true;
            }
        }

        private void buttonOr_Click(object sender, EventArgs e)
        {
            // Create a new OR gate
            newGate = new OrGate(0, 0);
        }

        private void buttonPad_Click(object sender, EventArgs e)
        {
            // Create a new Pad gate
            newGate = new PadGate(0, 0);
        }

        private void buttonInput_Click(object sender, EventArgs e)
        {
            // Create a new input source
            newGate = new InputSource(0, 0);
        }

        private void buttonOutput_Click(object sender, EventArgs e)
        {
            // Create a new output lamp
            newGate = new OutputLamp(0, 0);
        }

        private void buttonAnd_Click(object sender, EventArgs e)
        {
            // Create a new AND gate
            newGate = new AndGate(0, 0);
        }

        private void buttonUndo_Click(object sender, EventArgs e)
        {
            // Determine what type of object was last created
            if(creationHistory[creationHistory.Count - 1] == "gate")
            {
                // Delete the most recent gate
                gates.Remove(gates[gates.Count - 1]);
            }
            else if (creationHistory[creationHistory.Count - 1] == "wire")
            {
                // Get the latest wire
                Wire wire = wires[wires.Count - 1];
                // Check to see if the wire was connected to an output lamp
                if(wire.ToPin.Owner is OutputLamp)
                {
                    // Turn off the output lamp if it was
                    ((OutputLamp)wire.ToPin.Owner).HighVoltage = false;
                }
                // Remove the wire from the input pin
                wire.ToPin.InputWire = null;
                // Delete the wire
                wires.RemoveAt(wires.Count - 1);
            }
            else
            {
                // Undo last compound gate creation
                Compound cpdGate = (Compound)gates[gates.Count - 1];
                foreach(Gate g in cpdGate.Gates)
                {
                    // Transfer gate to main list of gates
                    gates.Add(g);
                }
                // Delete compound gate
                gates.Remove(cpdGate);
            }

            // Remove the latest element in the creation history
            creationHistory.RemoveAt(creationHistory.Count - 1);
            this.Invalidate();
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            // Create a new compound gate
            newCompound = new Compound(0, 0);
            // Disable copy mode
            copyMode = false;
            // Toggle the group buttons
            ToggleGroupButtons();
        }

        private void buttonEnd_Click(object sender, EventArgs e)
        {
            // Deselect the gates in the compound
            newCompound.Selected = false;
            // Move the new compound gate into new gate
            //newGate = newCompound;
            // Add the new compound gate to the list of gates
            gates.Add(newCompound);
            creationHistory.Add("compound");
            // Reset the new compound gate
            newCompound = null;
            // Toggle the group buttons
            ToggleGroupButtons();
            this.Invalidate();
        }

        protected void ToggleGroupButtons()
        {
            // Toggle the end group button
            buttonEnd.Enabled = !buttonEnd.Enabled;
            // Toggle the start group button
            buttonStart.Enabled = !buttonStart.Enabled;

            if (buttonEnd.Enabled)
            {
                // Disable other buttons to prevent human error when creating a
                // new compound gate
                buttonCopy.Enabled = false;
                buttonUndo.Enabled = false;
                buttonClear.Enabled = false;
            }
            else
            {
                // Turn other buttons back on after creating new compound gate
                buttonCopy.Enabled = true;
                buttonUndo.Enabled = true;
                buttonClear.Enabled = true;
            }
        }

        private void buttonNot_Click(object sender, EventArgs e)
        {
            // Create a new NOT gate
            newGate = new NotGate(0, 0);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            // Clear all lists
            gates = new List<Gate>();
            wires = new List<Wire>();
            creationHistory = new List<string>();
            // Reset all variables
            toggleVoltage = true;
            startX = startY = -1;
            current = null;
            newGate = null;
            currentX = currentY = 0;
            newCompound = null;
            copyMode = false;
            this.Invalidate();
        }

        private void buttonCopy_Click(object sender, EventArgs e)
        {
            // Toggle copy mode
            copyMode = true;
            buttonCopy.Enabled = false;
            // Clear new gate
            newGate = null;
            this.Invalidate();
        }

        private void CheckInputSource(InputSource input, int x, int y)
        {
            // Check if the input source has been clicked
            if (input.IsMouseOn(x, y))
            {
                // Source has been selected, toggle high voltage
                input.HighVoltage = !input.HighVoltage;
                this.Invalidate();
            }
        }
    }
}
