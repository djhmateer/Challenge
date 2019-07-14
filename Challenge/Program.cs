using System;
using System.Collections.Generic;

namespace Challenge
{
    public class Program
    {
        // Requires .NET Core 2.2
        // Run the Console App to see Ship1,2,3 runs

        // This is an initial effort to get it working 
        // and is developed in a TDD style

        // Instructions:

        // determines each sequence of ship positions and reports the final position of the ship
        // ship position:
        //   x,y
        //   orientation: N,S,E,W
        // ship instruction:
        //   L - turn left 90 degrees and remain on current grid
        //   R - turn right 90 and remain on current grid
        //   Forward - move forward.
        //     N - (x,y) to (x,y+1)
        // grid is rectangular
        // lost ships leave a warning at the last grid position
        // an instruction to move 'off' the world from a grid point from which a ship has been previously lost is ignored

        // input
        // first line is top right of the grid eg 5 3
        // lower left is 0,0
        // second line is initial coordinate of first ship and Orientation.. all separated by whitespace on 1 line
        // third line is ship instruction: eg LFRFRF
        // then 4 and 5 etc...
        // each ship is processed sequentially ie finishes executing before the next ship begins
        // max value of coordinate is 50, all instruction strings < 100 characters in length

        static void Main()
        {
            while (true)
            {
                Console.WriteLine("Welcome to the Ship Challenge (x to exit)\n");
                const string ship1 = "5 3\n1 1 E\nRFRFRFRF";
                const string ship2 = "5 3\n3 2 N\nFRRFLLFFRRFLL";
                const string ship3 = "0 3 W\nLLFFFLFLFL";
                Console.WriteLine($"\n1 for ship1:\n{ship1}");
                Console.WriteLine($"\n2 for ship2:\n{ship2}");
                Console.WriteLine($"\n3 for ship2 and ship3:\n{ship2}\n{ship3}");

                var keypress = Console.ReadKey().Key;
                var list = new List<string>();
                if (keypress == ConsoleKey.X) break;
                if (keypress == ConsoleKey.D1) list.Add(ship1);
                if (keypress == ConsoleKey.D2) list.Add(ship2);
                if (keypress == ConsoleKey.D3) list.AddRange(new[] { ship2, ship3 });
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n\n Output: {Run(list.ToArray())} \n\n");
                Console.ResetColor();
            }
        }

        // accepts an array of strings which are ships:
        // gridMaxX, gridMaxY
        // x,y, orientation
        // instructions
        // eg 5 3\n1 1 E\nRFRFRFRF
        public static string Run(string[] inputs)
        {
            // if there are multiple ships the grid is defined only by the first ship
            // currentPosX, Y, currentOrientation, isLost are defined here as need to be returned from this method
            int gridMaxX = 0, gridMaxY = 0, currentPosX = 0, currentPosY = 0;
            string currentOrientation = null;
            var isLost = false;
            // warningCoordsList needs to be passed between ships eg see ship2 and ship3 test
            var warningCoordsList = new List<(int x, int y, string direction)>();

            // loop over each ship 
            for (var i = 0; i < inputs.Length; i++)
            {
                // split input newline
                var lines = inputs[i].Split("\n");

                // if multiple ships then only the first includes the grid size on the first line
                var gridSizeLine = 0;
                var startPositionLine = 1;
                var instructionPositionLine = 2;

                // if it is the first ship assume gridsize is on first line
                if (i == 0)
                {
                    // make the grid based on the the first line
                    var gridSizeStringArray = lines[gridSizeLine].Split(' '); // eg 5 3  assuming a single space
                    gridMaxX = int.Parse(gridSizeStringArray[0]); // eg 5 
                    gridMaxY = int.Parse(gridSizeStringArray[1]); // eg 3
                    if (gridMaxX > 50 || gridMaxY > 50) throw new ArgumentException("Max grid size is 50");
                }
                else
                // not the first ship so first line will be startPositionLine.. 
                {
                    startPositionLine--;
                    instructionPositionLine--;
                }

                // get starting position of the ship
                var startPosStringArray = lines[startPositionLine].Split(' '); //eg 1 1 E
                var startPosX = int.Parse(startPosStringArray[0]); // eg 1
                var startPosY = int.Parse(startPosStringArray[1]); // eg 1
                if (startPosX > 50 || startPosY > 50)
                    throw new ArgumentException("StartPositions should less or equal to 50");
                var startPosOrientation = startPosStringArray[2]; // eg E

                // get the instructions
                var instructionsString = lines[instructionPositionLine];
                if (instructionsString.Length >= 100)
                    throw new ArgumentException("Instructions should be less than 100");

                // set current position and orientation of ship (starting position)
                currentPosX = startPosX;
                currentPosY = startPosY;
                currentOrientation = startPosOrientation;
                isLost = false;
                // iterate over each instruction
                foreach (char c in instructionsString)
                {
                    var instruction = c.ToString();
                    if (instruction == "L" || instruction == "R")
                        currentOrientation = Rotate(currentOrientation, instruction);
                    if (instruction == "F")
                    {
                        (currentPosX, currentPosY, isLost) = Move(currentPosX, currentPosY, currentOrientation, gridMaxX, gridMaxY,
                            warningCoordsList);
                    }

                    if (isLost)
                    {
                        // add to the warningCoordsList for next ship if there is one
                        warningCoordsList.Add((currentPosX, currentPosY, currentOrientation));
                        break; // out of foreach. Gone off the grid and lost
                    }
                }
            }
            return currentPosX + " " + currentPosY + " " + currentOrientation + (isLost ? " LOST" : null);
        }

        public static (int x, int y, bool isLost) Move(int x, int y, string currentOrientation, int gridMaxX, int gridMaxY,
            List<(int, int, string)> warningCoordsList)
        {
            // make an empty list if no warningCoordsList to make easier below
            if (warningCoordsList == null) warningCoordsList = new List<(int, int, string)>();
            var isLost = false;

            if (currentOrientation == "N")
            {
                var ytemp = y + 1;
                // will this go out of bounds
                if (ytemp > gridMaxY)
                    if (warningCoordsList.Contains((x, y, "N"))) { }
                    else
                        isLost = true;
                else
                    y++;
            }

            if (currentOrientation == "S")
            {
                var ytemp = y - 1;
                if (ytemp < 0)
                    isLost = !warningCoordsList.Contains((x, y, "S"));
                else
                    y--;
            }

            if (currentOrientation == "E")
            {
                var xtemp = x + 1;
                if (xtemp > gridMaxX)
                    isLost = !warningCoordsList.Contains((x, y, "E"));
                else
                    x++;
            }

            if (currentOrientation == "W")
            {
                var xtemp = x - 1;
                if (xtemp < 0)
                    isLost = !warningCoordsList.Contains((x, y, "W"));
                else
                    x--;
            }
            return (x, y, isLost);
        }

        public static string Rotate(string current, string direction)
        {
            var compass = "NESW";
            int index = compass.IndexOf(current); // eg 1 is East

            if (direction == "R") index++; else index--;

            if (index == 4) index = 0; // from W to N (rotate Right)
            if (index == -1) index = 3; // from N to W (rotate Left) 

            return compass[index].ToString();
        }
    }
}
