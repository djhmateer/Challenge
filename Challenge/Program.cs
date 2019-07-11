using System;
using Xunit;
using Xunit.Abstractions;

namespace Challenge
{
    public class Program
    {
        // determines each sequence of ship positions and reports the final position of the ship
        // ship position:
        //   x,y
        //   orientation: N,S,E,W
        // ship instruction:
        //   L - turn left 90 degrees and remain on current grid
        //   R - turn right 90 and reamin on current grid
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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        static string Run(string input)
        {
            // split input into a string array based on newline character
            string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

            // make the grid based on the the first line
            var gridSizeStringArray = lines[0].Split(' '); // eg 5 3  assuming a single space
            var gridMaxX = int.Parse(gridSizeStringArray[0]); // eg 5 
            var gridMaxY = int.Parse(gridSizeStringArray[1]); // eg 3


            // get starting position of the ship
            var startPosStringArray = lines[1].Split(' '); //eg 1 1 E
            var startPosX = int.Parse(startPosStringArray[0]); // eg 1
            var startPosY = int.Parse(startPosStringArray[1]); // eg 1
            var startPosOrientation = startPosStringArray[2]; // eg E

            // get the instructions
            var instructionsString = lines[2];

            // 0,0 is lower left, and 5,3 is top right

            // iterate over each instruction
            // using starting position
            int currentPosX = startPosX;
            int currentPosY = startPosY;
            string currentOrientation = startPosOrientation;
            foreach (char instruction in instructionsString)
            {
                var instruc = instruction.ToString();
                if (instruc == "R")
                {
                    // want to turn 90 degrees right
                }

            }
            return lines[2];
        }

        enum Direction
        {
            North,
            East,
            South,
            West
        }
        static string Rotate(string current, string direction)
        {
            // N E S W
            var stuff = "NESW";
            int thing = stuff.IndexOf(current); // eg 1 is East

            if (direction == "R") thing++; else thing--;

            if (thing == 4) thing = 0; // from W to N

            // HERE
            return "";
        }

        [Fact]
        public void RotateTest() => Assert.Equal("S", Rotate("E", "R"));
        [Fact]
        public void RotateWestToNorth() => Assert.Equal("N", Rotate("W", "R"));


        [Fact]
        public void RunTest() => Assert.Equal("1 1 E", Run("5 3\n1 1 E\nRFRFRFRF"));
    }
}
