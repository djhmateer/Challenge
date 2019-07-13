using System;
using System.Collections.Generic;
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

        // accepts an array of strings which are ships:
        // gridMaxX, gridMaxY
        // x,y, orientation
        // instructions
        // eg 5 3\n1 1 E\nRFRFRFRF
        static string Run(string[] inputs)
        {
            int gridMaxX, gridMaxY, currentPosX = 0, currentPosY = 0;
            string currentOrientation = null;
            var isLost = false;
            var warningCoordsList = new List<(int x, int y, string direction)>();

            // loop over each ship 
            foreach (var input in inputs)
            {
                // split input into a string array based on newline character
                string[] lines = input.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);

                // make the grid based on the the first line
                var gridSizeStringArray = lines[0].Split(' '); // eg 5 3  assuming a single space
                gridMaxX = int.Parse(gridSizeStringArray[0]); // eg 5 
                gridMaxY = int.Parse(gridSizeStringArray[1]); // eg 3
                if (gridMaxX > 50 || gridMaxY > 50) throw new ArgumentException("Max grid size is 50");

                // get starting position of the ship
                var startPosStringArray = lines[1].Split(' '); //eg 1 1 E
                var startPosX = int.Parse(startPosStringArray[0]); // eg 1
                var startPosY = int.Parse(startPosStringArray[1]); // eg 1
                if (startPosX > 50 || startPosY > 50) throw new ArgumentException("StartPositions should less or equal to 50");
                var startPosOrientation = startPosStringArray[2]; // eg E

                // get the instructions
                var instructionsString = lines[2];
                if (instructionsString.Length >= 100) throw new ArgumentException("Instructions should be less than 100");

                // 0,0 is lower left, and 5,3 is top right

                // iterate over each instruction
                currentPosX = startPosX;
                currentPosY = startPosY;
                currentOrientation = startPosOrientation;
                isLost = false;
                foreach (char i in instructionsString)
                {
                    var instruction = i.ToString();
                    if (instruction == "L" || instruction == "R")
                        currentOrientation = Rotate(currentOrientation, instruction);
                    if (instruction == "F")
                    {
                        (currentPosX, currentPosY, isLost) = Move(currentPosX, currentPosY, currentOrientation, gridMaxX, gridMaxY,
                            warningCoordsList);
                    }
                    if (isLost)
                    {
                        // add to the warningCoordsList
                        warningCoordsList.Add((currentPosX, currentPosY, currentOrientation));
                        break; // Gone off the grid and lost
                    }
                }
            }
            return currentPosX + " " + currentPosY + " " + currentOrientation + (isLost ? " LOST" : null);
        }

        static (int x, int y, bool isLost) Move(int x, int y, string currentOrientation, int gridMaxX, int gridMaxY,
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

        [Fact]
        public void MoveWithWarningCoordsN()
        {
            var expected = (0, 2, false);
            Assert.Equal(expected, Move(0, 2, "N", 2, 2, new List<(int, int, string)> { (0, 2, "N") }));
        }
        [Fact]
        public void MoveWithWarningCoordsS()
        {
            var expected = (2, 0, false);
            Assert.Equal(expected, Move(2, 0, "S", 2, 2, new List<(int, int, string)> { (2, 0, "S") }));
        }
        [Fact]
        public void MoveWithWarningCoordsE()
        {
            var expected = (2, 0, false);
            Assert.Equal(expected, Move(2, 0, "E", 2, 2, new List<(int, int, string)> { (2, 0, "E") }));
        }
        [Fact]
        public void MoveWithWarningCoordsW()
        {
            var expected = (0, 2, false);
            Assert.Equal(expected, Move(0, 2, "W", 2, 2, new List<(int, int, string)> { (0, 2, "W") }));
        }


        [Theory]
        // normal movement
        // 9,9 is gridMaxY, gridMaxY
        [InlineData(0, 0, "N", 9, 9, null, 0, 1, false)]
        [InlineData(0, 1, "S", 9, 9, null, 0, 0, false)]
        [InlineData(0, 0, "E", 9, 9, null, 1, 0, false)]
        [InlineData(1, 0, "W", 9, 9, null, 0, 0, false)]

        // isLost (with no previous isLosts)
        // North - go off top so lost, gridMaxX 2, gridMaxY 2, expectedX 2, expectedY 2
        [InlineData(2, 2, "N", 2, 2, null, 2, 2, true)]
        // South - go off bottom
        [InlineData(0, 0, "S", 2, 2, null, 0, 0, true)]
        // East
        [InlineData(2, 1, "E", 2, 2, null, 2, 1, true)]
        // West
        [InlineData(0, 1, "W", 2, 2, null, 0, 1, true)]


        public void MoveTests(int currentX, int currentY, string orientation, int gridMaxX, int gridMaxY,
            List<(int, int, string)> isLostCoordsList, int expectedX, int expectedY, bool isLost)
        {
            Assert.Equal((expectedX, expectedY, isLost), Move(currentX, currentY, orientation, gridMaxX, gridMaxY, isLostCoordsList));
        }


        [Fact]
        public void RunFirstShip() => Assert.Equal("1 1 E", Run(new[] { "5 3\n1 1 E\nRFRFRFRF" }));

        [Fact]
        public void RunSecondShip() => Assert.Equal("3 3 N LOST", Run(new[] { "5 3\n3 2 N\nFRRFLLFFRRFLL" }));

        // ThirdShip depends on the knowledge of SecondShip for a warning
        // so need to run SecondShip and ThirdShip together
        [Fact]
        public void RunThirdShip() => Assert.Equal("2 3 S", Run(new[] { "5 3\n3 2 N\nFRRFLLFFRRFLL", "5 3\n0 3 W\nLLFFFLFLFL" }));

        [Fact]
        public void RunAllShips() => Assert.Equal("2 3 S", Run(new[] { "5 3\n1 1 E\nRFRFRFRF", "5 3\n3 2 N\nFRRFLLFFRRFLL", "5 3\n0 3 W\nLLFFFLFLFL" }));


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

        [Fact]
        public void GridMaxXShouldThrowIfMoreThan50() =>
                    Assert.Throws<ArgumentException>(() => Run(new[] { "51 3\n1 1 E\nRFRFRFRF" }));

        [Fact]
        public void GridMaxYShouldThrowIfMoreThan50() =>
            Assert.Throws<ArgumentException>(() => Run(new[] { "5 51\n1 1 E\nRFRFRFRF" }));

        [Fact]
        public void XShouldThrowIfMoreThan50() =>
            Assert.Throws<ArgumentException>(() => Run(new[] { "5 3\n51 1 E\nRFRFRFRF" }));

        [Fact]
        public void YShouldThrowIfMoreThan50() =>
            Assert.Throws<ArgumentException>(() => Run(new[] { "5 3\n5 51 E\nRFRFRFRF" }));


        [Fact]
        public void InstructionStringMoreOrEqualTo100ShouldThrow()
        {
            var instructions = new string('R', 100);
            Assert.Throws<ArgumentException>(() => Run(new[] { $"5 3\n1 1 E\n{instructions}" }));
        }
        [Fact]
        public void InstructionStringLessThan100ShouldNotThrow()
        {
            var instructions = new string('R', 99);
            // if no exception thrown the test will pass
            var result = Run(new[] { $"5 3\n1 1 E\n{instructions}" });
        }


    }
}
