
import heapq as pq
import pprint

# ----------
# Implement the function below
#
# Compute the optimal path from start to goal.
# The car is moving on a 2D grid and
# its orientation can be chosen from four different directions:
forward = [[-1,  0],    # 0: go up
           [ 0, -1],    # 1: go left
           [ 1,  0],    # 2: go down
           [ 0,  1]]    # 3: go right

# ====== 3d ======
# The car can perform 3 actions: 0: right turn, 1: no turn, 2: left turn
action = [-1, 0, 1]
action_name = ['R', 'F', 'L']
cost = [1, 1, 1]    # corresponding cost values

print "Cost: ", [(action_name[i], cost[i]) for i in xrange(len(action_name))]

# ====== 2d ======
# action_name = ['U', 'L', 'R', 'D']
# cost = [1, 1, 1, 1] # corresponding cost values

# GRID:
#     0 = navigable space
#     1 = unnavigable space 
grid = [[1, 1, 1, 0, 0, 0],
        [1, 1, 1, 0, 1, 0],
        [0, 0, 0, 0, 0, 0],
        [1, 1, 1, 0, 1, 1],
        [1, 1, 1, 0, 1, 1]]

print "Grid: "
pprint.pprint(grid)

start = [4, 3, 0]   # [grid row, grid col, direction]
goal = [2, 0]   # [grid row, grid col]

heuristic = [[2, 3, 4, 5, 6, 7],
        [1, 2, 3, 4, 5, 6],
        [0, 1, 2, 3, 4, 5],
        [1, 2, 3, 4, 5, 6],
        [2, 3, 4, 5, 6, 7]]

print "Heuristic: "
pprint.pprint(heuristic)


def compute_plan(grid, start, goal, cost, heuristic):
    # ----------------------------------------
    # modify the code below
    # ----------------------------------------
    closed = [[[-1 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[-1 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[-1 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]]
    parent = [[[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
             [[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
             [[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
             [[' ' for row in range(len(grid[0]))] for col in range(len(grid))]]

    plan = [['-' for row in range(len(grid[0]))] for col in range(len(grid))]

    # Initially you may want to ignore theta, that is, plan in 2D.
    # To do so, set actions=forward, cost = [1, 1, 1, 1], and action_name = ['U', 'L', 'R', 'D']
    # Similarly, set closed=[[0 for row in range(len(grid[0]))] for col in range(len(grid))]
    # and parent=[[' ' for row in range(len(grid[0]))] for col in range(len(grid))]

    # # ============================================================================================================

    # start node
    x = start[0]
    y = start[1]
    theta = start[2]
    g = 0
    h = heuristic[x][y]
    f = g + h
    start = [f, g, h, x, y, theta]
    closed[theta][x][y] = g
    parent[theta][x][y] = ('S', )   # start node

    # priority queue
    open = []
    pq.heappush(open, (f, start))   # use f as priority key

    fail = False
    x_curr = -1
    y_curr = -1
    theta_curr = -1

    while not fail:
        if len(open) == 0:
            fail = True
            return None

        # print "current priority queue: ", open
        # print "current closed: "
        # pprint.pprint(closed)
        # print "current parent: "
        # pprint.pprint(parent)

        # get current node
        current = pq.heappop(open)[1]
        x_curr = current[3]
        y_curr = current[4]
        theta_curr = current[5]

        # print "current node: ", x_curr, y_curr, theta_curr

        # find the goal
        if x_curr == goal[0] and y_curr == goal[1]:
            break

        for index, act in enumerate(action):
            # get next node
            # next node orientation
            theta_next = (theta_curr + act) % 4
            # use orientation to determine position
            x_next = x_curr + forward[theta_curr][0]
            y_next = y_curr + forward[theta_curr][1]
            act_name = action_name[index]

            # boundary condition
            if 0 <= x_next < len(grid) and 0 <= y_next < len(grid[0]):
                # get the new cost
                new_cost = closed[theta_curr][x_curr][y_curr] + cost[index]
                # the next node has not been expanded or has a lower cost, and the next node is in navigable space
                if (closed[theta_next][x_next][y_next] == -1 or new_cost < closed[theta_next][x_next][y_next]) and grid[x_next][y_next] == 0:
                    closed[theta_next][x_next][y_next] = new_cost
                    h_next = heuristic[x_next][y_next]
                    f_next = new_cost + h_next
                    next_node = [f_next, new_cost, h_next, x_next, y_next, theta_next]
                    pq.heappush(open, (f_next, next_node))
                    # print "next node: ", next_node, "push into priority queue"
                    # add parent
                    parent[theta_next][x_next][y_next] = (act_name, current)

    # get the path
    plan[x_curr][y_curr] = 'G'  # reach the target node
    while True:
        pre = parent[theta_curr][x_curr][y_curr]
        if pre[0] == 'S':
            break
        theta_curr = pre[1][5]
        x_curr = pre[1][3]
        y_curr = pre[1][4]
        plan[x_curr][y_curr] = pre[0]

    # # ============================================================================================================

    # # 2d astar ignore theta
    # closed = [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]  # store the cost so far
    # parent = [[' ' for row in range(len(grid[0]))] for col in range(len(grid))]
    #
    # plan = [['-' for row in range(len(grid[0]))] for col in range(len(grid))]
    # expand = [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]
    #
    # # start node
    # x = start[0]
    # y = start[1]
    # theta = start[2]
    # g = 0
    # h = heuristic[x][y]
    # f = g + h
    # start = [f, g, h, x, y, theta]
    # closed[x][y] = g
    #
    # open = []  # priority queue
    # pq.heappush(open, (f, start))
    #
    # fail = False
    #
    # step = 0
    #
    # while not fail:
    #     if len(open) == 0:
    #         fail = True
    #         print "Can't find path."
    #         return 'Fail'
    #
    #     # get current node
    #     current = pq.heappop(open)[1]
    #     x_curr = current[3]
    #     y_curr = current[4]
    #     theta_curr = current[5]
    #
    #     step = step + 1
    #     expand[x_curr][y_curr] = step
    #
    #     if x_curr == goal[0] and y_curr == goal[1]:
    #         print "Find path."
    #         break
    #
    #     for index, forw in enumerate(forward):
    #         # get next node
    #         x_next = x_curr + forw[0]
    #         y_next = y_curr + forw[1]
    #
    #         # boundary condition
    #         if 0 <= x_next < len(grid) and 0 <= y_next < len(grid[0]):
    #             new_cost = closed[x_curr][y_curr] + cost[index]
    #             if (closed[x_next][y_next] == -1 or new_cost < closed[x_next][y_next]) and grid[x_next][y_next] == 0:
    #                 closed[x_next][y_next] = new_cost
    #                 h_next = heuristic[x_next][y_next]
    #                 f_next = new_cost + h_next
    #                 next_node = [f_next, new_cost, h_next, x_next, y_next, theta]
    #                 pq.heappush(open, (f_next, next_node))

    return plan


def show(p):
    if p is None:
        print "Cannot find the path."
        return

    print "Find the path."
    for i in range(len(p)):
        print p[i]


show(compute_plan(grid, start, goal, cost, heuristic))
