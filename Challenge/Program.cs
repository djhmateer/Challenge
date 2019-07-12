using System;
using Xunit;

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

        static void Main(string[] args) => Console.WriteLine("Hello World!");

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
            int currentPosX = startPosX;
            int currentPosY = startPosY;
            string currentOrientation = startPosOrientation;
            bool isLost = false;
            foreach (char i in instructionsString)
            {
                var instruction = i.ToString();
                if (instruction == "L" || instruction == "R")
                    currentOrientation = Rotate(currentOrientation, instruction);
                if (instruction == "F")
                    (currentPosX, currentPosY, isLost) = Move(currentPosX, currentPosY, currentOrientation, gridMaxX, gridMaxY);
                if (isLost) break; 
            }

            var message = isLost ? " LOST" : null;
            return currentPosX + " " + currentPosY + " " + currentOrientation + message;
        }

        static (int x, int y, bool isLost) Move(int x, int y, string currentOrientation, int gridMaxX, int gridMaxY)
        {
            bool isLost = false;
            if (currentOrientation == "N")
            {
                y++;
                // does this go out of bounds?
                if (y > gridMaxY)
                {
                    isLost = true;
                    y--;
                }
            }

            if (currentOrientation == "S")
            {
                y--;
                if (y < 0)
                {
                    isLost = true;
                    y++;
                }
            }

            if (currentOrientation == "E")
            {
                x++;
                if (x > gridMaxX)
                {
                    isLost = true;
                    x--;
                }
            }

            if (currentOrientation == "W")
            {
                x--;
                if (x < 0)
                {
                    isLost = true;
                    x++;
                }
            }

            return (x, y, isLost);
        }

        [Theory]
        // normal movement
        // 9,9 is gridMaxY, gridMaxY
        [InlineData(0, 0, "N", 9, 9, 0, 1, false)]
        [InlineData(0, 1, "S", 9, 9, 0, 0, false)]
        [InlineData(0, 0, "E", 9, 9, 1, 0, false)]
        [InlineData(1, 0, "W", 9, 9, 0, 0, false)]
        
        // isLost (with no previous isLosts)
        // North - go off top so lost, gridMaxX 2, gridMaxY 2, expectedX 2, expectedY 2
        [InlineData(2, 2, "N", 2, 2, 2, 2, true)]
        // South - go off bottom
        [InlineData(0, 0, "S", 2, 2, 0, 0, true)]
        // East
        [InlineData(2, 1, "E", 2, 2, 2, 1, true)]
        // West
        [InlineData(0, 1, "W", 2, 2, 0, 1, true)]
        public void MoveTests(int currentX, int currentY, string orientation, int gridMaxX, int gridMaxY, int expectedX, int expectedY, bool isLost)
        {
            Assert.Equal((expectedX, expectedY, isLost), Move(currentX, currentY, orientation, gridMaxX, gridMaxY));
        }

        [Fact]
        public void RunFirstShip() => Assert.Equal("1 1 E", Run("5 3\n1 1 E\nRFRFRFRF"));
        [Fact]
        public void RunSecondShip() => Assert.Equal("3 3 N LOST", Run("5 3\n3 2 N\nFRRFLLFFRRFLL"));



        static string Rotate(string current, string direction)
        {
            var compass = "NESW";
            int index = compass.IndexOf(current); // eg 1 is East

            if (direction == "R") index++; else index--;

            if (index == 4) index = 0; // from W to N (rotate Right)
            if (index == -1) index = 3; // from N to W (rotate Left) 

            return compass[index].ToString();
        }

        [Fact]
        public void RotateEastToSouth() => Assert.Equal("S", Rotate("E", "R"));
        [Fact]
        public void RotateWestToNorth() => Assert.Equal("N", Rotate("W", "R"));
        [Fact]
        public void RotateSouthToEast() => Assert.Equal("E", Rotate("S", "L"));
        [Fact]
        public void RotateNorthToWest() => Assert.Equal("W", Rotate("N", "L"));

    }
}
