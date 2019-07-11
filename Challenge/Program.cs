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

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");
        }

        static string Run(string input)
        {

            string[] lines = input.Split( new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None );

            return lines[2];
        }

        [Fact]
        public void RunTest() => Assert.Equal("d e", Run(@"a b\nc\nd e"));
    }
}
