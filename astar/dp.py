
import pprint

# ----------
# Implement the function below.
#
# Compute the optimal path from start to goal.
# The car The car is moving on a 2D grid and
# its orientation can be chosen from four different directions:
forward = [[-1,  0], # 0: go up
           [ 0, -1], # 1: go left
           [ 1,  0], # 2: go down
           [ 0,  1]] # 3: go right

# ===== 3d =====
# The car can perform 3 actions: 0: right turn, 1: no turn, 2: left turn
action = [-1, 0, 1]
action_name = ['R', 'F', 'L']
action_dict = {'R': -1, 'F': 0, 'L': 1}
cost = [1, 1, 1]    # corresponding cost values

# ===== 2d =====
# cost = [1, 1, 1, 1]
# action_name = ['U', 'L', 'D', 'R']

# GRID:
#     0 = navigable space
#     1 = unnavigable space 
grid = [[1, 1, 1, 0, 0, 0],
        [1, 1, 1, 0, 1, 0],
        [0, 0, 0, 0, 0, 0],
        [1, 1, 1, 0, 1, 1],
        [1, 1, 1, 0, 1, 1]]

pprint.pprint(grid)

start = [4, 3, 0]   # [grid row, grid col, direction]
                
goal = [2, 0]   # [grid row, grid col]

# ----------------------------------------
# modify code below
# ----------------------------------------


def compute_policy(grid, start, goal, cost):
    # Initially you may want to ignore orientation, that is, plan in 2D.
    # To do so, set actions=forward, cost = [1, 1, 1, 1], and action_name = ['U', 'L', 'R', 'D']
    # Similarly, set value=[[999 for row in range(len(grid[0]))] for col in range(len(grid))]
    # and policy = [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]

    value = [[[999 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[999 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[999 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[999 for row in range(len(grid[0]))] for col in range(len(grid))]]

    policy = [[[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
             [[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
             [[' ' for row in range(len(grid[0]))] for col in range(len(grid))],
             [[' ' for row in range(len(grid[0]))] for col in range(len(grid))]]

    # return the optimal policy from the start.
    plan = [['-' for row in range(len(grid[0]))] for col in range(len(grid))]

    update = True
    while update:
        update = False
        # pprint.pprint(value)
        # print '-' * 10
        for x in xrange(len(grid)):
            for y in xrange(len(grid[0])):
                for theta in xrange(4):
                    # reach the goal
                    if x == goal[0] and y == goal[1] and value[theta][x][y] > 0:
                        value[theta][x][y] = 0
                        policy[theta][x][y] = 'G'
                        update = True

                    else:
                        for index, act in enumerate(action):
                            # get previous node
                            theta_pre = (theta - act) % 4
                            x_pre = x - forward[theta_pre][0]
                            y_pre = y - forward[theta_pre][1]

                            # active action
                            if 0 <= x_pre < len(grid) and 0 <= y_pre < len(grid[0]) and grid[x_pre][y_pre] != 1:
                                value_new = value[theta][x][y] + cost[index]
                                if value_new < value[theta_pre][x_pre][y_pre]:
                                    value[theta_pre][x_pre][y_pre] = value_new
                                    policy[theta_pre][x_pre][y_pre] = action_name[index]
                                    update = True

    # get the optimal policy from start
    x_curr = start[0]
    y_curr = start[1]
    theta_curr = start[2]
    isGoal = False
    while not isGoal:
        if x_curr == goal[0] and y_curr == goal[1]:
            plan[x_curr][y_curr] = 'G'
            isGoal = True
            break

        act_name = policy[theta_curr][x_curr][y_curr]
        plan[x_curr][y_curr] = act_name
        x_curr = x_curr + forward[theta_curr][0]
        y_curr = y_curr + forward[theta_curr][1]
        theta_curr = (theta_curr + action_dict[act_name]) % 4

    # # ===============================================================================================================
    # # 2d dp
    # value = [[999 for row in range(len(grid[0]))] for col in range(len(grid))]
    # policy = [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]
    #
    # update = True
    # while update:
    #     update = False
    #     for x in xrange(len(grid)):
    #         for y in xrange(len(grid[0])):
    #             # reach the goal
    #             if x == goal[0] and y == goal[1] and value[x][y] > 0:
    #                 value[x][y] = 0
    #                 policy[x][y] = 'Dist'
    #                 update = True
    #
    #             else:
    #                 for index, forw in enumerate(forward):
    #                     # get previous node
    #                     x_pre = x - forw[0]
    #                     y_pre = y - forw[1]
    #
    #                     # active action
    #                     if 0 <= x_pre < len(grid) and 0 <= y_pre < len(grid[0]) and grid[x_pre][y_pre] != 1:
    #                         value_new = value[x][y] + cost[index]
    #                         if value_new < value[x_pre][y_pre]:
    #                             value[x_pre][y_pre] = value_new
    #                             policy[x_pre][y_pre] = action_name[index]
    #                             update = True
    #
    # pprint.pprint(value)
    # pprint.pprint(policy)

    return plan


def show(p):
    for i in range(len(p)):
        print p[i]


show(compute_policy(grid, start, goal, cost))
