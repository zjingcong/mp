import numpy as np
from Tkinter import *
import time
import functools
from agent import Agent
import math

"""
    Initalize parameters to run a simulation
"""
dt = 0.05 # the simulation time step
scenarioFile='crossing_agents.csv'
doExport = True # export the simulation?
agents = [] # the simulated agents
trajectories = [] # keep track of the agents' traces
ittr = 0 # keep track of simulation iterations 
maxIttr = 500  #how many time steps we want to simulate
globalTime = 0  # simuation time      
reachedGoals = False # have all agents reached their goals

""" 
    Drawing parameters
"""
pixelsize = 1024
framedelay = 30
drawVels = True
QUIT = False
paused = False
step = False
circles = []
velLines = []
gvLines = []

"""
    Agents parameters
"""
ksi = 0.1
dhor = 5
timehor = 2


"""
    Grid maps
"""
dx = dhor
dy = dhor
Nx = 0
Ny = 0


def readScenario(fileName, scalex=1., scaley=1.):
    """
        Read a scenario from a file
    """
    
    fp = open(fileName, 'r')
    lines = fp.readlines()
    fp.close()
    for line in lines:
        # create an agent and add it to the list (csvParms, ksi, dhor, timehor, goalRadiusSq, maxF)
        agents.append(Agent(line.split(','), ksi, dhor, timehor))
    
    # define the boundaries of the environment
    positions = [a.pos for a in agents]
    goals = [a.goal for a in agents]
    x_min =	min(np.amin(np.array(positions)[:,0]), np.amin(np.array(goals)[:,0]))*scalex - 2.
    y_min =	min(np.amin(np.array(positions)[:,1]), np.amin(np.array(goals)[:,1]))*scaley - 2.
    x_max =	max(np.amax(np.array(positions)[:,0]), np.amax(np.array(goals)[:,0]))*scalex + 2.
    y_max =	max(np.amax(np.array(positions)[:,1]), np.amax(np.array(goals)[:,1]))*scaley + 2.

    num = len(agents)

    return x_min, x_max, y_min, y_max 


def initWorld(canvas):
    """
        initialize the agents 
    """
    print ("")
    print ("Simulation of Agents on a 2D plane.")
    print ("Green Arrow is Goal Velocity, Red Arrow is Current Velocity")
    print ("SPACE to pause, 'S' to step frame-by-frame, 'V' to turn the velocity display on/off.")
    print ("")
       
    colors = ["#FAA", "blue","yellow", "white"]
    for a in agents:
        circles.append(canvas.create_oval(0, 0, a.radius, a.radius, fill=colors[a.gid%4])) # color the disc of an agenr based on its group id
        velLines.append(canvas.create_line(0,0,10,10,fill="red"))
        gvLines.append(canvas.create_line(0,0,10,10,fill="green"))


def drawWorld():
    """
        draw the agents
    """

    for i in range(len(agents)):
        agent = agents[i]
        if not agent.atGoal:
            canvas.coords(circles[i],world_scale*(agent.pos[0]- agent.radius - world_xmin), world_scale*(agent.pos[1] - agent.radius - world_ymin), world_scale*(agent.pos[0] + agent.radius - world_xmin), world_scale*(agent.pos[1] + agent.radius - world_ymin))
            canvas.coords(velLines[i],world_scale*(agent.pos[0] - world_xmin), world_scale*(agent.pos[1] - world_ymin), world_scale*(agent.pos[0]+ agent.radius*agent.vel[0] - world_xmin), world_scale*(agent.pos[1] + agent.radius*agent.vel[1] - world_ymin))
            canvas.coords(gvLines[i],world_scale*(agent.pos[0] - world_xmin), world_scale*(agent.pos[1] - world_ymin), world_scale*(agent.pos[0]+ agent.radius*agent.gvel[0] - world_xmin), world_scale*(agent.pos[1] + agent.radius*agent.gvel[1] - world_ymin))
            if drawVels:
                canvas.itemconfigure(velLines[i], state="normal")
                canvas.itemconfigure(gvLines[i], state="normal")
            else:
                canvas.itemconfigure(velLines[i], state="hidden")
                canvas.itemconfigure(gvLines[i], state="hidden")


def on_key_press(event):
    """
        keyboard events
    """                    
    global paused, step, QUIT, drawVels

    if event.keysym == "space":
        paused = not paused
    if event.keysym == "s":
        step = True
        paused = False
    if event.keysym == "v":
        drawVels = not drawVels
    if event.keysym == "Escape":
        QUIT = True


