using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    class Program
    {
        static void Main(string[] args)
        {
            Maze maze = new Maze(20, 20);
            AILearning ai = new AILearning(maze);
            maze.GenerateMaze();
            ai.Start();    

            //Console.ReadKey();
            //maze.SolveMaze();
            //maze.ShowMaze();
            Console.ReadKey();
        }

        
    }
}
