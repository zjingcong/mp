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

# The car can perform 3 actions: 0: right turn, 1: no turn, 2: left turn
action = [-1, 0, 1]
action_name = ['R', 'F', 'L']
cost = [1, 1, 1] # corresponding cost values

# GRID:
#     0 = navigable space
#     1 = unnavigable space 
grid = [[1, 1, 1, 0, 0, 0],
        [1, 1, 1, 0, 1, 0],
        [0, 0, 0, 0, 0, 0],
        [1, 1, 1, 0, 1, 1],
        [1, 1, 1, 0, 1, 1]]

start = [4, 3, 0] #[grid row, grid col, direction]
                
goal = [2, 0] #[grid row, grid col]




# ----------------------------------------
# modify code below
# ----------------------------------------

def compute_policy(grid,start,goal,cost):
    value = [[[999 for row in range(len(grid[0]))] for col in range(len(grid))], 
             [[999 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[999 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[999 for row in range(len(grid[0]))] for col in range(len(grid))]]
    
    policy = [[[-1 for row in range(len(grid[0]))] for col in range(len(grid))], 
             [[-1  for row in range(len(grid[0]))] for col in range(len(grid))],
             [[-1 for row in range(len(grid[0]))] for col in range(len(grid))],
             [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]]
    
    # your code: implement dynamic programming

    # Initially you may want to ignore orientation, that is, plan in 2D.
    # To do so, set actions=forward, cost = [1, 1, 1, 1], and action_name = ['U', 'L', 'R', 'D']
    # Similarly, set value=[[999 for row in range(len(grid[0]))] for col in range(len(grid))]
    # and policy = [[-1 for row in range(len(grid[0]))] for col in range(len(grid))]

    # return the optimal policy from the start.
    plan =[['-' for row in range(len(grid[0]))] for col in range(len(grid))]
    x = start[0]
    y = start[1]
    orientation = start[2]
    # your code
    
    return plan

def show(p):
    for i in range(len(p)):
        print p[i]
       
show(compute_policy(grid,start,goal,cost))
