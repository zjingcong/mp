
import heapq as pq

# ----------
# Implement the function below
#
# Compute the optimal path from start to goal.
# The car is moving on a 2D grid and
# its orientation can be chosen from four different directions:
forward = [[-1,  0], # 0: go up
           [ 0, -1], # 1: go left
           [ 1,  0], # 2: go down
           [ 0,  1]] # 3: go right

# ====== 3d ======
# The car can perform 3 actions: 0: right turn, 1: no turn, 2: left turn
# action = [-1, 0, 1]
# action_name = ['R', 'F', 'L']
# cost = [1, 1, 1] # corresponding cost values

# ====== 2d ======
action_name = ['U', 'L', 'R', 'D']
cost = [1, 1, 1, 1] # corresponding cost values

# GRID:
#     0 = navigable space
#     1 = unnavigable space 
grid = [[1, 1, 1, 0, 0, 0],
        [1, 1, 1, 0, 1, 0],
        [0, 0, 0, 0, 0, 0],
        [1, 1, 1, 0, 1, 1],
        [1, 1, 1, 0, 1, 1]]

start = [4, 3, 0]   # [grid row, grid col, direction]
                
goal = [2, 0]   # [grid row, grid col]

heuristic = [[2, 3, 4, 5, 6, 7],
        [1, 2, 3, 4, 5, 6],
        [0, 1, 2, 3, 4, 5],
        [1, 2, 3, 4, 5, 6],
        [2, 3, 4, 5, 6, 7]]


def compute_plan(grid, start, goal, cost, heuristic):
    # ----------------------------------------
    # modify the code below
    # ----------------------------------------
    # closed = [[[0 for row in range(len(grid[0]))] for col in range(len(grid))],
    #          [[0 for row in range(len(grid[0]))] for col in range(len(grid))],
    #          [[0 for row in range(len(grid[0]))] for col in range(len(grid))],
    #          [[0 for row in range(len(grid[0]))] for col in range(len(grid))]]
    # parent = [[[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
    #          [[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
    #          [[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
    #          [[' ' for row in range(len(grid[0]))] for col in range(len(grid))]]
    #
    # closed[start[2]][start[0]][start[1]] = 1
    #
    # plan = [['-' for row in range(len(grid[0]))] for col in range(len(grid))]
    #
    # x = start[0]
    # y = start[1]
    # theta = start[2]
    # g = 0
    # h = heuristic[x][y]
    # f = g+h
    # open = [[f, g, h, x, y, theta]]

    # your code: implement A*

    # Initially you may want to ignore theta, that is, plan in 2D.
    # To do so, set actions=forward, cost = [1, 1, 1, 1], and action_name = ['U', 'L', 'R', 'D']
    # Similarly, set closed=[[0 for row in range(len(grid[0]))] for col in range(len(grid))]
    # and parent=[[' ' for row in range(len(grid[0]))] for col in range(len(grid))]

    # 2d astar ignore theta
    closed = [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]  # store the cost so far
    parent = [[' ' for row in range(len(grid[0]))] for col in range(len(grid))]


    plan = [['-' for row in range(len(grid[0]))] for col in range(len(grid))]
    expand = [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]

    # start node
    x = start[0]
    y = start[1]
    theta = start[2]
    g = 0
    h = heuristic[x][y]
    f = g + h
    start = [f, g, h, x, y, theta]
    closed[x][y] = g

    open = []  # priority queue
    pq.heappush(open, (f, start))

    fail = False

    step = 0

    while not fail:
        if len(open) == 0:
            fail = True
            print "Can't find path."
            return 'Fail'

        # get current node
        current = pq.heappop(open)[1]
        x_curr = current[3]
        y_curr = current[4]
        theta_curr = current[5]

        step = step + 1
        expand[x_curr][y_curr] = step

        if x_curr == goal[0] and y_curr == goal[1]:
            print "Find path."
            break

        for index, forw in enumerate(forward):
            # get next node
            x_next = x_curr + forw[0]
            y_next = y_curr + forw[1]

            # boundary condition
            if x_next >= 0 and x_next < len(grid) and y_next >= 0 and y_next < len(grid[0]):
                new_cost = closed[x_curr][y_curr] + cost[index]
                if (closed[x_next][y_next] == -1 or new_cost < closed[x_next][y_next]) and grid[x_next][y_next] == 0:
                    closed[x_next][y_next] = new_cost
                    h_next = heuristic[x_next][y_next]
                    f_next = new_cost + h_next
                    next = [f_next, new_cost, h_next, x_next, y_next, theta]
                    pq.heappush(open, (f_next, next))

    # return plan
    return expand


def show(p):
    for i in range(len(p)):
        print p[i]


show(compute_plan(grid, start, goal, cost, heuristic))
