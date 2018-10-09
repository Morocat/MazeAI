using System;
using System.Collections.Generic;
using System.Threading;
using System.Diagnostics;

namespace Maze
{
    class AILearning
    {
        const int GENERATION_SIZE = 100;
        const double SELECTION_PERCENT = 0.25;
        public static double MUTATION_PERCENT = 0.05;
        const int GERIATRIC_AGE = 10;
        const int STALENESS_STEP_LIMIT = 2;
        const int STALENESS_CUTOFF = 3;
        int generationCount = 1;
        int stalenessCount = 0;

        List<Node> nodes = new List<Node>();
        Maze maze;

        public AILearning(Maze maze)
        {
            this.maze = maze;
        }

        public void Start()
        {
            SpawnInitialPopulation();

            while (true)
            {
                for (int i = 0; i < nodes.Count; i++)
                    maze.markSpot(nodes[i].x, nodes[i].y, i);
                //nodes.ForEach(n => maze.markSpot(n.x, n.y));
                if (CalcFitness() >= 0)
                {
                    // we found a winner
                    Console.WriteLine("Winner winner chicken dinner!");
                    Console.WriteLine(generationCount + " generations");
                    maze.ShowMaze();
                    return;
                }
                generationCount++;
                //maze.ShowMaze();
                Reproduce();
                MoveAround();
                //Console.ReadKey();
                if (generationCount % 50 == 0)
                {
                    maze.ShowMaze();
                    Console.WriteLine("Generation Count: " + generationCount);
                }
                maze.CleanMaze();
            }
        }

        void SpawnInitialPopulation()
        {
            int move;
            // create nodes
            for (int i = 0; i < GENERATION_SIZE; i++)
            {
                nodes.Add(new Node());
                while (nodes[i].moves.Count < Node.NumMoves)
                {
                    move = GenerateMove(nodes[i], -1);
                    nodes[i].moves.Add(move);
                    Move(nodes[i], move);
                }
            }
        }

        int CalcFitness()
        {
            int retVal = -1;
            int fitness;
            List<int> moves;

            //nodes.ForEach(n => Debug.Assert(n.moves.Count == Node.NumMoves));
            for (int i = 0; i < nodes.Count; i++)
            {
                // check to see if it finished the maze
                if (nodes[i].x == maze.Width - 2 && nodes[i].y == maze.Height - 1)
                    retVal = i;

                // this fitness function sucks, lets try a different one
                //fitness = nodes[i].x + nodes[i].y; // fitness function
                // this fitness function determines how many steps into the maze its taken
                moves = new List<int>(nodes[i].moves);
                // remove invalid moves
                int x = 1, y = 0;
                for (int j = 0; j < moves.Count; j++)
                {
                    switch (moves[j])
                    {
                        case 0:
                            x++;
                            break;
                        case 1:
                            y++;
                            break;
                        case 2:
                            x--;
                            break;
                        case 3:
                            y--;
                            break;
                    }
                    if (!maze.isValidMove(x, y))
                    {
                        switch (moves[j])
                        {
                            case 0:
                                x--;
                                break;
                            case 1:
                                y--;
                                break;
                            case 2:
                                x++;
                                break;
                            case 3:
                                y++;
                                break;
                        }
                        moves.RemoveAt(j--);
                    }
                }

                // remove equal, opposite moves
                for (int j = 1; j < moves.Count; j++)
                {
                    if (moves[j - 1] % 2 == moves[j] % 2 && moves[j - 1] != moves[j])
                    {
                        moves.RemoveRange(j - 1, 2);
                        j--;
                    }
                }

                fitness = moves.Count;

                if (fitness > nodes[i].maxFitness)
                    nodes[i].maxFitness = fitness;

                // check for staleness
                //if (fitness - nodes[i].maxFitness < STALENESS_STEP_LIMIT && nodes[i].staleness < 5)
                //    nodes[i].staleness++;

                // kill old nodes that are becoming stale
                //if (++nodes[i].age >= GERIATRIC_AGE)
                //{
                if (nodes[i].staleness >= STALENESS_CUTOFF && nodes.Count > 10)
                {
                    nodes.RemoveAt(i--);
                    if (stalenessCount < 10)
                        stalenessCount++;
                }
                //}
                if (i >= 0 && i < nodes.Count && nodes[i] != null)
                    nodes[i].fitness = fitness;
            }

            // sort nodes based on fitness so we can choose the fittest 
            // portion of the population to reproduce
            nodes.Sort(delegate (Node n1, Node n2)
            {
                return n1.fitness.CompareTo(n2.fitness);
            });

            //nodes.ForEach(n => Debug.Assert(n.moves.Count == Node.NumMoves));

            return retVal;
        }

