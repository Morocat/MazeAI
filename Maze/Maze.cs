using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Maze
{
    class Maze
    {
        
        int width, height;
        int[] maze;

        public int Width { get => width; }
        public int Height { get => height; }

        public Maze(int width, int height)
        {
            this.width = width * 2 + 3;
            this.height = height * 2 + 3;
            maze = new int[this.width * this.height];
        }

        public bool isValidMove(int x, int y)
        {
            if (x < 0 || y < 0 || x >= width || y >= width)
                return false;
            return maze[y * width + x] != 1;
        }

        public void markSpot(int x, int y, int i)
        {
            //maze[y * width + x]--;
            maze[y * width + x] = -i;
        }

        public void CleanMaze()
        {
            for (int i = 0; i < width * height; i++)
                if (maze[i] < 0)
                    maze[i] = 0;
        }

        public void ShowMaze()
        {
            int x, y;
            Console.Write("\n");
            for (y = 0; y < height; y++)
            {
                for (x = 0; x < width; x++)
                {
                    if (maze[y * width + x] >= 0)
                    {
                        switch (maze[y * width + x])
                        {
                            case 1: Console.Write("**"); break;
                            case 2: Console.Write("++"); break;
                            default: Console.Write("  "); break;
                        }
                    }
                    else
                    {
                        Console.Write(String.Format("{0,2:###}", Math.Abs(maze[y * width + x])));
                    }
                }
                Console.Write("\n");
            }
        }

        public void GenerateMaze()
        {

            int x, y;

            /* Initialize the maze. */
            for (x = 0; x < width * height; x++)
            {
                maze[x] = 1;
            }
            maze[1 * width + 1] = 0;

            /* Carve the maze. */
            for (y = 1; y < height; y += 2)
            {
                for (x = 1; x < width; x += 2)
                {
                    CarveMaze(x, y);
                }
            }

            /* Set up the entry and exit. */
            maze[0 * width + 1] = 0;
            maze[(height - 1) * width + (width - 2)] = 0;

        }

        void CarveMaze(int x, int y)
        {
            int x1, y1;
            int x2, y2;
            int dx, dy;
            int dir, count;

            dir = MyRand.NextInt(4);
            count = 0;
            while (count < 4)
            {
                dx = 0; dy = 0;
                switch (dir)
                {
                    case 0: dx = 1; break;
                    case 1: dy = 1; break;
                    case 2: dx = -1; break;
                    default: dy = -1; break;
                }
                x1 = x + dx;
                y1 = y + dy;
                x2 = x1 + dx;
                y2 = y1 + dy;
                if (x2 > 0 && x2 < width && y2 > 0 && y2 < height && maze[y1 * width + x1] == 1 && maze[y2 * width + x2] == 1)
                {
                    maze[y1 * width + x1] = 0;
                    maze[y2 * width + x2] = 0;
                    x = x2; y = y2;
                    dir = MyRand.NextInt(4);
                    count = 0;
                }
                else
                {
                    dir = (dir + 1) % 4;
                    count += 1;
                }
            }

        }

        public void SolveMaze()
        {

            int dir, count;
            int x, y;
            int dx, dy;
            int forward;

            /* Remove the entry and exit. */
            maze[0 * width + 1] = 2;
            maze[(height - 1) * width + (width - 2)] = 1;

            forward = 1;
            dir = 0;
            count = 0;
            x = 1;
            y = 1;
            while (x != width - 2 || y != height - 2)
            {
                dx = 0; dy = 0;
                switch (dir)
                {
                    case 0: dx = 1; break;
                    case 1: dy = 1; break;
                    case 2: dx = -1; break;
                    default: dy = -1; break;
                }
                if ((forward > 0 && maze[(y + dy) * width + (x + dx)] == 0)
                    || (forward == 0 && maze[(y + dy) * width + (x + dx)] == 2))
                {
                    maze[y * width + x] = forward > 0 ? 2 : 3;
                    x += dx;
                    y += dy;
                    forward = 1;
                    count = 0;
                    dir = 0;
                }
                else
                {
                    dir = (dir + 1) % 4;
                    count += 1;
                    if (count > 3)
                    {
                        forward = 0;
                        count = 0;
                    }
                }

                //ShowMaze(maze, width, height);
                //Sleep(100);
            }

            /* Replace the entry and exit. */
            maze[(height - 2) * width + (width - 2)] = 2;
            maze[(height - 1) * width + (width - 2)] = 2;

        }
    }
}