# build grid map
def grid_map():
    global grid, world_xmin, world_ymin

    grid = [[] for id in xrange(Nx * Ny)]
    for agent in agents:
        x = agent.pos[0]
        y = agent.pos[1]
        i = int(math.floor((x - world_xmin) / dx))
        j = int(math.floor((y - world_ymin) / dy))
        grid[i + Nx * j].append(agent)


# proximity queries
def get_neighbors(curr_agent):
    global grid, world_xmin, world_ymin

    neighbors = list()
    curr_pos = curr_agent.pos

    # =====================================================
    # O(n_sqrt)
    for agent in agents:
        if agent is not curr_agent:
            pos = agent.pos
            dist = np.linalg.norm((pos - curr_pos), ord=1)
            if dist <= curr_agent.dhor:
                neighbors.append(agent)
    # ======================================================

    # # ======================================================
    # # grid maps for proximity queries
    # # possibly inside cells
    # x = curr_pos[0]
    # y = curr_pos[1]
    # il = int(math.floor((x - world_xmin - dhor) / dx))
    # if il < 0:
    #     il = 0
    # ir = int(math.floor((x - world_xmin + dhor) / dx))
    # if ir > Nx - 1:
    #     ir = Nx - 1
    # jl = int(math.floor((y - world_ymin - dhor) / dy))
    # if jl < 0:
    #     jl = 0
    # ju = int(math.floor((y - world_ymin + dhor) / dy))
    # if ju > Ny - 1:
    #     ju = Ny - 1
    #
    # for i in xrange(il, ir + 1):
    #     for j in xrange(jl, ju + 1):
    #         for agent in grid[i + Nx * j]:
    #             if agent is not curr_agent:
    #                 pos = agent.pos
    #                 dist = np.linalg.norm((pos - curr_pos), ord=1)
    #                 if dist <= curr_agent.dhor:
    #                     neighbors.append(agent)
    # # ======================================================

    return neighbors


def updateSim(dt):
    """
        Update the simulation 
    """

    global reachedGoals, grid

    # grid_map()
   
    # compute the forces acting on each agent
    for agent in agents:
        neighbors = get_neighbors(agent)
        agent.computeForces(neighbors)
    
    reachedGoals = True    
    for agent in agents:
        agent.update(dt)
        if not agent.atGoal:
            reachedGoals = False


def drawFrame(dt):
    """
        simulate and draw frames 
    """

    global start_time,step,paused,ittr,globalTime

    if reachedGoals or ittr > maxIttr or QUIT: #Simulation Loop
        print("%s itterations ran ... quitting"%ittr)
        win.destroy()
    else:
        elapsed_time = time.time() - start_time
        start_time = time.time()
        if not paused:
            updateSim(dt)
            ittr += 1
            globalTime += dt
            for agent in agents:
                if not agent.atGoal:
                   trajectories.append([agent.id, agent.gid, agent.pos[0], agent.pos[1], agent.vel[0], agent.vel[1], agent.radius, globalTime])

        drawWorld()
        if step == True:
            step = False
            paused = True    
        
        win.title('Multi-Agent Navigation')
        win.after(framedelay,lambda: drawFrame(dt))
  

#=======================================================================================================================
# Main execution of the code
#=======================================================================================================================
world_xmin, world_xmax, world_ymin, world_ymax = readScenario(scenarioFile)
world_width = world_xmax - world_xmin
world_height = world_ymax - world_ymin
world_scale = pixelsize/world_width

# set the grid
Nx = int(math.ceil(world_width / dx))
Ny = int(math.ceil(world_height / dy))
grid = [[] for i in xrange(Nx * Ny)]

# set the visualizer
win = Tk()
# keyboard interaction
win.bind("<space>",on_key_press)
win.bind("s",on_key_press)
win.bind("<Escape>",on_key_press)
win.bind("v",on_key_press)
# the drawing canvas
canvas = Canvas(win, width=pixelsize, height=pixelsize*world_height/world_width, background="#666")
canvas.pack()
initWorld(canvas)
start_time = time.time()
# the main loop of the program
win.after(framedelay, lambda: drawFrame(dt))
mainloop()
if doExport:
    header = "id,gid,x,y,v_x,v_y,radius,time"
    exportFile = scenarioFile.split('.csv')[0] + "_sim.csv"
    np.savetxt(exportFile, trajectories, delimiter=",", fmt='%d,%d,%f,%f,%f,%f,%f,%f', header=header, comments='')