        void Reproduce()
        {
            Node p1, p2;
            int[] parents = new int[2];
            int length;

            //if (stalenessCount > 5 && AILearning.MUTATION_PERCENT < .5)
            //    AILearning.MUTATION_PERCENT += .05;
            //else if (stalenessCount <= 5 && AILearning.MUTATION_PERCENT > .05)
            //    AILearning.MUTATION_PERCENT -= .05;

            //if (stalenessCount > 5 && Node.CrossoverPoint >= 0.1)
            //    Node.CrossoverPoint -= 0.1;
            //else if (stalenessCount <= 5 && Node.CrossoverPoint < .75)
            //    Node.CrossoverPoint += .01;

            // select the x% fittest population to have an orgy; slaughter the rest
            int numToRemove = ((int)(GENERATION_SIZE * (1.0 - SELECTION_PERCENT))) - (GENERATION_SIZE - nodes.Count);
            if (numToRemove >= 0 && nodes.Count - numToRemove >= 10)
                nodes.RemoveRange(0, numToRemove);
            if (nodes[nodes.Count - 1].moves.Count != nodes[0].moves.Count)
                nodes.RemoveAt(nodes.Count - 1);
            length = nodes.Count;
            while (nodes.Count < GENERATION_SIZE)
            {
                // pick two parents
                MyRand.GenerateRandomList(2, length, parents);
                p1 = nodes[parents[0]];
                p2 = nodes[parents[1]];
                //Debug.Assert(p1.moves.Count == p2.moves.Count);
                // ahh, the magic of life
                nodes.AddRange(Node.Reproduce(p1, p2));
            }
            //nodes.ForEach(n => Debug.Assert(n.moves.Count == Node.NumMoves));

            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].x = 1;
                nodes[i].y = 0;
            }
        }

        void MoveAround()
        {
            int move;

            // nodes get x more moves per generation
            if (generationCount % 5 == 0)
                Node.NumMoves += Node.NUM_MOVES_STEP_SIZE;
            for (int i = 0; i < nodes.Count; i++)
            {
                // run the maze to ensure all moves are valid and fix those that aren't
                for (int j = 0; j < nodes[i].moves.Count; j++)
                {
                    move = GenerateMove(nodes[i], nodes[i].moves[j]);
                    nodes[i].moves[j] = move;
                    Move(nodes[i], move);
                }
                // add more moves every generation
                while (nodes[i].moves.Count < Node.NumMoves)
                {
                    move = GenerateMove(nodes[i], -1);
                    nodes[i].moves.Add(move);
                    Move(nodes[i], move);
                }
            }
        }

        int GenerateMove(Node n, int expectedMove)
        {
            return MyRand.rand.Next(4);
            int[] moves = new int[4];
            MyRand.GenerateRandomList(4, moves);
            int x, y;
            int move;
            int index = 0;
            do
            {
                // this is essentially our repair function for invalid paths
                // this introduces more variability into the population as well
                move = expectedMove >= 0 ? expectedMove : moves[index++];
                //move = moves[0];
                x = n.x;
                y = n.y;
                switch(move)
                {
                    case 0: x++;
                        break;
                    case 1: y++;
                        break;
                    case 2: x--;
                        break;
                    case 3: y--;
                        break;
                }
                expectedMove = -1;
            } while (!maze.isValidMove(x, y));

            return move;
        }

        void Move(Node n, int move)
        {
            int x = n.x;
            int y = n.y;
            switch (move)
            {
                case 0:
                    x++;
                    break;
                case 1:
                    y++;
                    break;
                case 2:
                    x--;
                    break;
                case 3:
                    y--;
                    break;
            }
            if (maze.isValidMove(x, y))
            {
                n.x = x;
                n.y = y;
            }
        }

    }

    class Node
    {
        public static int NumMoves = 10;
        public const int NUM_MOVES_STEP_SIZE = 5;
        public static double CrossoverPoint = 0.65;
        public static int highestFitness = 0, lowestFitness = int.MaxValue;

        public List<int> moves = new List<int>();
        public int fitness, maxFitness;
        public int x = 1, y = 0;
        public int staleness, age;

        public static List<Node> Reproduce(Node p1, Node p2)
        {
            int initialMove;
            List<Node> children = new List<Node>();
            children.Add(new Node());
            children.Add(new Node());
            children[1].staleness = children[0].staleness = (p1.staleness + p2.staleness) / 2;

            for (int i = 0; i < (int)(NumMoves * CrossoverPoint); i++)
            {
                // copy parent genes
                children[0].moves.Add(p1.moves[i]);
                children[1].moves.Add(p2.moves[i]);
                // add mutation
                for (int j = 0; j < 2; j++)
                    if (MyRand.rand.NextDouble() < AILearning.MUTATION_PERCENT)
                    {
                        initialMove = children[j].moves[i];
                        do
                        {
                            children[j].moves[i] = MyRand.NextInt(4);
                        } while (children[j].moves[i] != initialMove);
                    }
            }

            for (int i = (int)(NumMoves * CrossoverPoint); i < NumMoves; i++)
            {
                // copy parent genes
                children[0].moves.Add(p2.moves[i]);
                children[1].moves.Add(p1.moves[i]);
                // add mutation
                for (int j = 0; j < 2; j++)
                    if (MyRand.rand.NextDouble() < AILearning.MUTATION_PERCENT)
                    {
                        initialMove = children[j].moves[i];
                        do
                        {
                            children[j].moves[i] = MyRand.NextInt(4);
                        } while (children[j].moves[i] != initialMove);
                    }
            }

            //Debug.Assert(children[0].moves.Count == children[1].moves.Count);

            return children;
        }
    }
    
}
