/* Maze generator in C.
* Joe Wingbermuehle
* 19990805
*/

#include <stdio.h>
#include <stdlib.h>
#include <time.h>
#include <Windows.h>

/* Display the maze. */
void ShowMaze(const char *maze, int width, int height) {
	int x, y;
	for (y = 0; y < height; y++) {
		for (x = 0; x < width; x++) {
			switch (maze[y * width + x]) {
			case 1:  printf("%c", 178);  break;
			case 2:  printf("+");  break;
			default: printf(" ");  break;
			}
		}
		printf("\n");
	}
}

/*  Carve the maze starting at x, y. */
void CarveMaze(char *maze, int width, int height, int x, int y) {

	int x1, y1;
	int x2, y2;
	int dx, dy;
	int dir, count;

	dir = rand() % 4;
	count = 0;
	while (count < 4) {
		dx = 0; dy = 0;
		switch (dir) {
		case 0:  dx = 1;  break;
		case 1:  dy = 1;  break;
		case 2:  dx = -1; break;
		default: dy = -1; break;
		}
		x1 = x + dx;
		y1 = y + dy;
		x2 = x1 + dx;
		y2 = y1 + dy;
		if (x2 > 0 && x2 < width && y2 > 0 && y2 < height
			&& maze[y1 * width + x1] == 1 && maze[y2 * width + x2] == 1) {
			maze[y1 * width + x1] = 0;
			maze[y2 * width + x2] = 0;
			x = x2; y = y2;
			dir = rand() % 4;
			count = 0;
		}
		else {
			dir = (dir + 1) % 4;
			count += 1;
		}
	}

}

/* Generate maze in matrix maze with size width, height. */
void GenerateMaze(char *maze, int width, int height) {

	int x, y;

	/* Initialize the maze. */
	for (x = 0; x < width * height; x++) {
		maze[x] = 1;
	}
	maze[1 * width + 1] = 0;

	/* Seed the random number generator. */
	srand(time(0));

	/* Carve the maze. */
	for (y = 1; y < height; y += 2) {
		for (x = 1; x < width; x += 2) {
			CarveMaze(maze, width, height, x, y);
		}
	}

	/* Set up the entry and exit. */
	maze[0 * width + 1] = 0;
	maze[(height - 1) * width + (width - 2)] = 0;

}

/* Solve the maze. */
void SolveMaze(char *maze, int width, int height) {

	int dir, count;
	int x, y;
	int dx, dy;
	int forward;

	/* Remove the entry and exit. */
	maze[0 * width + 1] = 1;
	maze[(height - 1) * width + (width - 2)] = 1;

	forward = 1;
	dir = 0;
	count = 0;
	x = 1;
	y = 1;
	while (x != width - 2 || y != height - 2) {
		dx = 0; dy = 0;
		switch (dir) {
		case 0:  dx = 1;  break;
		case 1:  dy = 1;  break;
		case 2:  dx = -1; break;
		default: dy = -1; break;
		}
		if ((forward  && maze[(y + dy) * width + (x + dx)] == 0)
			|| (!forward && maze[(y + dy) * width + (x + dx)] == 2)) {
			maze[y * width + x] = forward ? 2 : 3;
			x += dx;
			y += dy;
			forward = 1;
			count = 0;
			dir = 0;
		}
		else {
			dir = (dir + 1) % 4;
			count += 1;
			if (count > 3) {
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

void AILearning(char *maze, int width, int height) {

}

int main(int argc, char *argv[]) {

	int width, height;
	char *maze;

	printf("Maze by Joe Wingbermuehle 19990805\n");
	width = 20 * 2 + 3;
	height = 20 * 2 + 3;

	/* Allocate the maze array. */
	maze = (char*)malloc(width * height * sizeof(char));
	if (maze == NULL) {
		printf("error: not enough memory\n");
		exit(EXIT_FAILURE);
	}

	/* Generate and display the maze. */
	GenerateMaze(maze, width, height);
	ShowMaze(maze, width, height);

	/* Solve the maze if requested. */
	getchar();
	
	SolveMaze(maze, width, height);
	ShowMaze(maze, width, height);
	getchar();
	/* Clean up. */
	free(maze);
	exit(EXIT_SUCCESS);

}
