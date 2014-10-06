Graph
=====
How to run this program:

1. This program needs to be passed a command line argument containing the path of
a file that contains the graph data. I included a sample file called graph.txt with
my submission that contains the example data from the prompt. If you run this
program in Visual Studio, open the RoutePlanner.cs file, go to Debug > Trains Properties
and add the path of graph.txt to the Command Line Arguments text box, in quotes.

2. To get the desired input, your input string needs to have a flag character to
let the program know what type of output to produce.

p - total distance of exact path given
example: A-B-C p; A-D p; A-D-C p; a-e-b-c-d P; A-E-D p

m - how many routes between two towns have a max number of stops that is the number given
example: C-C m 3

x - how many routes between two towns have the exact number of stops as the number given
example: A-C x 4

l - how many routes between two towns have length less than the number given
example: C-C l 30

s- finds the shortest route between two given towns
examples: A-C s; B-B s 

3. Optimizations and Things to Improve:

Comments in files for assumtions and TODO statements of some of the 
optimizations I would have liked to make to this program given more time. Besides the
ones noted, such as implementing Dijkstra's single-source shortest path algorithm
for finding the shortest path between two towns, the main things I would have liked
to improve would be splitting up the unit tests and user input parsing function into
smaller functions, creating objects to pass parameters into the functions that 
calculate results to simplify them, and optimising the townCharToInt() and 
townIntToChar() functions by converting them into hash tables, and a few other things.



